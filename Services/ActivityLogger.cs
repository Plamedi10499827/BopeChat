using System;

namespace BopeChat.Services
{
    internal class ActivityLogger
    {
        /// <summary>
        /// Logs an activity to the database.
        /// </summary>
        public void LogActivity(string action, string details = "")
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return;
            }

            try
            {
                Database.Log(action, details);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Activity logging error: " + ex.Message);
            }
        }

        /// <summary>
        /// Logs when a task is created.
        /// </summary>
        public void LogTaskCreated(string taskDescription)
        {
            LogActivity("Task Created", taskDescription);
        }

        /// <summary>
        /// Logs when a task is completed.
        /// </summary>
        public void LogTaskCompleted(string taskDescription)
        {
            LogActivity("Task Completed", taskDescription);
        }

        /// <summary>
        /// Logs when a task is deleted.
        /// </summary>
        public void LogTaskDeleted(string taskDescription)
        {
            LogActivity("Task Deleted", taskDescription);
        }

        /// <summary>
        /// Logs when a reminder is created.
        /// </summary>
        public void LogReminderCreated(string reminderDescription)
        {
            LogActivity("Reminder Created", reminderDescription);
        }

        /// <summary>
        /// Logs when a cybersecurity quiz is started.
        /// </summary>
        public void LogQuizStarted()
        {
            LogActivity("Quiz Started", "Cybersecurity quiz started.");
        }

        /// <summary>
        /// Logs when a cybersecurity quiz is completed.
        /// </summary>
        public void LogQuizCompleted(string score)
        {
            LogActivity("Quiz Completed", score);
        }

        /// <summary>
        /// Logs chatbot interactions.
        /// </summary>
        public void LogChatInteraction(string message)
        {
            LogActivity("Chat Interaction", message);
        }
    }
}