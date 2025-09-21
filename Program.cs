// Program.cs
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// --- DATABASE & CACHE CONFIGURATION ---

// 1. PostgreSQL Configuration
var connectionString = "Host=host.docker.internal;Port=5433;Database=pastebindb;Username=postgres;Password=bin007";
builder.Services.AddDbContext<PasteDb>(options => options.UseNpgsql(connectionString));

// 2. Redis Cache Configuration
// This line connects to our Redis container.
var redisConnection = ConnectionMultiplexer.Connect("host.docker.internal:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);


var app = builder.Build();


// ---> SECURITY MIDDLEWARE <---
var apiKey = app.Configuration.GetValue<string>("ApiKey");

app.Use(async (context, next) =>
{
    // We only want to protect the POST endpoint for creating pastes.
    if (context.Request.Path.StartsWithSegments("/pastes") && context.Request.Method == "POST")
    {
        // Check if the request header has our API Key.
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        // Check if the provided key is the correct one.
        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid API Key.");
            return;
        }
    }

    // If the key is valid or the endpoint doesn't need a key, continue to the next step.
    await next();
});

// --- ENDPOINTS ---

app.MapPost("/pastes", async (PasteIn input, PasteDb db) =>
{
    var paste = new Paste { Content = input.Content };
    await db.Pastes.AddAsync(paste);
    await db.SaveChangesAsync();
    return Results.Created($"/pastes/{paste.Id}", paste);
});

// GET Endpoint with Caching Logic!
app.MapGet("/pastes/{id:int}", async (int id, PasteDb dbContext, IConnectionMultiplexer redis) =>
{
    var cache = redis.GetDatabase();
    string cacheKey = $"paste:{id}";

    // 1. Try to get the paste from the Redis cache first.
    var cachedPaste = await cache.StringGetAsync(cacheKey);

    if (!cachedPaste.IsNullOrEmpty)
    {
        // CACHE HIT: The data was in the cache. Return it instantly.
        app.Logger.LogInformation("Cache Hit for key: {CacheKey}", cacheKey);
        var pasteFromCache = JsonSerializer.Deserialize<Paste>(cachedPaste);
        return Results.Ok(pasteFromCache);
    }
    else
    {
        // CACHE MISS: The data was not in the cache.
        app.Logger.LogInformation("Cache Miss for key: {CacheKey}", cacheKey);

        // 2. Get the data from the PostgreSQL database.
        var pasteFromDb = await dbContext.Pastes.FindAsync(id);

        if (pasteFromDb is null)
        {
            return Results.NotFound();
        }

        // 3. Save the data to the cache for next time.
        // We'll set it to expire after 1 hour to save memory.
        await cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(pasteFromDb), TimeSpan.FromHours(1));

        return Results.Ok(pasteFromDb);
    }
});


app.Run();