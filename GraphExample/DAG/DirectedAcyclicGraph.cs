using System;
using System.Collections.Generic;
using System.Linq;

namespace DAG
{
  public class DirectedAcyclicGraph<TValue, TVisitor, TId> 
    where TValue : IVisitable<TVisitor> 
    where TId : class, IEquatable<TId>
  {
    private readonly Dictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();

    public void AddNode(TId id, TId parentId, TValue value)
    {
      var node = new VisitableNode(value, id);
      if (parentId != null)
      {
        Bind(node, parentId);
      }
      else
      {
        AssertNotADuplicateRoot(node);
      }

      AssertNoBoundNodeChange(id, value);

      _nodes[id] = node;
      //bug this implementation is wrong - 
      //the node is overwritten by whatever is last, thus we need to check whether this is the same node as the previous one added with the same ID
      //maybe make a special method for overwrite?
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

    private void AssertNotADuplicateRoot(VisitableNode node)
    {
      var maybeExistingRoot = _nodes.Values.FirstOrDefault(n => n.MatchesRootCondition());
      if (maybeExistingRoot != null)
      {
        throw DuplicateRootException.Create(node.Id, maybeExistingRoot.Id); //bug refactor
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

    public void Accept(TVisitor visitor)
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

  }
}