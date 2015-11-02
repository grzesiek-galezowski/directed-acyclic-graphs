using System;
using DAG.Interfaces;

namespace DAG
{
  public class RootlessGraph<TValue, TVisitor, TId> :
    GraphState<TValue, TVisitor, TId>
    where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    private readonly GraphHooks<TId> _graphHooks;
    private readonly GraphStates<TValue, TVisitor, TId> _graphStates;

    public RootlessGraph(GraphHooks<TId> graphHooks, GraphStates<TValue, TVisitor, TId> graphStates)
    {
      _graphHooks = graphHooks;
      _graphStates = graphStates;
    }

    public void SetRoot(
      DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph, TId id, TValue value)
    {
      directedAcyclicGraph.Store(id, directedAcyclicGraph.NewNode(id, value));
      directedAcyclicGraph.SetGraphState(_graphStates.Rooted);
    }

    public void AcceptStartingFromRoot(TVisitor visitor, DirectedAcyclicGraph<TValue, TVisitor, TId> directedAcyclicGraph)
    {
      _graphHooks.VisitorPassedToEmptyGraph();
    }
  }
}