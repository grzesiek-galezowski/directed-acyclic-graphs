using System;
using System.Collections.Generic;
using DAG.Interfaces;

namespace DAG
{
  public class NodeStorage<TValue, TVisitor, TId, TNode> 
    where TValue : IVisitable<TVisitor> 
    where TId : class, IEquatable<TId>
    where TNode : VisitableNode<TValue, TVisitor, TId, TNode>
  {
    public readonly IDictionary<TId, TNode> _state;
    private readonly Func<TId, TValue, TNode> _nodeFactory;

    public NodeStorage(IDictionary<TId, TNode> state, Func<TId, TValue, TNode> nodeFactory)
    {
      _state = state;
      _nodeFactory = nodeFactory;
    }

    public void Store(TId id, TNode node)
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

    public TNode ObtainNode(TId id, TValue value)
    {
      SetNode(id, value);
      return _state[id];
    }

    public TNode ObtainNode(TId parentId)
    {
      return _state[parentId];
    }
  }
}