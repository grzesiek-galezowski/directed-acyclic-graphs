using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class GraphStates
    {
      private readonly Func<TId, TValue, VisitableNode> _nodeFactory;
      private readonly GraphState _rootedGraph;
      private readonly GraphState _rootlessGraph;

      public GraphStates(GraphHooks<TId> graphHooks, Func<TId, TValue, VisitableNode> nodeFactory)
      {
        _nodeFactory = nodeFactory;
        _rootedGraph = new RootedGraph(graphHooks, this);
        _rootlessGraph = new RootlessGraph(graphHooks, this, nodeFactory);
      }

      public GraphState Rootless
      {
        get { return _rootlessGraph; }
      }

      public GraphState Rooted
      {
        get { return _rootedGraph; }
      }
    }
  }
}