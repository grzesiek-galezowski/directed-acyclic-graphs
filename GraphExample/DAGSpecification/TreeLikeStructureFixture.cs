using DAG;
using NSubstitute;
using static DAGSpecification.GraphRoot;

namespace DAGSpecification
{
  public class TreeLikeStructureFixture
  {
    private readonly Agency _root;
    private readonly Group _police;
    private readonly Group _fireforce;
    private readonly Device _radio;
    private readonly IAuthorizationEntityVisitor _anyVisitor;
    private readonly DirectedAcyclicGraphs<IAuthorizationEntity, IAuthorizationEntityVisitor, string>.DirectedAcyclicGraph _graph;

    public TreeLikeStructureFixture()
    {
      _root = Substitute.For<Agency>();
      _police = Substitute.For<Group>();
      _fireforce = Substitute.For<Group>();
      _radio = Substitute.For<Device>();
      _anyVisitor = Substitute.For<IAuthorizationEntityVisitor>();

      _graph = CreateGraph();
      _graph.AddNode(nameof(_root), null, _root);
      _graph.AddNode(nameof(_police), nameof(_root), _police);
      _graph.AddNode(nameof(_fireforce), nameof(_root), _fireforce);
      _graph.AddNode(nameof(_radio), nameof(_police), _radio);

    }

    public void GraphShouldContainAllAddedNodes()
    {
      _graph.AssertContainsOnly(
        Entry(nameof(_root), _root),
        Entry(nameof(_police), _police),
        Entry(nameof(_fireforce), _fireforce),
        Entry(nameof(_radio), _radio)
        );
    }

    public void RightSideShouldBeVisitedDownwards()
    {
      Received.InOrder(() =>
      {
        _root.Accept(_anyVisitor);
        _police.Accept(_anyVisitor);
      });
    }

    public void LeftSideShouldBeVisitedDownwards()
    {
      Received.InOrder(() =>
      {
        _root.Accept(_anyVisitor);
        _police.Accept(_anyVisitor);
        _radio.Accept(_anyVisitor);
      });
    }

    public void RootShouldBeVisitedOnlyOnce()
    {
      _root.Received(1).Accept(_anyVisitor);
    }

    public void WhenIPassVisitorThroughTheWholeGraph()
    {
      _graph.AcceptStartingFromRoot(_anyVisitor);
    }
  }
}