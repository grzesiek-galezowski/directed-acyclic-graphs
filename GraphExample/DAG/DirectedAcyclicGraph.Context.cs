using System.Collections.Generic;

namespace DAG
{
  public abstract partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public interface GraphContext
    {
      TId RootId();
      void RemoveOldRoot();
      VisitableNode ObtainNode(TId id, TValue value);
      void Store(TId id, VisitableNode invoke);
      void SetGraphState(GraphState newState);
    }

    public partial class DirectedAcyclicGraph
    {
      void GraphContext.Store(TId id, VisitableNode node)
      {
        _nodeStorage.Store(id, node);
      }

      void GraphContext.SetGraphState(GraphState newState)
      {
        _currentGraphState = newState;
      }

      VisitableNode GraphContext.ObtainNode(TId id, TValue value)
      {
        return _nodeStorage.ObtainNode(id, value);
      }

      void GraphContext.RemoveOldRoot()
      {
        var root = _nodeStorage.Root();
        root.RemoveFrom(_nodeStorage);
      }

      TId GraphContext.RootId()
      {
        return _nodeStorage.Root().Id;
      }
    }

  }

}