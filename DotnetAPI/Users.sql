DROP TABLE IF EXISTS DotnetAPISchema.Users;

-- IF OBJECT_ID('DotnetAPISchema.Users') IS NOT NULL
--     DROP TABLE DotnetAPISchema.Users;

CREATE TABLE DotnetAPISchema.Users
(
    UserId INT IDENTITY(1, 1) PRIMARY KEY
    , FirstName NVARCHAR(50)
    , LastName NVARCHAR(50)
    , Email NVARCHAR(50) UNIQUE
    , Gender NVARCHAR(50)
    , Active BIT
);

DROP TABLE IF EXISTS DotnetAPISchema.UserSalary;

-- IF OBJECT_ID('DotnetAPISchema.UserSalary') IS NOT NULL
--     DROP TABLE DotnetAPISchema.UserSalary;

CREATE TABLE DotnetAPISchema.UserSalary
(
    UserId INT UNIQUE
    , Salary DECIMAL(18, 4)
);

DROP TABLE IF EXISTS DotnetAPISchema.UserJobInfo;

-- IF OBJECT_ID('DotnetAPISchema.UserJobInfo') IS NOT NULL
--     DROP TABLE DotnetAPISchema.UserJobInfo;

CREATE TABLE DotnetAPISchema.UserJobInfo
(
    UserId INT UNIQUE
    , JobTitle NVARCHAR(50)
    , Department NVARCHAR(50),
);