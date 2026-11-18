using System;

namespace BopeChat.Models
{
    internal class TaskItem
    {
        // Task ID from the database
        public int Id { get; set; }

        // Task title
        public string Title { get; set; } = string.Empty;

        // Task description
        public string Description { get; set; } = string.Empty;

        // Reminder date and time
        public DateTime? ReminderDate { get; set; }

        // Whether the task has been completed
        public bool IsCompleted { get; set; }

        // Whether the reminder notification has already been sent
        public bool ReminderNotified { get; set; }

        // Date the task was created
        public DateTime CreatedAt { get; set; }

        // Default constructor
        public TaskItem()
        {
        }

        // Constructor for creating a new task
        public TaskItem(
            string title,
            string description,
            DateTime? reminderDate = null)
        {
            Title = title;
            Description = description;
            ReminderDate = reminderDate;
            IsCompleted = false;
            ReminderNotified = false;
            CreatedAt = DateTime.Now;
        }

        // Constructor for loading a task from the database
        public TaskItem(
            int id,
            string title,
            string description,
            DateTime? reminderDate,
            bool isCompleted,
            bool reminderNotified,
            DateTime createdAt)
        {
            Id = id;
            Title = title;
            Description = description;
            ReminderDate = reminderDate;
            IsCompleted = isCompleted;
            ReminderNotified = reminderNotified;
            CreatedAt = createdAt;
        }

        // Returns a readable task description
        public override string ToString()
        {
            string status = IsCompleted ? "Completed" : "Pending";

            string reminder = ReminderDate.HasValue
                ? ReminderDate.Value.ToString("dd MMM yyyy HH:mm")
                : "No reminder";

            return $"{Title} | {status} | {reminder}";
        }
    }
}