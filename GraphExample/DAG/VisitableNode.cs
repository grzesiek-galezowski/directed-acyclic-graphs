using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class VisitableNode
    {
      private readonly IDictionary<TId, VisitableNode> _children =
        new SortedDictionary<TId, VisitableNode>();

      private readonly IDictionary<TId, VisitableNode> _parents =
        new SortedDictionary<TId, VisitableNode>();

      public VisitableNode(TId id, TValue value)
      {
        Value = value;
        Id = id;
      }

      public TId Id { get; private set; }
      public TValue Value { get; set; }

      public ImmutableHashSet<VisitableNode> Parents
      {
        get { return _parents.Values.ToImmutableHashSet(); }
      }

      public ImmutableHashSet<VisitableNode> Children
      {
        get { return _children.Values.ToImmutableHashSet(); }
      }

      public void BindWithChild(VisitableNode child)
      {
        Value.AssertNonTerminal();
        _children[child.Id] = child;
        child._parents[Id] = this;
      }

      public void AddParent(VisitableNode parent)
      {
        _parents[parent.Id] = parent;
        parent._children[Id] = this;
      }

      public void CreateBindingBetween(TId parentId, NodeStorage nodeStorage)
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

      public void RemoveFrom(IDirectedAcyclicGraph directedAcyclicGraph)
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
}