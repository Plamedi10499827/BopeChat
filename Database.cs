
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace BopeChat
{
    internal static class Database
    {
        // Replace the placeholder with your MySQL root password.
        // Keep your password private.
        private const string DatabasePassword = "Plamedimingabope2190.";

        private static readonly string connectionString =
            new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = 3306,
                UserID = "root",
                Password = DatabasePassword,
                Database = "bopechat_db"
            }.ConnectionString;

        private static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Initialise the database and required tables.
        public static void Initialize()
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string createActivityTable = @"
                    CREATE TABLE IF NOT EXISTS activity_log
                    (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        action_name VARCHAR(100) NOT NULL,
                        details TEXT NOT NULL,
                        created_at DATETIME NOT NULL
                            DEFAULT CURRENT_TIMESTAMP
                    );";

                using (MySqlCommand command =
                    new MySqlCommand(createActivityTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Add reminder_notified if it does not already exist.
                string checkColumn = @"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'bopechat_db'
                    AND TABLE_NAME = 'tasks'
                    AND COLUMN_NAME = 'reminder_notified';";

                using (MySqlCommand command =
                    new MySqlCommand(checkColumn, connection))
                {
                    int columnExists =
                        Convert.ToInt32(command.ExecuteScalar());

                    if (columnExists == 0)
                    {
                        string addColumn = @"
                            ALTER TABLE tasks
                            ADD COLUMN reminder_notified
                            TINYINT(1) NOT NULL DEFAULT 0;";

                        using (MySqlCommand alterCommand =
                            new MySqlCommand(addColumn, connection))
                        {
                            alterCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        // Record an activity.
        public static void Log(string action, string details = "")
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    INSERT INTO activity_log
                    (action_name, details)
                    VALUES (@action, @details);";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@action", action);
                    command.Parameters.AddWithValue("@details", details);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Add a new task.
        public static void AddTask(
            string title,
            string description,
            DateTime? reminder)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    INSERT INTO tasks
                    (title, description, reminder_date)
                    VALUES
                    (@title, @description, @reminder);";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.Parameters.AddWithValue(
                        "@description", description);

                    command.Parameters.AddWithValue(
                        "@reminder",
                        reminder.HasValue
                            ? (object)reminder.Value
                            : DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }

            Log("Task added", "Added task: " + title);
        }

        // Retrieve all tasks.
        public static DataTable GetTasks()
        {
            DataTable table = new DataTable();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT
                        id AS TaskId,
                        title AS Title,
                        description AS Description,
                        reminder_date AS ReminderAt,
                        is_completed AS IsCompleted,
                        created_at AS CreatedAt
                    FROM tasks
                    ORDER BY created_at DESC;";

                using (MySqlDataAdapter adapter =
                    new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // Mark a task as completed or incomplete.
        public static void SetCompleted(int taskId, bool completed)
        {
            using (MySqlConnection connection = GetConnection())
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
                        "@completed", completed ? 1 : 0);

                    command.Parameters.AddWithValue("@id", taskId);

                    command.ExecuteNonQuery();
                }
            }

            Log(
                completed ? "Task completed" : "Task reopened",
                "Task ID: " + taskId);
        }

        // Delete a task.
        public static void DeleteTask(int taskId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = "DELETE FROM tasks WHERE id = @id;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", taskId);
                    command.ExecuteNonQuery();
                }
            }

            Log("Task deleted", "Task ID: " + taskId);
        }

        // Find reminders that are due and have not been displayed.
        public static DataTable DueReminders()
        {
            DataTable table = new DataTable();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT
                        id AS TaskId,
                        title AS Title,
                        reminder_date AS ReminderAt
                    FROM tasks
                    WHERE reminder_date IS NOT NULL
                    AND reminder_date <= NOW()
                    AND is_completed = 0
                    AND reminder_notified = 0;";

                using (MySqlDataAdapter adapter =
                    new MySqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // Mark a reminder as already displayed.
        public static void MarkReminderNotified(int taskId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    UPDATE tasks
                    SET reminder_notified = 1
                    WHERE id = @id;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", taskId);
                    command.ExecuteNonQuery();
                }
            }

            Log("Reminder displayed", "Task ID: " + taskId);
        }

        // Retrieve recent activity.
        public static DataTable GetRecentActivity(int limit)
        {
            DataTable table = new DataTable();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT
                        id AS LogId,
                        action_name AS Action,
                        details AS Details,
                        created_at AS CreatedAt
                    FROM activity_log
                    ORDER BY created_at DESC, id DESC
                    LIMIT @limit;";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@limit", Math.Max(1, limit));

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }

            return table;
        }
    }
}
