
using DotnetAPI.Data;
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


    [HttpGet("testconnection")]
    public DateTime GetCurrentTime()
    {
        
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");

        
    }






    //Maps the method to respond to HTTP GET requests at the specified route.
    //handles GET requests to the /user/GetUsers route.
    [HttpGet("GetUsers/{testValue}")]
    //public IActionResult Test()
    public string[] GetUsers(String testValue)
    {
        string[] responseArray = new string[] {
            "test1", 
            "test2",
            testValue
        };
       

        return responseArray;
    }
}

