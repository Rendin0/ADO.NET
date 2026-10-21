// using Microsoft.Data.SqlClient;

// await using var con = new SqlConnection("Server=localhost;Database=college;User Id=sa;Password=P@ssw0rd!;TrustServerCertificate=true");

// await using var cmd = con.CreateCommand(); 

// cmd.CommandText = """
//     DROP TABLE IF EXISTS Students;
//     DROP TABLE IF EXISTS Groups;

//     CREATE TABLE Groups(
//         id INT PRIMARY KEY IDENTITY(1, 1),
//         name NVARCHAR(100) NOT NULL
//     );

//     CREATE TABLE Students(
//         id INT PRIMARY KEY IDENTITY(1, 1),
//         name NVARCHAR(100) NOT NULL,
//         score INT    ,
//         groupId INT NOT NULL REFERENCES dbo.Groups(id)
//     );

//     INSERT INTO Groups(name) VALUES (N'РП0-2'), (N'РПО-5'), (N'РПО-6');
//     INSERT INTO Students(name, score, groupId) VALUES
//     (N'Кузнецов', 80, 2), (N'Фролов', NULL, 2),
//     (N'Петров', 70, 3), (N'Орлова', 60, 3), (N'Иванова', 90, 3);
// """;

// await con.OpenAsync();
// await cmd.ExecuteNonQueryAsync();