using MinimalNoteApp.Datas;
using MinimalNoteApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NoteDbContext>(options => 
options.UseSqlServer (
    builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



//Create
app.MapPost("/notes",async (Note not,NoteDbContext db) => 
{  
    not.CreatedAt = DateTime.Now;
    db.Notes.Add (not);
    await db.SaveChangesAsync();
    return Results.Ok("Note is created");
});

//Read All
app.MapGet("/notes", async (NoteDbContext db) =>
{
    var notes = await db.Notes.ToListAsync();
    return Results.Ok(notes);   
});

//Read By Id
app.MapGet("/notes/{id:int}", async(int id,NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync (id);
    if (note == null)
        return Results.NotFound();
    return Results.Ok(note);
});

//Update
app.MapPut("/notes/{id:int}", async(int id, Note updatedNote,NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null)
        return Results.NotFound();
    note.Title = updatedNote.Title;
    note.Content = updatedNote.Content;
    await db.SaveChangesAsync();
    return Results.Ok("Note is updated");
});

//Delete
app.MapDelete("/notes/{id:int}", async (int id, NoteDbContext db) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null)
        return Results.NotFound();
    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.Ok("Note is deleted");
});

app.Run();

