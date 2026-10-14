using Microsoft.EntityFrameworkCore;

using CollegeContext college = new CollegeContext();

var users = await college.Users
    .Where(u => u.Password != null)
    .Select(u => new { u.Name, u.Password } )
    .OrderByDescending(u => u.Password)
    .ToListAsync();

var id3 = await college.Users.FindAsync(3);
var guest = await college.Users.FirstOrDefaultAsync(u => u.Name == "top_guest");
var manager = await college.Users.SingleAsync(u => u.Name == "top_manager");

Console.WriteLine($"{guest?.Id} | {guest?.Name} | {guest?.Password}");

// foreach (var user in users)
// {
//     Console.WriteLine($"{user.Name} | {user.Password ?? "<null>"}");
// }
