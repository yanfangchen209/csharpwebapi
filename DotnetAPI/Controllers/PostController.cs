using DotnetAPI.Data;
using DotnetAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    /// <summary>
    /// Controller for handling posts. Requires authorization for all actions.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase 
    {
        private readonly DataContextDapper _dapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostController"/> class.
        /// </summary>
        /// <param name="config">Configuration object for database connection.</param>
        public PostController(IConfiguration config) 
        {
            _dapper = new DataContextDapper(config);
        }

        /// <summary>
        /// Adds a new post to the database.
        /// </summary>
        /// <param name="postToAdd">DTO containing post details.</param>
        /// <returns>Status of the addition operation.</returns>
        [HttpPost("AddPost")]
        public IActionResult AddPost(PostToAddDto postToAdd) 
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

        /// <summary>
        /// Retrieves all posts from the database.
        /// </summary>
        /// <returns>List of all posts.</returns>
        [HttpGet("AllPosts")]
        public IEnumerable<Post> GetAllPosts() 
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                From DotnetAPISchema.Posts";
            return _dapper.LoadData<Post>(sql);
        }

        /// <summary>
        /// Retrieves a single post by its ID.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <returns>The requested post.</returns>
        [HttpGet("GetSinglePost/{postId}")]
        public Post GetSinglePostById(int postId) 
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                From DotnetAPISchema.Posts
                Where PostId = " + postId.ToString();
            return _dapper.LoadDataSingle<Post>(sql);
        }

        /// <summary>
        /// Retrieves all posts by a specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>List of posts by the user.</returns>
        [HttpGet("GetPostsByUserId/{userId}")]
        public IEnumerable<Post> GetPostsByUserId(int userId) 
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                From DotnetAPISchema.Posts
                Where UserId = " + userId.ToString();
            return _dapper.LoadData<Post>(sql);
        }

        /// <summary>
        /// Edits an existing post.
        /// </summary>
        /// <param name="postToEdit">DTO containing updated post details.</param>
        /// <returns>Status of the edit operation.</returns>
        [HttpPut("EditPost")]
        public IActionResult EditPost(PostToEditDto postToEdit) 
        {
            string sql = @"UPDATE DotnetAPISchema.Posts 
                        SET PostContent = '" + postToEdit.PostContent + 
                        "', PostTitle = '" + postToEdit.PostTitle + 
                        @"', PostUpdated = GETDATE()
                        WHERE PostId = " + postToEdit.PostId.ToString() +
                        "AND UserId = " + this.User.FindFirst("userId")?.Value;   

            if (_dapper.ExecuteSql(sql)) 
            {
                return Ok(postToEdit);
            }
            throw new Exception("Failed to edit the post!");
        }

        /// <summary>
        /// Deletes a post by ID, only if it belongs to the current user.
        /// </summary>
        /// <param name="postId">ID of the post to delete.</param>
        /// <returns>Status of the deletion operation.</returns>
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

        /// <summary>
        /// Searches for posts containing specific text in the title or content.
        /// </summary>
        /// <param name="searchText">Text to search for.</param>
        /// <returns>List of posts that match the search criteria.</returns>
        [HttpGet("SearchPost/{searchText}")]
        public IEnumerable<Post> SearchPost(string searchText) 
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM DotnetAPISchema.Posts
                    WHERE PostTitle LIKE '%" + searchText + "%'" +
                        " OR PostContent LIKE '%" + searchText + "%'";
                
            return _dapper.LoadData<Post>(sql);
        }
    }
}
