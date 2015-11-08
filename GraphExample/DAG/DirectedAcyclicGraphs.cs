using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAG.Interfaces;

namespace DAG
{
  public partial class DirectedAcyclicGraphs<TValue, TVisitor, TId> where TValue : IVisitable<TVisitor> where TId : class, IEquatable<TId>
  {
  }
}
