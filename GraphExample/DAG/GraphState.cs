using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public interface GraphState
    {
      void SetRoot(DirectedAcyclicGraph directedAcyclicGraph, TId id, TValue value);
      void AcceptStartingFromRoot(TVisitor visitor, DirectedAcyclicGraph directedAcyclicGraph, NodeStorage nodeStorage);
    }
  }
}