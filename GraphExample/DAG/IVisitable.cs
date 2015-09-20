namespace DAG
{
  public interface IVisitable<TVisitor>
  {
    void Accept(TVisitor visitor);
    void AssertNonTerminal();
  }
}