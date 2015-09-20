using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DAG
{
  public class VisitableNode<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor> where TId : IEquatable<TId>
  {
    readonly Dictionary<TId, VisitableNode<TValue, TVisitor, TId>> _children = 
      new Dictionary<TId, VisitableNode<TValue, TVisitor, TId>>();
    readonly Dictionary<TId, VisitableNode<TValue, TVisitor, TId>> _parents = 
      new Dictionary<TId, VisitableNode<TValue, TVisitor, TId>>();

    public VisitableNode(TValue value, TId id)
    {
      Value = value;
      Id = id;
    }

    public TId Id { get; private set; }
    public TValue Value { get; private set; }

    public ImmutableHashSet<VisitableNode<TValue, TVisitor, TId>> Parents
    {
      get { return _parents.Values.ToImmutableHashSet(); }
    }

    public ImmutableHashSet<VisitableNode<TValue, TVisitor, TId>> Children
    {
      get { return _children.Values.ToImmutableHashSet(); }
    }

    public void AddChild(VisitableNode<TValue, TVisitor, TId> child)
    {
      Value.AssertNonTerminal();
      _children[child.Id] = child;
      child._parents[Id] = this;
    }

    public void AddParent(VisitableNode<TValue, TVisitor, TId> parent)
    {
      _parents[parent.Id] = parent;
      parent._children[Id] = this;
    }

    public void Accept(TVisitor visitor)
    {
      Value.Accept(visitor);
      foreach (var child in Children)
      {
        child.Accept(visitor);
      }
    }

    public void RemoveDirectChild(TId childId)
    {
      _children[childId]._parents.Remove(Id);
      _children.Remove(childId);
    }

    public bool MatchesRootCondition()
    {
      return _parents.Count == 0;
    }

  }
}