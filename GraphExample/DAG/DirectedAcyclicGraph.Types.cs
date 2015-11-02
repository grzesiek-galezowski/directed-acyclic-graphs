using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraph<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
    public class VisitableNode : VisitableNode<TValue, TVisitor, TId, VisitableNode>
    {
      public VisitableNode(TId id, TValue value) :
        base(value, id)
      {
      }


    }

    public interface IVisitable : IVisitable<TVisitor>
    {

    }

    public class NodeStorage : NodeStorage<TValue, TVisitor, TId, VisitableNode>
    {
      public NodeStorage(IDictionary<TId, VisitableNode> state, Func<TId, TValue, VisitableNode> nodeFactory)
        : base(state, nodeFactory)
      {
      }


    }
  }
}
