# Ephemeral RabbitMQ Context

The paranoid.software.ephemerals.RabbitMQ library provides tools to manage and interact with ephemeral RabbitMQ virtual hosts. It's designed to simplify the process of setting up, using, and tearing down temporary RabbitMQ virtual hosts.

## Prerequisites

  - .NET environment compatible with the library.
  - Installed EasyNetQ.Management.Client package.

## Nuget Package

The package is available at Nuget Gallery at https://www.nuget.org/packages/paranoid.software.ephemerals.RabbitMQ/

## Quickstart

To use the library we must install it by using the nuget package manager:

```shell
Install-Package paranoid.software.ephemerals.RabbitMQ -Version x.x.x
```

Then we can reference the package on our test classes and start using it.

```csharp
using paranoid.software.ephemerals.RabbitMQ;

public class MyTestsClass {
  
  [Fact]
  public void MyTestMethod()
  {
    using var ctx = new EphemeralRabbitMqContextBuilder()
        .AddExchange("todo", "topic")
        .AddQueue("tasks")
        .AddQueueBinding("tasks", "todo", "tasks.with.priority.5")
        .PublishMessages("todo", "tasks.with.priority.5", new List<string> { "Mi first task" })
        .PublishMessage("todo", "tasks.with.priority.5", Encoding.UTF8.GetBytes("Another cool task"))
        .Build(_connectionParams);
    var vhostName = ctx.VhostName;
    // Perform our tests and asserts using the ephemeral vhost
  }
  
}
```

In the code shown above:

- EphemeralRabbitMqContextBuilder() // Will initialize the context builder.
- AddExchange("todo", "topic") // Will add the "todo" exchange.
- AddQueue("tasks") // Will add the "tasks" queue.
- PublishMessages(...) // Will publish a list of messages to the queue.
- PublishMessage(...) // Will publish another message to the queue.
- Build(_connectionParams); // Will create the vhost along with all the specified operations.

> When the ephemeral context is disposed the vhost will be DELETED.
