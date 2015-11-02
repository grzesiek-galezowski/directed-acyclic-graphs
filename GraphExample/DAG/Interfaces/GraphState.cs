using System;

namespace DAG.Interfaces
{
  public interface GraphState<TValue, TVisitor, TId>
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    void SetRoot(DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, TId id, TValue value);
    void AcceptStartingFromRoot(TVisitor visitor, DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph);
  }
}