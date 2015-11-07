using System.Collections.Generic;

namespace DAG
{
  public abstract partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public partial class DirectedAcyclicGraph : IDirectedAcyclicGraph, GraphContext
    {
      private GraphState _currentGraphState;
      private readonly NodeStorage _nodeStorage;

      public DirectedAcyclicGraph(GraphState currentGraphState, NodeStorage nodeStorage)
      {
        _currentGraphState = currentGraphState;
        _nodeStorage = nodeStorage;
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
          node.BindWithParent(parentId, _nodeStorage);
        }

        //bug this implementation is wrong - there is no test that verifies the dictionary itself after the addition
      }

      private static bool IsParentIdOfRoot(TId parentId)
      {
        return parentId == null;
      }

      public void RemoveAssociation(TId id, TId parentId)
      {
        _currentGraphState.RemoveAssociation(this, id, parentId, _nodeStorage);
      }

      public void AcceptStartingFromRoot(TVisitor visitor)
      {
        _currentGraphState.AcceptStartingFromRoot(visitor, this, _nodeStorage);
      }

      public void RemoveNode(TId id)
      {
        _nodeStorage.Remove(id);
      }


      public void AssertContainsOnly(params KeyValuePair<TId, TValue>[] elements)
      {
        _nodeStorage.AssertContainsOnly(elements);
      }

    }
  }

}