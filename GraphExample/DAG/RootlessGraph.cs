using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class RootlessGraph : GraphState
    {
      private readonly GraphHooks<TId> _graphHooks;
      private readonly GraphStates _graphStates;

      public RootlessGraph(GraphHooks<TId> graphHooks, GraphStates graphStates)
      {
        _graphHooks = graphHooks;
        _graphStates = graphStates;
      }

      public void SetRoot(DirectedAcyclicGraph directedAcyclicGraph, TId id, TValue value)
      {
        directedAcyclicGraph.Store(id, directedAcyclicGraph.NewNode(id, value));
        directedAcyclicGraph.SetGraphState(_graphStates.Rooted);
      }

      public void AcceptStartingFromRoot(TVisitor visitor, DirectedAcyclicGraph directedAcyclicGraph)
      {
        _graphHooks.VisitorPassedToEmptyGraph();
      }
    }
  }
}