using System;
using NSubstitute;
using NUnit.Framework;

namespace DAG
{
  public class ExampleCode
  {
    [Test]
    public void Main()
    {
      var securityGroup = new SecurityGroup();
      var device = new Device();
      var visitor = Substitute.For<MyVisitor>();
      var groupNode = new VisitableNode<Entity, MyVisitor, int>(securityGroup, 1);
      var deviceNode = new VisitableNode<Entity, MyVisitor, int>(device, 2);
      groupNode.AddChild(deviceNode);

      groupNode.Accept(visitor);

      Received.InOrder(() =>
      {
        visitor.Accept(securityGroup);
        visitor.Accept(device);
      });
       
    }
  }

  public interface Entity :IVisitable<MyVisitor>
  {
    
  }

  public interface MyVisitor
  {
    void Accept(Device device);
    void Accept(SecurityGroup device);
  }

  public class ConcreteVisitor : MyVisitor
  {
    public void Accept(Device device)
    {
      Console.WriteLine("device");
    }

    public void Accept(SecurityGroup group)
    {
      Console.WriteLine("group");
    }
  }

  public class Device : Entity
  {
    public void Accept(MyVisitor visitor)
    {
      visitor.Accept(this);
    }

    public void AssertNonTerminal()
    {
      throw new TerminalNodeException();
    }
  }

  public class TerminalNodeException : Exception
  {
  }

  public class SecurityGroup : Entity
  {
    public void Accept(MyVisitor visitor)
    {
      visitor.Accept(this);
    }

    public void AssertNonTerminal()
    {
      
    }
  }

  
}
