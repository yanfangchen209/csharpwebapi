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

    // Constructor that initializes the entity framework context and AutoMapper for object mapping.
    // Maps UserToAddDto to User because Entity Framework recognizes only User.
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        
        // Map UserToAddDto to User, because Entity Framework only recognizes the User entity
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            //used for user editting
            cfg.CreateMap<User, User>().ReverseMap();
            //used for usersalary editting
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            //used for userjobinfo editting
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
        }));
    }

    // Get a single user by their userId
    /// <summary>
    /// Retrieves a user by their UserId.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The User object if found, otherwise throws an exception.</returns>
    [HttpGet("GetSingeleUser/{userId}")]
    public User GetUsers(int userId)
    {
        User? user = _entityFramework.User!.Where(u => u.UserId == userId).FirstOrDefault<User>();
        if (user != null)
        {
            return user;
        }
        throw new Exception("Fail to find user!");
    }

    // Get all users from the database
    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <returns>A list of User objects if found, otherwise throws an exception.</returns>
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetAllUsers()
    {
        IEnumerable<User> users = _entityFramework.User!.ToList<User>();
        if (users != null)
        {
            return users;
        }
        throw new Exception("Fail to find users!");
    }


    /// <summary>
    /// Updates an existing user in the database using AutoMapper.
    /// </summary>
    /// <param name="user">The User object containing updated data.</param>
    /// <returns>Returns an HTTP OK status if successful, otherwise throws an exception.</returns>
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userToUpdate = _entityFramework.User!
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            // Map the incoming User object to the existing user in the database
            _mapper.Map(user, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating User failed on save");
        }

        throw new Exception("Failed to update User");
    }

    // Manually edit user without AutoMapper (commented out for future reference)
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

    // Use AutoMapper to add a new user
    /// <summary>
    /// Adds a new user to the database using AutoMapper.
    /// </summary>
    /// <param name="user">The UserToAddDto object containing the user's data.</param>
    /// <returns>Returns an HTTP OK status if the user is added successfully, otherwise throws an exception.</returns>
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        // Map the UserToAddDto to the User entity
        User userDb = _mapper.Map<User>(user);
        _entityFramework.Add(userDb);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }

    // Delete a user by their userId
    /// <summary>
    /// Deletes a user by their UserId.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>Returns an HTTP OK status if successful, otherwise throws an exception.</returns>
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



       /// <summary>
    /// Retrieves the salary details for a specific user based on userId.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of UserSalary objects related to the specified userId.</returns>
    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalaryEF(int userId)
    {
        return _entityFramework.UserSalary!
            .Where(u => u.UserId == userId)
            .ToList();
    }

    /// <summary>
    /// Adds a new UserSalary record to the database.
    /// </summary>
    /// <param name="userForInsert">The UserSalary object containing the salary details to be added.</param>
    /// <returns>Returns OK status if the insertion was successful; otherwise throws an exception.</returns>
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

    /// <summary>
    /// Updates an existing UserSalary record based on the userId.
    /// Uses AutoMapper to map the properties of the updated UserSalary object.
    /// </summary>
    /// <param name="userForUpdate">The UserSalary object containing updated salary details.</param>
    /// <returns>Returns OK status if the update was successful; otherwise throws an exception.</returns>
    [HttpPut("UserSalary")]
    public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
    {
        UserSalary? userToUpdate = _entityFramework.UserSalary!
            .Where(u => u.UserId == userForUpdate.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            // Use property setting to update the UserSalary fields
            _mapper.Map(userForUpdate, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to Update");
    }

    /// <summary>
    /// Deletes a UserSalary record based on the userId.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose salary record is to be deleted.</param>
    /// <returns>Returns OK status if the deletion was successful; otherwise throws an exception.</returns>
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

    /// <summary>
    /// Retrieves the job information for a specific user based on userId.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of UserJobInfo objects related to the specified userId.</returns>
    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
    {
        return _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .ToList();
    }

    /// <summary>
    /// Adds new UserJobInfo record to the database.
    /// </summary>
    /// <param name="userForInsert">The UserJobInfo object containing job information to be added.</param>
    /// <returns>Returns OK status if the insertion was successful; otherwise throws an exception.</returns>
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
    /// Updates an existing UserJobInfo record based on the userId using AutoMapper.
    /// The mapping configuration cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap() is necessary
    /// to tell AutoMapper how to copy the properties from userForUpdate to userToUpdate. Without it,
    /// AutoMapper won't know how to handle mapping between two objects of the same type.
    /// </summary>
    /// <param name="userForUpdate">The UserJobInfo object containing updated job information.</param>
    /// <returns>Returns OK status if the update was successful; otherwise throws an exception.</returns>
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

    /// <summary>
    /// Deletes a UserJobInfo record based on the userId.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose job info record is to be deleted.</param>
    /// <returns>Returns OK status if the deletion was successful; otherwise throws an exception.</returns>
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

