using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding psql
builder.Services.AddDbContext<PasteDb>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// create
app.MapPost("/pastes", (PasteIn input, PasteDb db) =>
{
    var paste = new Paste
    {
        Content = input.Content,
        CreatedAt = DateTime.UtcNow
    };

    db.Pastes.Add(paste);
    db.SaveChanges();

    return Results.Created($"/pastes/{paste.Id}", paste);
});

// read all
app.MapGet("/pastes", (PasteDb db) =>
{
    var pastes = db.Pastes.ToList();
    return Results.Ok(pastes);
});

// read by id
app.MapGet("/pastes/{id}", (int id, PasteDb db) =>
{
    var paste = db.Pastes.Find(id);

    if (paste == null)
        return Results.NotFound("Paste not found");

    return Results.Ok(paste);
});

// update
app.MapPut("/pastes/{id}", (int id, PasteIn input, PasteDb db) =>
{
    var paste = db.Pastes.Find(id);

    if (paste == null)
        return Results.NotFound("Paste not found");

    paste.Content = input.Content;
    db.SaveChanges();

    return Results.Ok(paste);
});

// delete
app.MapDelete("/pastes/{id}", (int id, PasteDb db) =>
{
    var paste = db.Pastes.Find(id);

    if (paste == null)
        return Results.NotFound("Paste not found");

    db.Pastes.Remove(paste);
    db.SaveChanges();

    return Results.Ok("Paste deleted successfully!");
});

app.Run();