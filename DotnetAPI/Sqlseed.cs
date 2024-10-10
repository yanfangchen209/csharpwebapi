// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Numerics;
// using Microsoft.Extensions.Configuration;
// using Newtonsoft.Json;
// using Microsoft.Data.SqlClient;
// using System.Data;
// using System.Globalization;
// using DotnetAPI.Data;
// using DotnetAPI.Model;

// namespace DotnetAPI
// {
//     /// <summary>
//     /// Class responsible for seeding the database with initial data.
//     /// </summary>
//     public class Sqlseed
//     {
//         /// <summary>
//         /// Main entry point for the application. 
//         /// Reads configuration settings, creates database tables, and populates them with data from JSON files.
//         /// </summary>
//         /// <param name="args">Command-line arguments (not used).</param>
//         public static void Main(string[] args)
//         {
//             // Build configuration from appsettings.json
//             IConfiguration config = new ConfigurationBuilder()
//                     .AddJsonFile("appsettings.json")
//                     .Build();

//             // Initialize the data context for Dapper
//             DataContextDapperSqlseed dataContextDapper = new DataContextDapperSqlseed(config);

//             // Read and execute SQL script for creating users table
//             string tableCreateSql = System.IO.File.ReadAllText("Users.sql");
//             Console.WriteLine(tableCreateSql);
//             dataContextDapper.ExecuteSQL(tableCreateSql);
            
//             // Read user data from JSON file
//             string usersJson = System.IO.File.ReadAllText("Users.json");
//             IEnumerable<Users>? users = JsonConvert.DeserializeObject<IEnumerable<Users>>(usersJson);

//             // If users data is available, insert it into the database
//             if (users != null)
//             {
//                 using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
//                 {
//                     string sql = "SET IDENTITY_INSERT DotnetAPISchema.Users ON;"
//                                 + "INSERT INTO DotnetAPISchema.Users (UserId, FirstName, LastName, Email, Gender, Active) VALUES";
                    
//                     // Iterate over each user and construct the SQL insert statement
//                     foreach (Users singleUser in users)
//                     {
//                         // Prepare SQL values while handling potential SQL injection issues by by escaping single quotes in string data.
//                         string sqlToAdd = "(" + singleUser.UserId
//                                         + ", '" + singleUser.FirstName?.Replace("'", "''")
//                                         + "', '" + singleUser.LastName?.Replace("'", "''")
//                                         + "', '" + singleUser.Email?.Replace("'", "''")
//                                         + "', '" + singleUser.Gender
//                                         + "', '" + singleUser.Active
//                                         + "'),";

//                         ///Check if the SQL statement exceeds 4000 characters, and execute if it does
//                         ///Constructs SQL insert commands for bulk insertion while monitoring the length to avoid exceeding SQL Server limits.
//                         ///execute SQL commands in batches when necessary.*/
//                         if ((sql + sqlToAdd).Length > 4000)
//                         {
//                             dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                             sql = "SET IDENTITY_INSERT DotnetAPISchema.Users ON;"
//                                 + "INSERT INTO DotnetAPISchema.Users (UserId, FirstName, LastName, Email, Gender, Active) VALUES";
//                         }
//                         sql += sqlToAdd;
//                     }
//                     // Execute any remaining SQL statement
//                     dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                 }
//             }
//             // Disable IDENTITY_INSERT for the Users table
//             dataContextDapper.ExecuteSQL("SET IDENTITY_INSERT DotnetAPISchema.Users OFF");

//             // Read user salary data from JSON file
//             string userSalaryJson = System.IO.File.ReadAllText("UserSalary.json");
//             IEnumerable<UserSalary>? userSalary = JsonConvert.DeserializeObject<IEnumerable<UserSalary>>(userSalaryJson);

//             // Truncate UserSalary table to remove existing data
//             dataContextDapper.ExecuteSQL("TRUNCATE TABLE DotnetAPISchema.UserSalary");

//             // If user salary data is available, insert it into the database
//             if (userSalary != null)
//             {
//                 using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
//                 {
//                     string sql = "INSERT INTO DotnetAPISchema.UserSalary (UserId, Salary) VALUES";
                    
//                     // Iterate over each user's salary and construct the SQL insert statement
//                     foreach (UserSalary singleUserSalary in userSalary)
//                     {
//                         // Prepare SQL values while formatting the salary to two decimal places
//                         string sqlToAdd = "(" + singleUserSalary.UserId
//                                         + ", '" + singleUserSalary.Salary.ToString("0.00", CultureInfo.InvariantCulture)
//                                         + "'),";
//                         // Check if the SQL statement exceeds 4000 characters, and execute if it does
//                         if ((sql + sqlToAdd).Length > 4000)
//                         {
//                             dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                             sql = "INSERT INTO DotnetAPISchema.UserSalary (UserId, Salary) VALUES";
//                         }
//                         sql += sqlToAdd;
//                     }
//                     // Execute any remaining SQL statement
//                     dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                 }
//             }

//             // Read user job information data from JSON file
//             string userJobInfoJson = System.IO.File.ReadAllText("UserJobInfo.json");
//             IEnumerable<UserJobInfo>? userJobInfo = JsonConvert.DeserializeObject<IEnumerable<UserJobInfo>>(userJobInfoJson);

//             // Truncate UserJobInfo table to remove existing data
//             dataContextDapper.ExecuteSQL("TRUNCATE TABLE DotnetAPISchema.UserJobInfo");

//             // If user job information data is available, insert it into the database
//             if (userJobInfo != null)
//             {
//                 using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
//                 {
//                     string sql = "INSERT INTO DotnetAPISchema.UserJobInfo (UserId, Department, JobTitle) VALUES";
                    
//                     // Iterate over each user's job information and construct the SQL insert statement
//                     foreach (UserJobInfo singleUserJobInfo in userJobInfo)
//                     {
//                         string sqlToAdd = "(" + singleUserJobInfo.UserId
//                                         + ", '" + singleUserJobInfo.Department
//                                         + "', '" + singleUserJobInfo.JobTitle
//                                         + "'),";
//                         // Check if the SQL statement exceeds 4000 characters, and execute if it does
//                         if ((sql + sqlToAdd).Length > 4000)
//                         {
//                             dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                             sql = "INSERT INTO DotnetAPISchema.UserJobInfo (UserId, Department, JobTitle) VALUES";
//                         }
//                         sql += sqlToAdd;
//                     }
//                     // Execute any remaining SQL statement
//                     dataContextDapper.ExecuteProcedureMulti(sql.Trim(','), dbConnection);
//                 }
//             }
            
//             // Indicate that the SQL seeding process is complete
//             Console.WriteLine("SQL Seed Completed Successfully");
//         }
//     }
// }
