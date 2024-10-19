using DataInsertProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataInsertProject.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DataModel> DataModels { get; set; }
    }
}
