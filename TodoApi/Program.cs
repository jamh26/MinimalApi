using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ItemRepository>();

var app = builder.Build();

app.MapGet("/items", ([FromServices] ItemRepository items) =>
{
    return items.GetAll();
});

app.MapPost("/items", ([FromServices] ItemRepository items, Item item) =>
{
    if (items.GetById(item.id) != null)
    {
        return Results.BadRequest();
    }

    items.Add(item);

    return Results.Created($"/Items/{item.id}", item);
});

app.MapGet("/items/{id}", ([FromServices] ItemRepository items, int id) =>
{
    var item = items.GetById(id);

    return item == null ? Results.NotFound() : Results.Ok(item);
});

app.MapGet("/", () => "Hello from Minimal API");
app.Run();

record Item(int id, string title, bool IsCompleted);

class ItemRepository
{
    private Dictionary<int, Item> items = new Dictionary<int, Item>();

    public ItemRepository()
    {
        var item1 = new Item(1, "Go to the gym", false);
        var item2 = new Item(2, "drink water", true);
        var item3 = new Item(3, "watch tv", false);
        var item4 = new Item(4, "read book", false);

        items.Add(item1.id, item1);
        items.Add(item2.id, item2);
        items.Add(item3.id, item3);
        items.Add(item4.id, item4);
    }

    public IEnumerable<Item> GetAll() => items.Values;
    public Item GetById(int id)
    {
        if (items.ContainsKey(id))
        {
            return items[id];
        }
        return null;
    }
    public void Add(Item item) => items.Add(item.id, item);
    public void Update(Item item) => items[item.id] = item;
    public void Delete(int id) => items.Remove(id);
}