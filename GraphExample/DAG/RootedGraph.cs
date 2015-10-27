using System;

namespace DAG
{
  public interface GraphState<TValue, TVisitor, TId>
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    void SetRoot(DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, GraphStates<TValue, TVisitor, TId> graphStates, TId id, TValue value);
  }

  public class RootedGraph<TValue, TVisitor, TId> : 
    GraphState<TValue, TVisitor, TId> 
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    public void SetRoot(DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, GraphStates<TValue, TVisitor, TId> graphStates, TId id, TValue value)
    {
      var node = directedAcyclicGraph.ObtainNode(id, value);
      directedAcyclicGraph.NotifyRootNodeOverwritten(node);
      directedAcyclicGraph.RemoveOldRoot();
      directedAcyclicGraph.Store(id, node);
    }
  }
}