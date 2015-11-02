using System;
using System.Collections.Generic;
using System.Linq;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class NodeStorage
    {
      private readonly IDictionary<TId, VisitableNode> _state;
      private readonly Func<TId, TValue, VisitableNode> _nodeFactory;

      public NodeStorage(IDictionary<TId, VisitableNode> state, Func<TId, TValue, VisitableNode> nodeFactory)
      {
        _state = state;
        _nodeFactory = nodeFactory;
      }

      public void Store(TId id, VisitableNode node)
      {
        _state[id] = node;
      }

      public void ModifyExistingNode(TId id, TValue value)
      {
        var node = _state[id];
        node.Value = value;
      }


      public void SetNode(TId id, TValue value)
      {
        if (_state.ContainsKey(id))
        {
          //this is not to lose the bound children and parents
          ModifyExistingNode(id, value);
        }
        else
        {
          Store(id, _nodeFactory.Invoke(id, value));
        }
      }

      public VisitableNode ObtainNode(TId id, TValue value)
      {
        SetNode(id, value);
        return _state[id];
      }

      public VisitableNode ObtainNode(TId parentId)
      {
        return _state[parentId];
      }

      public void Remove(TId id)
      {
        _state.Remove(id);
      }

      public VisitableNode Root()
      {
        return _state.First(n => n.Value.MatchesRootCondition()).Value;
      }

      public void AssertContainsOnly(KeyValuePair<TId, TValue>[] elements)
      {
        var currentNodes = _state.Select(n => new KeyValuePair<TId, TValue>(n.Key, n.Value.Value)).ToList();
        var intersect = currentNodes.Except(elements).ToArray();
        if (intersect.Length != 0)
        {
          throw new Exception("There are " + intersect.Length + " non-intersecting elements: <" + DirectedAcyclicGraph.FormatPairs(intersect) +
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