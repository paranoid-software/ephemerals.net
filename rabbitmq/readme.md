# Ephemeral Redis Context

The paranoid.software.ephemerals.Redis library provides tools to manage and interact with ephemeral Redis databases. It's designed to simplify the process of setting up, using, and tearing down temporary Redis instances.

## Prerequisites

  - .NET environment compatible with the library.
  - Installed StackExchange.Redis package.

## Nuget Package

The package is available at Nuget Gallery at https://www.nuget.org/packages/paranoid.software.ephemerals.Redis/

## Quickstart

To use the library we must install it by using the nuget package manager:

```shell
Install-Package paranoid.software.ephemerals.Redis -Version x.x.x
```

Then we can reference the package on our test classes and start using it.

```csharp
using paranoid.software.ephemerals.Redis;

public class MyTestsClass {
  
  [Fact]
  public void MyTestMethod()
  {
    using var ctx = new EphemeralRedisDbContextBuilder()
        .AddScriptSentences(new List<string>
        {
            "SET 1 Andres",
            "LPUSH 2 A B C",
            "SADD 3 Quito Guayaquil Manta"
        })
        .Build("localhost,password=pwd");
    var dbNumber = DatabaseNumber;
    // Perform our tests and asserts using the locked database
  }
  
}
```

In the code shown above:

- EphemeralRedisDbContextBuilder(); // Will initialize the context builder.
- AddScriptSentences(new List<string> ...); // Will add the given keys with its corresponding values.
- Build("localhost,password=pwd"); // Will lock a database on the server, execute the creation ops, and return an ephemeral DB context instance with the chosen database number.

> When the ephemeral DB context is disposed the locked database will be RELEASED.

You can also add multiple script sentences or load them from a file:

```csharp
using paranoid.software.ephemerals.Redis;

public class MyTestsClass {
  
  [Fact]
  public void MyTestMethod()
  {
    using var ctx = new EphemeralRedisDbContextBuilder()
        .AddScriptSentencesFromFile("path_to_script_file")
        .Build("localhost,password=pwd");
    var dbNumber = DatabaseNumber;
    // Perform our tests and asserts using the locked database
  }
  
}
```
