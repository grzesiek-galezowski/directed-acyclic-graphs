using System;

namespace DAG.Interfaces
{
  public interface IDirectedAcyclicGraph<in TValue, in TVisitor, TId> 
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    void AddNode(TId id, TId parentId, TValue value);
    void RemoveNode(TId id, TId parentId);
    void AcceptStartingFromRoot(TVisitor visitor);
  }
}