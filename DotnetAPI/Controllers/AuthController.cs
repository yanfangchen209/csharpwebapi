using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers{

[ApiController]
[Route("[controller]")]
    public class AuthController: ControllerBase {
        DataContextDapper _dapper;
        AuthHelper _authHelper;
        public AuthController (IConfiguration config) {
            _dapper  = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

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



    }
}








