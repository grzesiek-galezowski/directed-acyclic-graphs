using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;

namespace DAG
{
  public class DirectedAcyclicGraph<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor> where TId : IEquatable<TId>
  {
    private Dictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();

    public void AddNode(TId id, TId parentId, TValue value)
    {
      var node = new VisitableNode(value, id);
      if (parentId != null)
      {
        var parentNode = _nodes[parentId];
        parentNode.AddChild(node);
      }
      _nodes[id] = node;
    }

    public void RemoveNode(TId id, TId parentId)
    {
      var parentNode = _nodes[parentId];
      parentNode.RemoveDirectChild(id);
      _nodes.Remove(id);
    }

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