using System;

namespace DAG
{
  public class GraphStates<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor>
    where TId : class, IEquatable<TId>
  {
    private readonly GraphState<TValue, TVisitor, TId> _rootedGraph;
    private readonly GraphState<TValue, TVisitor, TId> _rootlessGraph;

    public GraphStates()
    {
      _rootedGraph = new RootedGraph<TValue, TVisitor, TId>();
      _rootlessGraph = new RootlessGraph<TValue, TVisitor, TId>();
    }

    public GraphState<TValue, TVisitor, TId> Rootless
    {
      get { return _rootlessGraph; }
    }

    public GraphState<TValue, TVisitor, TId> Rooted
    {
      get { return _rootedGraph; }
    }
  }
}