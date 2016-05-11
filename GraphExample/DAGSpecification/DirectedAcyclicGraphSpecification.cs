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
    

    [Test]
    public void ShouldTraverseNodeAddedToTwoDifferentParentsTwice()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(a1), nameof(fireforce), a1);
      graph.AddNode(nameof(a1), nameof(police), a1);

      //WHEN
      graph.AcceptStartingFrom(nameof(police), anyVisitor);

      //THEN
      Received.InOrder(() =>
      {
        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });

      fireforce.ClearReceivedCalls(); a1.ClearReceivedCalls(); //TODO move to per-context

      //WHEN
      graph.AcceptStartingFrom(nameof(fireforce), anyVisitor);

      //THEN
      Received.InOrder(() =>
      {
        fireforce.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });

      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(fireforce), fireforce), GraphRoot.Entry(nameof(police), police), GraphRoot.Entry(nameof(a1), a1)
      );
    }

    [Test]
    public void ShouldOverwritePreviousNodeWhenNewNodeWithTheSameIdIsAddedEvenWithDifferentParent()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

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
      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(police), police), GraphRoot.Entry(nameof(fireforce), fireforce), GraphRoot.Entry(nameof(a1), newA1)
      );

    }

    [Test]
    public void ShouldRemoveOnlyTheNodeWithSpecificParentWhenNodeIsAddedWithTwoParents()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

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


      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(police), police), GraphRoot.Entry(nameof(fireforce), fireforce), GraphRoot.Entry(nameof(a1), a1)
      );
    }

    [Test]
    public void ShouldRemoveTheNodeCompletelyAlongWithSubtreeWhenAllNodeAssociationsAreRemoved()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(fireforce), nameof(root), fireforce);
      graph.AddNode(nameof(police), nameof(root), police);
      graph.AddNode(nameof(police), nameof(fireforce), police);
      graph.AddNode(nameof(a1), nameof(police), a1);
      graph.RemoveAssociation(nameof(police), nameof(fireforce));
      graph.RemoveAssociation(nameof(police), nameof(root));

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      a1.DidNotReceive().Accept(anyVisitor);
      police.DidNotReceive().Accept(anyVisitor);


      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(fireforce), fireforce)
      );
    }

    [Test]
    public void ShouldThrowNodeNotFoundExceptionWhenTryingToRemoveNodeThatDoesNotExist()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();
      var root = Substitute.For<Agency>();

      graph.AddNode(nameof(root), null, root);


      //WHEN - THEN
      Assert.Throws<NodeNotFoundException<string>>(() =>
        graph.RemoveAssociation(Any.String(), nameof(root)));

    }

    [Test]
    public void ShouldNotifyWhenTryingToRemoveAssociationFromRootlessGraph()
    {
      //GIVEN
      var hooks = Substitute.For<GraphHooks<string>>();
      var graph = GraphRoot.CreateGraph(hooks);
      var id = Any.String();
      var parentId = Any.String();

      //WHEN
      graph.RemoveAssociation(id, parentId);

      //THEN
      hooks.Received(1).TriedToRemoveAssociationFromEmptyGraph(id, parentId);
    }

    [Test]
    public void ShouldNotifyWhenTryingToPassVisitorStartingFromArbitraryNodeToEmptyGraph()
    {
      //GIVEN
      var hooks = Substitute.For<GraphHooks<string>>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();
      var graph = GraphRoot.CreateGraph(hooks);
      var id = Any.String();

      //WHEN
      graph.AcceptStartingFrom(id, anyVisitor);

      //THEN
      hooks.Received(1).VisitorPassedToEmptyGraph(id);
    }

    [Test]
    public void ShouldAllowAddingTheSameNodeManyTimesAndTreatItAsSingleNode()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

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

      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(police), police)
      );
    }

    [Test]
    public void ShouldNotifyObserverWhenRootNodeIsOverwritten()
    {
      //GIVEN
      var observer = Substitute.For<GraphHooks<string>>();
      var graph = GraphRoot.CreateGraph(observer);

      var root = Substitute.For<Agency>();
      var root2 = Substitute.For<Agency>();

      //WHEN
      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(root2), null, root2);

      //THEN
      observer.Received(1).RootNodeOverwritten(nameof(root), nameof(root2));
      XReceived.Only(() => observer.RootNodeOverwritten(nameof(root), nameof(root2)));

      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root2), root2)
      );

    }

    [Test]
    public void ShouldNotUseOldRootWhenItIsOverwrittenWithTheSameId()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

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

      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root2), root2)
      );
    }

    [Test]
    public void ShouldNotifyObserverWhenObserverIsPassedToEmptyGraph()
    {
      var graphHooks = Substitute.For<GraphHooks<string>>();
      var graph = GraphRoot.CreateGraph(graphHooks);
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      graphHooks.Received(1).VisitorPassedToEmptyGraph();
    }

    [Test]
    public void ShouldNotifyObserverWhenObserverIsPassedToGraphWhereRootWasRemoved()
    {
      var graphHooks = Substitute.For<GraphHooks<string>>();
      var graph = GraphRoot.CreateGraph(graphHooks);
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();
      var root = Substitute.For<Agency>();

      graph.AddNode(nameof(root), null, root);
      graph.RemoveAssociation(nameof(root), null);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      graphHooks.Received(1).VisitorPassedToEmptyGraph();
    }


    [Test]
    public void ShouldThrowNodeNotFoundExceptionWhenAddingNodeToParentThatDoesNotExist()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph(Any.Instance<GraphHooks<string>>());

      //WHEN - THEN
      Assert.Throws<NodeNotFoundException<string>>(() =>
        graph.AddNode(Any.String(), Any.String(), Any.Instance<IAuthorizationEntity>())
      );
    }

    [Test]
    //TODO move this to per-context, this test is badly written
    public void ShouldThrowNodeNotFoundExceptionWhenRemovingAssociationWithWrongParent()
    {
      //GIVEN
      var graph = GraphRoot.CreateGraph();

      var root = Substitute.For<Agency>();
      var childOfRoot1 = Substitute.For<Group>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(childOfRoot1), nameof(root), childOfRoot1);

      //WHEN - THEN
      Assert.Throws<NodeNotFoundException<string>>( () =>
        graph.RemoveAssociation(nameof(childOfRoot1), Any.StringOtherThan(nameof(root)))
      );

      //WHEN - THEN
      graph.AssertContainsOnly(GraphRoot.Entry(nameof(root), root), GraphRoot.Entry(nameof(childOfRoot1), childOfRoot1)
      );

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      Received.InOrder(() =>
      {
        root.Accept(anyVisitor);
        childOfRoot1.Accept(anyVisitor);
      });


    }

  }

  //TODO visiting from chosen node
  //TODO od zaraz - different implementations when there is no root
  //TODO change state to rootless when root is removed
  //TODO operations on empty graph
  //TODO add operation RemoveAllNodes() without parent id
  //TODO removing subtrees
  //TODO removing an item that's not added
  //TODO removing item that's added but with wrong parent
  //TODO removing item that has parent correct but wrong id
  //TODO removing root node
  //TODO adding node with itself as parent
  //TODO detecting cycles

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
