using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Marks the class as a controller, which handles API requests
[ApiController]
// Defines the route for the controller, using the controller's name-"User"
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    /// <summary>
    /// Constructor to initialize the UserController with a Dapper DataContext.
    /// </summary>
    /// <param name="config">The configuration used to connect to the database.</param>
    public UserController(IConfiguration config)
    {
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    /// <summary>
    /// Test connection to the database by getting the current server time.
    /// </summary>
    /// <returns>Current DateTime from the server.</returns>
    [HttpGet("testconnection")]
    public DateTime GetCurrentTime()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    /// <summary>
    /// Gets a single user by their userId.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The User object with the specified userId.</returns>
    [HttpGet("GetSingleUser/{userId}")]
    // public IActionResult Test()
    public User GetUsers(int userId)
    {
        string sqlSelectSingleUser = @"
            SELECT
                UserId,
                FirstName,
                LastName,
                Email,
                Gender,
                Active
            FROM DotnetAPIschema.users where UserId = " + userId.ToString();
        return _dapper.LoadDataSingle<User>(sqlSelectSingleUser);
    }

    /// <summary>
    /// Retrieves the salary of a single user by their userId.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The UserSalary object for the specified user.</returns>
    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {
        string sql = @"
            SELECT UserSalary.UserId,
                   UserSalary.Salary
            FROM DotnetAPISchema.UserSalary
            WHERE UserId = " + userId.ToString();
        return _dapper.LoadDataSingle<UserSalary>(sql);
    }

    /// <summary>
    /// Retrieves job information for a single user by their userId.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The UserJobInfo object for the specified user.</returns>
    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        string sql = @"
            SELECT UserJobInfo.UserId,
                   UserJobInfo.JobTitle,
                   UserJobInfo.Department
            FROM DotnetAPISchema.UserJobInfo
            WHERE UserId = @UserId";

        var parameters = new { UserId = userId };

        return _dapper.LoadDataSingleWithParameter<UserJobInfo>(sql, parameters);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>An enumerable list of User objects.</returns>
    [HttpGet("GetAllUsers")]
    public IEnumerable<User> GetAllUsers()
    {
        string sqlSelectAllUsers = @"
            SELECT
                UserId,
                FirstName,
                LastName,
                Email,
                Gender,
                Active
            FROM DotnetAPIschema.users";
        return _dapper.LoadData<User>(sqlSelectAllUsers);
    }

    /// <summary>
    /// Retrieves salary information for all users.
    /// </summary>
    /// <returns>An enumerable list of UserSalary objects.</returns>
    [HttpGet("GetAllUserSalary")]
    public IEnumerable<UserSalary> GetAllUserSalary()
    {
        string sql = @"
            SELECT
                UserId,
                Salary
            FROM DotnetAPISchema.UserSalary";
        return _dapper.LoadData<UserSalary>(sql);
    }

    /// <summary>
    /// Retrieves job information for all users.
    /// </summary>
    /// <returns>An enumerable list of UserJobInfo objects.</returns>
    [HttpGet("GetAllUserJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {
        string sql = @"
            SELECT UserJobInfo.UserId,
                   UserJobInfo.JobTitle,
                   UserJobInfo.Department
            FROM DotnetAPISchema.UserJobInfo";
        return _dapper.LoadData<UserJobInfo>(sql);
    }

    /*
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string editUserSql = @"
            UPDATE DotnetAPISchema.Users
            SET [FirstName] = '" + user.FirstName.Replace("'", "''") + 
                "', [LastName] = '" + user.LastName.Replace("'", "''") +
                "', [Email] = '" + user.Email.Replace("'", "''") + 
                "', [Gender] = '" + user.Gender.Replace("'", "''") + 
                "', [Active] = '" + user.Active + 
            "' WHERE UserId = " + user.UserId;

        Console.WriteLine(editUserSql);
        if (_dapper.ExecuteSql(editUserSql))
        {
            return Ok();
        }
        throw new Exception("Edit user failed");
    }
    */
    
    /// <summary>
    /// Updates the user information using parameterized SQL to prevent SQL injection.
    /// </summary>
    /// <param name="user">The User object containing updated information.</param>
    /// <returns>Returns OK if the update is successful, otherwise returns an error status.</returns>
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string editUserSql = @"
            UPDATE DotnetAPISchema.Users
            SET FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                Gender = @Gender,
                Active = @Active
            WHERE UserId = @UserId";

        var parameters = new
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Gender = user.Gender,
            Active = user.Active,
            UserId = user.UserId
        };

        try
        {
            bool rowsAffected = _dapper.ExecuteSqlWithParameter(editUserSql, parameters);
            if (rowsAffected)
            {
                return Ok(); // User update successful
            }
            else
            {
                return NotFound("User not found"); // UserId does not exist
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating user: " + ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Updates the salary of a user by their userId.
    /// </summary>
    /// <param name="userSalary">The UserSalary object containing updated salary information.</param>
    /// <returns>Returns OK if the update is successful.</returns>
    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"
            UPDATE DotnetAPISchema.UserSalary
            SET Salary = @Salary
            WHERE UserId = @UserId";

        var parameters = new
        {
            UserId = userSalary.UserId,
            Salary = userSalary.Salary
        };

        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            return Ok(userSalary);
        }
        throw new Exception("Edit user salary failed");
    }

    /// <summary>
    /// Updates the job information for a user by their userId.
    /// </summary>
    /// <param name="userJobInfo">The UserJobInfo object containing updated job details.</param>
    /// <returns>Returns OK if the update is successful.</returns>
    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
            UPDATE DotnetAPISchema.UserJobInfo
            SET JobTitle = @JobTitle,
                Department = @Department
            WHERE UserId = @UserId";

        var parameters = new
        {
            UserId = userJobInfo.UserId,
            JobTitle = userJobInfo.JobTitle,
            Department = userJobInfo.Department
        };

        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            return Ok(userJobInfo);
        }
        throw new Exception("Edit user job info failed");
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The UserToAddDto object containing user details to add.</param>
    /// <returns>Returns OK if the user is successfully added.</returns>
    //database will automatically generate user id, we don't need automapper if we use paramterized query
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"INSERT INTO DotnetAPISchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName + 
                "', '" + user.LastName +
                "', '" + user.Email + 
                "', '" + user.Gender + 
                "', '" + user.Active + 
            "')";
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Add User");
    }

    // Requires the userId to be provided for the salary insertion
/// <summary>
/// Adds a new salary record for a user.
/// </summary>
/// <param name="userSalaryForInsert">The UserSalary object containing the user ID and the salary to be inserted.</param>
/// <returns>Returns OK with the inserted UserSalary object if successful, otherwise throws an exception.</returns>
[HttpPost("AddUserSalary")]
public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
{
    string sql = @"
        INSERT INTO DotnetAPISchema.UserSalary (
            UserId,
            Salary
        ) VALUES (@UserId, @Salary)";

    var parameters = new
    {
        UserId = userSalaryForInsert.UserId,
        Salary = userSalaryForInsert.Salary
    };

    // Execute the SQL command with parameters and return the inserted salary object
    if (_dapper.ExecuteSqlWithParameter(sql, parameters))
    {
        // This sends an HTTP 200 OK response with the userSalaryForInsert object as the response body.
        return Ok(userSalaryForInsert); 
    }
    throw new Exception("Adding User Salary failed on save");
}

// Requires the userId to be provided for the job info insertion
/// <summary>
/// Adds new job information for a user.
/// </summary>
/// <param name="userJobInfo">The UserJobInfo object containing the user ID, job title, and department to be inserted.</param>
/// <returns>Returns OK with the inserted UserJobInfo object if successful, otherwise throws an exception.</returns>
[HttpPost("AddUserJobInfo")]
public IActionResult PostUserJobInfo(UserJobInfo userJobInfo)
{
    string sql = @"
        INSERT INTO DotnetAPISchema.UserJobInfo (
            UserId,
            JobTitle,
            Department
        ) VALUES (@UserId, @JobTitle, @Department)";

    var parameters = new
    {
        UserId = userJobInfo.UserId,
        JobTitle = userJobInfo.JobTitle,
        Department = userJobInfo.Department
    };

    // Execute the SQL command with parameters and return the inserted job info object
    if (_dapper.ExecuteSqlWithParameter(sql, parameters))
    {
        return Ok(userJobInfo); // Return the inserted job info object as the response body
    }
    throw new Exception("Adding User jobinfo failed on save");
}

/// <summary>
/// Deletes a user by their userId.
/// </summary>
/// <param name="userId">The ID of the user to be deleted.</param>
/// <returns>Returns OK if the deletion is successful, otherwise throws an exception.</returns>
[HttpDelete("DeleteUser/{userId}")]
public IActionResult DeleteUser(int userId)
{
    string sql = @"
        DELETE FROM DotnetAPISchema.Users 
            WHERE UserId = " + userId.ToString();
    
    Console.WriteLine(sql);

    // Execute the SQL delete command and return OK if successful
    if (_dapper.ExecuteSql(sql))
    {
        return Ok();
    } 

    throw new Exception("Failed to Delete User");
}

/// <summary>
/// Deletes a user's salary record by their userId.
/// </summary>
/// <param name="userId">The ID of the user whose salary is to be deleted.</param>
/// <returns>Returns OK with the userId if the deletion is successful, otherwise throws an exception.</returns>
[HttpDelete("DeleteUserSalary/{userId}")]
public IActionResult DeleteUserSalary(int userId)
{
    string sql = @"
        DELETE FROM DotnetAPISchema.UserSalary
            WHERE UserId = " + userId.ToString();

    // Execute the SQL delete command and return OK with the userId if successful
    if (_dapper.ExecuteSql(sql))
    {
        return Ok(userId); 
    } 

    throw new Exception("Failed to Delete UserSalary");
}

/// <summary>
/// Deletes a user's job information by their userId.
/// </summary>
/// <param name="userId">The ID of the user whose job information is to be deleted.</param>
/// <returns>Returns OK with the userId if the deletion is successful, otherwise throws an exception.</returns>
[HttpDelete("DeleteUserJobInfo/{userId}")]
public IActionResult DeleteUserJobInfo(int userId)
{
    string sql = @"
        DELETE FROM DotnetAPISchema.UserJobInfo
        WHERE UserId = @UserId";

    var parameters = new
    {
        UserId = userId,
    };

    // Execute the SQL delete command with parameters and return OK with the userId if successful
    if (_dapper.ExecuteSqlWithParameter(sql, parameters))
    {
        return Ok(userId);
    }
    throw new Exception("Fail to delete userJobinfo");
}

}

