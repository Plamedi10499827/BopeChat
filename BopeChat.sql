
CREATE DATABASE IF NOT EXISTS bopechat_db;

USE bopechat_db;

-- WARNING:
-- These commands delete the existing task and activity tables
-- and recreate them from scratch.
-- Any tasks and activity history stored in these tables will be lost.

DROP TABLE IF EXISTS activity_log;
DROP TABLE IF EXISTS tasks;

-- Create the Tasks table
CREATE TABLE tasks
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    description VARCHAR(500) NOT NULL,
    reminder_date DATETIME NULL,
    is_completed TINYINT(1) NOT NULL DEFAULT 0,
    reminder_notified TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create the Activity Log table
CREATE TABLE activity_log
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    action_name VARCHAR(100) NOT NULL,
    details TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Test the tables
SHOW COLUMNS FROM tasks;
SHOW COLUMNS FROM activity_log;

-- Display the tables
SELECT * FROM tasks;
SELECT * FROM activity_log;
