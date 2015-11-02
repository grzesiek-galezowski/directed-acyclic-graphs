using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DAG.Interfaces;

namespace DAG
{
  public class VisitableNode<TValue, TVisitor, TId, TNode> 
    where TValue : IVisitable<TVisitor> 
    where TId : class, IEquatable<TId> 
    where TNode : VisitableNode<TValue, TVisitor, TId, TNode>
  {
    readonly IDictionary<TId, VisitableNode<TValue, TVisitor, TId, TNode>> _children = 
      new SortedDictionary<TId, VisitableNode<TValue, TVisitor, TId, TNode>>();
    readonly IDictionary<TId, VisitableNode<TValue, TVisitor, TId, TNode>> _parents = 
      new SortedDictionary<TId, VisitableNode<TValue, TVisitor, TId, TNode>>();

    public VisitableNode(TValue value, TId id)
    {
      Value = value;
      Id = id;
    }

    public TId Id { get; private set; }
    public TValue Value { get; set; }

    public ImmutableHashSet<VisitableNode<TValue, TVisitor, TId, TNode>> Parents
    {
      get { return _parents.Values.ToImmutableHashSet(); }
    }

    public ImmutableHashSet<VisitableNode<TValue, TVisitor, TId, TNode>> Children
    {
      get { return _children.Values.ToImmutableHashSet(); }
    }

    public void BindWithChild(VisitableNode<TValue, TVisitor, TId, TNode> child)
    {
      Value.AssertNonTerminal();
      _children[child.Id] = child;
      child._parents[Id] = this;
    }

    public void AddParent(VisitableNode<TValue, TVisitor, TId, TNode> parent)
    {
      _parents[parent.Id] = parent;
      parent._children[Id] = this;
    }

    public void CreateBindingBetween(TId parentId, NodeStorage<TValue, TVisitor, TId, TNode> nodeStorage)
    {
      var parentNode = nodeStorage.ObtainNode(parentId);
      parentNode.BindWithChild(this);
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

    public void RemoveFrom(IDirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph)
    {
      foreach (var parent in Parents)
      {
        directedAcyclicGraph.RemoveNode(this.Id, parent.Id);
      }

      foreach (var child in Children)
      {
        child.RemoveFrom(directedAcyclicGraph);
      }
    }

  }
}