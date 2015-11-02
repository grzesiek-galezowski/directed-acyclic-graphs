using System;
using System.Collections.Generic;
using System.Linq;
using DAG.Interfaces;
using NSubstitute.Exceptions;

namespace DAG
{
  public abstract partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class DirectedAcyclicGraph : IDirectedAcyclicGraph
    {
      private readonly IDictionary<TId, VisitableNode> _nodes = new Dictionary<TId, VisitableNode>();
      private readonly GraphHooks<TId> _graphHooks;
      private GraphState _currentGraphState;
      private readonly NodeStorage _nodeStorage;
      private readonly Func<TId, TValue, VisitableNode> _nodeFactory;

      public DirectedAcyclicGraph(GraphHooks<TId> graphHooks, GraphState currentGraphState)
      {
        _graphHooks = graphHooks;
        _currentGraphState = currentGraphState;
        _nodeFactory = (id, value) => new VisitableNode(id, value);
        _nodeStorage = new NodeStorage(_nodes, _nodeFactory);
      }

      public void AddNode(TId id, TId parentId, TValue value)
      {
        if (IsParentIdOfRoot(parentId))
        {
          _currentGraphState.SetRoot(this, id, value);
        }
        else
        {
          var node = _nodeStorage.ObtainNode(id, value);
          //bug when replacing node, remove its subtree
          node.CreateBindingBetween(parentId, _nodeStorage);
        }

        //bug this implementation is wrong - there is no test that verifies the dictionary itself after the addition
      }

      public void SetGraphState(GraphState currentGraphState)
      {
        _currentGraphState = currentGraphState;
      }

      public void Store(TId id, VisitableNode node)
      {
        _nodeStorage.Store(id, node);
      }

      private static bool IsParentIdOfRoot(TId parentId)
      {
        return parentId == null;
      }

      public VisitableNode ObtainNode(TId id, TValue value)
      {
        return _nodeStorage.ObtainNode(id, value);
      }

      public void RemoveOldRoot()
      {
        var root = Root();
        root.RemoveFrom(this);
      }

      public void RemoveNode(TId id, TId parentId)
      {
        var parentNode = _nodes[parentId];
        RemoveChild(parentNode, id);
      }

      private void RemoveChild(VisitableNode parentNode, TId id)
      {
        //bug removing a child that does not exist
        var node = parentNode.Children.First(n => n.Id == id);
        parentNode.RemoveDirectChild(id);
        if (!node.Parents.Any())
        {
          //bug test this one!
          _nodes.Remove(id);
        }
      }

//bug this implementation is wrong - the node is always removed from _nodes even when it occurs many times in the graph

      public void AcceptStartingFromRoot(TVisitor visitor)
      {
        _currentGraphState.AcceptStartingFromRoot(visitor, this);
      }

      public VisitableNode Root()
      {
        return _nodes.First(n => n.Value.MatchesRootCondition()).Value;
      }



      public VisitableNode NewNode(TId id, TValue value)
      {
        return _nodeFactory.Invoke(id, value);
      }

      public TId RootId()
      {
        return Root().Id;
      }

      public void AssertContainsOnly(params KeyValuePair<TId, TValue>[] elements)
      {
        AssertContainsOnly(elements, _nodes);
      }

      private static void AssertContainsOnly(KeyValuePair<TId, TValue>[] elements,
        IDictionary<TId, VisitableNode> visitableNodes)
      {
        var currentNodes = visitableNodes.Select(n => new KeyValuePair<TId, TValue>(n.Key, n.Value.Value)).ToList();
        var intersect = currentNodes.Except(elements).ToArray();
        if (intersect.Length != 0)
        {
          throw new Exception("There are " + intersect.Length + " non-intersecting elements: <" + FormatPairs(intersect) +
                              ">");
        }

        var intersect2 = elements.Except(currentNodes).ToArray();
        if (intersect2.Length != 0)
        {
          throw new Exception("There are " + intersect2.Length + " non-intersecting elements: <" +
                              FormatPairs(intersect2) + ">");
        }
      }

      private static string FormatPairs(IEnumerable<KeyValuePair<TId, TValue>> keyValuePairs)
      {
        return keyValuePairs.Aggregate("", (s, pair) => s + "[" + pair.Key + "=>" + pair.Value + "]");
      }
    }
  }

}