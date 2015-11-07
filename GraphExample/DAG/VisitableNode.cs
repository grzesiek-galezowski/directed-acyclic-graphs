using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

      private void BindWithChild(VisitableNode child)
      {
        Value.AssertNonTerminal();
        _children[child.Id] = child;
        child._parents[Id] = this;
      }

      public void BindWithParent(TId parentId, NodeStorage nodeStorage)
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

      public void RemoveDirectChild(TId childId, NodeStorage nodeStorage)
      {
        var child = _children[childId];
        child._parents.Remove(Id);
        _children.Remove(childId);

        if (child.HasNoParentsLeft())
        {
          child.RemoveFrom(nodeStorage);
        }
      }

      private bool HasNoParentsLeft()
      {
        return _parents.Count == 0;
      }

      public bool MatchesRootCondition()
      {
        return _parents.Count == 0;
      }

      public void RemoveFrom(NodeStorage nodeStorage) //bug use this instead of DAG instance
      {
        foreach (var parent in Parents)
        {
          RemoveAssociation(nodeStorage, parent.Id);
        }

        foreach (var child in Children)
        {
          child.RemoveFrom(nodeStorage);
        }

        if (!Parents.Any())
        {
          nodeStorage.Remove(this.Id);
        }
      }

      public void RemoveAssociation(NodeStorage nodeStorage, TId parentId)
      {
        if (!IsRoot())
        {
          var parentNode = nodeStorage.ObtainNode(parentId);
          parentNode.RemoveDirectChild(Id, nodeStorage);
        }
      }

      public bool IsRoot()
      {
        return _parents.Count == 0;
      }
    }
  }
}