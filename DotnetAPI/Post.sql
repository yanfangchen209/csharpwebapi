USE DotnetAPI

CREATE TABLE DotnetAPISchema.Posts (
    PostId INT IDENTITY(1, 1),
    UserId INT,
    PostTitle NVARCHAR(255),
    PostContent NVARCHAR(MAX),
    PostCreated DATETIME,
    PostUpdated DATETIME
)

//sort posts records based on UserId, then postId, because search post by UserId is frequent
CREATE CLUSTERED INDEX Posts_UserId_PostId ON DotnetAPISchema.Posts(UserId, PostId)