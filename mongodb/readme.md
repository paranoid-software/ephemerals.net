# Ephemeral MongoDB Context

C# library to allow  databases integration tests easy coding.

## Nuget Package

The package is available at Nuget Gallery at https://www.nuget.org/packages/paranoid.software.ephemerals.MongoDB/

## Quickstart

To use the library we must install it by using the nuget package manager:

```shell
Install-Package paranoid.software.ephemerals.MongoDB -Version x.x.x
```

Then we can reference the package on our test classes and start using it.

```csharp
using paranoid.software.ephemerals.MongoDB;

public class MyTestsClass {
  
  [Fact]
  public void MyTestMethod()
  {
    using var ctx = new EphemeralMongoDbContextBuilder()
        .AddItems("books", new List<dynamic>
        {
            new {name="Pinocchio"}
        })
        .Build(new ConnectionParams{ HostName = "localhost", PortNumber = 27017, Username = "root", Password = "pwd", IsDirectConnection = true});
    var dbName = ctx.DbName;
    // Perform our tests and asserts using the provisioned database
  }
  
}
```

In the code shown above:

- EphemeralMongoDbContextBuilder(); // Will initialize the context builder.
- AddItems("books", ..); // Will set the books collection creation operation if necessary (this line is optional)
- Build(new ConnectionParams{ ...); // Will create a database on the server under a unique random name, execute the creation ops, and return an ephemeral DB context instance with the generated database name.

When the ephemeral DB context is disposed the created database will be DROPPED.
