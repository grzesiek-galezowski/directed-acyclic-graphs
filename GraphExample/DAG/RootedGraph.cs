using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class RootedGraph : GraphState
    {
      private readonly GraphHooks<TId> _graphHooks;
      private readonly GraphStates _graphStates;

      public RootedGraph(GraphHooks<TId> graphHooks, GraphStates graphStates)
      {
        _graphHooks = graphHooks;
        _graphStates = graphStates;
      }

      public void SetRoot(GraphContext directedAcyclicGraph, TId id, TValue value)
      {
        var oldRootId = directedAcyclicGraph.RootId();
        directedAcyclicGraph.RemoveOldRoot(); //bug this removes current root if it has the same id!!!!!! - add test
        var node = directedAcyclicGraph.ObtainNode(id, value);
        _graphHooks.RootNodeOverwritten(oldRootId, node.Id);
      }

      public void AcceptStartingFromRoot(TVisitor visitor, NodeStorage nodeStorage)
      {
        nodeStorage.Root().Accept(visitor);
      }

      public void RemoveAssociation(GraphContext context, TId id, TId parentId, NodeStorage nodeStorage)
      {
        var node = nodeStorage.ObtainNode(id);
        var isRoot = node.IsRoot();
        node.RemoveAssociation(nodeStorage, parentId);
        if (isRoot)
        {
          context.SetGraphState(_graphStates.Rootless);
        }
      }

      public void AcceptStartingFrom(TId id, TVisitor visitor, NodeStorage nodeStorage)
      {
        var node = nodeStorage.ObtainNode(id);
        node.Accept(visitor);
      }
    }
  }
}