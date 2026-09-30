using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

var connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=P@ssw0rd!;TrustServerCertificate=true";
await using var connection = new SqlConnection(connectionString);

await connection.OpenAsync();

await using var dbCmd = connection.CreateCommand();
dbCmd.CommandText = """
    DROP DATABASE IF EXISTS college;
    CREATE DATABASE college;
""";
await dbCmd.ExecuteNonQueryAsync();

await using var tx = await connection.BeginTransactionAsync();
await using var cmd = connection.CreateCommand();
cmd.Transaction = (SqlTransaction)tx;

cmd.CommandText = """
    DROP TABLE IF EXISTS Users;
""";
await cmd.ExecuteNonQueryAsync();

cmd.CommandText = """
    CREATE TABLE college.[dbo].Users(
        id INT PRIMARY KEY IDENTITY(1,1),
        password NVARCHAR(100),
        name NVARCHAR(100) NOT NULL
    );

    INSERT INTO college.[dbo].Users(name, password) VALUES
        ('admin', 'superpassword'),
        ('user', 'notsuper'),
        ('manager', NULL);
""";
await cmd.ExecuteNonQueryAsync();

await tx.CommitAsync();

