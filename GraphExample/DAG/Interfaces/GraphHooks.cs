namespace DAG.Interfaces
{
  public interface GraphHooks<in T>
  {
    void RootNodeOverwritten(T oldRootId, T newRootId);
    void VisitorPassedToEmptyGraph();
  }
}