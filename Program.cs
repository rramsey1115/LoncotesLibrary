using Loncotes.Models;
using Microsoft.EntityFrameworkCore;

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

/* *****************************************************************************************************************
                                                    ENDPOINTS
 ******************************************************************************************************************* */

// get all materitals
app.MapGet("/api/materials", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Include(m => m.MaterialType)
    .Include(m => m.Genre)
    .Where(m => m.OutOfCirculationSince != null)
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

// get material by id
app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) => {
    Material foundM = db.Materials
        .Include(m => m.Genre)
        .Include(m => m.MaterialType)
        .Include(m => m.Checkouts).ThenInclude(c => c.Patron)
        .SingleOrDefault(m => m.Id == id);

    if (foundM == null)
    {
        return Results.NotFound("No material matches id given");
    }

    return Results.Ok(new MaterialDTO
    {   
        Id = foundM.Id,
        MaterialName = foundM.MaterialName,
        MaterialTypeId = foundM.MaterialTypeId,
        MaterialType = new MaterialTypeDTO
        {
            Id = foundM.MaterialType.Id,
            Name = foundM.MaterialType.Name
        },
        GenreId = foundM.GenreId,
        Genre = new GenreDTO
        {
            Id = foundM.Genre.Id,
            Name = foundM.Genre.Name
        },
        Checkouts = foundM.Checkouts.Select(c => new CheckoutDTO
        {
            Id = c.Id,
            MaterialId = c.MaterialId,
            PatronId = c.PatronId,
            CheckoutDate = c.CheckoutDate,
            ReturnDate = c.ReturnDate
        }).ToList()
    });



});

// GET materials by Genre AND/OR MaterialType
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

// Remove a Material from circulation
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
app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes
    .Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckoutDays = mt.CheckoutDays
    }).ToList();
});

// Get all genres
app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.Select(genre => new GenreDTO
    {
        Id = genre.Id,
        Name = genre.Name
    }).ToList();
});

// Get all patrons
app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
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
app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
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

// Update Patron email and address
app.MapPut("/api/patrons/{id}/update", (LoncotesLibraryDbContext db, int id, Patron newPatronObj) =>
{

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
app.MapPut("/api/patrons/{id}/deactivate", (LoncotesLibraryDbContext db, int id) =>
{
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
// Automatically sets the checkout date to DateTime.Now.
app.MapPost("/api/checkout", (LoncotesLibraryDbContext db, Checkout checkoutObj) =>
{
    try
    {
        checkoutObj.CheckoutDate = DateTime.Now;
        db.Checkouts.Add(checkoutObj);
        db.SaveChanges();
        return Results.Created($"/api/checkout/{checkoutObj.Id}", checkoutObj);
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
// Sets return date to DateTime.Now.
app.MapPut("/api/return/{id}", (LoncotesLibraryDbContext db, int id) =>
{   
    Checkout foundCheckout = db.Checkouts.SingleOrDefault(co => co.Id == id);

    if (foundCheckout == null)
    {
        return Results.NotFound("No checkout matches the given id");
    }

    if (foundCheckout.ReturnDate != null)
    {
        return Results.BadRequest("checkout has already been marked as returned.");
    }

    try
    {
        foundCheckout.ReturnDate = DateTime.Now;
        db.SaveChanges();
        return Results.Created($"/api/return/{foundCheckout.Id}", foundCheckout);
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

// get all checkouts w/ all embeded info
app.MapGet("/api/checkouts", (LoncotesLibraryDbContext db) => {
    return db.Checkouts
    .Include(co => co.Patron)
    .Include(co => co.Material).ThenInclude(m => m.MaterialType)
    .Include(co => co.Material).ThenInclude(m => m.Genre)
    .Select(co => new CheckoutDTO
    {
        Id = co.Id,
        PatronId = co.Id,
        Patron = new PatronDTO
        {
            Id = co.Patron.Id,
            FirstName = co.Patron.FirstName,
            LastName = co.Patron.LastName,
            Address = co.Patron.Address,
            Email = co.Patron.Email
        },
        Material = new MaterialDTO
        {
            Id = co.Material.Id,
            MaterialName = co.Material.MaterialName,
            MaterialTypeId = co.Material.MaterialTypeId,
            MaterialType = new MaterialTypeDTO
            {
                Id = co.Material.MaterialType.Id,
                Name = co.Material.MaterialType.Name
            },
            GenreId = co.Material.GenreId,
            Genre = new GenreDTO
            {
                Id = co.Material.Genre.Id,
                Name = co.Material.Genre.Name
            },
            OutOfCirculationSince = co.Material.OutOfCirculationSince
        },
        CheckoutDate = co.CheckoutDate,
        ReturnDate = co.ReturnDate
    }).ToList();
});

// get all AVAILABLE materials
app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
    .Select(material => new MaterialDTO
    {
        Id = material.Id,
        MaterialName = material.MaterialName,
        MaterialTypeId = material.MaterialTypeId,
        GenreId = material.GenreId,
        OutOfCirculationSince = material.OutOfCirculationSince
    })
    .ToList();
});

// get all OVERDUE checkouts
app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType)
    .Where(co =>
        (DateTime.Today - co.CheckoutDate).Days >
        co.Material.MaterialType.CheckoutDays &&
        co.ReturnDate == null)
        .Select(co => new CheckoutWithLateFeeDTO
        {
            Id = co.Id,
            MaterialId = co.MaterialId,
            Material = new MaterialDTO
            {
                Id = co.Material.Id,
                MaterialName = co.Material.MaterialName,
                MaterialTypeId = co.Material.MaterialTypeId,
                MaterialType = new MaterialTypeDTO
                {
                    Id = co.Material.MaterialTypeId,
                    Name = co.Material.MaterialType.Name,
                    CheckoutDays = co.Material.MaterialType.CheckoutDays
                },
                GenreId = co.Material.GenreId,
                OutOfCirculationSince = co.Material.OutOfCirculationSince
            },
            PatronId = co.PatronId,
            Patron = new PatronDTO
            {
                Id = co.Patron.Id,
                FirstName = co.Patron.FirstName,
                LastName = co.Patron.LastName,
                Address = co.Patron.Address,
                Email = co.Patron.Email,
                IsActive = co.Patron.IsActive
            },
            CheckoutDate = co.CheckoutDate,
            ReturnDate = co.ReturnDate
        })
    .ToList();
});



app.Run();
