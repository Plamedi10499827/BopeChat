using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using BopeChat.Models;

namespace BopeChat.Data
{
    internal class TaskRepository
    {
        private readonly string connectionString =
            DbConfig.GetConnectionString();

        // Add a new task to the database
        public int AddTask(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO tasks
                    (title, description, reminder_date, is_completed, reminder_notified)
                    VALUES
                    (@title, @description, @reminderDate, @isCompleted, @reminderNotified);

                    SELECT LAST_INSERT_ID();";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@title", task.Title);

                    command.Parameters.AddWithValue(
                        "@description", task.Description);

                    command.Parameters.AddWithValue(
                        "@reminderDate",
                        task.ReminderDate.HasValue
                            ? (object)task.ReminderDate.Value
                            : DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@isCompleted", task.IsCompleted);

                    command.Parameters.AddWithValue(
                        "@reminderNotified", task.ReminderNotified);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        // Retrieve all tasks
        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT id, title, description, reminder_date,
                           is_completed, reminder_notified, created_at
                    FROM tasks
                    ORDER BY created_at DESC;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(ReadTask(reader));
                    }
                }
            }

            return tasks;
        }

        // Alias for retrieving all tasks
        public List<TaskItem> GetTasks()
        {
            return GetAllTasks();
        }

        // Retrieve a task using its ID
        public TaskItem? GetTaskById(int id)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT id, title, description, reminder_date,
                           is_completed, reminder_notified, created_at
                    FROM tasks
                    WHERE id = @id
                    LIMIT 1;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadTask(reader);
                        }
                    }
                }
            }

            return null;
        }

        // Update an existing task
        public bool UpdateTask(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE tasks
                    SET title = @title,
                        description = @description,
                        reminder_date = @reminderDate,
                        is_completed = @isCompleted,
                        reminder_notified = @reminderNotified
                    WHERE id = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id", task.Id);

                    command.Parameters.AddWithValue(
                        "@title", task.Title);

                    command.Parameters.AddWithValue(
                        "@description", task.Description);

                    command.Parameters.AddWithValue(
                        "@reminderDate",
                        task.ReminderDate.HasValue
                            ? (object)task.ReminderDate.Value
                            : DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@isCompleted", task.IsCompleted);

                    command.Parameters.AddWithValue(
                        "@reminderNotified", task.ReminderNotified);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Mark a task as completed or incomplete
        public bool SetCompleted(int taskId, bool completed)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE tasks
                    SET is_completed = @completed
                    WHERE id = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id", taskId);

                    command.Parameters.AddWithValue(
                        "@completed", completed);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Delete a task
        public bool DeleteTask(int taskId)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    DELETE FROM tasks
                    WHERE id = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id", taskId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Retrieve tasks that are due and have not been notified
        public List<TaskItem> GetDueReminders()
        {
            List<TaskItem> reminders = new List<TaskItem>();

            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT id, title, description, reminder_date,
                           is_completed, reminder_notified, created_at
                    FROM tasks
                    WHERE reminder_date IS NOT NULL
                      AND reminder_date <= NOW()
                      AND reminder_notified = 0
                      AND is_completed = 0
                    ORDER BY reminder_date ASC;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reminders.Add(ReadTask(reader));
                    }
                }
            }

            return reminders;
        }

        // Mark a reminder as already notified
        public bool MarkReminderNotified(int taskId)
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE tasks
                    SET reminder_notified = 1
                    WHERE id = @id;";

                using (MySqlCommand command =
                       new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@id", taskId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Convert a database row into a TaskItem object
        private TaskItem ReadTask(MySqlDataReader reader)
        {
            return new TaskItem(
                id: Convert.ToInt32(reader["id"]),
                title: Convert.ToString(reader["title"]) ?? string.Empty,
                description: Convert.ToString(reader["description"]) ?? string.Empty,
                reminderDate: reader["reminder_date"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["reminder_date"]),
                isCompleted: Convert.ToBoolean(reader["is_completed"]),
                reminderNotified: Convert.ToBoolean(reader["reminder_notified"]),
                createdAt: Convert.ToDateTime(reader["created_at"])
            );
        }
    }
}