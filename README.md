# BopeChat – Cybersecurity Awareness Chatbot

## 1. Project Overview

BopeChat is a Windows desktop application developed in C# using Windows Forms. It is designed to promote cybersecurity awareness while providing users with an interactive chatbot and useful task-management features.

The application allows users to interact with a chatbot, manage tasks and reminders, test their cybersecurity knowledge through a quiz, and view an activity log.

The project combines cybersecurity education with practical desktop application development and database integration.

---

## 2. Project Objectives

The main objectives of BopeChat are:

- To educate users about cybersecurity awareness.
- To provide an interactive chatbot experience.
- To help users organise and manage tasks.
- To allow users to create reminders for tasks.
- To test users' cybersecurity knowledge through a quiz.
- To record activities performed within the application.
- To store task and activity information in a MySQL database.
- To demonstrate object-oriented programming principles in C#.

---

## 3. Features

### 3.1 BopeChat Chatbot

The chatbot provides an interactive interface where users can type messages and receive responses.

The chatbot includes:

- A chat display area.
- A message input field.
- A Send button.
- A Start button.
- A chatbot greeting.
- Cybersecurity-related responses.
- A graphical user interface.
- A logo and audio greeting.

### 3.2 Task Assistant

The Task Assistant allows users to manage their tasks within the application.

Users can:

- Add new tasks.
- Enter a task title.
- Enter a task description.
- Set an optional reminder date and time.
- View saved tasks.
- Mark tasks as completed.
- Delete tasks.
- Refresh the task list.

Task information is stored in the MySQL database.

### 3.3 Reminder System

The reminder functionality allows users to assign reminder dates and times to tasks.

The application checks for due reminders and notifies the user when a reminder becomes due.

Reminder functionality includes:

- Setting reminder dates.
- Setting reminder times.
- Checking due reminders.
- Preventing repeated reminder notifications.
- Tracking reminder status.

### 3.4 Cybersecurity Awareness Quiz

The application includes an interactive cybersecurity quiz to help users test their cybersecurity knowledge.

The quiz covers topics such as:

- Phishing.
- Password security.
- Suspicious links.
- Malware.
- Firewalls.
- Two-factor authentication.
- Public Wi-Fi security.
- Encryption.
- Ransomware.
- Software updates.
- Social engineering.
- Data backups.

The quiz includes multiple-choice questions, answer checking, scoring, feedback, and final results.

### 3.5 Activity Log

The Activity Log records activities performed within the application.

It provides a record of events such as application startup and other logged activities.

The activity log includes:

- Activity ID.
- Action name.
- Activity details.
- Date and time of the activity.

The activity records are stored in the MySQL database and can be viewed through the Activity Log tab.

---

## 4. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| .NET 8 | Application framework |
| Windows Forms | Graphical user interface |
| Visual Studio | Development environment |
| MySQL 8 | Database management system |
| MySQL Workbench | Database creation and management |
| MySql.Data | MySQL connectivity from C# |
| SQL | Database queries and data manipulation |

---

## 5. System Requirements

The following software is required to run and develop the project:

- Windows operating system.
- Visual Studio with Windows Forms development support.
- .NET 8 SDK.
- MySQL Server 8.
- MySQL Workbench.
- MySql.Data NuGet package.

---

## 6. Database Design

The application uses a MySQL database called:

`bopechat_db`

The database contains two main tables.

### 6.1 Tasks Table

The `tasks` table stores task information.

| Column | Data Type | Description |
|---|---|---|
| id | INT | Primary key and unique task identifier |
| title | VARCHAR(150) | Task title |
| description | VARCHAR(500) | Task description |
| reminder_date | DATETIME | Optional reminder date and time |
| is_completed | TINYINT(1) | Indicates whether the task is completed |
| reminder_notified | TINYINT(1) | Indicates whether the reminder has been sent |
| created_at | DATETIME | Date and time the task was created |

### 6.2 Activity Log Table

The `activity_log` table stores application activity records.

| Column | Data Type | Description |
|---|---|---|
| id | INT | Primary key and unique activity identifier |
| action_name | VARCHAR(100) | Name of the recorded action |
| details | TEXT | Additional information about the activity |
| created_at | DATETIME | Date and time of the activity |

---

## 7. Database Setup

### Step 1: Open MySQL Workbench

Connect to your MySQL Server instance.

### Step 2: Create the database

Run the following SQL script:

```sql
CREATE DATABASE IF NOT EXISTS bopechat_db;

USE bopechat_db;
```

### Step 3: Create the tasks table

```sql
CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    description VARCHAR(500) NOT NULL,
    reminder_date DATETIME NULL,
    is_completed TINYINT(1) NOT NULL DEFAULT 0,
    reminder_notified TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### Step 4: Create the activity_log table

```sql
CREATE TABLE IF NOT EXISTS activity_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    action_name VARCHAR(100) NOT NULL,
    details TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### Step 5: Execute the SQL script

Click the Execute button in MySQL Workbench to create the database and tables.

---

## 8. Database Connection

The application connects to MySQL through its database configuration and database-access classes.

The connection settings must match the MySQL Server installation.

Typical connection settings include:

- Server address.
- Database name.
- Username.
- Password.
- MySQL port.

The database name used by the application is:

`bopechat_db`

The MySQL connection must be configured correctly before running database-dependent features.

---

## 9. How to Run the Application

1. Open Visual Studio.
2. Open the BopeChat project or solution.
3. Ensure the MySQL Server is running.
4. Ensure the database has been created.
5. Check that the MySQL connection settings are correct.
6. Restore the NuGet packages if necessary.
7. Build the solution.
8. Run the application.

---

## 10. How to Use BopeChat

### Chatbot

1. Open the BopeChat tab.
2. Click Start if required.
3. Type a message into the message input field.
4. Click Send or press Enter.
5. Read the chatbot's response.

### Task Assistant

1. Open the Task Assistant tab.
2. Enter a task title.
3. Enter a task description.
4. Select the reminder checkbox if a reminder is required.
5. Choose the reminder date and time.
6. Click Add Task.
7. Select a task from the task list to complete or delete it.
8. Use Refresh to reload the task list.

### Cybersecurity Quiz

1. Open the Cybersecurity Quiz tab.
2. Start the quiz.
3. Read each question.
4. Select an answer.
5. Review feedback.
6. Continue to the next question.
7. View your final score when the quiz is completed.

### Activity Log

1. Open the Activity Log tab.
2. View the recorded application activities.
3. Click Refresh to reload the activity records.

---

## 11. Project Structure

The project is organised into classes to separate different responsibilities.

### Main Application

- `Form1.cs` – Main form, graphical interface, event handling, and integration of the application features.
- `Form1.Designer.cs` – Windows Forms designer-generated interface code.
- `Program.cs` – Application entry point.

### Chatbot

- `ChatEngine.cs` – Handles chatbot responses and conversation logic.

### Models

- `TaskItem.cs` – Represents a task and its properties.
- `QuizQuestion.cs` – Represents a cybersecurity quiz question, answer options, correct answer, and explanation.

### Data

- `Database.cs` – Handles database operations.
- `DbConfig.cs` – Contains database connection configuration.
- `TaskRepository.cs` – Handles task-related database operations.

### Services

- `ActivityLogger.cs` – Handles activity logging functionality.
- `ChatbotEngine.cs` – Supports chatbot application logic.
- `FuzzyMatcher.cs` – Supports matching text or user input.
- `NlpService.cs` – Supports natural language processing functionality.
- `QuizService.cs` – Handles quiz questions, answers, scoring, and results.
- `ReminderParser.cs` – Supports parsing reminder date and time input.
- `TaskDescriptions.cs` – Provides task-related descriptions and help information.

---

## 12. Object-Oriented Programming

The project uses object-oriented programming concepts to organise the application.

### Encapsulation

Classes contain their own properties and methods to manage their responsibilities.

For example, the `TaskItem` class stores task information, while the `TaskRepository` class handles task database operations.

### Abstraction

Database operations and application services are separated from the graphical user interface, allowing the main form to interact with classes that handle specific functionality.

### Separation of Responsibilities

Different classes are used for different purposes, including:

- User interface management.
- Database operations.
- Task management.
- Quiz functionality.
- Reminder handling.
- Chatbot logic.
- Activity logging.

This makes the application easier to maintain, understand, and extend.

---

## 13. Error Handling

The application uses error handling to manage unexpected situations.

Examples include:

- Database connection errors.
- Invalid task input.
- Missing task information.
- Invalid reminder dates.
- Unexpected application errors.

Error messages help users understand when an operation could not be completed.

---

## 14. Testing

The application should be tested to confirm that each feature works correctly.

| Test | Expected Result |
|---|---|
| Start the chatbot | Greeting is displayed |
| Send a message | Chatbot responds |
| Add a task | Task is saved and displayed |
| Add a task with a reminder | Reminder information is saved |
| Complete a task | Task completion status changes |
| Delete a task | Task is removed |
| Refresh task list | Saved tasks are displayed |
| Start quiz | Quiz begins |
| Answer a question | Answer is checked |
| Complete quiz | Final score is displayed |
| Open Activity Log | Recorded activities are displayed |
| Refresh Activity Log | Activity records reload |
| Restart application | Database-stored tasks remain available |

---

## 15. Troubleshooting

### Database Connection Error

Check that:

- MySQL Server is running.
- The database exists.
- The connection string is correct.
- The username and password are correct.
- The MySQL port is correct.

### Tasks Are Not Displayed

Check that:

- The `tasks` table exists.
- The application is connected to the correct database.
- The task was successfully saved.
- The task list has been refreshed.

### Quiz Does Not Start

Check that:

- The quiz page has been built correctly.
- The quiz questions are loaded.
- The `QuizQuestion` and `QuizService` classes are present.

### Reminder Does Not Appear

Check that:

- The reminder checkbox was selected.
- A valid reminder date and time were entered.
- The reminder time has arrived.
- The reminder timer is running.

### Activity Log Is Empty

Check that:

- The `activity_log` table exists.
- Activities are being logged.
- The application is connected to the correct database.
- The Activity Log has been refreshed.

---

## 16. Future Improvements

Possible future improvements include:

- More cybersecurity quiz questions.
- Additional chatbot cybersecurity topics.
- Improved natural language understanding.
- Better task searching and filtering.
- Task priority levels.
- Improved reminder notifications.
- More detailed activity logging.
- Improved user interface design.
- User accounts and authentication.
- Exporting activity logs and tasks.

---

## 17. Conclusion

BopeChat is a cybersecurity awareness chatbot developed using C#, Windows Forms, and MySQL.

The application combines chatbot interaction, task management, reminders, cybersecurity quizzes, and activity logging into one desktop application.

The project demonstrates the use of object-oriented programming, database integration, graphical user interface development, event handling, and basic cybersecurity education.

BopeChat aims to make cybersecurity awareness more interactive while providing practical features that users can utilise in their daily activities.

---

**Project Name:** BopeChat – Cybersecurity Awareness Chatbot  
**Programming Language:** C#  
**Framework:** .NET 8 Windows Forms  
**Database:** MySQL 8  
**Development Environment:** Visual Studio  
