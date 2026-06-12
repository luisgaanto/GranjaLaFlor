using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
// "ApplicationDbContext" in general terms is the main bridge between the application and the database using Entity Framework Core.
//Define how models relate to each other and how they map in the database.
//IMPORTANT: Essentially, Models (roles, users, broods...) define the properties of the table mirrowing the attributes in the DB tables; while ApplicationDbContext holds the tables´s data and connects/interacts with database defining relationship between tables. 
namespace DB_GranjaLaFlor.Data.Context
{
    //DbContext is the EF Core base class used to interact with the database. Class "ApplicationDbContext" inherits from DbContext, meaining all funtionabilities to query DB. 
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            //Sends the config to the base DbContext constructor 
            : base(options) {}
        //It tells EF how to manage tables, map classes (Roles, Users...) to tables within DB. 
        //     public DbSet<Usuario> = Represents a table. It has a collection of ojects. 
        public DbSet<Role> Roles { get; set; }
    }
}
