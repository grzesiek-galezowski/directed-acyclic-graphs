using System;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId>
  {
    public class RootlessGraph : GraphState
    {
      readonly GraphHooks<TId> _graphHooks;
      readonly GraphStates _graphStates;
      readonly Func<TId, TValue, VisitableNode> _nodeFactory;

      public RootlessGraph(GraphHooks<TId> graphHooks, GraphStates graphStates, Func<TId, TValue, VisitableNode> nodeFactory)
      {
        _graphHooks = graphHooks;
        _graphStates = graphStates;
        _nodeFactory = nodeFactory;
      }

      public void SetRoot(GraphContext directedAcyclicGraph, TId id, TValue value)
      {
        directedAcyclicGraph.Store(id, _nodeFactory.Invoke(id, value));
        directedAcyclicGraph.SetGraphState(_graphStates.Rooted);
      }

      public void AcceptStartingFromRoot(TVisitor visitor, NodeStorage nodeStorage)
      {
        _graphHooks.VisitorPassedToEmptyGraph();
      }

      public void RemoveAssociation(GraphContext context, TId id, TId parentId, NodeStorage nodeStorage)
      {
        _graphHooks.TriedToRemoveAssociationFromEmptyGraph(id, parentId);
      }

      public void AcceptStartingFrom(TId id, TVisitor visitor, NodeStorage nodeStorage)
      {
        _graphHooks.VisitorPassedToEmptyGraph(id);
      }
    }
  }

  public class CouldNotRemoveAssociationFromRootlessGraphException<T> : Exception
  {
    public CouldNotRemoveAssociationFromRootlessGraphException(T id, T parentId) :
      base("Could not remove association with ID: "+ id +" and parent ID: " + parentId + "from rootless graph")
    {
      
    }

    public CouldNotRemoveAssociationFromRootlessGraphException(T id)
      : base("Could not remove association with ID: "+ id +" and from rootless graph")
    {
      
    }
  }
}