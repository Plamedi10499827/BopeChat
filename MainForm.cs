using System.Data;

namespace BopeChat;

public sealed class MainForm : Form
{
    private readonly DataGridView tasksGrid = MakeGrid();
    private readonly DataGridView activityGrid = MakeGrid();
    private readonly TextBox titleBox = new() { Width = 230, PlaceholderText = "Task title (e.g. Enable 2FA)" };
    private readonly TextBox descriptionBox = new() { Width = 300, PlaceholderText = "Description" };
    private readonly CheckBox reminderCheck = new() { Text = "Set reminder", AutoSize = true };
    private readonly DateTimePicker reminderPicker = new() { Width = 190, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", ShowUpDown = true, Enabled = false };
    private readonly TextBox chatInput = new() { Dock = DockStyle.Fill, PlaceholderText = "Try: add task enable 2FA, show activity log, start quiz, list tasks" };
    private readonly RichTextBox chatOutput = new() { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.White, Font = new Font("Segoe UI", 10) };
    private readonly Label quizQuestion = new() { AutoSize = true, MaximumSize = new Size(760, 0), Font = new Font("Segoe UI", 12, FontStyle.Bold) };
    private readonly FlowLayoutPanel quizOptions = new() { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
    private readonly Label quizStatus = new() { AutoSize = true, Text = "Quiz not started." };
    private readonly Button nextQuestionButton = new() { Text = "Start quiz", AutoSize = true };
    private readonly System.Windows.Forms.Timer reminderTimer = new() { Interval = 30_000 };
    private readonly HashSet<int> notifiedReminderIds = new();
    private List<QuizItem> quiz = new();
    private int quizIndex;
    private int quizScore;
    private bool quizAnswered;
    private bool quizCompleted;

    private sealed record QuizItem(string Question, string[] Options, int Answer, string Explanation);

    public MainForm()
    {
        Text = "Cybersecurity Awareness Chatbot";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 650);
        Size = new Size(1120, 760);
        Font = new Font("Segoe UI", 9.5f);
        BuildInterface();

        try
        {
            Database.Initialize();
            RefreshTasks();
            RefreshActivity();
            Say("Chatbot", "Welcome! Add cybersecurity tasks, set reminders, play the quiz, or ask me to show your activity log.");
            reminderTimer.Tick += (_, _) => CheckReminders();
            reminderTimer.Start();
            CheckReminders();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Could not connect to MySQL. Check that MySQL is running, create the database using Database/schema.sql, and update the connection string in Database.cs.\n\nDetails: " + ex.Message,
                "Database connection problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Say("System", "Database connection failed. Update the MySQL connection string in Database.cs and restart.");
        }
    }

    private void BuildInterface()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        Controls.Add(tabs);

        var tasksTab = new TabPage("Task Assistant");
        var taskLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        taskLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        taskLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        taskLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var formPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        formPanel.Controls.Add(titleBox);
        formPanel.Controls.Add(descriptionBox);
        formPanel.Controls.Add(reminderCheck);
        formPanel.Controls.Add(reminderPicker);
        reminderCheck.CheckedChanged += (_, _) => reminderPicker.Enabled = reminderCheck.Checked;
        var addButton = new Button { Text = "Add task", AutoSize = true };
        addButton.Click += (_, _) => AddTaskFromFields();
        formPanel.Controls.Add(addButton);
        taskLayout.Controls.Add(formPanel, 0, 0);
        tasksGrid.Dock = DockStyle.Fill;
        tasksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tasksGrid.MultiSelect = false;
        taskLayout.Controls.Add(tasksGrid, 0, 1);
        var taskButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        var completeButton = new Button { Text = "Mark selected completed", AutoSize = true };
        completeButton.Click += (_, _) => SetSelectedCompleted();
        var deleteButton = new Button { Text = "Delete selected", AutoSize = true };
        deleteButton.Click += (_, _) => DeleteSelectedTask();
        var refreshButton = new Button { Text = "Refresh", AutoSize = true };
        refreshButton.Click += (_, _) => RefreshTasks();
        taskButtons.Controls.AddRange(new Control[] { completeButton, deleteButton, refreshButton });
        taskLayout.Controls.Add(taskButtons, 0, 2);
        tasksTab.Controls.Add(taskLayout);
        tabs.TabPages.Add(tasksTab);

        var quizTab = new TabPage("Cybersecurity Quiz");
        var quizPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(18), ColumnCount = 1 };
        quizPanel.Controls.Add(quizQuestion);
        quizPanel.Controls.Add(quizOptions);
        quizPanel.Controls.Add(quizStatus);
        nextQuestionButton.Click += (_, _) => AdvanceQuiz();
        quizPanel.Controls.Add(nextQuestionButton);
        quizTab.Controls.Add(quizPanel);
        tabs.TabPages.Add(quizTab);

        var chatTab = new TabPage("Chat / NLP");
        var chatLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(10) };
        chatLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        chatLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        chatLayout.Controls.Add(chatOutput, 0, 0);
        var chatEntry = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
        chatEntry.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        chatEntry.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        chatEntry.Controls.Add(chatInput, 0, 0);
        var sendButton = new Button { Text = "Send", AutoSize = true };
        sendButton.Click += (_, _) => ProcessCommand();
        chatEntry.Controls.Add(sendButton, 1, 0);
        chatInput.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ProcessCommand(); } };
        chatLayout.Controls.Add(chatEntry, 0, 1);
        chatTab.Controls.Add(chatLayout);
        tabs.TabPages.Add(chatTab);

        var activityTab = new TabPage("Activity Log");
        var activityLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(10) };
        var activityHeader = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        activityHeader.Controls.Add(new Label { Text = "Most recent actions (up to 10)", AutoSize = true, Padding = new Padding(0, 8, 12, 0) });
        var activityRefresh = new Button { Text = "Refresh log", AutoSize = true };
        activityRefresh.Click += (_, _) => RefreshActivity();
        activityHeader.Controls.Add(activityRefresh);
        activityLayout.Controls.Add(activityHeader, 0, 0);
        activityGrid.Dock = DockStyle.Fill;
        activityLayout.Controls.Add(activityGrid, 0, 1);
        activityTab.Controls.Add(activityLayout);
        tabs.TabPages.Add(activityTab);
    }

    private static DataGridView MakeGrid() => new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White,
        RowHeadersVisible = false
    };

    private void AddTaskFromFields()
    {
        var title = titleBox.Text.Trim();
        if (title.Length == 0) { MessageBox.Show("Please enter a task title."); return; }
        try
        {
            DateTime? reminder = reminderCheck.Checked ? reminderPicker.Value : null;
            Database.AddTask(title, descriptionBox.Text.Trim(), reminder);
            Database.Log($"Task added: {title}" + (reminder.HasValue ? $" (reminder {reminder:yyyy-MM-dd HH:mm})" : ""));
            Say("Chatbot", $"Task added: “{title}”." + (reminder.HasValue ? $" Reminder set for {reminder:yyyy-MM-dd HH:mm}." : " No reminder set."));
            titleBox.Clear(); descriptionBox.Clear(); reminderCheck.Checked = false;
            RefreshTasks(); RefreshActivity();
        }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }

    private int? SelectedTaskId()
    {
        if (tasksGrid.CurrentRow == null || tasksGrid.CurrentRow.Cells["id"].Value == null) return null;
        return Convert.ToInt32(tasksGrid.CurrentRow.Cells["id"].Value);
    }

    private void SetSelectedCompleted()
    {
        var id = SelectedTaskId();
        if (id == null) { MessageBox.Show("Select a task first."); return; }
        try
        {
            var title = Convert.ToString(tasksGrid.CurrentRow!.Cells["title"].Value) ?? "Task";
            Database.SetCompleted(id.Value, true);
            Database.Log($"Task marked completed: {title}");
            Say("Chatbot", $"Marked “{title}” as completed.");
            RefreshTasks(); RefreshActivity();
        }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }
    private void DeleteSelectedTask()
    {
        var id = SelectedTaskId();
        if (id == null) { MessageBox.Show("Select a task first."); return; }
        var title = Convert.ToString(tasksGrid.CurrentRow!.Cells["title"].Value) ?? "Task";
        if (MessageBox.Show($"Delete “{title}”?", "Confirm deletion", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        try
        {
            Database.DeleteTask(id.Value);
            Database.Log($"Task deleted: {title}");
            Say("Chatbot", $"Deleted “{title}”.");
            RefreshTasks(); RefreshActivity();
        }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }

    private void RefreshTasks()
    {
        try
        {
            tasksGrid.DataSource = Database.GetTasks();
            if (tasksGrid.Columns.Contains("id")) tasksGrid.Columns["id"].Visible = false;
            if (tasksGrid.Columns.Contains("title")) tasksGrid.Columns["title"].HeaderText = "Task";
            if (tasksGrid.Columns.Contains("description")) tasksGrid.Columns["description"].HeaderText = "Description";
            if (tasksGrid.Columns.Contains("reminder_at")) tasksGrid.Columns["reminder_at"].HeaderText = "Reminder";
            if (tasksGrid.Columns.Contains("is_completed")) tasksGrid.Columns["is_completed"].HeaderText = "Completed";
            if (tasksGrid.Columns.Contains("created_at")) tasksGrid.Columns["created_at"].HeaderText = "Created";
        }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }

    private void RefreshActivity()
    {
        try { activityGrid.DataSource = Database.GetRecentActivity(10); }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }

    private void CheckReminders()
    {
        try
        {
            var due = Database.DueReminders();
            foreach (DataRow row in due.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                if (!notifiedReminderIds.Add(id)) continue;
                var title = Convert.ToString(row["title"]) ?? "Cybersecurity task";
                MessageBox.Show($"Reminder: {title}", "Cybersecurity task reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Database.Log($"Reminder displayed for task: {title}");
                RefreshActivity();
            }
        }
        catch
        {
            // A temporary database outage should not crash the WinForms message loop.
        }
    }

    private void ProcessCommand()
    {
        var original = chatInput.Text.Trim();
        if (original.Length == 0) return;
        chatInput.Clear();
        Say("User", original);
        var input = original.ToLowerInvariant();

        try
        {
            if (ContainsAny(input, "show activity", "activity log", "what have you done", "recent actions", "show log"))
            {
                var recent = Database.GetRecentActivity(10);
                if (recent.Rows.Count == 0) Say("Chatbot", "There are no recorded actions yet.");
                else
                {
                    var lines = recent.Rows.Cast<DataRow>().Select(r => $"{Convert.ToDateTime(r["Time"]):yyyy-MM-dd HH:mm:ss} — {r["Action"]}");
                    Say("Chatbot", "Here are your 10 most recent actions:\n" + string.Join("\n", lines));
                }
                return;
            }
            if (ContainsAny(input, "start quiz", "take quiz", "play quiz", "quiz me", "cybersecurity quiz"))
            {
                StartQuiz();
                Say("Chatbot", "The quiz is ready. Open the Cybersecurity Quiz tab and select Start quiz.");
                Database.Log("Quiz started from chat");
                RefreshActivity();
                return;
            }
            if (ContainsAny(input, "list tasks", "show tasks", "my tasks", "view tasks"))
            {
                var table = Database.GetTasks();
                if (table.Rows.Count == 0) Say("Chatbot", "You have no tasks yet.");
                else Say("Chatbot", "Your tasks:\n" + string.Join("\n", table.Rows.Cast<DataRow>().Select(r =>
                    $"- {r["title"]} [{(Convert.ToBoolean(r["is_completed"]) ? "completed" : "open")}]" +
                    (r["reminder_at"] == DBNull.Value ? "" : $" — reminder {Convert.ToDateTime(r["reminder_at"]):yyyy-MM-dd HH:mm}"))));
                return;
            }
            if (ContainsAny(input, "add task", "create task", "new task", "remind me to", "set a reminder"))
            {
                var title = ExtractTaskTitle(original);
                if (string.IsNullOrWhiteSpace(title))
                {
                    Say("Chatbot", "Please enter the task in the Task Assistant tab, or try: “add task — Review privacy settings”.");
                    return;
                }
                DateTime? reminder = ParseRelativeReminder(input);
                Database.AddTask(title, "Created from natural-language chat command.", reminder);
                Database.Log($"Task added through NLP: {title}" + (reminder.HasValue ? $" (reminder {reminder:yyyy-MM-dd HH:mm})" : ""));
                Say("Chatbot", $"Task added: “{title}”." + (reminder.HasValue ? $" Reminder set for {reminder:yyyy-MM-dd HH:mm}." : " Would you like to set a reminder? Use “remind me to [task] tomorrow” or the Task Assistant tab."));
                RefreshTasks(); RefreshActivity();
                return;
            }
            if (ContainsAny(input, "help", "what can you do", "commands"))
            {
                Say("Chatbot", "I can manage cybersecurity tasks, set reminders, list tasks, start the quiz, and show the activity log. Examples: “Add task — Enable 2FA”, “Remind me to update my password tomorrow”, “List tasks”, “Start quiz”, “Show activity log”.");
                return;
            }
            if (ContainsAny(input, "password", "phishing", "2fa", "two-factor", "privacy", "cybersecurity", "scam"))
            {
                Say("Chatbot", "Security tip: use unique passwords, enable multi-factor authentication, verify links and senders before clicking, and never share passwords or one-time codes. You can say “add task — Enable 2FA” to save a task.");
                return;
            }
            Say("Chatbot", "I didn’t quite understand that. Try “add task — Enable 2FA”, “list tasks”, “start quiz”, or “show activity log”.");
        }
        catch (Exception ex) { ShowDatabaseError(ex); }
    }

    private static bool ContainsAny(string text, params string[] terms) => terms.Any(text.Contains);

    private static string ExtractTaskTitle(string original)
    {
        var title = original.Trim();
        var lower = title.ToLowerInvariant();
        foreach (var prefix in new[] { "remind me to ", "set a reminder to ", "set reminder for ", "add a task to ", "add task ", "add task: ", "add task - ", "add task — ", "create task ", "new task " })
        {
            if (lower.StartsWith(prefix)) { title = title[prefix.Length..].Trim(); break; }
        }
        title = System.Text.RegularExpressions.Regex.Replace(title, @"\s+(today|tomorrow|in \d+ days?|in \d+ hours?)$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return title.Trim(' ', '.', '"', '\'', '-', '—', ':');
    }

    private static DateTime? ParseRelativeReminder(string input)
    {
        var now = DateTime.Now;
        if (input.Contains("tomorrow")) return now.Date.AddDays(1).AddHours(9);
        var days = System.Text.RegularExpressions.Regex.Match(input, @"in\s+(\d+)\s+days?");
        if (days.Success && int.TryParse(days.Groups[1].Value, out int d)) return now.AddDays(d);
        var hours = System.Text.RegularExpressions.Regex.Match(input, @"in\s+(\d+)\s+hours?");
        if (hours.Success && int.TryParse(hours.Groups[1].Value, out int h)) return now.AddHours(h);
        if (input.Contains("today")) return now.Date.AddHours(17);
        return null;
    }
    private void StartQuiz()
    {
        quiz = new List<QuizItem>
        {
            new("What should you do with an email asking for your password?", new[]{"Reply with your password","Report it as phishing","Forward it to friends","Click its link"}, 1, "Legitimate services should not ask you to disclose your password by email."),
            new("Which is the strongest password practice?", new[]{"Reuse one long password everywhere","Use your birthday","Use unique passwords stored in a reputable password manager","Share passwords with coworkers"}, 2, "Unique passwords limit the damage if one service is breached."),
            new("What does multi-factor authentication add?", new[]{"An extra verification step","A public profile","A shorter password","Automatic public Wi-Fi"}, 0, "MFA requires an additional proof of identity beyond the password."),
            new("A website link looks slightly misspelled. What should you do?", new[]{"Click quickly","Enter your details to check","Avoid it and verify the official address independently","Share it first"}, 2, "Lookalike domains are commonly used to steal credentials."),
            new("What is phishing?", new[]{"A backup method","A deceptive attempt to steal information or make you take an unsafe action","A type of monitor","A software update"}, 1, "Phishing uses misleading messages or sites to trick people."),
            new("Why install security updates?", new[]{"They always make devices faster","They patch bugs and known security vulnerabilities","They remove the need for passwords","They make every website safe"}, 1, "Updates often fix vulnerabilities attackers could exploit."),
            new("What should you do before opening an unexpected attachment?", new[]{"Check the sender and context; verify if unsure","Disable antivirus","Enable macros immediately","Forward it to everyone"}, 0, "Unexpected attachments can contain malware; verify them before opening."),
            new("Which network is safest for sensitive work?", new[]{"Any open public Wi-Fi","A trusted network with appropriate protection, using a VPN when needed","A network with a familiar name only","A network that asks for no password"}, 1, "Open networks can expose traffic; verify networks and use suitable protections."),
            new("What is social engineering?", new[]{"Manipulating people into revealing information or taking actions","Encrypting a hard drive","Replacing a keyboard","A browser theme"}, 0, "Social engineering targets human trust and decision-making."),
            new("What should you do if you suspect an account was compromised?", new[]{"Ignore it","Tell everyone your password","Use a trusted device to change the password, secure MFA, and report it","Delete all security alerts"}, 2, "Act promptly through official channels and secure the account."),
            new("Which information should you share in response to an unsolicited request for a one-time login code?", new[]{"The code only if they know your name","The code if they claim to be support","Never share the code; verify through official channels","The code after asking a security question"}, 2, "One-time codes can let attackers bypass account protections."),
            new("What is a good backup practice against data loss or ransomware?", new[]{"Keep only one copy on the same device","Maintain regular backups, including a protected or offline copy","Never test backups","Store backups with the same account password in a public note"}, 1, "Separate protected backups can help recovery if a device or account is compromised.")
        };
        quizIndex = 0; quizScore = 0; quizAnswered = false; quizCompleted = false;
        quizStatus.Text = $"Quiz ready: {quiz.Count} questions.";
        quizQuestion.Text = "Press Start quiz to begin.";
        quizOptions.Controls.Clear();
        nextQuestionButton.Text = "Start quiz";
    }

    private void AdvanceQuiz()
    {
        if (quiz.Count == 0)
            StartQuiz();

        if (quizCompleted)
        {
            StartQuiz();
            ShowQuestion();
            return;
        }

        if (nextQuestionButton.Text == "Start quiz")
        {
            ShowQuestion();
            return;
        }

        if (!quizAnswered)
        {
            MessageBox.Show("Choose an answer before continuing.");
            return;
        }

        quizIndex++;

        if (quizIndex >= quiz.Count)
        {
            int percentage = (int)Math.Round(100.0 * quizScore / quiz.Count);
            quizQuestion.Text = $"Quiz complete! Final score: {quizScore}/{quiz.Count} ({percentage}%).";
            quizOptions.Controls.Clear();
            quizStatus.Text = quizScore >= 10
                ? "Excellent work — keep practising safe online habits."
                : "Keep learning: review the feedback and try again.";
            nextQuestionButton.Text = "Restart quiz";
            quizCompleted = true;

            try
            {
                Database.Log($"Quiz completed: {quizScore}/{quiz.Count} ({percentage}%)");
                RefreshActivity();
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }

            return;
        }

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        quizAnswered = false;
        var item = quiz[quizIndex];
        quizQuestion.Text = $"Question {quizIndex + 1} of {quiz.Count}: {item.Question}";
        quizOptions.Controls.Clear();
        for (int i = 0; i < item.Options.Length; i++)
        {
            int choice = i;
            var button = new Button { Text = $"{(char)('A' + i)}. {item.Options[i]}", Width = 760, Height = 42, TextAlign = ContentAlignment.MiddleLeft, AutoSize = false };
            button.Click += (_, _) => AnswerQuiz(choice);
            quizOptions.Controls.Add(button);
        }
        quizStatus.Text = $"Score: {quizScore}/{quiz.Count}";
        nextQuestionButton.Text = quizIndex == quiz.Count - 1 ? "Finish quiz" : "Next question";
    }

    private void AnswerQuiz(int choice)
    {
        if (quizAnswered) return;
        quizAnswered = true;
        var item = quiz[quizIndex];
        bool correct = choice == item.Answer;
        if (correct) quizScore++;
        quizStatus.Text = (correct ? "Correct. " : "Not quite. ") + item.Explanation + $"  Score: {quizScore}/{quiz.Count}";
        foreach (Control control in quizOptions.Controls) control.Enabled = false;
    }

    private void Say(string speaker, string message)
    {
        chatOutput.AppendText($"{speaker}: {message}\n\n");
        chatOutput.SelectionStart = chatOutput.TextLength;
        chatOutput.ScrollToCaret();
    }

    private void ShowDatabaseError(Exception ex) =>
        MessageBox.Show("Database operation failed. Check MySQL and the connection string.\n\n" + ex.Message,
            "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
