using System.Security.Claims;
using DotnetAPI.Data;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    /// <summary>
    /// Controller for handling post-related operations, including creating, updating, retrieving, and deleting posts.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase 
    {
        private readonly DataContextDapper _dapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsController"/> class.
        /// </summary>
        /// <param name="config">Configuration object for database connection.</param>
        public PostsController(IConfiguration config) 
        {
            _dapper = new DataContextDapper(config);
        }

        /// <summary>
        /// Upserts (inserts or updates) a post. 
        /// If PostId is provided and greater than 0, checks if it exists, and updates the post if found; otherwise, adds a new post.
        /// If PostId is not provided or null, a new post is created.
        /// </summary>
        /// <param name="postToUpsert">The post object to be upserted.</param>
        /// <returns>Status indicating success or failure of the operation.</returns>
        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert) 
        {
            string sql = @"EXEC DotnetAPISchema.spPosts_Upsert
             @PostId = @PostIdParam, 
             @UserId = @UserIdParam, 
             @PostTitle = @PostTitleParam, 
             @PostContent = @PostContentParam";

            var parameters = new {
                PostIdParam = postToUpsert.PostId > 0 ? postToUpsert.PostId : (int?)null,
                UserIdParam = this.User.FindFirst("userId")?.Value,
                PostTitleParam = postToUpsert.PostTitle,
                PostContentParam = postToUpsert.PostContent
            };

            if (_dapper.ExecuteSqlWithParameter(sql, parameters)) 
            {
                return Ok();
            }
            throw new Exception("Failed to update/create a post!");
        }

        /// <summary>
        /// Retrieves posts based on optional PostId, UserId, and search parameters.
        /// Provides default values to allow Swagger UI to generate a default input.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve. Default is 0.</param>
        /// <param name="userId">The ID of the user who created the posts to retrieve. Default is 0.</param>
        /// <param name="searchParam">Search keyword for post content or title. Default is "None".</param>
        /// <returns>A collection of posts that match the criteria.</returns>
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
        /// Retrieves posts created by the currently authenticated user.
        /// </summary>
        /// <returns>A collection of posts created by the authenticated user.</returns>
        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts() 
        {
            string sql = @"Exec DotnetAPISchema.spPosts_Get
            @UserId = " + this.User.FindFirst("userId")?.Value;
            IEnumerable<Post> myResult = _dapper.LoadData<Post>(sql);
            return myResult;
        }

        /// <summary>
        /// Deletes a specified post if it was created by the authenticated user.
        /// </summary>
        /// <param name="postId">The ID of the post to delete.</param>
        /// <returns>Status indicating success or failure of the deletion.</returns>
        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId) 
        {
            string sql = @"DELETE FROM DotnetAPISchema.Posts 
                WHERE PostId = " + postId.ToString() +
                    " AND UserId = " + this.User.FindFirst("userId")?.Value; 

            if (_dapper.ExecuteSql(sql)) 
            {
                return Ok();
            }
            throw new Exception("Failed to delete the post!");
        }
    }
}
