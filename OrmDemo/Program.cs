using Microsoft.EntityFrameworkCore;

using CollegeContext college = new CollegeContext();

var students = await college.Students
    .Include(s => s.Group)
    .ToListAsync();

foreach (var student in students)
    Console.WriteLine($"{student.Id} | {student.Name} | {student.Score?.ToString() ?? "н/д"} | {student.Group.Name} ");

var stats = await college.Groups
    .Select(g => new
    {
        g.Name,
        Count = g.Students.Count(),
        Average = g.Students.Average(s => s.Score),
        Best = g.Students.Max(s => s.Score)
    })
    .ToListAsync();

foreach (var stat in stats)
    Console.WriteLine($"{stat.Name}: Кол-во: {stat.Count}; Среднее: {stat.Average?.ToString() ?? "н/д"}; Топ1: {stat.Best?.ToString() ?? "н/д"}");

var groups = await college.Students
    .GroupBy(s => s.Group.Name)
    .Select(g => new { GroupName = g.Key, StudentsCount = g.Count() })
    .ToListAsync();

foreach (var group in groups)
    Console.WriteLine($"{group.GroupName}: {group.StudentsCount}");

var highScores = await (
    from s in college.Students
    where s.Score >= 60
    join g in college.Groups on s.GroupId equals g.Id
    orderby g.Name descending
    select new { StudentName = s.Name, GroupName = g.Name, s.Score }
).ToListAsync();

foreach (var highScore in highScores)
    Console.WriteLine($"{highScore.StudentName} | {highScore.GroupName} | {highScore.Score}");

// var users = await college.Users
//     .Where(u => u.Password != null)
//     .Select(u => new { u.Name, u.Password } )
//     .OrderByDescending(u => u.Password)
//     .ToListAsync();

// var id3 = await college.Users.FindAsync(3);
// var guest = await college.Users.FirstOrDefaultAsync(u => u.Name == "top_guest");
// var manager = await college.Users.SingleAsync(u => u.Name == "top_manager");

// Console.WriteLine($"{guest?.Id} | {guest?.Name} | {guest?.Password}");

// foreach (var user in users)
// {
//     Console.WriteLine($"{user.Name} | {user.Password ?? "<null>"}");
// }
