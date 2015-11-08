using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public interface GraphState
    {
      void SetRoot(GraphContext directedAcyclicGraph, TId id, TValue value);
      void AcceptStartingFromRoot(TVisitor visitor, NodeStorage nodeStorage);
      void RemoveAssociation(GraphContext context, TId id, TId parentId, NodeStorage nodeStorage);
      void AcceptStartingFrom(TId id, TVisitor visitor, NodeStorage nodeStorage);
    }
  }
}