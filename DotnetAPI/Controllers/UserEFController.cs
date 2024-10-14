using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            cfg.CreateMap<User, User>().ReverseMap();
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();

        }));

    }



    [HttpGet("GetSingeleUser/{userId}")]
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
    
    //implement editing without automapper
    // [HttpPut("EditUser")]
    // public IActionResult EditUser(User user)
    // {
    //     User? userDb = _entityFramework.User!
    //         .Where(u => u.UserId == user.UserId)
    //         .FirstOrDefault<User>();
            
    //     if (userDb != null)
    //     {
    //         userDb.Active = user.Active;
    //         userDb.FirstName = user.FirstName;
    //         userDb.LastName = user.LastName;
    //         userDb.Email = user.Email;
    //         userDb.Gender = user.Gender;
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok();
    //         } 

    //         throw new Exception("Failed to Update User");
    //     }
        
    //     throw new Exception("Failed to update User");

    // }

    //use automapper for editing 
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userToUpdate = _entityFramework.User!
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            _mapper.Map(user, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating User failed on save");
        }

    
        throw new Exception("Failed to update User");

    }

    //without mapping, we have to manually setting each property
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

    //user automapper to map UserToAddDto to User
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

     //
     [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalaryEF(int userId)
    {
        return _entityFramework.UserSalary!
            .Where(u => u.UserId == userId)
            .ToList();
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalaryEf(UserSalary userForInsert)
    {
        _entityFramework.UserSalary!.Add(userForInsert);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Adding UserSalary failed on save");
    }


    [HttpPut("UserSalary")]
    public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
    {
        UserSalary? userToUpdate = _entityFramework.UserSalary!
            .Where(u => u.UserId == userForUpdate.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            //use property setting: userToUpdate.salary = userForUpdate.salary;
            _mapper.Map(userForUpdate, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to Update");
    }


    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEf(int userId)
    {
        UserSalary? userToDelete = _entityFramework.UserSalary!
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

        if (userToDelete != null)
        {
            _entityFramework.UserSalary!.Remove(userToDelete);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Deleting UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to delete");
    }


    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
    {
        return _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .ToList();
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
    {
        _entityFramework.UserJobInfo.Add(userForInsert);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Adding UserJobInfo failed on save");
    }

/// <summary>
/// The mapping configuration cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap() is necessary
/// to tell AutoMapper how to copy the properties from userForUpdate to userToUpdate. Without it, 
/// AutoMapper won't know how to handle mapping between two objects of the same type.
/// </summary>
/// <param name="userForUpdate"></param>
/// <returns></returns>
/// <exception cref="Exception"></exception>
    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
    {
        UserJobInfo? userToUpdate = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userForUpdate.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            _mapper.Map(userForUpdate, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to Update");
    }


    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEf(int userId)
    {
        UserJobInfo? userToDelete = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

        if (userToDelete != null)
        {
            _entityFramework.UserJobInfo.Remove(userToDelete);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Deleting UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to delete");
    }
    

}