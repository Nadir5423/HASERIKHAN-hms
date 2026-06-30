using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Core;
using HMS.Repositories;
using HMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

// Add Dependency Injection for repositories and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ICrudService<>), typeof(CrudService<>));
builder.Services.AddScoped<BookingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseSession(); // Enable session middleware

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Seed database services if empty
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    try
    {
        // Dynamically drop any check constraints on ROOM.Status and ensure the updated CK_ROOM_Status exists
        string updateConstraintsSql = @"
DECLARE @ConstraintName NVARCHAR(MAX);
DECLARE @Sql NVARCHAR(MAX);

DECLARE constraint_cursor CURSOR FOR
SELECT dc.name 
FROM sys.check_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE dc.parent_object_id = OBJECT_ID('dbo.ROOM') AND c.name = 'Status';

OPEN constraint_cursor;
FETCH NEXT FROM constraint_cursor INTO @ConstraintName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Sql = 'ALTER TABLE dbo.ROOM DROP CONSTRAINT ' + QUOTENAME(@ConstraintName);
    EXEC sp_executesql @Sql;
    FETCH NEXT FROM constraint_cursor INTO @ConstraintName;
END;

CLOSE constraint_cursor;
DEALLOCATE constraint_cursor;

ALTER TABLE dbo.ROOM ADD CONSTRAINT CK_ROOM_Status CHECK (Status IN ('Available', 'Occupied', 'Maintenance', 'Reserved', 'Deleted'));
";
        context.Database.ExecuteSqlRaw(updateConstraintsSql);
    }
    catch (System.Exception ex)
    {
        System.Console.WriteLine($"DB Schema Update Error: {ex.Message}");
    }

    try
    {
        if (!context.Services.Any())
        {
            context.Services.AddRange(
                new HMS.Models.Entities.SERVICE { ServiceName = "Breakfast Buffets", ServiceType = "Food", Description = "Daily premium breakfast buffet in the dining hall", Price = 15.00m, IsAvailable = true },
                new HMS.Models.Entities.SERVICE { ServiceName = "Airport Shuttle", ServiceType = "Transport", Description = "Two-way premium airport shuttle service", Price = 25.00m, IsAvailable = true },
                new HMS.Models.Entities.SERVICE { ServiceName = "Spa & Wellness Access", ServiceType = "Spa", Description = "Full-day access to sauna, pool, and massage therapy", Price = 40.00m, IsAvailable = true },
                new HMS.Models.Entities.SERVICE { ServiceName = "Late Checkout", ServiceType = "Other", Description = "Extend checkout time up to 6:00 PM", Price = 20.00m, IsAvailable = true }
            );
            context.SaveChanges();
        }
    }
    catch (System.Exception ex)
    {
        System.Console.WriteLine($"DB Seeding Error: {ex.Message}");
    }
}

app.Run();
