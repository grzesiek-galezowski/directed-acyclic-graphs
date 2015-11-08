using DAG.Interfaces;

namespace DAG
{
  public class NullObserver<T> : GraphHooks<T>
  {
    public void RootNodeOverwritten(T oldRootName, T newRootName)
    {
      // Method intentionally left empty.
    }

    public void VisitorPassedToEmptyGraph()
    {
      // Method intentionally left empty.
    }

    public void TriedToRemoveAssociationFromEmptyGraph(T id, T parentId)
    {
      // Method intentionally left empty.
    }

    public void VisitorPassedToEmptyGraph(T id)
    {
      // Method intentionally left empty.
    }
  }
}