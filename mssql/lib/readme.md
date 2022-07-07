# Ephemeral MsSql DB Context

C# library to allow easy coding of databases integration tests.

## Nuget Package

The package is available at Nuget Gallery at https://www.nuget.org/packages/paranoid.software.ephemerals.MsSql/

## Quickstart

To use the library first we install it by using the nuget package manager:

```shell
Install-Package paranoid.software.ephemerals.MsSql -Version 1.0.2
```

Then we can include the package on our test classes and start using it. Lets say we need to test some queries to our database:

```csharp
using paranoid.software.ephemerals.MsSql;

public class MyTestsClass {
  [Fact]
  public void MyTestMethod()
  {
    var connectionString = "Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;";
    using var context = new EphemeralMsSqlDbContext(connectionString)
                    .AddScriptFromFile("table-creation-script.sql")
                    .AddScriptFromFile("table-mock-data.sql")
                    .Build();
    var dbName = context.DbName;  
    // Perform our tests and asserts using the provisioned database
  }
}
```

In the code shown above:

- EphemeralMsSqlDbContext(connectionString); // Will set the connection string.
- AddScriptFromFile("table-creation-script.sql"); // Will set a table creation script if necessary (this line is optional)
- AddScriptFromFile("table-mock-data.sql"); // Will set a data insertion script if necessary (this is also optional)
- Build(); // Will create a database on the server under a unique random name, execute the scripts specified on every AddScriptFromFile methods, and return an ephemeral DB context instance with the generated DatabaseName.

When the ephemeral DB context is disposed the created database will be DROPPED.
