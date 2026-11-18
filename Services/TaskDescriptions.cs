using System;

namespace BopeChat.Services
{
    internal class TaskDescriptions
    {
        public string GetTaskHelp()
        {
            return
                "I can help you manage your tasks!\n\n" +
                "Available task commands:\n\n" +
                "• Add a task: Add task study cybersecurity\n" +
                "• View tasks: List tasks\n" +
                "• Complete a task: Complete task\n" +
                "• Delete a task: Delete task\n" +
                "• Set a reminder: Remind me tomorrow to study\n\n" +
                "You can also use the Task Assistant tab to manage your tasks.";
        }

        public string GetAddTaskDescription()
        {
            return "Enter the title and description of the task you want to add.";
        }

        public string GetCompleteTaskDescription()
        {
            return "Select the task you want to mark as completed.";
        }

        public string GetDeleteTaskDescription()
        {
            return "Select the task you want to delete.";
        }

        public string GetReminderDescription()
        {
            return "You can set reminders for your tasks using a date and time.";
        }

        public string GetTaskSuccessMessage(string taskName)
        {
            return $"Your task '{taskName}' has been added successfully!";
        }

        public string GetTaskCompletedMessage(string taskName)
        {
            return $"Great job! Your task '{taskName}' has been completed.";
        }

        public string GetTaskDeletedMessage(string taskName)
        {
            return $"Your task '{taskName}' has been deleted.";
        }
    }
}