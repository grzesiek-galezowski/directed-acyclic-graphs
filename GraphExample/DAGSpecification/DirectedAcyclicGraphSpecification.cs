using System;
using System.Collections.Generic;
using DAG;
using DAG.Interfaces;
using NSubstitute;
using NUnit.Framework;
using TddEbook.TddToolkit;
using TddEbook.TddToolkit.NSubstitute;

namespace DAGSpecification
{
  public class DirectedAcyclicGraphSpecification
  {
    private static DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.DirectedAcyclicGraph CreateGraph()
    {
      return CreateGraph(Any.Instance<GraphHooks<string>>());
    }

    private static DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.DirectedAcyclicGraph CreateGraph(GraphHooks<string> graphHooks)
    {
      DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.GraphStates graphStates = new DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.GraphStates(graphHooks);
      return new DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.DirectedAcyclicGraph(graphHooks, graphStates.Rootless);
    }

    [Test]
    public void ShouldTraverseNodeAndAllChildren()
    {
      //GIVEN
      var graph = CreateGraph();
      
      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(a1), nameof(police), a1);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      root.Received(1).Accept(anyVisitor);

      Received.InOrder(() =>
      {
        root.Accept(anyVisitor); 
        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });

      Received.InOrder(() =>
      {
        root.Accept(anyVisitor); 
        police.Accept(anyVisitor);
      });

      graph.AssertContainsOnly(
        Entry(nameof(root), root),
        Entry(nameof(police), police),
        Entry(nameof(fireforce), fireforce),
        Entry(nameof(a1), a1)
      );
    }

    [Test]
    public void ShouldTraverseNodeAddedToTwoDifferentParentsTwice()
    {
      //GIVEN
      var graph = CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(a1), nameof(police), a1);
      graph.AddNode(nameof(a1), nameof(fireforce), a1);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      a1.Received(2).Accept(anyVisitor);

      Received.InOrder(() =>
      {
        root.Accept(anyVisitor);

        fireforce.Accept(anyVisitor);
        a1.Accept(anyVisitor);

        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });

      graph.AssertContainsOnly(
        Entry(nameof(root), root),
        Entry(nameof(police), police),
        Entry(nameof(fireforce), fireforce),
        Entry(nameof(a1), a1)
      );

    }

    [Test]
    public void ShouldOverwritePreviousNodeWhenNewNodeWithTheSameIdIsAddedEvenWithDifferentParent()
    {
      //GIVEN
      var graph = CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var newA1 = Substitute.For<Device>();
      var visitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(a1), nameof(police), a1);
      graph.AddNode(nameof(a1), nameof(fireforce), newA1);

      //WHEN
      graph.AcceptStartingFromRoot(visitor);

      //THEN
      a1.DidNotReceiveWithAnyArgs().Accept(visitor);
      newA1.Received(2).Accept(visitor);
      graph.AssertContainsOnly(
        Entry(nameof(root), root),
        Entry(nameof(police), police),
        Entry(nameof(fireforce), fireforce),
        Entry(nameof(a1), newA1)
      );

    }

    private static KeyValuePair<string, IAuthorizationEntity> Entry(string name, IAuthorizationEntity root)
    {
      return new KeyValuePair<string, IAuthorizationEntity>(name, root);
    }

    [Test]
    public void ShouldRemoveOnlyTheNodeWithSpecificParentWhenNodeIsAddedWithTwoParents()
    {
      //GIVEN
      var graph = CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(a1), nameof(police), a1);
      graph.AddNode(nameof(a1), nameof(fireforce), a1);
      graph.RemoveAssociation(nameof(a1), nameof(fireforce));

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      a1.Received(1).Accept(anyVisitor);


      graph.AssertContainsOnly(
        Entry(nameof(root), root),
        Entry(nameof(police), police),
        Entry(nameof(fireforce), fireforce),
        Entry(nameof(a1), a1)
      );
    }

    [Test]
    public void ShouldAllowAddingTheSameNodeManyTimesAndTreatItAsSingleNode()
    {
      //GIVEN
      var graph = CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(police), nameof(root), police);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      police.Received(1).Accept(anyVisitor);

      //bug there should be a notification

      graph.AssertContainsOnly(
        Entry(nameof(root), root),
        Entry(nameof(police), police)
      );
    }

    [Test]
    public void ShouldNotifyObserverWhenRootNodeIsOverwritten()
    {
      //GIVEN
      var observer = Substitute.For<GraphHooks<string>>();
      var graph = CreateGraph(observer);

      var root = Substitute.For<Agency>();
      var root2 = Substitute.For<Agency>();

      //WHEN
      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(root2), null, root2);

      //THEN
      observer.Received(1).RootNodeOverwritten(nameof(root), nameof(root2));
      XReceived.Only(() => observer.RootNodeOverwritten(nameof(root), nameof(root2)));

      graph.AssertContainsOnly(
        Entry(nameof(root2), root2)
      );

    }

    [Test]
    public void ShouldNotUseOldRootWhenItIsOverwrittenWithTheSameId()
    {
      //GIVEN
      var graph = CreateGraph();

      var root = Substitute.For<Agency>();
      var root2 = Substitute.For<Agency>();
      var childOfRoot1 = Substitute.For<Group>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(childOfRoot1), nameof(root), childOfRoot1);
      graph.AddNode(nameof(root2), null, root2);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      root2.Received(1).Accept(anyVisitor);
      root.DidNotReceive().Accept(Arg.Any<IAuthorizationEntityVisitor>());

      graph.AssertContainsOnly(
        Entry(nameof(root2), root2)
      );
    }

    [Test]
    public void ShouldNotifyObserverWhenObserverIsPassedToEmptyGraph()
    {
      var graphHooks = Substitute.For<GraphHooks<string>>();
      var graph = CreateGraph(graphHooks);
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      graphHooks.Received(1).VisitorPassedToEmptyGraph();
    }

  }


  //TODO allow supplying policies w.g. what happens when someone adds second root
  //TODO operations on empty graph
  //TODO add operation RemoveAllNodes() without parent id
  //TODO removing subtrees
  //TODO removing an item that's not added
  //TODO removing item that's added but with wrong parent
  //TODO removing item that has parent correct but wrong id
  //TODO removing root node

  public interface Group : IAuthorizationEntity
  {
  }

  public interface Device : IAuthorizationEntity
  {
  }

  public interface Agency : IAuthorizationEntity
  {
  }


  public interface IAuthorizationEntityVisitor
  {
  }

  public interface IAuthorizationEntity : IVisitable<IAuthorizationEntityVisitor> 
  {
  }

}
