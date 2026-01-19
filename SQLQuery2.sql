CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordSalt VARBINARY(16) NOT NULL,
    PasswordHash VARBINARY(64) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL
        CHECK (UserRole IN ('JobSeeker', 'Recruiter'))
);
CREATE TABLE Jobs (
    JobId BIGINT IDENTITY(1,1) NOT NULL,
    job_title NVARCHAR(200) NULL,
    description NVARCHAR(MAX) NULL,
    company NVARCHAR(200) NULL,
    location NVARCHAR(200) NULL,
    salary_string NVARCHAR(100) NULL,
    min_annual_salary INT NULL,
    max_annual_salary INT NULL,
    date_posted DATETIME NULL,
    final_url NVARCHAR(500) NULL,
    source_url NVARCHAR(500) NULL,
    isUserCreated BIT NULL,
    UserId INT NULL,

    CONSTRAINT PK_Jobs PRIMARY KEY CLUSTERED (JobId)
);
ALTER TABLE Jobs
ADD CONSTRAINT FK_Jobs_Users
FOREIGN KEY (UserId) REFERENCES Users (UserId);

CREATE TABLE SavedJobs (
    SavedJobId INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL,
    JobId BIGINT NOT NULL,
    SavedAt DATETIME DEFAULT (GETDATE()) NOT NULL,

    CONSTRAINT PK_SavedJobs PRIMARY KEY CLUSTERED (SavedJobId),
    CONSTRAINT UQ_User_Job UNIQUE NONCLUSTERED (UserId, JobId),
    CONSTRAINT FK_SavedJobs_Jobs
        FOREIGN KEY (JobId) REFERENCES Jobs (JobId)
);
ALTER TABLE SavedJobs
ADD CONSTRAINT FK_SavedJobs_Users
FOREIGN KEY (UserId) REFERENCES Users (UserId);

CREATE TABLE JobReviews (
    ReviewId INT IDENTITY (1,1) NOT NULL,
    UserId INT NOT NULL,
    JobId BIGINT NOT NULL,
    Rating INT NOT NULL,
    ReviewText NVARCHAR(800) NULL,
    CreatedAt DATETIME DEFAULT (GETDATE()) NOT NULL,

    CONSTRAINT PK_JobReviews PRIMARY KEY CLUSTERED (ReviewId),
    CONSTRAINT UQ_JobReviews_User_Job
        UNIQUE NONCLUSTERED (UserId, JobId),
    CONSTRAINT FK_JobReviews_Users
        FOREIGN KEY (UserId) REFERENCES Users (UserId),
    CONSTRAINT FK_JobReviews_Jobs
        FOREIGN KEY (JobId) REFERENCES Jobs (JobId),
    CONSTRAINT CK_JobReviews_Rating
        CHECK (Rating >= 1 AND Rating <= 5)
);

CREATE TABLE Courses (
    CourseId INT IDENTITY(1, 1) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Level NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ThumbnailUrl NVARCHAR(500) NULL,
    IsActive BIT DEFAULT (1) NOT NULL,
    CreatedAt DATETIME DEFAULT (GETDATE()) NOT NULL,
    PRIMARY KEY CLUSTERED (CourseId ASC)
);
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1, 1) NOT NULL,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrolledAt DATETIME DEFAULT (GETDATE()) NOT NULL,
    PRIMARY KEY CLUSTERED (EnrollmentId ASC),
    CONSTRAINT FK_Enrollments_Users
        FOREIGN KEY (UserId) REFERENCES Users (UserId),
    CONSTRAINT FK_Enrollments_Courses
        FOREIGN KEY (CourseId) REFERENCES Courses (CourseId)
);
CREATE TABLE CourseVideos (
    VideoId INT IDENTITY(1, 1) NOT NULL,
    CourseId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    VideoUrl NVARCHAR(500) NOT NULL,
    OrderIndex INT NOT NULL,
    DurationSec INT NULL,
    IsActive BIT DEFAULT (1) NOT NULL,
    PRIMARY KEY CLUSTERED (VideoId ASC),
    CONSTRAINT FK_CourseVideos_Courses
        FOREIGN KEY (CourseId) REFERENCES Courses (CourseId)
);

CREATE TABLE VideoProgress (
    ProgressId INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL,
    VideoId INT NOT NULL,
    LastWatchedSec INT DEFAULT (0) NOT NULL,
    IsCompleted BIT DEFAULT (0) NOT NULL,
    UpdatedAt DATETIME DEFAULT (GETDATE()) NOT NULL,

    CONSTRAINT PK_VideoProgress PRIMARY KEY CLUSTERED (ProgressId),
    CONSTRAINT FK_VideoProgress_Users
        FOREIGN KEY (UserId) REFERENCES Users (UserId),
    CONSTRAINT FK_VideoProgress_CourseVideos
        FOREIGN KEY (VideoId) REFERENCES CourseVideos (VideoId)
);