using DotnetAPI.Data;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase 
    {
        private readonly DataContextDapper _dapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostController"/> class.
        /// </summary>
        /// <param name="config">Configuration object for database connection.</param>
        public PostsController(IConfiguration config) 
        {
            _dapper = new DataContextDapper(config);
        }

        /// <summary>
        /// Adds a new post to the database.
        /// </summary>
        /// <param name="postToAdd">DTO containing post details.</param>
        /// <returns>Status of the addition operation.</returns>
        [HttpPost("UpsertPost")]
        public IActionResult UpsertPost(PostToAddDto postToAdd) 
        {
            string sql = @"
            INSERT INTO DotnetAPISchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES (" + this.User.FindFirst("userId")?.Value
                + ",'" + postToAdd.PostTitle
                + "','" + postToAdd.PostContent
                + "', GETDATE(), GETDATE() )";

            if (_dapper.ExecuteSql(sql)) 
            {
                return Ok(postToAdd);
            }
            throw new Exception("Failed to create a new post!");
        }


        [HttpGet("GetPosts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None") 
        {
            string sql = @"Exec DotnetAPISchema.spPosts_Get";
            string parameters = "";

            if (postId != 0)
            {
                parameters += ", @PostId=" + postId.ToString();
            }
            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += ", @SearchValue='" + searchParam + "'";
            }

            if (parameters.Length > 0)
            { 
                sql += parameters.Substring(1);
            }
                
            return _dapper.LoadData<Post>(sql);
        }

        /// <summary>
        /// Retrieves all posts by a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>List of posts by the user.</returns>
        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts() 
        {
            string sql = @"Exec DotnetAPISchema.spPosts_Get
            @UserId = " + this.User.FindFirst("userId")?.Value;
            IEnumerable<Post> myResult = _dapper.LoadData<Post>(sql);
            return myResult;
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId) 
        {
            string sql = @"DELETE FROM DotnetAPISchema.Posts 
                WHERE PostId = " + postId.ToString() +
                    "AND UserId = " + this.User.FindFirst("userId")?.Value; 

            if (_dapper.ExecuteSql(sql)) 
            {
                return Ok();
            }
            throw new Exception("Failed to delete the post!");
        }
    

}
