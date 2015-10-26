using System;
using System.Collections.Generic;
using System.Linq;

namespace DAG
{
  public class DirectedAcyclicGraph<TValue, TVisitor, TId> 
    where TValue : IVisitable<TVisitor> 
    where TId : class, IEquatable<TId>
  {
    private readonly IDictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();
    private RootOverwriteObserver<TId> _rootOverwriteObserver;

    public DirectedAcyclicGraph()
    {
      var nullObserver = new NullObserver<TId>();
      _rootOverwriteObserver = nullObserver;
    }

    public void AddNode(TId id, TId parentId, TValue value)
    {
      VisitableNode node;

      if (_nodes.ContainsKey(id))
      {
        node = _nodes[id];
        node.Value = value;
      }
      else
      {
        node = new VisitableNode(value, id);
      }

      if (parentId == null && HasRoot())
      {
        NotifyOnOverwrittenRoot(node);
        RemoveOldRoot();
      }
      else if (parentId != null)
      {
        //bug when replacing node, remove its subtree
        Bind(node, parentId);
      }

      _nodes[id] = node;
      //bug this implementation is wrong - there is no test that verifies the dictionary itself after the addition
    }

    private bool HasRoot()
    {
      return _nodes.Any(n => n.Value.MatchesRootCondition());
    }

    private void RemoveOldRoot()
    {
      var root = Root();
      _nodes.Remove(root.Id);
    }

    private void AssertNoBoundNodeChange(TId id, TValue value)
    {
      if (_nodes.ContainsKey(id))
      {
        var previousNode = _nodes[id];
        if (!previousNode.Value.Equals(value))
        {
          throw new BoundNodeOverwriteException();
        }
      }
    }

    private void Bind(VisitableNode node, TId parentId)
    {
      var parentNode = _nodes[parentId];
      parentNode.AddChild(node);
    }

    private void NotifyOnOverwrittenRoot(VisitableNode node)
    {
      var maybeExistingRoot = _nodes.Values.FirstOrDefault(n => n.MatchesRootCondition());
      if (maybeExistingRoot != null)
      {
        _rootOverwriteObserver.RootNodeOverwritten(maybeExistingRoot.Id, node.Id);
      }
    }

    public void RemoveNode(TId id, TId parentId)
    {
      var parentNode = _nodes[parentId];
      RemoveChild(parentNode, id);
    }

    private void RemoveChild(VisitableNode parentNode, TId id)
    {
      parentNode.RemoveDirectChild(id);
      _nodes.Remove(id);
    }

//bug this implementation is wrong - the node is always removed from _nodes even when it occurs many times in the graph

    public void AcceptStartingFromRoot(TVisitor visitor)
    {
      var root = Root();
      root.Accept(visitor);
    }

    private VisitableNode Root()
    {
      return _nodes.First(n => n.Value.MatchesRootCondition()).Value;
    }

    public class VisitableNode : VisitableNode<TValue, TVisitor, TId>
    {
      public VisitableNode(TValue value, TId id) : base(value, id)
      {
      }
    }

    public interface IVisitable : IVisitable<TVisitor>
    {
       
    }

    public void NotifyOnRootOverwrite(RootOverwriteObserver<TId> observer)
    {
      _rootOverwriteObserver = observer;
    }
  }

  public class NullObserver<T> : RootOverwriteObserver<T>
  {
    public void RootNodeOverwritten(T oldRootName, T newRootName)
    {
      // Method intentionally left empty.
    }
  }

  public interface RootOverwriteObserver<T>
  {
    void RootNodeOverwritten(T oldRootId, T newRootId);
  }
}