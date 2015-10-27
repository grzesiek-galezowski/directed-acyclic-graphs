using System;
using System.Collections.Generic;
using System.Linq;

namespace DAG
{
  public interface IDirectedAcyclicGraph<in TValue, in TVisitor, TId> 
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    void AddNode(TId id, TId parentId, TValue value);
    void RemoveNode(TId id, TId parentId);
    void AcceptStartingFromRoot(TVisitor visitor);
    void UseHooksFrom(GraphHooks<TId> observer);
  }

  public class DirectedAcyclicGraph<TValue, TVisitor, TId> : IDirectedAcyclicGraph<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor> 
    where TId : class, IEquatable<TId>
  {
    private readonly IDictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();
    private GraphHooks<TId> _graphHooks;
    private GraphState<TValue, TVisitor, TId> _currentGraphState;
    private readonly GraphStates<TValue, TVisitor, TId> _graphStates;

    public DirectedAcyclicGraph()
    {
      _graphStates = new GraphStates<TValue, TVisitor, TId>();
      _graphHooks = new NullObserver<TId>();
      _currentGraphState = _graphStates.Rootless;
    }

    public void AddNode(TId id, TId parentId, TValue value)
    {
      if (IsParentIdOfRoot(parentId))
      {
        _currentGraphState.SetRoot(this, _graphStates, id, value);
      }
      else
      {
        var node = ObtainNode(id, value);
        //bug when replacing node, remove its subtree
        CreateBindingBetween(node, parentId);
        Store(id, node);
      }

      //bug this implementation is wrong - there is no test that verifies the dictionary itself after the addition
    }

    public void SetGraphState(GraphState<TValue, TVisitor, TId> currentGraphState)
    {
      _currentGraphState = currentGraphState;
    }

    public void NotifyRootNodeOverwritten(VisitableNode node)
    {
      _graphHooks.RootNodeOverwritten(Root().Id, node.Id);
    }

    public void Store(TId id, VisitableNode node)
    {
      _nodes[id] = node;
    }

    private static bool IsParentIdOfRoot(TId parentId)
    {
      return parentId == null;
    }

    public VisitableNode ObtainNode(TId id, TValue value)
    {
      VisitableNode node;
      if (_nodes.ContainsKey(id))
      {
        //this is not to lose the bound children and parents
        node = _nodes[id];
        node.Value = value;
      }
      else
      {
        node = new VisitableNode(id, value);
      }
      return node;
    }

    public bool HasRoot()
    {
      return _nodes.Any(n => n.Value.MatchesRootCondition());
    }

    public void RemoveOldRoot()
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

    private void CreateBindingBetween(VisitableNode node, TId parentId)
    {
      var parentNode = _nodes[parentId];
      parentNode.AddChild(node);
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
      if (!HasRoot())
      {
        _graphHooks.VisitorPassedToEmptyGraph();
      }
      else
      {
        var root = Root();
        root.Accept(visitor);
      }
    }

    private VisitableNode Root()
    {
      return _nodes.First(n => n.Value.MatchesRootCondition()).Value;
    }

    public class VisitableNode : VisitableNode<TValue, TVisitor, TId>
    {
      public VisitableNode(TId id, TValue value) : base(value, id)
      {
      }
    }

    public interface IVisitable : IVisitable<TVisitor>
    {
       
    }

    public void UseHooksFrom(GraphHooks<TId> observer)
    {
      _graphHooks = observer;
    }

    public VisitableNode NewNode(TId id, TValue value)
    {
      return new VisitableNode(id, value);
    }
  }
}