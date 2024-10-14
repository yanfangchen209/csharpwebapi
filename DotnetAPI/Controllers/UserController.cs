
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

//Marks the class as a controller, which handles API requests
[ApiController]
//Defines the route for the controller, using the controller's name.
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;

    public UserController(IConfiguration config){
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);

    }

//test conncetion to database
    [HttpGet("testconnection")]
    public DateTime GetCurrentTime()
    {
        
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");

        
    }



    //Maps the method to respond to HTTP GET requests at the specified route.
    //handles GET requests to the /user/GetUsers route.
    [HttpGet("GetSingleUser/{userId}")]
    //public IActionResult Test()
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

    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {

        string sql = @"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  DotnetAPISchema.UserSalary
            WHERE UserId = " + userId.ToString(); 
        return _dapper.LoadDataSingle<UserSalary>(sql);
    }

    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        string sql = @"
            SELECT UserJobInfo.UserId,
                UserJobInfo.JobTitle,
                UserJobInfo.Department
            From DotnetAPISchema.UserJobInfo
            WHERE UserId = @UserId";

        var parameters = new 
        {
            UserId = userId

        };

        return _dapper.LoadDataSingleWithParameter<UserJobInfo>(sql, parameters);

    }


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


    [HttpGet("GetAllUserSalary")]
    public IEnumerable<UserSalary> GetAllUserSalary()
    {

        string sql = @"
            SELECT
                UserId,
                Salary
            FROM DotnetAPIschema.UserSalary";
        return _dapper.LoadData<UserSalary>(sql);
    }

    [HttpGet("GetAllUserJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {

        string sql = @"
            SELECT UserJobInfo.UserId,
                UserJobInfo.JobTitle,
                UserJobInfo.Department
            From DotnetAPISchema.UserJobInfo";

        return _dapper.LoadData<UserJobInfo>(sql);
    }



    /*
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user){
        string editUsersql= @"
        UPDATE DotnetAPISchema.Users
            SET [FirstName] = '" + user.FirstName.Replace("'", "''") + 
                "', [LastName] = '" + user.LastName.Replace("'", "''") +
                "', [Email] = '" + user.Email.Replace("'", "''") + 
                "', [Gender] = '" + user.Gender.Replace("'", "''") + 
                "', [Active] = '" + user.Active + 
            "' WHERE UserId = " + user.UserId;

        Console.WriteLine(editUsersql);
        if(_dapper.ExecuteSql(editUsersql)){
            //Ok() is inherited from controllerbase class
            return Ok();
        }
        throw new Exception("Edit user failed");

    }
    */

 // Use parameterized SQL query to prevent SQL injection
/// <summary>
/// This approach simplifies the code and avoids having to manually handle single quotes or other special characters in user input
/// </summary>
/// <param name="user"></param>
/// <returns></returns>
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

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary){

        string sql = @"
            UPDATE DotnetAPISchema.UserSalary
            SET Salary = @Salary
            WHERE UserId = @UserId";

        var parameters = new 
        {
            UserId = userSalary.UserId,
            Salary = userSalary.Salary

        };

        if(_dapper.ExecuteSqlWithParameter(sql, parameters)){
            
            return Ok(userSalary);
        }
        throw new Exception("Edit usersalary failed");

    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo){

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

        if(_dapper.ExecuteSqlWithParameter(sql, parameters)){
            
            return Ok(userJobInfo);
        }
        throw new Exception("Edit userjobinfo failed");

    }

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

    //have to give userid
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

        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            //This sends an HTTP 200 OK response with the userSalaryForInsert object as the response body. The client will receive the inserted salary data as part of the response.
            return Ok(userSalaryForInsert);
            //return Ok();
        }
        throw new Exception("Adding User Salary failed on save");
    }

    //have to give userid
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

        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            
            return Ok(userJobInfo);
           
        }
        throw new Exception("Adding User jobinfo failed on save");
    }

//when we switch to using parameterized queries, you don’t need .ToString() at all, since Dapper takes care of type conversions and safely injects the values into the query.
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM DotnetAPISchema.Users 
                WHERE UserId = " + userId.ToString();
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");
    }
    
    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @"
            DELETE FROM DotnetAPISchema.UserSalary
                WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userId);
        } 

        throw new Exception("Failed to Delete UserSalary");
    }


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

        if (_dapper.ExecuteSqlWithParameter(sql, parameters))
        {
            
            return Ok(userId);
           
        }
        throw new Exception("Fail to delete userJobinfo");

    }


}





