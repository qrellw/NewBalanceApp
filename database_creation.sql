USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BalanceAppDB_v2')
BEGIN
    CREATE DATABASE BalanceAppDB_v2;
END
GO

USE BalanceAppDB_v2;
GO

-- 1. Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        PasswordHash NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(100) NULL,
        Role NVARCHAR(MAX) NOT NULL DEFAULT 'Doctor'
    );
END
GO

-- 2. Patients Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients')
BEGIN
    CREATE TABLE Patients (
        PatientId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        MedicalId NVARCHAR(50) NULL,
        DateOfBirth DATETIME2 NOT NULL,
        Gender NVARCHAR(MAX) NOT NULL,
        Address NVARCHAR(200) NULL,
        PhoneNumber NVARCHAR(20) NULL,
        Ethnicity NVARCHAR(MAX) NULL,
        Height FLOAT NOT NULL,
        Weight FLOAT NOT NULL,
        Job NVARCHAR(MAX) NULL,
        MedicalHistory NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL
    );
END
GO

-- 3. TestSessions Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSessions')
BEGIN
    CREATE TABLE TestSessions (
        SessionId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        TestDate DATETIME2 NOT NULL,
        Notes NVARCHAR(500) NULL,
        MeanX FLOAT NOT NULL,
        MeanY FLOAT NOT NULL,
        BMI FLOAT NOT NULL,
        CONSTRAINT FK_TestSessions_Patients_PatientId FOREIGN KEY (PatientId) REFERENCES Patients(PatientId) ON DELETE CASCADE
    );
END
GO

-- 4. TestSamples Table (Heavy Data)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSamples')
BEGIN
    CREATE TABLE TestSamples (
        SampleId BIGINT IDENTITY(1,1) PRIMARY KEY,
        SessionId INT NOT NULL,
        [Index] INT NOT NULL,
        TimestampMs FLOAT NOT NULL,
        X FLOAT NOT NULL,
        Y FLOAT NOT NULL,
        Force1 FLOAT NOT NULL,
        Force2 FLOAT NOT NULL,
        Force3 FLOAT NOT NULL,
        Force4 FLOAT NOT NULL,
        CONSTRAINT FK_TestSamples_TestSessions_SessionId FOREIGN KEY (SessionId) REFERENCES TestSessions(SessionId) ON DELETE CASCADE
    );

    CREATE INDEX IX_TestSamples_SessionId ON TestSamples(SessionId);
END
GO
