```csharp
using MySqlConnector;

namespace CybersecurityAwarenessChatbot;

public sealed class DatabaseService
{
    // Change the password below or set the CHATBOT_MYSQL_CONNECTION environment variable.
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("CHATBOT_MYSQL_CONNECTION")
        ?? "Server=localhost;Port=3306;Database=cybersecurity_chatbot;User ID=root;Password=CHANGE_ME;";

    public async Task<bool> CanConnectAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return true;
    }

    public async Task<List<CyberTask>> GetTasksAsync()
    {
        var tasks = new List<CyberTask>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT task_id, title, description, reminder_at, is_completed
            FROM tasks ORDER BY is_completed, reminder_at IS NULL, reminder_at, task_id DESC
            """;

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new CyberTask
            {
                Id = reader.GetInt32("task_id"),
                Title = reader.GetString("title"),
                Description = reader.GetString("description"),
                ReminderAt = reader.IsDBNull(reader.GetOrdinal("reminder_at"))
                    ? null : reader.GetDateTime("reminder_at"),
                IsCompleted = reader.GetBoolean("is_completed")
            });
        }

        return tasks;
    }

    public async Task<int> AddTaskAsync(string title, string description, DateTime? reminderAt)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO tasks(title, description, reminder_at, is_completed, reminder_notified)
            VALUES (@title, @description, @reminder, 0, 0)
            """;

        await using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@title", title.Trim());
            command.Parameters.AddWithValue("@description", description.Trim());
            command.Parameters.AddWithValue("@reminder",
                reminderAt.HasValue ? (object)reminderAt.Value : DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        await using var idCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
        return Convert.ToInt32(await idCommand.ExecuteScalarAsync());
    }

    public async Task SetCompletedAsync(int id, bool completed)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "UPDATE tasks SET is_completed=@completed WHERE task_id=@id", connection);

        command.Parameters.AddWithValue("@completed", completed);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "DELETE FROM tasks WHERE task_id=@id", connection);

        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<CyberTask>> GetDueRemindersAsync()
    {
        var tasks = new List<CyberTask>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
            SELECT task_id, title, description, reminder_at, is_completed
            FROM tasks
            WHERE reminder_at IS NOT NULL AND reminder_at <= NOW()
              AND is_completed=0 AND reminder_notified=0
            """;

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new CyberTask
            {
                Id = reader.GetInt32("task_id"),
                Title = reader.GetString("title"),
                Description = reader.GetString("description"),
                ReminderAt = reader.GetDateTime("reminder_at"),
                IsCompleted = reader.GetBoolean("is_completed")
            });
        }

        return tasks;
    }

    public async Task MarkReminderNotifiedAsync(int id)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "UPDATE tasks SET reminder_notified=1 WHERE task_id=@id", connection);

        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }

    public async Task AddLogAsync(string action, string details)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "INSERT INTO activity_log(action_name, details) VALUES (@action, @details)",
            connection);

        command.Parameters.AddWithValue("@action", action);
        command.Parameters.AddWithValue("@details", details);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<string>> GetRecentLogsAsync(int limit = 10)
    {
        var logs = new List<string>();

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "SELECT created_at, action_name, details FROM activity_log ORDER BY log_id DESC LIMIT @limit",
            connection);

        command.Parameters.AddWithValue("@limit", Math.Clamp(limit, 1, 100));

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            logs.Add($"{reader.GetDateTime("created_at"):yyyy-MM-dd HH:mm:ss} — " +
                     $"{reader.GetString("action_name")}: {reader.GetString("details")}");
        }

        return logs;
    }
}