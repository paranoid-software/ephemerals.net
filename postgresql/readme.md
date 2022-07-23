# Ephemeral PostgreSql DB Context

C# library to allow  databases integration tests easy coding.

## Nuget Package

The package is available at Nuget Gallery at https://www.nuget.org/packages/paranoid.software.ephemerals.PostgreSql/

## Quickstart

To use the library we must install it by using the nuget package manager:

```shell
Install-Package paranoid.software.ephemerals.PostgreSql -Version x.x.x
```

Then we can reference the package on our test classes and start using it.

```csharp
using paranoid.software.ephemerals.PostgreSql;

public class MyTestsClass {
  
  [Fact]
  public void MyTestMethod()
  {
    var connectionString = "Server=localhost;Port=5432;User Id=my-user;Password=my-New-pwd;";
    using var context = new EphemeralPostrgreSqlDbContext()
                    .AddScriptFromFile("table-one-creation-script.sql")
                    .AddScriptFromFile("table-one-mocking-data-insertion-script.sql")
                    .Build(connectionString);
    var ephemeralDbName = context.DbName;
    // Perform our tests and asserts using the provisioned database
  }
  
}
```

In the code shown above:

- EphemeralPostgreSqlDbContext(); // Will initialize the context builder.
- AddScriptFromFile("table-one-creation-script.sql"); // Will set a table creation script if necessary (this line is optional)
- AddScriptFromFile("table-one-mocking-data-insertion-script.sql"); // Will set a data insertion script if necessary (this is also optional)
- Build(connectionString); // Will create a database on the server under a unique random name, execute the added scripts, and return an ephemeral DB context instance with the generated database name.

When the ephemeral DB context is disposed the created database will be DROPPED.
