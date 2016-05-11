using System;
using System.Collections.Generic;
using DAG.Interfaces;
using TddEbook.TddToolkit;
using static DAG.DirectedAcyclicGraphs<DAGSpecification.IAuthorizationEntity,DAGSpecification.IAuthorizationEntityVisitor,string>;

namespace DAGSpecification
{
  static internal class GraphRoot
  {
    public static DirectedAcyclicGraph CreateGraph()
    {
      return CreateGraph(Any.Instance<GraphHooks<string>>());
    }

    public static DirectedAcyclicGraph CreateGraph(GraphHooks<string> graphHooks)
    {
      var nodeFactory = CreateNodeFactory();
      return new DirectedAcyclicGraph(CreateGraphStates(graphHooks, nodeFactory).Rootless, CreateNodeStorage(nodeFactory));
    }

    private static GraphStates CreateGraphStates(GraphHooks<string> graphHooks, Func<string, IAuthorizationEntity, VisitableNode> nodeFactory)
    {
      return new GraphStates(graphHooks, nodeFactory);
    }

    private static Func<string, IAuthorizationEntity, VisitableNode> CreateNodeFactory()
    {
      return (id, value) => new VisitableNode(id, value);
    }

    private static NodeStorage CreateNodeStorage(Func<string, IAuthorizationEntity, VisitableNode> nodeFactory)
    {
      return new NodeStorage(new SortedDictionary<string, VisitableNode>(), nodeFactory);
    }

    public static KeyValuePair<string, IAuthorizationEntity> Entry(string name, IAuthorizationEntity root)
    {
      return new KeyValuePair<string, IAuthorizationEntity>(name, root);
    }
  }
}