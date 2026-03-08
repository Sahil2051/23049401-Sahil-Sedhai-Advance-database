IF DB_ID('CinemaTicketSystemDb') IS NULL
BEGIN
    CREATE DATABASE [CinemaTicketSystemDb];
END
GO

USE [CinemaTicketSystemDb];
GO

IF OBJECT_ID('[dbo].[User]', 'U') IS NULL
BEGIN
    CREATE TABLE [User]
    (
        User_Id INT IDENTITY(1,1) PRIMARY KEY,
        User_Name NVARCHAR(100) NOT NULL,
        User_Email NVARCHAR(120) NOT NULL,
        User_Contact_Number NVARCHAR(20) NOT NULL,
        User_Address NVARCHAR(250) NOT NULL,
        User_Registration_Date DATE NOT NULL
    );
END
GO

IF OBJECT_ID('[dbo].[Theater]', 'U') IS NULL
BEGIN
    CREATE TABLE Theater
    (
        Theater_Id INT IDENTITY(1,1) PRIMARY KEY,
        Theater_Name NVARCHAR(120) NOT NULL,
        Theater_City NVARCHAR(60) NOT NULL,
        Theater_Location NVARCHAR(200) NOT NULL,
        Theater_Contact_Number NVARCHAR(20) NOT NULL,
        Theater_Total_Halls INT NOT NULL
    );
END
GO

IF OBJECT_ID('[dbo].[Hall]', 'U') IS NULL
BEGIN
    CREATE TABLE Hall
    (
        Hall_Id INT IDENTITY(1,1) PRIMARY KEY,
        Hall_Number NVARCHAR(20) NOT NULL,
        Hall_Seating_Capacity INT NOT NULL,
        Hall_Type NVARCHAR(50) NOT NULL,
        Hall_Status NVARCHAR(40) NOT NULL,
        Theater_Id INT NOT NULL,
        CONSTRAINT FK_Hall_Theater FOREIGN KEY (Theater_Id) REFERENCES Theater(Theater_Id)
    );
END
GO

IF OBJECT_ID('[dbo].[Movie]', 'U') IS NULL
BEGIN
    CREATE TABLE Movie
    (
        Movie_Id INT IDENTITY(1,1) PRIMARY KEY,
        Movie_Title NVARCHAR(150) NOT NULL,
        Movie_Duration INT NOT NULL,
        Movie_Language NVARCHAR(50) NOT NULL,
        Movie_Genre NVARCHAR(50) NOT NULL,
        Movie_Release_Date DATE NOT NULL
    );
END
GO

IF OBJECT_ID('[dbo].[Show]', 'U') IS NULL
BEGIN
    CREATE TABLE [Show]
    (
        Show_Id INT IDENTITY(1,1) PRIMARY KEY,
        Show_Date DATE NOT NULL,
        Show_Time TIME NOT NULL,
        Show_Rating NVARCHAR(10) NOT NULL,
        Movie_Id INT NOT NULL,
        Hall_Id INT NOT NULL,
        CONSTRAINT FK_Show_Movie FOREIGN KEY (Movie_Id) REFERENCES Movie(Movie_Id),
        CONSTRAINT FK_Show_Hall FOREIGN KEY (Hall_Id) REFERENCES Hall(Hall_Id)
    );
END
GO

IF OBJECT_ID('[dbo].[Booking]', 'U') IS NULL
BEGIN
    CREATE TABLE Booking
    (
        Booking_Id INT IDENTITY(1,1) PRIMARY KEY,
        Booking_Date DATE NOT NULL,
        Booking_Status NVARCHAR(30) NOT NULL,
        Total_Amount DECIMAL(10,2) NOT NULL,
        User_Id INT NOT NULL,
        Show_Id INT NOT NULL,
        CONSTRAINT FK_Booking_User FOREIGN KEY (User_Id) REFERENCES [User](User_Id),
        CONSTRAINT FK_Booking_Show FOREIGN KEY (Show_Id) REFERENCES [Show](Show_Id)
    );
END
GO

IF OBJECT_ID('[dbo].[Ticket]', 'U') IS NULL
BEGIN
    CREATE TABLE Ticket
    (
        Ticket_Id INT IDENTITY(1,1) PRIMARY KEY,
        Seat_Number NVARCHAR(20) NOT NULL,
        Ticket_Status NVARCHAR(30) NOT NULL,
        Ticket_Price DECIMAL(10,2) NOT NULL,
        Booking_Id INT NOT NULL,
        CONSTRAINT FK_Ticket_Booking FOREIGN KEY (Booking_Id) REFERENCES Booking(Booking_Id)
    );
END
GO
