
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

/* 
IDbConnection is an interface from the ADO.NET library  in the System.Data namespace that defines the basic functionality 
required for a database connection. It provides a generic way to work with database connections, 
allowing you to use various types of database connections (like SQL Server, Oracle, MySQL) 
without being tied to a specific implementation.
SqlConnection is a class that implements the IDbConnection interface
*/
namespace DotnetAPI.Data {
    public class DataContextDapper{

        private IConfiguration _config;
        /// <summary>
        /// in ASP.NET Core, IConfiguration is injected automatically by the framework through dependency 
        /// injection (DI). When you define a constructor like this in your UserController class, ASP.NET 
        /// Core automatically resolves and provides the IConfiguration instance when the controller is 
        /// instantiated.
        /// </summary>
        /// <param name="config"></param>
        public DataContextDapper(IConfiguration config)
        {
            _config = config;

        }



        public IEnumerable<T> LoadData<T>(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }
        
        //return numbers of lines affacted(add/update/delete operation to db)
        public int ExecuteSqlWithRowCount(string sql) {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }



        




        

    }
}
