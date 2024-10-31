using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers{

[Authorize]//this attribute tell app to validate token when user try to access this controller
[ApiController]
[Route("[controller]")]
    public class AuthController: ControllerBase {
        DataContextDapper _dapper;
        AuthHelper _authHelper;
        public AuthController (IConfiguration config) {
            _dapper  = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }
        
        [AllowAnonymous] //this attribute tell app don't need token to access this endpoint
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM DotnetAPISchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = _authHelper.GenerateSaltForPassword();

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO DotnetAPISchema.Auth ([Email], [PasswordHash], [PasswordSalt]) 
                        VALUES (@Email, @PasswordHash, @PasswordSalt)";
                    //we can also use ExecuteSqlWithParameter(sql, parameterobject) if we have DTO with fields of email, passwordhash, passwordsalt
                    List<SqlParameter> sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Email", SqlDbType.NVarChar) { Value = userForRegistration.Email },
                        new SqlParameter("@PasswordHash", SqlDbType.VarBinary) { Value = passwordHash },
                        new SqlParameter("@PasswordSalt", SqlDbType.VarBinary) { Value = passwordSalt }
                    };

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        
                        string sqlAddUser = @"
                            INSERT INTO DotnetAPISchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (" +
                                "'" + userForRegistration.FirstName + 
                                "', '" + userForRegistration.LastName +
                                "', '" + userForRegistration.Email + 
                                "', '" + userForRegistration.Gender + 
                                "', 1)";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not match!");
        }

        //if email exisit->get salt and passwordhash from db->if passwod right->get userid from db(given email)
        //->create token with userid and tokenkey from appsettting file and return token;
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            //retrieve passwordhash, salt from db
            string sqlForHashAndSalt = @"SELECT 
                    [PasswordHash],
                    [PasswordSalt] FROM DotnetAPISchema.Auth WHERE Email = '" +
                    userForLogin.Email + "'";

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            if (userForConfirmation != null) {
                //hash user entered password
               byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);
               //compare entered passwordhash to passwordhash from db
               for (int index = 0; index < passwordHash.Length; index++)
               {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index]){
                    return StatusCode(401, "Incorrect password!");
                }
               }

               //get userid from db using entered email, userid will be used to create token
                string userIdSql = @"
                    SELECT UserId FROM DotnetAPISchema.Users WHERE Email = '" +
                    userForLogin.Email + "'";
                
                //after successfully log in, create token using userid and tokenkey and return 
                int userId = _dapper.LoadDataSingle<int>(userIdSql);
                return Ok(new Dictionary<string, string>
                {
                    {"token", _authHelper.CreateToken(userId)}
                });
            }
            throw new Exception("User doesn't exist!");

        }

        //after login in, if user want refresh token: use Controllerbase to extract userid from token and create a new token
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value + "");
            
            // string userIdSql = @"
            //     SELECT UserId FROM DotnetAPISchema.Users WHERE UserId = '" +
            //     User.FindFirst("userId")?.Value + "'";
            
            // int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }


    }
}








