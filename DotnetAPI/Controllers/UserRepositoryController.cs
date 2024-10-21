using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling User-related actions
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserRepoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor for UserRepoController
        /// </summary>
        /// <param name="config">Configuration for the application</param>
        /// <param name="userRepository">Repository interface for user-related operations</param>
        public UserRepoController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;

            // Initialize AutoMapper with mapping configurations for User, UserSalary, and UserJobInfo entities
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDto, User>();
                cfg.CreateMap<User, User>().ReverseMap();
                cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
                cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
            }));
        }

        /// <summary>
        /// Get a single user by userId
        /// </summary>
        /// <param name="userId">ID of the user to retrieve</param>
        /// <returns>User object corresponding to the provided ID</returns>
        [HttpGet("GetSingleUser/{userId}")]
        public User GetUsers(int userId)
        {
            User user = _userRepository.GetSingleUser(userId);
            return user;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all User objects</returns>
        [HttpGet("GetUsers")]
        public IEnumerable<User> GetAllUsers()
        {
            IEnumerable<User> users = _userRepository.GetUsers();
            return users;
        }

        /// <summary>
        /// Edit an existing user
        /// </summary>
        /// <param name="user">User object containing the updated information</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            User? userDb = _userRepository.GetSingleUser(user.UserId);

            if (userDb != null)
            {
                _mapper.Map(user, userDb);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating User failed on save");
            }

            throw new Exception("Failed to update User");
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">DTO object containing the new user's information</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDto user)
        {
            User userDb = _mapper.Map<User>(user);
            _userRepository.AddEntity(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Add User");
        }

        /// <summary>
        /// Delete a user by userId
        /// </summary>
        /// <param name="userId">ID of the user to delete</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _userRepository.GetSingleUser(userId);

            if (userDb != null)
            {
                _userRepository.RemoveEntity(userDb);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Failed to Delete User");
            }
            throw new Exception("Failed to Delete User");
        }

        /// <summary>
        /// Get the salary details of a single user by userId
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>UserSalary object for the specified user</returns>
        [HttpGet("GetSingleUserSalary/{userId}")]
        public UserSalary GetUserSalaryEF(int userId)
        {
            return _userRepository.GetSingleUserSalary(userId);
        }

        /// <summary>
        /// Add a new user salary record
        /// </summary>
        /// <param name="userForInsert">UserSalary object to insert</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPost("AddUserSalary")]
        public IActionResult PostUserSalaryEf(UserSalary userForInsert)
        {
            _userRepository.AddEntity(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        /// <summary>
        /// Edit an existing user salary record
        /// </summary>
        /// <param name="userForUpdate">UserSalary object containing the updated information</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPut("EditUserSalary")]
        public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
        {
            UserSalary? userToUpdate = _userRepository.GetSingleUserSalary(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to Update");
        }

        /// <summary>
        /// Delete a user salary record by userId
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalaryEf(int userId)
        {
            UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity(userToDelete);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to delete");
        }

        /// <summary>
        /// Get the job information of a single user by userId
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>UserJobInfo object for the specified user</returns>
        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetSingleUserJobInfo(userId);
        }

        /// <summary>
        /// Add a new user job info record
        /// </summary>
        /// <param name="userForInsert">UserJobInfo object to insert</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPost("AddUserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
        {
            _userRepository.AddEntity(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }

        /// <summary>
        /// Edit an existing user job info record
        /// </summary>
        /// <param name="userForUpdate">UserJobInfo object containing the updated information</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpPut("EditUserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
        {
            UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }

        /// <summary>
        /// Delete a user job info record by userId
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>HTTP status code indicating success or failure</returns>
        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfoEf(int userId)
        {
            UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfo(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity(userToDelete);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }
    }
}

