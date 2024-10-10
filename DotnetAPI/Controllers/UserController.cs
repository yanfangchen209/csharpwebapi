
using DotnetAPI.Data;
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
    [HttpGet("GetUsers/{userId}")]
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


    [HttpGet("GetUsers")]
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
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user){
                string editUsersql = @"
                    UPDATE DotnetAPISchema.Users
                        SET [FirstName] = '" + user.FirstName + 
                            "', [LastName] = '" + user.LastName +
                            "', [Email] = '" + user.Email + 
                            "', [Gender] = '" + user.Gender + 
                            "', [Active] = '" + user.Active + 
                        "' WHERE UserId = " + user.UserId;

                Console.WriteLine(editUsersql);
                if(_dapper.ExecuteSql(editUsersql)){
                    //Ok() is inherited from controllerbase class
                    return Ok();
                }
                throw new Exception("Edit user failed");

    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(User user)
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




}

