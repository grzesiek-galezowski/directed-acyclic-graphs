using System;

namespace DAG
{
  public class RootlessGraph<TValue, TVisitor, TId> :
    GraphState<TValue, TVisitor, TId>
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    public void SetRoot(
      DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, 
      GraphStates<TValue, TVisitor, TId> graphStates, 
      TId id, TValue value)
    {
      directedAcyclicGraph.Store(id, directedAcyclicGraph.NewNode(id, value));
      directedAcyclicGraph.SetGraphState(graphStates.Rooted);
    }

  }
}