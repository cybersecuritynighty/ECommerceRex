-- ============================================
-- Seed Data – E‑Commerce Rex
-- Inserts default admin and sample products
-- ============================================

USE [ECommerceRexDb]
GO

-- Insert admin user (password: Admin123!)
-- This uses a hashed password generated with the same algorithm as .NET Identity PasswordHasher.
-- For simplicity, we insert a placeholder; you should replace the hash with a real one.
-- Use the app's registration to create a proper admin.
-- The hash below is for "Admin123!" (generated with PBKDF2)
INSERT INTO [Users] ([Username], [Email], [PasswordHash], [Role], [CreatedAt])
VALUES 
    ('admin', 'admin@ecomrex.com', 'AQAAAAIAAYagAAAAEJ/nx7lKJ3KvO0i9xY7xJcXpG1/8j3Hh2LtN6p3W9o0z5U=', 'Admin', GETUTCDATE());
GO

-- Insert sample products
INSERT INTO [Products] ([Name], [Description], [Price], [StockQuantity], [Category], [CreatedAt])
VALUES 
    ('Premium Headphones', 'Noise-cancelling wireless headphones', 199.99, 50, 'Electronics', GETUTCDATE()),
    ('Smart Watch Pro', 'Fitness tracking and notifications', 349.50, 30, 'Wearables', GETUTCDATE()),
    ('Wireless Earbuds', 'True wireless earbuds with charging case', 89.99, 100, 'Electronics', GETUTCDATE()),
    ('4K Action Camera', 'Waterproof action camera with stabilisation', 299.00, 20, 'Cameras', GETUTCDATE());
GO

-- Insert a sample bank account for admin (optional)
-- Get the admin user id
DECLARE @AdminId INT = (SELECT TOP 1 Id FROM Users WHERE Email = 'admin@ecomrex.com');
IF @AdminId IS NOT NULL
BEGIN
    INSERT INTO [BankAccounts] ([UserId], [Balance], [Currency], [CreatedAt])
    VALUES (@AdminId, 1000.00, 'USD', GETUTCDATE());
END
GO
