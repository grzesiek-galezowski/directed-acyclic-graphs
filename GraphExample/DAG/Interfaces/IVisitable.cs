namespace DAG.Interfaces
{
  public interface IVisitable<in TVisitor>
  {
    void Accept(TVisitor visitor);
    void AssertNonTerminal();
  }
}