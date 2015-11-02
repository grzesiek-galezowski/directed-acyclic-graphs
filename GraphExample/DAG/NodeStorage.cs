using System;
using System.Collections.Generic;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class NodeStorage
    {
      private readonly IDictionary<TId, VisitableNode> _state;
      private readonly Func<TId, TValue, VisitableNode> _nodeFactory;

      public NodeStorage(IDictionary<TId, VisitableNode> state, Func<TId, TValue, VisitableNode> nodeFactory)
      {
        _state = state;
        _nodeFactory = nodeFactory;
      }

      public void Store(TId id, VisitableNode node)
      {
        _state[id] = node;
      }

      public void ModifyExistingNode(TId id, TValue value)
      {
        var node = _state[id];
        node.Value = value;
      }


      public void SetNode(TId id, TValue value)
      {
        if (_state.ContainsKey(id))
        {
          //this is not to lose the bound children and parents
          ModifyExistingNode(id, value);
        }
        else
        {
          Store(id, _nodeFactory.Invoke(id, value));
        }
      }

      public VisitableNode ObtainNode(TId id, TValue value)
      {
        SetNode(id, value);
        return _state[id];
      }

      public VisitableNode ObtainNode(TId parentId)
      {
        return _state[parentId];
      }
    }
  }
}