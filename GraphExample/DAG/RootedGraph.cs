using System;
using DAG.Interfaces;

namespace DAG
{
  public class RootedGraph<TValue, TVisitor, TId> : 
    GraphState<TValue, TVisitor, TId> 
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    private readonly GraphHooks<TId> _graphHooks;
    private readonly GraphStates<TValue, TVisitor, TId> _graphStates;

    public RootedGraph(GraphHooks<TId> graphHooks, GraphStates<TValue, TVisitor, TId> graphStates)
    {
      _graphHooks = graphHooks;
      _graphStates = graphStates;
    }

    public void SetRoot(
      DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, TId id, TValue value)
    {
      var oldRootId = directedAcyclicGraph.RootId();
      directedAcyclicGraph.RemoveOldRoot(); //bug this removes current root if it has the same id!!!!!! - add test
      var node = directedAcyclicGraph.ObtainNode(id, value);
      _graphHooks.RootNodeOverwritten(oldRootId, node.Id);
    }

    public void AcceptStartingFromRoot(TVisitor visitor, DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph)
    {
      directedAcyclicGraph.Root().Accept(visitor);
    }
  }
}