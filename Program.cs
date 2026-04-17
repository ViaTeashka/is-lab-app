var builder = WebApplication.CreateBuilder(args);
var notes = new List<Note>();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/health", () => new {
    status = "ok",
    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});

app.MapGet("/version", (IConfiguration conf) => new {
    name = conf["App:Name"],
    version = conf["App:Version"]
});


// Получить все заметки
app.MapGet("/api/notes", () => notes);

// Получить одну по ID
app.MapGet("/api/notes/{id}", (int id) =>
    notes.FirstOrDefault(n => n.Id == id) is Note note ? Results.Ok(note) : Results.NotFound());

// Создать новую (с минимальной валидацией)
app.MapPost("/api/notes", (Note note) => {
    if (string.IsNullOrEmpty(note.Title)) return Results.BadRequest("Заголовок не может быть пустым");

    var newNote = note with { CreatedAt = DateTime.Now }; // Ставим текущую дату
    notes.Add(newNote);
    return Results.Created($"/api/notes/{newNote.Id}", newNote);
});

// Удалить
app.MapDelete("/api/notes/{id}", (int id) => {
    notes.RemoveAll(n => n.Id == id);
    return Results.NoContent();
});

app.MapGet("/db/ping", (IConfiguration conf) => {
    try
    {
        var connectionString = conf.GetConnectionString("Mssql");
        // Тут логика проверки подключения (пока просто имитация)
        return Results.Ok(new { status = "ok", message = "Connection string found" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { status = "error", message = ex.Message });
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record Note(int Id, string Title, string Text, DateTime CreatedAt);
