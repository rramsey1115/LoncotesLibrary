using Microsoft.EntityFrameworkCore;
using Loncotes.Models;

public class LoncotesLibraryDbContext : DbContext
{
    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Material Types
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
        new MaterialType { Id = 1, Name="Book", CheckoutDays = 30},
        new MaterialType { Id = 2, Name="Magazine", CheckoutDays = 14},
        new MaterialType { Id = 3, Name="DVD", CheckoutDays = 7},
        new MaterialType { Id = 4, Name="Audio Book", CheckoutDays = 21},
        new MaterialType { Id = 5, Name="E-book", CheckoutDays = 14}
        });

        // Genres
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
        new Genre { Id = 1, Name = "Fiction"},
        new Genre { Id = 2, Name = "Science Fiction"},
        new Genre { Id = 3, Name = "History"},
        new Genre { Id = 4, Name = "Nonfiction"},
        new Genre { Id = 5, Name = "Mystery"}
        });

        // Patrons
        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
        new Patron { Id = 1, FirstName="Lukas", LastName="Alecksen", Address="4199 High Street", Email="lalecksen@gmail.com", IsActive = true},
        new Patron { Id = 2, FirstName="Anna", LastName="Smith", Address="123 Main Street", Email="asmith@gmail.com", IsActive = true},
        new Patron { Id = 3, FirstName="John", LastName="Doe", Address="456 Oak Avenue", Email="jdoe@gmail.com", IsActive = true},
        new Patron { Id = 4, FirstName="Emily", LastName="Johnson", Address="789 Pine Lane", Email="ejohnson@gmail.com", IsActive = true},
        new Patron { Id = 5, FirstName="Daniel", LastName="Brown", Address="101 Cedar Road", Email="dbrown@gmail.com", IsActive = true},
        new Patron { Id = 6, FirstName="Olivia", LastName="Miller", Address="202 Birch Street", Email="omiller@gmail.com", IsActive = true},
        new Patron { Id = 7, FirstName="Michael", LastName="White", Address="303 Maple Avenue", Email="mwhite@gmail.com", IsActive = true},
        new Patron { Id = 8, FirstName="Sophia", LastName="Davis", Address="404 Elm Boulevard", Email="sdavis@gmail.com", IsActive = true},
        new Patron { Id = 9, FirstName="Ethan", LastName="Anderson", Address="505 Oak Drive", Email="eanderson@gmail.com", IsActive = false},
        new Patron { Id = 10, FirstName="Ava", LastName="Martinez", Address="606 Pine Circle", Email="amartinez@gmail.com", IsActive = false},
        });

        // Materials
        modelBuilder.Entity<Material>().HasData(new Material[]
        {
        new Material { Id = 1, MaterialName ="The Great Gatsby", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null},
        new Material { Id = 2, MaterialName ="National Geographic", MaterialTypeId = 2, GenreId = 4, OutOfCirculationSince = null},
        new Material { Id = 3, MaterialName ="Inception (DVD)", MaterialTypeId = 3, GenreId = 2, OutOfCirculationSince = null},
        new Material { Id = 4, MaterialName ="The Brief History of Time", MaterialTypeId = 4, GenreId = 3, OutOfCirculationSince = null},
        new Material { Id = 5, MaterialName ="The Da Vinci Code (E-book)", MaterialTypeId = 5, GenreId = 5, OutOfCirculationSince = null},
        new Material { Id = 6, MaterialName ="The Art of War", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = null},
        new Material { Id = 7, MaterialName ="Time Magazine", MaterialTypeId = 2, GenreId = 4, OutOfCirculationSince = null},
        new Material { Id = 8, MaterialName ="The Matrix (DVD)", MaterialTypeId = 3, GenreId = 2, OutOfCirculationSince = null},
        new Material { Id = 9, MaterialName ="Sapiens (Audio Book)", MaterialTypeId = 4, GenreId = 3, OutOfCirculationSince = null},
        new Material { Id = 10, MaterialName ="Neuromancer (E-book)", MaterialTypeId = 5, GenreId = 2, OutOfCirculationSince = null},
        new Material { Id = 11, MaterialName ="National Geographic Kids", MaterialTypeId = 2, GenreId = 4, OutOfCirculationSince = null},
        new Material { Id = 12, MaterialName ="Spider-Man: Into the Spider-Verse (DVD)", MaterialTypeId = 3, GenreId = 1, OutOfCirculationSince = null},
        new Material { Id = 13, MaterialName ="A Brief History of Nearly Everything", MaterialTypeId = 4, GenreId = 3, OutOfCirculationSince = null},
        new Material { Id = 14, MaterialName ="The Hitchhiker's Guide to the Galaxy (E-book)", MaterialTypeId = 5, GenreId = 2, OutOfCirculationSince = null},
        new Material { Id = 15, MaterialName ="Cook's Illustrated", MaterialTypeId = 4, GenreId = 5, OutOfCirculationSince = null},
        new Material { Id = 16, MaterialName ="Black Mirror (DVD)", MaterialTypeId = 3, GenreId = 1, OutOfCirculationSince = null},
        new Material { Id = 17, MaterialName ="The Catcher in the Rye", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = new DateTime(2011, 12, 30)},
        new Material { Id = 18, MaterialName ="Wired Magazine", MaterialTypeId = 2, GenreId = 4, OutOfCirculationSince = null},
        new Material { Id = 19, MaterialName ="The Shining (DVD)", MaterialTypeId = 3, GenreId = 5, OutOfCirculationSince = new DateTime(1999, 09, 22)},
        new Material { Id = 20, MaterialName ="The Girl with the Dragon Tattoo (E-book)", MaterialTypeId = 5, GenreId = 5, OutOfCirculationSince = null}
        });

        // Checkouts
        modelBuilder.Entity<Checkout>().HasData(new Checkout[]
        {
            new Checkout { Id = 1, MaterialId = 7, PatronId = 1, CheckoutDate = new DateTime(2023, 01, 15), ReturnDate = new DateTime(2023, 02, 28), Paid = true},
            new Checkout { Id = 2, MaterialId = 11, PatronId = 4, CheckoutDate = new DateTime(2023, 02, 20), ReturnDate = new DateTime(2023, 03, 05), Paid = true},
            new Checkout { Id = 3, MaterialId = 14, PatronId = 7, CheckoutDate = new DateTime(2023, 03, 10), ReturnDate = new DateTime(2023, 05, 24), Paid = false},
            new Checkout { Id = 4, MaterialId = 17, PatronId = 9, CheckoutDate = new DateTime(2023, 04, 05), ReturnDate = new DateTime(2023, 04, 10), Paid = false},
            new Checkout { Id = 5, MaterialId = 20, PatronId = 10, CheckoutDate = new DateTime(2023, 12, 01), ReturnDate = new DateTime(2023, 12, 03), Paid = true},
            new Checkout { Id = 6, MaterialId = 8, PatronId = 2, CheckoutDate = new DateTime(2023, 12, 12), ReturnDate = null, Paid = true},
            new Checkout { Id = 7, MaterialId = 13, PatronId = 1, CheckoutDate = new DateTime(2023, 12, 13), ReturnDate = null, Paid = true},
            new Checkout { Id = 8, MaterialId = 12, PatronId = 6, CheckoutDate = new DateTime(2023, 12, 13), ReturnDate = null, Paid = true},
            new Checkout { Id = 9, MaterialId = 5, PatronId = 3, CheckoutDate = new DateTime(2023, 10, 13), ReturnDate = null, Paid = false},
            new Checkout { Id = 10, MaterialId = 9, PatronId = 5, CheckoutDate = new DateTime(2023, 11, 02), ReturnDate = null, Paid = true}
        });
    }
}
