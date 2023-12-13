using Loncotes.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/* *********************************************************************
                            ENDPOINTS
 *********************************************************************** */

// get all materitals-----------------------------------------------------------------------------------
app.MapGet("/api/materials", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Select(m => new MaterialDTO
    {
        Id = m.Id,
        MaterialName = m.MaterialName,
        MaterialTypeId = m.MaterialTypeId,
        GenreId = m.GenreId,
        OutOfCirculationSince = m.OutOfCirculationSince
    }).ToList();
});

/* GET materials by Genre AND/OR MaterialType-----------------------------------------------------------------
The librarians also like to search for materials by genre and type. 
Add query string parameters to the above endpoint for materialTypeId and genreId. 
Update the logic of the above endpoint to include both, either, 
or neither of these filters, depending which are passed in. 
Remember, query string parameters are always optional when making an HTTP request, 
so you have to account for the possibility that any of them will be missing. */
app.MapGet("/api/materials/{mtId}/{gId}", (LoncotesLibraryDbContext db, int mtId, int gId) =>
{

    var returnMaterials = new List<Material>();
    // both ids exist
    if (mtId != 0 && gId != 0)
    {
        returnMaterials = db.Materials.Where(m => m.MaterialTypeId == mtId && m.GenreId == gId).ToList();
    }
    // materialId exists = genreId does not
    if (mtId != 0 && gId == 0)
    {
        returnMaterials = db.Materials.Where(m => m.MaterialTypeId == mtId).ToList();
    }
    // genreId exists, materialId does not
    if (mtId == 0 && gId != 0)
    {
        returnMaterials = db.Materials.Where(m => m.GenreId == gId).ToList();
    }
    // neither id exists
    if (mtId == 0 && gId == 0)
    {
        returnMaterials = db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Select(m => new Material 
        {
            Id = m.Id,
            MaterialName = m.MaterialName,
            MaterialTypeId = m.MaterialTypeId,
            GenreId = m.GenreId,
            OutOfCirculationSince = m.OutOfCirculationSince
        }).ToList();
    }

    return Results.Ok(returnMaterials.Select(rm => {
        Genre foundGenre = db.Genres.FirstOrDefault(g => g.Id == rm.GenreId);
        MaterialType foundMType = db.MaterialTypes.FirstOrDefault(mt => mt.Id == rm.MaterialTypeId);

    return new MaterialDTO
    {
        Id = rm.Id,
        MaterialName = rm.MaterialName,
        MaterialTypeId = rm.MaterialTypeId,
        MaterialType = new MaterialType ()
        {
            Id = foundMType.Id,
            Name = foundMType.Name,
            CheckoutDays= foundMType.CheckoutDays
        },
        GenreId = rm.GenreId,
        Genre = new Genre() 
        {
            Id = foundGenre.Id,
            Name = foundGenre.Name
        },
        OutOfCirculationSince = rm.OutOfCirculationSince
    };}).ToList());
});


// POST/Add a Material
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material materialObj) => {

});

/* Remove a Material from circulation
Add an endpoint that expects an id in the url, which sets the OutOfCirculationSince property of the material that matches the material id to DateTime.Now. 
(This is called a soft delete, where a row is not deleted from the database, 
but instead has a flag that says the row is no longer active.) 
The endpoint to get all materials should already be filtering these items out. */


// GET all materialTypes



// Get all genres


// Get all patrons



// Get patron with checkouts
// This endpoint should get a patron and include their checkouts, and further include the materials and their material types.


// Update Patron
// Sometimes patrons move or change their email address. Add an endpoint that updates these properties only.



// Deactivate Patron
// Sometimes patrons move out of the county. Allow the librarians to deactivate a patron (another soft delete example!).



// Checkout a Material
//  Add an endpoint to create a new Checkout for a material and patron. 
// Automatically set the checkout date to DateTime.Today.


// Return a Material
// Add an endpoint expecting a checkout id, and update the checkout with a return date of DateTime.Today.

app.Run();
