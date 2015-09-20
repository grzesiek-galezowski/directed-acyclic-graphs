using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAG;
using NSubstitute;
using NUnit.Framework;

namespace DAGSpecification
{
  public class DirectedAcyclicGraphSpecification
  {
    [Test]
    public void ShouldTraverseNodeAndAllChildren()
    {
      //GIVEN
      var graph = new DirectedAcyclicGraph<IAuthorizationEntity, IAuthorizationEntityVisitor, string>();
      
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
      graph.Accept(anyVisitor);

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
      var graph = new DirectedAcyclicGraph<IAuthorizationEntity, IAuthorizationEntityVisitor, string>();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode("root", null, root);
      graph.AddNode("police", "root", police);
      graph.AddNode("fireforce", "root", fireforce);
      graph.AddNode("A1", "police", a1);
      graph.AddNode("A1", "fireforce", a1);

      //WHEN
      graph.Accept(anyVisitor);

      //THEN
      a1.Received(2).Accept(anyVisitor);

      Received.InOrder(() =>
      {
        root.Accept(anyVisitor);
        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
        fireforce.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });
    }


    [Test]
    public void ShouldRemoveOnlyTheNodeWithSpecificParentWhenNodeIsAddedWithTwoParents()
    {
      //GIVEN
      var graph = new DirectedAcyclicGraph<IAuthorizationEntity, IAuthorizationEntityVisitor, string>();

      var root = Substitute.For<Agency>();
      var police = Substitute.For<Group>();
      var fireforce = Substitute.For<Group>();
      var a1 = Substitute.For<Device>();
      var anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      graph.AddNode("root", null, root);
      graph.AddNode("police", "root", police);
      graph.AddNode("fireforce", "root", fireforce);
      graph.AddNode("A1", "police", a1);
      graph.AddNode("A1", "fireforce", a1);
      graph.RemoveNode("fireforce");

      //WHEN
      graph.Accept(anyVisitor);

      //THEN
      a1.Received(2).Accept(anyVisitor);

      Received.InOrder(() =>
      {
        root.Accept(anyVisitor);
        police.Accept(anyVisitor);
        a1.Accept(anyVisitor);
        fireforce.Accept(anyVisitor);
        a1.Accept(anyVisitor);
      });
    }

  }


  //TODO adding the same edge twice - what should happen? adding duplicated node
  //TODO cannot add more than one root
  //TODO operations on empty tree
  //TODO add operation RemoveAllNodes() without parent id
  //TODO removing subtrees

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
