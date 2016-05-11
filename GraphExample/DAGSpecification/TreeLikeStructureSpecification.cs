using NUnit.Framework;

namespace DAGSpecification
{
  public class TreeLikeStructureSpecification
  {
    private TreeLikeStructureFixture _treeLikeStructureFixture;

    [SetUp]
    public void SetUp()
    {
      _treeLikeStructureFixture = new TreeLikeStructureFixture();
      _treeLikeStructureFixture.WhenIPassVisitorThroughTheWholeGraph();
    }

    [Test]
    public void RootShouldBeVisitedOnlyOnce()
    {
      _treeLikeStructureFixture.RootShouldBeVisitedOnlyOnce();
    }

    [Test]
    public void LeftSideShouldBeVisitedDownwards()
    {
      _treeLikeStructureFixture.LeftSideShouldBeVisitedDownwards();
    }

    [Test]
    public void RightSideShouldBeVisitedDownwards()
    {
      _treeLikeStructureFixture.RightSideShouldBeVisitedDownwards();
    }

    [Test]
    public void GraphShouldContainAllAddedNodes()
    {
      _treeLikeStructureFixture.GraphShouldContainAllAddedNodes();
    }

  }
}