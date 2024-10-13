
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IMapper _mapper;

    public UserEFController(IConfiguration config){
        _entityFramework = new DataContextEF(config);
        //map UserToAddDto to User, because entityframework recognize only User
        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto, User>();
        }));

    }


    [HttpGet("GetSingeleUser/{userId}")]
    //public IActionResult Test()
    public User GetUsers(int userId)
    {
        User? user = _entityFramework.User!.Where(u => u.UserId == userId).FirstOrDefault<User>();
        if (user != null) {
            return user;
        }
        throw new Exception("Fail to find user!");

    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetAllUsers()
    {

        IEnumerable<User> users = _entityFramework.User!.ToList<User>();
        if (users != null) {
            return users;
        }
        throw new Exception("Fail to find users!");
        
    }
   
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _entityFramework.User!
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault<User>();
            
        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            } 

            throw new Exception("Failed to Update User");
        }
        
        throw new Exception("Failed to update User");

    }


    // [HttpPost("AddUser")]
    // public IActionResult AddUser(UserToAddDto user)
    // {
    //     //create a User object without explicitly providing a UserId
    //     User userDb = new User();
    //     userDb.Active = user.Active;
    //     userDb.FirstName = user.FirstName;
    //     userDb.LastName = user.LastName;
    //     userDb.Email = user.Email;
    //     userDb.Gender = user.Gender;

        
    //     _entityFramework.Add(userDb);
    //     if (_entityFramework.SaveChanges() > 0)
    //     {
    //         return Ok();
    //     } 

    //     throw new Exception("Failed to Add User");

    // }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);
        _entityFramework.Add(userDb);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Add User");

    }



    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.User!
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();
            
        if (userDb != null)
        {
            _entityFramework.User!.Remove(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            } 

            throw new Exception("Failed to Delete User");
        }
        throw new Exception("Failed to Delete User");

    }
    

}

