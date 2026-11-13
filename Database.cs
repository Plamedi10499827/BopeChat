CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE cybersecurity_chatbot;

CREATE TABLE IF NOT EXISTS tasks (
    task_id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL,
    reminder_at DATETIME NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    reminder_notified BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS activity_log (
    log_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    action_name VARCHAR(100) NOT NULL,
    details TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_activity_created_at (created_at)
);