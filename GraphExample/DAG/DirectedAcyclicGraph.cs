using System.Collections.Generic;

namespace DAG
{
  public class DirectedAcyclicGraph<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor>
  {
    private Dictionary<TId, VisitableNode<TValue, TVisitor, TId>> _nodes 
      = new Dictionary<TId, VisitableNode<TValue, TVisitor, TId>>();

    public void AddNode(TId id, TId parentId, TValue value)
    {
      var node = new VisitableNode<TValue, TVisitor, TId>(value, id);
      var parentNode = _nodes[parentId];
      parentNode.AddChild(node);
      _nodes[id] = node;
    }

    public void RemoveNode(TId id, TId parentId)
    {
      var parentNode = _nodes[parentId];
      parentNode.RemoveDirectChild(id);
      _nodes.Remove(id);
    }
  }
}