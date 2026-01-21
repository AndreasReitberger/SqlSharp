# SqlSharp
A simple library to perform database actions.

# Usage

## ConnectionBuilder
You can create a connection using the `SqlClientConnectionBuilder` class. This will return a `SqlClientSharp` instance.

```csharp
string _server = "serverName";
string _databaseName = "databaseName";
string _user = "userName";
string _password = "pwd";
string _connectionString = string.Empty;

_connectionString = $"Data Source={_server};Initial Catalog={_databaseName};Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
client = new SqlClientSharp.SqlClientConnectionBuilder()
    .WithConnectionString(_connectionString)
    .WithCredentials(_user, _password)
    .Build();
```

## Executing Queries
You can execute queries using the `QueryCommandAsync` method. This method takes a SQL query as a string and returns the result as a `DataTable`.

```csharp
string cmd = $"select * from MyTableName;";
DataTable logs = await client.QueryCommandAsync(cmd).ConfigureAwait(false);
```

## Disposing
Once done, dispose the client to free up resources.

```csharp
client?.Dispose();
```