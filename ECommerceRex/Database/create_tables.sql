-- ============================================
-- E‑Commerce Rex – Full Database Schema
-- Compatible with SQL Server 2022
-- ============================================

USE [master]
GO
IF DB_ID('ECommerceRexDb') IS NULL
BEGIN
    CREATE DATABASE [ECommerceRexDb]
    CONTAINMENT = NONE
    ON PRIMARY
    ( NAME = N'ECommerceRexDb', FILENAME = N'/var/opt/mssql/data/ECommerceRexDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
    LOG ON
    ( NAME = N'ECommerceRexDb_log', FILENAME = N'/var/opt/mssql/data/ECommerceRexDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
END
GO

USE [ECommerceRexDb]
GO

-- Users table
CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FullName] NVARCHAR(MAX) NULL,
    [Role] NVARCHAR(MAX) NULL,
    [TelegramId] NVARCHAR(MAX) NULL,
    [FaceEmbeddings] NVARCHAR(MAX) NULL,
    [RowHash] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

-- Products table
CREATE TABLE [Products] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [StockQuantity] INT NOT NULL,
    [Category] NVARCHAR(MAX) NULL,
    [Supplier] NVARCHAR(MAX) NULL,
    [RowHash] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

-- BankAccounts table
CREATE TABLE [BankAccounts] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [Balance] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(MAX) NULL,
    [RowHash] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_BankAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BankAccounts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_BankAccounts_UserId] ON [BankAccounts] ([UserId]);
GO

-- Transactions table
CREATE TABLE [Transactions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SenderId] INT NOT NULL,
    [ReceiverId] INT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Type] NVARCHAR(MAX) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [TransactionDate] DATETIME2 NOT NULL,
    [Status] NVARCHAR(MAX) NULL,
    [RowHash] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transactions_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_Transactions_SenderId] ON [Transactions] ([SenderId]);
CREATE INDEX [IX_Transactions_ReceiverId] ON [Transactions] ([ReceiverId]);
GO

-- Attendances table
CREATE TABLE [Attendances] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [CheckInTime] DATETIME2 NOT NULL,
    [CheckOutTime] DATETIME2 NULL,
    [ScanCode] NVARCHAR(MAX) NULL,
    [RowHash] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Attendances_UserId] ON [Attendances] ([UserId]);
GO
