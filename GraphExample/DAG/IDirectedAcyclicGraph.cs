using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public interface IDirectedAcyclicGraph
    {
      void AddNode(TId id, TId parentId, TValue value);
      void RemoveAssociation(TId id, TId parentId);
      void AcceptStartingFromRoot(TVisitor visitor);
    }
  }
}