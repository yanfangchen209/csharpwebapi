USE DotnetAPI;
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spUsers_Get
/*EXEC DotnetAPISchema.spUsers_Get @UserId=3*/
    @UserId INT = NULL
    , @Active BIT = NULL
AS
BEGIN
    DROP TABLE IF EXISTS #AverageDeptSalary

    SELECT UserJobInfo.Department
        , AVG(UserSalary.Salary) AvgSalary
        INTO #AverageDeptSalary
    FROM DotnetAPISchema.Users AS Users 
        LEFT JOIN DotnetAPISchema.UserSalary AS UserSalary
            ON UserSalary.UserId = Users.UserId
        LEFT JOIN DotnetAPISchema.UserJobInfo AS UserJobInfo
            ON UserJobInfo.UserId = Users.UserId
        GROUP BY UserJobInfo.Department

    CREATE CLUSTERED INDEX cix_AverageDeptSalary_Department ON #AverageDeptSalary(Department)

    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active],
        UserSalary.Salary,
        UserJobInfo.Department,
        UserJobInfo.JobTitle,
        AvgSalary.AvgSalary
    FROM DotnetAPISchema.Users AS Users 
        LEFT JOIN DotnetAPISchema.UserSalary AS UserSalary
            ON UserSalary.UserId = Users.UserId
        LEFT JOIN DotnetAPISchema.UserJobInfo AS UserJobInfo
            ON UserJobInfo.UserId = Users.UserId
        LEFT JOIN #AverageDeptSalary AS AvgSalary
            ON AvgSalary.Department = UserJobInfo.Department
        WHERE Users.UserId = ISNULL(@UserId, Users.UserId)
            AND ISNULL(Users.Active, 0) = COALESCE(@Active, Users.Active, 0)
END
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spUser_Upsert
	@FirstName NVARCHAR(50),
	@LastName NVARCHAR(50),
	@Email NVARCHAR(50),
	@Gender NVARCHAR(50),
	@JobTitle NVARCHAR(50),
	@Department NVARCHAR(50),
    @Salary DECIMAL(18, 4),
	@Active BIT = 1,
	@UserId INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM DotnetAPISchema.Users WHERE UserId = @UserId)
        BEGIN
        IF NOT EXISTS (SELECT * FROM DotnetAPISchema.Users WHERE Email = @Email)
            BEGIN
                DECLARE @OutputUserId INT

                INSERT INTO DotnetAPISchema.Users(
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                ) VALUES (
                    @FirstName,
                    @LastName,
                    @Email,
                    @Gender,
                    @Active
                )

                SET @OutputUserId = @@IDENTITY

                INSERT INTO DotnetAPISchema.UserSalary(
                    UserId,
                    Salary
                ) VALUES (
                    @OutputUserId,
                    @Salary
                )

                INSERT INTO DotnetAPISchema.UserJobInfo(
                    UserId,
                    Department,
                    JobTitle
                ) VALUES (
                    @OutputUserId,
                    @Department,
                    @JobTitle
                )
            END
        END
    ELSE 
        BEGIN
            UPDATE DotnetAPISchema.Users 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Gender = @Gender,
                    Active = @Active
                WHERE UserId = @UserId

            UPDATE DotnetAPISchema.UserSalary
                SET Salary = @Salary
                WHERE UserId = @UserId

            UPDATE DotnetAPISchema.UserJobInfo
                SET Department = @Department,
                    JobTitle = @JobTitle
                WHERE UserId = @UserId
        END
END
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spUser_Delete
    @UserId INT
AS
BEGIN
    DECLARE @Email NVARCHAR(50);

    SELECT  @Email = Users.Email
      FROM  DotnetAPISchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM DotnetAPISchema.UserSalary
     WHERE  UserSalary.UserId = @UserId;

    DELETE  FROM DotnetAPISchema.UserJobInfo
     WHERE  UserJobInfo.UserId = @UserId;

    DELETE  FROM DotnetAPISchema.Users
     WHERE  Users.UserId = @UserId;

    DELETE  FROM DotnetAPISchema.Auth
     WHERE  Auth.Email = @Email;
END;
GO



CREATE OR ALTER PROCEDURE DotnetAPISchema.spPosts_Get
/*EXEC DotnetAPISchema.spPosts_Get @UserId = 1003, @SearchValue='Second'*/
/*EXEC DotnetAPISchema.spPosts_Get @PostId = 2*/
    @UserId INT = NULL
    , @SearchValue NVARCHAR(MAX) = NULL
    , @PostId INT = NULL
AS
BEGIN
    SELECT [Posts].[PostId],
        [Posts].[UserId],
        [Posts].[PostTitle],
        [Posts].[PostContent],
        [Posts].[PostCreated],
        [Posts].[PostUpdated] 
    FROM DotnetAPISchema.Posts AS Posts
        WHERE Posts.UserId = ISNULL(@UserId, Posts.UserId)
            AND Posts.PostId = ISNULL(@PostId, Posts.PostId)
            AND (@SearchValue IS NULL
                OR Posts.PostContent LIKE '%' + @SearchValue + '%'
                OR Posts.PostTitle LIKE '%' + @SearchValue + '%')
END
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spPosts_Upsert
    @UserId INT
    , @PostTitle NVARCHAR(255)
    , @PostContent NVARCHAR(MAX)
    , @PostId INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM DotnetAPISchema.Posts WHERE PostId = @PostId)
        BEGIN
            INSERT INTO DotnetAPISchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            ) VALUES (
                @UserId,
                @PostTitle,
                @PostContent,
                GETDATE(),
                GETDATE()
            )
        END
    ELSE
        BEGIN
            UPDATE DotnetAPISchema.Posts 
                SET PostTitle = @PostTitle,
                    PostContent = @PostContent,
                    PostUpdated = GETDATE()
                WHERE PostId = @PostId
        END
END
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spPost_Delete
    @PostId INT
    , @UserId INT 
AS
BEGIN
    DELETE FROM DotnetAPISchema.Posts 
        WHERE PostId = @PostId
            AND UserId = @UserId
END
GO



CREATE OR ALTER PROCEDURE DotnetAPISchema.spLoginConfirmation_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM DotnetAPISchema.Auth AS Auth 
        WHERE Auth.Email = @Email
END;
GO

CREATE OR ALTER PROCEDURE DotnetAPISchema.spRegistration_Upsert
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS 
BEGIN
    IF NOT EXISTS (SELECT * FROM DotnetAPISchema.Auth WHERE Email = @Email)
        BEGIN
            INSERT INTO DotnetAPISchema.Auth(
                [Email],
                [PasswordHash],
                [PasswordSalt]
            ) VALUES (
                @Email,
                @PasswordHash,
                @PasswordSalt
            )
        END
    ELSE
        BEGIN
            UPDATE DotnetAPISchema.Auth 
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt
                WHERE Email = @Email
        END
END
GO