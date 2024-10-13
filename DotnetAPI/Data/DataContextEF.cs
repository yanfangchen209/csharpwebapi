using DotnetAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

/// <summary>
/// The DataContextEF class is responsible for configuring and managing the database context.
/// It uses Entity Framework to interact with the database and is configured to use SQL Server.
/// </summary>

namespace DotnetAPI.Data
{
    /// <summary>
    /// DataContextEF class inherits from DbContext and defines DbSet properties 
    /// for accessing User, UserJobInfo, and UserSalary entities.
    /// </summary>
    public class DataContextEF : DbContext
    {
        /// <summary>
        /// Holds the configuration settings, including the connection string.
        /// </summary>
        private IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the DataContextEF class, passing the configuration settings.
        /// </summary>
        /// <param name="config">The application configuration, typically containing the connection string.</param>
        public DataContextEF(IConfiguration config) {
            _config = config;
        }

        /// <summary>
        /// Represents the Users table in the database.
        /// </summary>
        public DbSet<User>? User { get; set; }

        /// <summary>
        /// Represents the UserJobInfo table in the database.
        /// </summary>
        public DbSet<UserJobInfo>? UserJobInfo { get; set; }

        /// <summary>
        /// Represents the UserSalary table in the database.
        /// </summary>
        public DbSet<UserSalary>? UserSalary { get; set; }

        /// <summary>
        /// Configures the database connection to use SQL Server.
        /// If the connection string is not already configured, it retrieves the connection string from the configuration.
        /// </summary>
        /// <param name="options">Options builder to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                // Configures the context to use SQL Server with the specified connection string.
                // The '!' operator suppresses the null warning for the connection string.
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection")!, 
                    options => options.EnableRetryOnFailure());
            }
        }

        /// <summary>
        /// Defines the model creation, setting schema and primary keys for each entity.
        /// </summary>
        /// <param name="modelBuilder">The model builder to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set default schema for the tables
            modelBuilder.HasDefaultSchema("DotnetAPISchema");

            // Configure the Users table with its schema and primary key
            modelBuilder.Entity<User>()
                .ToTable("Users", "DotnetAPISchema")
                .HasKey(u => u.UserId);

            // Configure UserSalary table with primary key
            modelBuilder.Entity<UserSalary>()
                .HasKey(u => u.UserId);

            // Configure UserJobInfo table with primary key
            modelBuilder.Entity<UserJobInfo>()
                .HasKey(u => u.UserId);
        }
    }
}
