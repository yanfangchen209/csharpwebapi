using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Marks the class as a controller, which handles API requests
[ApiController]
// Defines the route for the controller, using the controller's name-"User"
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    /// <summary>
    /// Constructor to initialize the UserCompleteController with a Dapper DataContext.
    /// </summary>
    /// <param name="config">The configuration used to connect to the database.</param>
    public UserCompleteController(IConfiguration config)
    {
        // Initialize the DataContextDapper with the provided configuration
        _dapper = new DataContextDapper(config);
    }

    /// <summary>
    /// Retrieves a list of users based on the specified user ID and active status.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve. Use 0 to get all users.</param>
    /// <param name="isActive">Indicates whether to return active users.</param>
    /// <returns>An enumerable collection of <see cref="UserComplete"/>.</returns>
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        // Define the SQL command with parameter placeholders.
        string sql = @"EXEC DotnetAPISchema.spUsers_Get 
                       @UserId = @UserIdParam, 
                       @Active = @ActiveParam";

        // Create the parameters object.
        var parameters = new
        {
            UserIdParam = userId != 0 ? userId : (int?)null,  // Use null if userId is 0
            ActiveParam = isActive
        };

        // Load the data using the parameters object
        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameter<UserComplete>(sql, parameters);
        return users;
    }

    /// <summary>
    /// Updates an existing user or adds a new user if they do not exist.
    /// </summary>
    /// <param name="user">The <see cref="UserComplete"/> object containing user details.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPut("UpdateOrAddUser")]
    public IActionResult UpdateOrAddUser(UserComplete user)
    {
        // Example validation checks
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            throw new ArgumentException("First name is required.");
        }
        if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email format.");
        }
        if (user.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        // Define the SQL command for upserting the user
        string sql = @"EXEC DotnetAPISchema.spUser_Upsert
            @FirstName = @FirstNameParam,
            @LastName = @LastNameParam,
            @Email = @EmailParam,
            @Gender = @GenderParam,
            @Active = @ActiveParam,
            @JobTitle = @JobTitleParam,
            @Department = @DepartmentParam,
            @Salary = @SalaryParam,
            @UserId = @UserIdParam";

        // Create the parameters object.
        var parameters = new
        {
            FirstNameParam = user.FirstName,
            LastNameParam = user.LastName,
            EmailParam = user.Email,
            GenderParam = user.Gender,
            ActiveParam = user.Active,
            JobTitleParam = user.JobTitle,
            DepartmentParam = user.Department,
            SalaryParam = user.Salary,
            UserIdParam = user.UserId
        };

        // Execute the SQL command and return Ok if successful
        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to update/add user!");
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to be deleted.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        // Define the SQL command for deleting a user
        string sql = @"EXEC DotnetAPISchema.spUser_Delete 
                @UserId = @UserIdParam";
        var parameter = new { UserIdParam = userId };

        // Execute the SQL delete command and return OK if successful
        if (_dapper.ExecuteSqlWithParameter(sql, parameter))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user");
    }





/*
//(0, active) -return all active users
//(0, inactive) -return all inactive users
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, Boolean isActive)
    {
        string sql = @"EXEC DotnetAPISchema.spUsers_Get";
        string parameters = "";

        
        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }
        if (isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }

        if (parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }
*/

/*if both the UserId and Email do not exist, a new UserId is automatically generated in the 
Users table, and that UserId is then used to insert related data into the UserSalary and 
UserJobInfo tables. This process creates a complete entry for the new user across all 
relevant tables.
//add user if not exist, otherwise update user
    [HttpPut("UpdateOrAddUser")]
    public IActionResult UpdateOrAddUser(UserComplete user)
    {

        string sql = @"EXEC DotnetAPISchema.spUser_Upsert
            @FirstName = '" + user.FirstName + 
            "', @LastName = '" + user.LastName +
            "', @Email = '" + user.Email + 
            "', @Gender = '" + user.Gender + 
            "', @Active = '" + user.Active + 
            "', @JobTitle = '" + user.JobTitle + 
            "', @Department = '" + user.Department + 
            "', @Salary = '" + user.Salary + 
            "', @UserId = " + user.UserId;


       if(_dapper.ExecuteSql(sql)){
            return Ok();
       }

       throw new Exception("Fail to update/add user!");

        
    }
*/


}

