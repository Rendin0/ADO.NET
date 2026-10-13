using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

DbProviderFactory factory = DbProviderFactories.GetFactory(config["Database:Provider"] ?? "Microsoft.Data.SqlClient");

var connectionString = config.GetConnectionString("Default");
await using DbConnection connection = factory.CreateConnection()!;
connection.ConnectionString = connectionString;

using DbCommand getUsersTable = connection.CreateCommand();
getUsersTable.CommandText = "SELECT id, name, password FROM Users";

DataSet dataSet = new("college");

DbDataAdapter dataAdapter = factory.CreateDataAdapter()!;
dataAdapter.SelectCommand = getUsersTable;
dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

DbCommandBuilder commandBuilder = factory.CreateCommandBuilder()!;
commandBuilder.DataAdapter = dataAdapter;

int loaded = dataAdapter.Fill(dataSet, "Users");
Console.WriteLine($"Строк - {loaded}; Состояние подключения - {connection.State}");

DataTable users = dataSet.Tables["Users"]!;

// DbDataAdapter - работа с синхронизацией данных из СУБД
// DataSet - объект, хранящий базу данных
// DataTable - конкретная таблица
// DataRow - конкретная строка

// Unchanged - Строка не была изменена
// Modified - Строка изменена

// Added - новая строка
// Deleted - строка удалена

// Detached - Строка не отслеживается

// DataRow admin = users.Rows.Find(1)!;
// Console.WriteLine($"До изменений - {admin.RowState}");
// admin["password"] = "qwerty";
// Console.WriteLine($"После изменений - {admin.RowState}: {admin["password",
//     DataRowVersion.Current]}; До изменений - {admin["password", DataRowVersion.Original]}");

// DataRow guest = users.NewRow();
// Console.WriteLine($"Новая строка - {guest.RowState}");
// guest["name"] = "guest";

// users.Rows.Add(guest);
// Console.WriteLine($"Строка добавлена - {guest.RowState}");

// DataRow ar = users.Rows.Find(4)!;
// ar.Delete();
// Console.WriteLine($"После удаления - {ar.RowState}");

// Console.WriteLine($"Наличие изменений - {dataSet.HasChanges()}");
// Console.WriteLine($"Количество изменений - {users.GetChanges()?.Rows.Count ?? 0}");

DataRow[] aUsers = users.Select("name LIKE '%a%'");
Console.WriteLine($"Имена с буквой а - {string.Join(", ", aUsers.Select(user => user["name"]))}");

DataView view = new(users)
{
    RowFilter = "password IS NULL",
    Sort = "name DESC"
};

Utils.PrintView(view);


// await connection.OpenAsync();

// await using var dbCmd = connection.CreateCommand();
// dbCmd.CommandText = """
//     DROP DATABASE IF EXISTS college;
//     CREATE DATABASE college;
// """;
// await dbCmd.ExecuteNonQueryAsync();

// await using var tx = await connection.BeginTransactionAsync();
// await using var cmd = connection.CreateCommand();
// cmd.Transaction = (SqlTransaction)tx;

// cmd.CommandText = """
//     DROP TABLE IF EXISTS Users;
// """;
// await cmd.ExecuteNonQueryAsync();

// cmd.CommandText = """
//     CREATE TABLE college.[dbo].Users(
//         id INT PRIMARY KEY IDENTITY(1,1),
//         password NVARCHAR(100),
//         name NVARCHAR(100) NOT NULL
//     );

//     INSERT INTO college.[dbo].Users(name, password) VALUES
//         ('admin', 'superpassword'),
//         ('user', 'notsuper'),
//         ('manager', NULL);
// """;
// await cmd.ExecuteNonQueryAsync();

// await tx.CommitAsync();

// cmd.CommandText = """
//     CREATE OR ALTER PROCEDURE GetUserByName
//         @name NVARCHAR(100)
//     AS
//     BEGIN
//         SELECT id, name, password FROM college.[dbo].Users WHERE name LIKE '%' + @name + '%'
//     END
// """;

// await cmd.ExecuteNonQueryAsync();

// cmd.CommandText = """
//     CREATE OR ALTER PROCEDURE AddUser
//         @name NVARCHAR(100),
//         @password NVARCHAR(100),
//         @newId INT OUTPUT
//     AS
//     BEGIN
//         INSERT INTO college.[dbo].Users(name, password) VALUES (@name, @password);
//         SET @newId = SCOPE_IDENTITY();
//     END
// """;

// await cmd.ExecuteNonQueryAsync();

// cmd.CommandText = """
//     CREATE OR ALTER PROCEDURE GetAllUsers AS
//     BEGIN
//         SELECT id, name, password FROM college.[dbo].Users
//     END
// """;

// await cmd.ExecuteNonQueryAsync();

// while (true)
// {
//     Console.Write("1. Вывести таблицу\n2. Добавить пользователя\n3. Удалить пользователя\n4. Получить по имени\n-1. Выход\n>_ ");
//     var userInput = Console.ReadLine();

//     switch (userInput)
//     {
//         case "1":
//             await GetAllusers(connection);
//             break;
//         case "2":
//             await AddUser(connection, factory);
//             break;
//         case "4":
//             await GetUserByName(connection, factory);
//             break;
//         case "-1":
//             return;
//         default:
//             break;
//     }
//     Console.ReadKey();
// }

// async Task GetAllusers(DbConnection connection)
// {
//     await using var cmd = connection.CreateCommand();
//     cmd.CommandType = CommandType.StoredProcedure;
//     cmd.CommandText = "GetAllUsers";

//     await using var reader = await cmd.ExecuteReaderAsync();

//     while (await reader.ReadAsync())
//     {
//         object[] values = new object[reader.FieldCount];
//         reader.GetValues(values);
//         Console.WriteLine(string.Join(" | ", values));
//     }
// }

// async Task AddUser(DbConnection connection, DbProviderFactory factory)
// {
//     Console.Write("Имя: ");
//     var name = Console.ReadLine();
//     Console.Write("Пароль: ");
//     var password = Console.ReadLine();

//     await using var cmd = connection.CreateCommand();
//     cmd.CommandType = CommandType.StoredProcedure;
//     cmd.CommandText = "AddUser";

//     cmd.Parameters.AddWithValue("@name", name, DbType.String, factory);
//     cmd.Parameters.AddWithValue("@password", password, DbType.String, factory);

//     var newId = cmd.Parameters.AddWithValue("@newId", null, DbType.Int32, factory, ParameterDirection.Output);

//     await cmd.ExecuteNonQueryAsync();

//     Console.WriteLine($"Успешно добавлена запись с id {newId.Value}");
// }

// async Task GetUserByName(DbConnection connection, DbProviderFactory factory)
// {
//     Console.Write("Введите имя пользователя: ");
//     var name = Console.ReadLine();

//     await using var cmd = connection.CreateCommand();
//     cmd.CommandType = CommandType.StoredProcedure;
//     cmd.CommandText = "GetUserByName";

//     cmd.Parameters.AddWithValue("@name", name, DbType.String, factory);

//     await using var reader = await cmd.ExecuteReaderAsync();

//     while (await reader.ReadAsync())
//     {
//         object[] values = new object[reader.FieldCount];
//         reader.GetValues(values);
//         Console.WriteLine(string.Join(" | ", values));
//     }
// }


// // $env:ConnectionStrings__Default = "<строка подключения>"