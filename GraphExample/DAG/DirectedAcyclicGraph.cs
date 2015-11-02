using System;
using System.Collections.Generic;
using DAG.Interfaces;

namespace DAG
{
  public abstract partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class DirectedAcyclicGraph : IDirectedAcyclicGraph
    {
      private readonly IDictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();
      private readonly GraphHooks<TId> _graphHooks;
      private GraphState _currentGraphState;
      private readonly NodeStorage _nodeStorage;
      private readonly Func<TId, TValue, VisitableNode> _nodeFactory;

      public DirectedAcyclicGraph(GraphHooks<TId> graphHooks, GraphState currentGraphState)
      {
        _graphHooks = graphHooks;
        _currentGraphState = currentGraphState;
        _nodeFactory = (id, value) => new VisitableNode(id, value);
        _nodeStorage = new NodeStorage(_nodes, _nodeFactory);
      }

      public void AddNode(TId id, TId parentId, TValue value)
      {
        if (IsParentIdOfRoot(parentId))
        {
          _currentGraphState.SetRoot(this, id, value);
        }
        else
        {
          var node = _nodeStorage.ObtainNode(id, value);
          //bug when replacing node, remove its subtree
          node.CreateBindingBetween(parentId, _nodeStorage);
        }

        //bug this implementation is wrong - there is no test that verifies the dictionary itself after the addition
      }

      public void SetGraphState(GraphState currentGraphState)
      {
        _currentGraphState = currentGraphState;
      }

      public void Store(TId id, VisitableNode node)
      {
        _nodeStorage.Store(id, node);
      }

      private static bool IsParentIdOfRoot(TId parentId)
      {
        return parentId == null;
      }

      public VisitableNode ObtainNode(TId id, TValue value)
      {
        return _nodeStorage.ObtainNode(id, value);
      }

      public void RemoveOldRoot()
      {
        var root = _nodeStorage.Root();
        root.RemoveFrom(_nodeStorage);
      }

      public void RemoveAssociation(TId id, TId parentId)
      {
        var node = _nodeStorage.ObtainNode(id);
        node.RemoveAssociation(_nodeStorage, parentId);
      }

//bug this implementation is wrong - the node is always removed from _nodes even when it occurs many times in the graph

      public void AcceptStartingFromRoot(TVisitor visitor)
      {
        _currentGraphState.AcceptStartingFromRoot(visitor, this, _nodeStorage);
      }

      public void RemoveNode(TId id)
      {
        _nodeStorage.Remove(id);
      }

      public VisitableNode NewNode(TId id, TValue value)
      {
        return _nodeFactory.Invoke(id, value);
      }

      public TId RootId()
      {
        return _nodeStorage.Root().Id;
      }

      public void AssertContainsOnly(params KeyValuePair<TId, TValue>[] elements)
      {
        _nodeStorage.AssertContainsOnly(elements);
      }

    }
  }

}