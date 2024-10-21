using DotnetAPI.Model;

namespace DotnetAPI.Data
{
    /// <summary>
    /// Repository class for handling User-related data operations using Entity Framework.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _entityFramework;

        /// <summary>
        /// Constructor for UserRepository
        /// </summary>
        /// <param name="config">Configuration object for initializing the data context.</param>
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        /// <summary>
        /// Saves all changes made in the current data context.
        /// </summary>
        /// <returns>True if the save was successful, otherwise false.</returns>
        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        /// <summary>
        /// Adds a new entity to the data store.
        /// </summary>
        /// <typeparam name="T">The type of the entity to add.</typeparam>
        /// <param name="entityToAdd">The entity object to add to the repository.</param>
        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        /// <summary>
        /// Removes an existing entity from the data store.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove.</typeparam>
        /// <param name="entityToRemove">The entity object to remove from the repository.</param>
        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        /// <summary>
        /// Retrieves all users from the data store.
        /// </summary>
        /// <returns>An enumerable collection of User objects.</returns>
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.User.ToList<User>();
            return users;
        }

        /// <summary>
        /// Retrieves a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The User object that matches the provided userId.</returns>
        public User GetSingleUser(int userId)
        {
            User? user = _entityFramework.User.Where(u => u.UserId == userId).FirstOrDefault<User>();
            if (user != null)
            {
                return user;
            }
            throw new Exception("Failed to Get User");
        }

        /// <summary>
        /// Retrieves the salary details of a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose salary information to retrieve.</param>
        /// <returns>The UserSalary object for the specified user.</returns>
        public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }
            throw new Exception("Failed to Get User Salary");
        }

        /// <summary>
        /// Retrieves the job information of a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose job information to retrieve.</param>
        /// <returns>The UserJobInfo object for the specified user.</returns>
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }
            throw new Exception("Failed to Get User Job Info");
        }
    }
}
