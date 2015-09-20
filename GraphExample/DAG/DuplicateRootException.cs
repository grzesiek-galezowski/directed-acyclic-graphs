using System;

namespace DAG
{
  public class DuplicateRootException : Exception
  {
    private readonly string _id;
    private readonly string _existingId;

    public static DuplicateRootException Create<TId>(TId id1, TId id2)
    {
      return new DuplicateRootException(id1.ToString(), id2.ToString());
    }

    public DuplicateRootException(string id, string existingId)
      : base("Trying to add root node " + id + " but there was an existing root: " + existingId)
    {
    }
  }

  public class BoundNodeOverwriteException : Exception
  {
    
  }
}