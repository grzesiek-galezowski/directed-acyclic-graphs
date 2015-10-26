using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAG;
using NSubstitute;
using NSubstitute.Exceptions;
using NUnit.Framework;
using TddEbook.TddToolkit;
using TddEbook.TddToolkit.NSubstitute;

namespace DAGSpecification
{
  public class DirectedAcyclicGraphSpecification
  {
    private static DirectedAcyclicGraph<IAuthorizationEntity, IAuthorizationEntityVisitor, string> CreateDirectedAcyclicGraph()
    {
      return new DirectedAcyclicGraph<IAuthorizationEntity, IAuthorizationEntityVisitor, string>();
    }

    [Test]
    public void ShouldTraverseNodeAndAllChildren()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();
      
      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode("root", null, root);
      graph.AddNode("police", "root", police);
      graph.AddNode("fireforce", "root", fireforce);
      graph.AddNode("A1", "police", a1);

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
    }

    [Test]
    public void ShouldTraverseNodeAddedToTwoDifferentParentsTwice()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();

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

    }

    [Test]
    public void ShouldOverwritePreviousNodeWhenNewNodeWithTheSameIdIsAddedEvenWithDifferentParent()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();

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
    }

    [Test]
    public void ShouldRemoveOnlyTheNodeWithSpecificParentWhenNodeIsAddedWithTwoParents()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();

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
      graph.RemoveNode(nameof(a1), nameof(fireforce));

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      Received.InOrder(() =>
      {
        root.Accept(anyVisitor);
        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
        fireforce.Accept(anyVisitor);
      });
    }

    [Test]
    public void ShouldAllowAddingTheSameNodeManyTimesAndTreatItAsSingleNode()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();

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
    }

    [Test]
    public void ShouldNotifyObserverWhenRootNodeIsOverwritten()
    {
      //GIVEN
      var observer = Substitute.For<RootOverwriteObserver<string>>();
      var graph = CreateDirectedAcyclicGraph();
      graph.NotifyOnRootOverwrite(observer);

      var root = Substitute.For<Agency>();
      var root2 = Substitute.For<Agency>();

      //WHEN
      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(root2), null, root2);

      //THEN
      observer.Received(1).RootNodeOverwritten(nameof(root), nameof(root2));
      XReceived.Only(() => observer.RootNodeOverwritten(nameof(root), nameof(root2)));
    }

    [Test]
    public void ShouldNotUseOldRootWhenItIsOverwrittenWithTheSameId()
    {
      //GIVEN
      var graph = CreateDirectedAcyclicGraph();

      var root = Substitute.For<Agency>();
      var root2 = Substitute.For<Agency>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode(nameof(root), null, root);
      graph.AddNode(nameof(root2), null, root2);

      //WHEN
      graph.AcceptStartingFromRoot(anyVisitor);

      //THEN
      root2.Received(1).Accept(anyVisitor);
      root.DidNotReceive().Accept(Arg.Any<IAuthorizationEntityVisitor>());
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
