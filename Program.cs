using Loncotes.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;

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
    .Include(m => m.MaterialType)
    .Include(m => m.Genre)
    .Select(m => new MaterialDTO
    {
        Id = m.Id,
        MaterialName = m.MaterialName,
        MaterialTypeId = m.MaterialTypeId,
        MaterialType = new MaterialTypeDTO
        {
            Id = m.MaterialType.Id,
            Name = m.MaterialType.Name,
            CheckoutDays = m.MaterialType.CheckoutDays
        },
        GenreId = m.GenreId,
        Genre = new GenreDTO
        {
            Id = m.Genre.Id,
            Name = m.Genre.Name
        },
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
            MaterialType = new MaterialType
            {
                Id = m.MaterialType.Id,
                Name = m.MaterialType.Name,
                CheckoutDays = m.MaterialType.CheckoutDays
            },
            GenreId = m.GenreId,
            Genre = new Genre
            {
                Id = m.Genre.Id,
                Name = m.Genre.Name
            },
            OutOfCirculationSince = m.OutOfCirculationSince
        }).ToList();
    }

    return Results.Ok(returnMaterials.Select(rm =>
    {
        Genre foundGenre = db.Genres.FirstOrDefault(g => g.Id == rm.GenreId);
        MaterialType foundMType = db.MaterialTypes.FirstOrDefault(mt => mt.Id == rm.MaterialTypeId);

        return new MaterialDTO
        {
            Id = rm.Id,
            MaterialName = rm.MaterialName,
            MaterialTypeId = rm.MaterialTypeId,
            MaterialType = new MaterialTypeDTO()
            {
                Id = foundMType.Id,
                Name = foundMType.Name,
                CheckoutDays = foundMType.CheckoutDays
            },
            GenreId = rm.GenreId,
            Genre = new GenreDTO()
            {
                Id = foundGenre.Id,
                Name = foundGenre.Name
            },
            OutOfCirculationSince = rm.OutOfCirculationSince
        };
    }).ToList());
});


// POST/Add a Material
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material materialObj) =>
{
    try
    {
    db.Materials.Add(materialObj);
    db.SaveChanges();
    return Results.Created($"/api/materials/{materialObj.Id}", materialObj);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Bad data submitted: {ex}");
    }
});

/* Remove a Material from circulation
Add an endpoint that expects an id in the url, 
which sets the OutOfCirculationSince property of the material that matches the material id to DateTime.Now. 
(This is called a soft delete, where a row is not deleted from the database, 
but instead has a flag that says the row is no longer active.) 
The endpoint to get all materials should already be filtering these items out. */
app.MapPut("/api/materials/{id}/remove_circulation", (LoncotesLibraryDbContext db, int id) => 
{
    Material materialtoUpdate = db.Materials.SingleOrDefault(m => m.Id == id);
    if (materialtoUpdate == null)
    {
        return Results.NotFound();
    }
    materialtoUpdate.OutOfCirculationSince = DateTime.Now;
    db.SaveChanges();
    return Results.NoContent();
});

// GET all materialTypes
app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) => {
    return db.MaterialTypes
    .Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckoutDays = mt.CheckoutDays
    }).ToList();
});

// Get all genres
app.MapGet("/api/genres", (LoncotesLibraryDbContext db) => {
    return db.Genres.Select(genre => new GenreDTO
    {
        Id = genre.Id,
        Name = genre.Name
    }).ToList();
});

// Get all patrons
app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) => {
    return db.Patrons.Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive
    }).ToList();
});

// Get patron by ID with checkouts
// This endpoint should get a single patron and include their checkouts, and further include the materials and their material types.
app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) => {
    return db.Patrons
    .Include(p => p.Checkouts).ThenInclude(c => c.Material).ThenInclude(m => m.MaterialType)
    .Include(p => p.Checkouts).ThenInclude(c => c.Material).ThenInclude(m => m.Genre)
    .Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive,
        Checkouts = db.Checkouts.Select(pco => 
            new CheckoutDTO
            {
                Id = pco.Id,
                PatronId = pco.PatronId,
                MaterialId = pco.MaterialId,
                Material = new MaterialDTO
                {
                    Id = pco.Material.Id,
                    MaterialName = pco.Material.MaterialName,
                    MaterialTypeId = pco.Material.MaterialTypeId,
                    MaterialType = new MaterialTypeDTO
                    {
                        Id = pco.Material.MaterialType.Id,
                        Name = pco.Material.MaterialType.Name,
                        CheckoutDays = pco.Material.MaterialType.CheckoutDays
                    },
                    GenreId = pco.Material.GenreId,
                    Genre = new GenreDTO 
                    {
                        Id = pco.Material.Genre.Id,
                        Name = pco.Material.Genre.Name
                    },
                    OutOfCirculationSince = pco.Material.OutOfCirculationSince
                },
                CheckoutDate = pco.CheckoutDate,
                ReturnDate = pco.ReturnDate
            }).ToList()
    }).ToList();
});

// Update Patron
// Sometimes patrons move or change their email address. Add an endpoint that updates these properties only.
app.MapPut("/api/patrons/{id}/update", (LoncotesLibraryDbContext db, int id, Patron newPatronObj) => {
    
    if (id != newPatronObj.Id)
    {
        return Results.BadRequest("Bad data sent");
    }

    Patron matchedPatron = db.Patrons.SingleOrDefault(p => p.Id == id);
    
    if (matchedPatron == null)
    {
        return Results.NotFound("No user found matching request");
    }

    matchedPatron.Address = newPatronObj.Address;
    matchedPatron.Email = newPatronObj.Email;
    db.SaveChanges();

    return Results.NoContent();
});


// Deactivate Patron
// Sometimes patrons move out of the county. Allow the librarians to deactivate a patron (another soft delete example!).
app.MapPut("/api/patrons/{id}/deactivate", (LoncotesLibraryDbContext db, int id) => {
    Patron matchedPatron = db.Patrons.SingleOrDefault(p => p.Id == id);
    
    if (matchedPatron == null)
    {
        return Results.NotFound("No user found matching request");
    }
    if (matchedPatron.IsActive == false)
    {
        return Results.BadRequest("Patron already marked as inactive");
    }

    matchedPatron.IsActive = false;
    db.SaveChanges();
    return Results.NoContent();
});

// Checkout a Material
// Add an endpoint to create a new Checkout for a material and patron. 
// Automatically set the checkout date to DateTime.Today.
app.MapPost("/api/checkout", (LoncotesLibraryDbContext db, Checkout checkoutObj) => {
    try
    {
        checkoutObj.CheckoutDate = DateTime.Now;
        db.Checkouts.Add(checkoutObj);
        db.SaveChanges();
        return Results.Created($"/api/reservations/{checkoutObj.Id}", checkoutObj);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Bad data submitted: {ex}");
    }
});

// Return a Material
// Add an endpoint expecting a checkout id, and update the checkout with a return date of DateTime.Today.



app.Run();
