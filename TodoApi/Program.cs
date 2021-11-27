using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

app.MapGet("/items", async (ApiDbContext db) =>
{
    return await db.Items.ToListAsync();
});

app.MapPost("/items", async (ApiDbContext db, Item item) =>
{
    if (await db.Items.FirstOrDefaultAsync(x => x.Id == item.Id) != null)
    {
        return Results.BadRequest();
    }

    db.Items.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/Items/{item.Id}", item);
});

app.MapGet("/items/{id}", async (ApiDbContext db, int id) =>
{
    var item = await db.Items.FirstOrDefaultAsync(x => x.Id == id);

    return item == null ? Results.NotFound() : Results.Ok(item);
});

app.MapPut("/items/{id}", async (ApiDbContext db, int id, Item item) =>
{
    var existItem = await db.Items.FirstOrDefaultAsync(x => x.Id == id);
    if (existItem == null)
    {
        return Results.NotFound();
    }

    existItem.Title = item.Title;
    existItem.IsCompleted = item.IsCompleted;

    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/items/{id}", async (ApiDbContext db, int id) =>
{
    var item = await db.Items.FirstOrDefaultAsync(x => x.Id == id);
    if (item == null)
    {
        return Results.NotFound();
    }

    db.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/", () => "Hello from Minimal API");
app.Run();

class Item
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

class ApiDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }
}