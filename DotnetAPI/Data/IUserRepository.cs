using DotnetAPI.Model;

namespace DotnetAPI.Data
{
    /// <summary>
    /// Interface representing the repository for User-related data operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Persists all changes to the data store.
        /// </summary>
        /// <returns>True if changes were successfully saved, false otherwise.</returns>
        public bool SaveChanges();

        /// <summary>
        /// Adds a new entity to the data store.
        /// </summary>
        /// <typeparam name="T">The type of the entity to add.</typeparam>
        /// <param name="entityToAdd">The entity object to add to the repository.</param>
        public void AddEntity<T>(T entityToAdd);

        /// <summary>
        /// Removes an existing entity from the data store.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove.</typeparam>
        /// <param name="entityToRemove">The entity object to remove from the repository.</param>
        public void RemoveEntity<T>(T entityToRemove);

        /// <summary>
        /// Retrieves all users from the data store.
        /// </summary>
        /// <returns>An enumerable collection of User objects.</returns>
        public IEnumerable<User> GetUsers();

        /// <summary>
        /// Retrieves a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The User object that matches the provided userId.</returns>
        public User GetSingleUser(int userId);

        /// <summary>
        /// Retrieves the salary details of a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose salary information to retrieve.</param>
        /// <returns>The UserSalary object for the specified user.</returns>
        public UserSalary GetSingleUserSalary(int userId);

        /// <summary>
        /// Retrieves the job information of a single user by their userId.
        /// </summary>
        /// <param name="userId">The ID of the user whose job information to retrieve.</param>
        /// <returns>The UserJobInfo object for the specified user.</returns>
        public UserJobInfo GetSingleUserJobInfo(int userId);
    }
}
