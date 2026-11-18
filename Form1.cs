using ChatBot;
using System;
using System.Data;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace BopeChat
{
    public partial class Form1 : Form
    {
        private readonly ChatEngine engine = new ChatEngine();
        private SoundPlayer player = null!;
        private TabControl part3Tabs = null!;
        private TabPage chatPage = null!;
        private TabPage tasksPage = null!;
        private TabPage quizPage = null!;
        private TabPage activityPage = null!;

        private DataGridView tasksGrid = null!;
        private DataGridView activityGrid = null!;

        private TextBox taskTitleInput = null!;
        private TextBox taskDescriptionInput = null!;
        private DateTimePicker reminderPicker = null!;
        private CheckBox reminderCheckBox = null!;

        private Label quizQuestionLabel = null!;
        private Label quizFeedbackLabel = null!;
        private Label quizScoreLabel = null!;
        private Button quizAnswerA = null!;
        private Button quizAnswerB = null!;
        private Button quizAnswerC = null!;
        private Button quizAnswerD = null!;
        private Button quizNextButton = null!;

        private System.Windows.Forms.Timer reminderTimer = null!;

        private int quizQuestionIndex = 0;
        private int quizScore = 0;
        private bool quizAnswered = false;

        private readonly string[,] quizQuestions =
        {
            {
                "Which password is the strongest?",
                "123456",
                "password",
                "Mango!River!92Cloud",
                "Your birthday",
                "C",
                "A long, unique password or passphrase is harder to guess."
            },
            {
                "What is phishing?",
                "A computer game",
                "A scam that tricks people into sharing information",
                "A type of printer",
                "A software update",
                "B",
                "Phishing uses fake messages or websites to steal information."
            },
            {
                "What should you do with a suspicious email link?",
                "Click it immediately",
                "Forward it to everyone",
                "Ignore all warnings",
                "Avoid clicking and verify the sender",
                "D",
                "Always verify unexpected messages before opening links."
            },
            {
                "What is malware?",
                "Harmful software",
                "A strong password",
                "A safe website",
                "A backup",
                "A",
                "Malware is software designed to harm devices or access information."
            },
            {
                "Why should you update your software?",
                "To delete all your files",
                "To make passwords shorter",
                "To fix security weaknesses",
                "To disable security",
                "C",
                "Updates often fix security vulnerabilities."
            },
            {
                "What does MFA mean?",
                "Multiple File Access",
                "Multifactor Authentication",
                "Main Firewall Application",
                "Managed File Account",
                "B",
                "MFA adds another verification step to protect accounts."
            },
            {
                "What should you do on public Wi-Fi?",
                "Share your passwords",
                "Disable all security",
                "Trust every network",
                "Use trusted networks and avoid suspicious activity",
                "D",
                "Check the network name and be careful when accessing sensitive accounts."
            },
            {
                "What is a backup?",
                "An extra copy of important files",
                "A type of virus",
                "A suspicious email",
                "A weak password",
                "A",
                "Backups help recover information if original files are lost."
            },
            {
                "What is social engineering?",
                "A computer update",
                "A password manager",
                "Manipulating people into unsafe actions",
                "A backup method",
                "C",
                "Social engineering tricks people into sharing information or taking unsafe actions."
            },
            {
                "Which information should you avoid sharing publicly?",
                "A general hobby",
                "Your home address and passwords",
                "A favourite colour",
                "A public weather report",
                "B",
                "Protect sensitive personal information such as addresses and passwords."
            },
            {
                "What should you do if you suspect your account was hacked?",
                "Ignore it",
                "Share your password",
                "Delete all security settings",
                "Change your password using the official website and review account activity",
                "D",
                "Secure the account from a trusted device and check for unfamiliar activity."
            },
            {
                "How can you reduce the risk of online scams?",
                "Verify unexpected requests independently",
                "Trust every message",
                "Send money immediately",
                "Share verification codes",
                "A",
                "Pause and verify unexpected requests before sharing money or information."
            }
        };

        public Form1()
        {
            InitializeComponent();

            BuildPart3Interface();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                Database.Initialize();

                Database.Log(
                    "Application opened",
                    "BopeChat started.");

                LoadTasks();
                LoadActivity();

                reminderTimer = new System.Windows.Forms.Timer();
                reminderTimer.Interval = 60000;
                reminderTimer.Tick += ReminderTimer_Tick;
                reminderTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to the database.\n\n" +
                    ex.Message +
                    "\n\nCheck your MySQL password and database connection.",
                    "Database connection error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            PlayGreeting();
            DisplayLogo();

            ChatInput.ReadOnly = true;

            ChatInput.AppendText(
                "BopeChat: Hello! Welcome to your cybersecurity awareness chatbot."
                + Environment.NewLine);

            ChatInput.AppendText(
                "BopeChat: What is your name?"
                + Environment.NewLine);
        }

        // -------------------------------------------------
        // BUILD THE PART 3 INTERFACE
        // -------------------------------------------------

        private void BuildPart3Interface()
        {
            Text = "BopeChat - Cybersecurity Awareness Chatbot";
            MinimumSize = new Size(800, 600);
            Size = new Size(1000, 700);

            part3Tabs = new TabControl();
            part3Tabs.Dock = DockStyle.Fill;

            chatPage = new TabPage("BopeChat");
            tasksPage = new TabPage("Task Assistant");
            quizPage = new TabPage("Cybersecurity Quiz");
            activityPage = new TabPage("Activity Log");

            part3Tabs.TabPages.Add(chatPage);
            part3Tabs.TabPages.Add(tasksPage);
            part3Tabs.TabPages.Add(quizPage);
            part3Tabs.TabPages.Add(activityPage);

            // Move the original designer controls into the Chat tab.
            Control[] originalControls =
            {
                ChatInput,
                ChatInput2,
                button2,
                button3,
                pictureBox1
            };

            foreach (Control control in originalControls)
            {
                if (control != null)
                {
                    Controls.Remove(control);
                    chatPage.Controls.Add(control);
                }
            }

            // Fix the layout of the original chatbot controls
            chatPage.SuspendLayout();

            pictureBox1.Location = new Point(15, 15);
            pictureBox1.Size = new Size(140, 140);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Visible = true;

            ChatInput.Location = new Point(170, 15);
            ChatInput.Size = new Size(700, 480);
            ChatInput.Anchor = AnchorStyles.Top |
                              AnchorStyles.Bottom |
                              AnchorStyles.Left |
                              AnchorStyles.Right;
            ChatInput.Visible = true;
            ChatInput.ReadOnly = true;

            ChatInput2.Location = new Point(170, 510);
            ChatInput2.Size = new Size(590, 35);
            ChatInput2.Anchor = AnchorStyles.Bottom |
                                AnchorStyles.Left |
                                AnchorStyles.Right;
            ChatInput2.Visible = true;
            ChatInput2.Enabled = true;

            button3.Location = new Point(770, 510);
            button3.Size = new Size(100, 35);
            button3.Anchor = AnchorStyles.Bottom |
                             AnchorStyles.Right;
            button3.Visible = true;
            button3.Enabled = true;
            button3.Text = "Send";

            button2.Location = new Point(15, 170);
            button2.Size = new Size(140, 40);
            button2.Visible = true;

            chatPage.ResumeLayout();

            Controls.Add(part3Tabs);
            part3Tabs.BringToFront();

            BuildTasksPage();
            BuildQuizPage();
            BuildActivityPage();
        }

        // -------------------------------------------------
        // TASK ASSISTANT
        // -------------------------------------------------
        private void BuildTasksPage()
        {
            Label heading = new Label
            {
                Text = "Cybersecurity Task Assistant",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label titleLabel = new Label
            {
                Text = "Task title:",
                Location = new Point(20, 65),
                AutoSize = true
            };

            taskTitleInput = new TextBox
            {
                Location = new Point(130, 62),
                Size = new Size(300, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            Label descriptionLabel = new Label
            {
                Text = "Description:",
                Location = new Point(20, 105),
                AutoSize = true
            };

            taskDescriptionInput = new TextBox
            {
                Location = new Point(130, 102),
                Size = new Size(300, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            reminderCheckBox = new CheckBox
            {
                Text = "Set reminder",
                Location = new Point(20, 145),
                AutoSize = true
            };

            reminderPicker = new DateTimePicker
            {
                Location = new Point(130, 142),
                Size = new Size(205, 28),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                ShowUpDown = true,
                Value = DateTime.Now.AddHours(1)
            };

            Button addButton = new Button
            {
                Text = "Add Task",
                Location = new Point(20, 185),
                Size = new Size(105, 36)
            };
            addButton.Click += AddTaskButton_Click;

            Button completeButton = new Button
            {
                Text = "Complete",
                Location = new Point(135, 185),
                Size = new Size(115, 36)
            };
            completeButton.Click += CompleteTaskButton_Click;

            Button deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(260, 185),
                Size = new Size(100, 36)
            };
            deleteButton.Click += DeleteTaskButton_Click;

            Button refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(370, 185),
                Size = new Size(100, 36)
            };
            refreshButton.Click += (s, e) => LoadTasks();

            tasksGrid = new DataGridView
            {
                Location = new Point(20, 235),
                Size = new Size(900, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 34,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray
            };

            tasksPage.Controls.AddRange(new Control[]
            {
                heading,
                titleLabel,
                taskTitleInput,
                descriptionLabel,
                taskDescriptionInput,
                reminderCheckBox,
                reminderPicker,
                addButton,
                completeButton,
                deleteButton,
                refreshButton,
                tasksGrid
            });
        }

        private void AddTaskButton_Click(object? sender, EventArgs e)
        {
            string title = taskTitleInput.Text.Trim();
            string description = taskDescriptionInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.");
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = "No description provided.";
            }

            DateTime? reminder = reminderCheckBox.Checked
                ? reminderPicker.Value
                : null;

            if (reminder.HasValue && reminder.Value <= DateTime.Now)
            {
                MessageBox.Show("Please select a future reminder time.");
                return;
            }

            try
            {
                Database.AddTask(title, description, reminder);

                MessageBox.Show("Your task has been added.");

                taskTitleInput.Clear();
                taskDescriptionInput.Clear();
                reminderCheckBox.Checked = false;

                LoadTasks();
                LoadActivity();
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private int? GetSelectedTaskId()
        {
            if (tasksGrid.CurrentRow == null)
            {
                MessageBox.Show("Please select a task first.");
                return null;
            }

            object? value = tasksGrid.CurrentRow.Cells["TaskId"].Value;

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(value);
        }

        private void CompleteTaskButton_Click(object? sender, EventArgs e)
        {
            int? id = GetSelectedTaskId();

            if (!id.HasValue)
                return;

            try
            {
                Database.SetCompleted(id.Value, true);

                MessageBox.Show("Task marked as completed.");

                LoadTasks();
                LoadActivity();
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private void DeleteTaskButton_Click(object? sender, EventArgs e)
        {
            int? id = GetSelectedTaskId();

            if (!id.HasValue)
                return;

            DialogResult answer = MessageBox.Show(
                "Are you sure you want to delete this task?",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                Database.DeleteTask(id.Value);

                MessageBox.Show("Task deleted.");

                LoadTasks();
                LoadActivity();
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        private void LoadTasks()
        {
            try
            {
                if (tasksGrid != null)
                {
                    tasksGrid.DataSource = Database.GetTasks();

                    if (tasksGrid.Columns.Contains("TaskId"))
                    {
                        tasksGrid.Columns["TaskId"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // -------------------------------------------------
        // REMINDERS
        // -------------------------------------------------

        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                DataTable reminders = Database.DueReminders();

                foreach (DataRow row in reminders.Rows)
                {
                    int id = Convert.ToInt32(row["TaskId"]);
                    string title = Convert.ToString(row["Title"]) ?? "Task";

                    MessageBox.Show(
                        "Reminder: " + title,
                        "BopeChat Reminder",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Database.MarkReminderNotified(id);
                }

                if (reminders.Rows.Count > 0)
                {
                    LoadTasks();
                    LoadActivity();
                }
            }
            catch (Exception ex)
            {
                // Avoid displaying a popup every minute if the database
                // temporarily becomes unavailable.
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        // -------------------------------------------------
        // CYBERSECURITY QUIZ
        // -------------------------------------------------
        private void BuildQuizPage()
        {
            Label heading = new Label
            {
                Text = "Cybersecurity Awareness Quiz",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            quizQuestionLabel = new Label
            {
                Location = new Point(20, 75),
                Size = new Size(900, 80),
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            quizAnswerA = CreateAnswerButton("A", 20, 170);
            quizAnswerB = CreateAnswerButton("B", 20, 215);
            quizAnswerC = CreateAnswerButton("C", 20, 260);
            quizAnswerD = CreateAnswerButton("D", 20, 305);

            quizFeedbackLabel = new Label
            {
                Location = new Point(20, 360),
                Size = new Size(900, 80),
                Font = new Font("Arial", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            quizScoreLabel = new Label
            {
                Location = new Point(20, 450),
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };

            quizNextButton = new Button
            {
                Text = "Start Quiz",
                Location = new Point(20, 490),
                Size = new Size(140, 38)
            };
            quizNextButton.Click += QuizNextButton_Click;

            quizPage.Controls.AddRange(new Control[]
            {
                heading,
                quizQuestionLabel,
                quizAnswerA,
                quizAnswerB,
                quizAnswerC,
                quizAnswerD,
                quizFeedbackLabel,
                quizScoreLabel,
                quizNextButton
            });

            SetQuizButtonsVisible(false);
            quizQuestionLabel.Text = "Test your cybersecurity knowledge with 12 questions.";
            quizScoreLabel.Text = "Score: 0 / 12";
        }
        private Button CreateAnswerButton(string answer, int x, int y)
        {
            Button button = new Button
            {
                Location = new Point(x, y),
                Size = new Size(700, 40),
                Text = answer,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10),
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            button.Click += QuizAnswerButton_Click;
            return button;
        }

        private void SetQuizButtonsVisible(bool visible)
        {
            quizAnswerA.Visible = visible;
            quizAnswerB.Visible = visible;
            quizAnswerC.Visible = visible;
            quizAnswerD.Visible = visible;
        }

        private void StartQuiz()
        {
            quizQuestionIndex = 0;
            quizScore = 0;
            quizAnswered = false;

            quizNextButton.Text = "Next Question";

            SetQuizButtonsVisible(true);
            ShowQuizQuestion();
        }
        private void ShowQuizQuestion()
        {
            if (quizQuestionIndex >= quizQuestions.GetLength(0))
            {
                FinishQuiz();
                return;
            }

            int number = quizQuestionIndex + 1;
            quizQuestionLabel.Text =
                "Question " + number + " of 12\n" +
                quizQuestions[quizQuestionIndex, 0];

            quizAnswerA.Text = "A. " + quizQuestions[quizQuestionIndex, 1];
            quizAnswerB.Text = "B. " + quizQuestions[quizQuestionIndex, 2];
            quizAnswerC.Text = "C. " + quizQuestions[quizQuestionIndex, 3];
            quizAnswerD.Text = "D. " + quizQuestions[quizQuestionIndex, 4];

            quizFeedbackLabel.Text = string.Empty;
            quizScoreLabel.Text = "Score: " + quizScore + " / 12";
            quizAnswered = false;

            quizAnswerA.Enabled = true;
            quizAnswerB.Enabled = true;
            quizAnswerC.Enabled = true;
            quizAnswerD.Enabled = true;
        }

        private void QuizAnswerButton_Click(object? sender, EventArgs e)
        {
            if (quizAnswered)
                return;

            if (sender is not Button selectedButton)
                return;

            string selectedAnswer = selectedButton.Text.Substring(0, 1);

            string correctAnswer =
                quizQuestions[quizQuestionIndex, 5];

            string explanation =
                quizQuestions[quizQuestionIndex, 6];

            if (selectedAnswer == correctAnswer)
            {
                quizScore++;

                quizFeedbackLabel.Text =
                    "Correct! " + explanation;
            }
            else
            {
                quizFeedbackLabel.Text =
                    "Incorrect. The correct answer is " +
                    correctAnswer + ". " + explanation;
            }

            quizAnswered = true;

            quizAnswerA.Enabled = false;
            quizAnswerB.Enabled = false;
            quizAnswerC.Enabled = false;
            quizAnswerD.Enabled = false;

            quizScoreLabel.Text = "Score: " + quizScore + " / 12";

            Database.Log(
                "Quiz answer",
                "Question " + (quizQuestionIndex + 1) +
                ", selected " + selectedAnswer +
                ", correct answer " + correctAnswer);
        }
        private void QuizNextButton_Click(object? sender, EventArgs e)
        {
            if (quizNextButton.Text == "Try Again")
            {
                StartQuiz();
                return;
            }

            if (quizQuestionIndex == 0 &&
                !quizAnswered &&
                quizNextButton.Text == "Start Quiz")
            {
                StartQuiz();
                return;
            }

            if (!quizAnswered)
            {
                MessageBox.Show("Please select an answer first.");
                return;
            }

            quizQuestionIndex++;

            if (quizQuestionIndex < quizQuestions.GetLength(0))
            {
                ShowQuizQuestion();
            }
            else
            {
                FinishQuiz();
            }
        }

        private void FinishQuiz()
        {
            SetQuizButtonsVisible(false);

            quizQuestionLabel.Text =
                "You have completed the cybersecurity quiz!";

            quizFeedbackLabel.Text =
                "Final result: " + quizScore + " out of 12.";

            quizScoreLabel.Text =
                "Final Score: " + quizScore + " / 12";

            quizNextButton.Text = "Try Again";

            Database.Log(
                "Quiz completed",
                "Final score: " + quizScore + " out of 12.");

            LoadActivity();

            MessageBox.Show(
                "Quiz completed!\nYour score is " +
                quizScore + " out of 12.",
                "Quiz Results",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // -------------------------------------------------
        // ACTIVITY LOG
        // -------------------------------------------------

        private void BuildActivityPage()
        {
            Label heading = new Label();

            heading.Text = "BopeChat Activity Log";
            heading.Font = new Font("Arial", 16, FontStyle.Bold);
            heading.Location = new Point(20, 20);
            heading.AutoSize = true;

            Button refreshButton = new Button();
            refreshButton.Text = "Refresh Activity";
            refreshButton.Location = new Point(20, 65);
            refreshButton.Size = new Size(140, 34);
            refreshButton.Click += (s, e) => LoadActivity();

            activityGrid = new DataGridView();

            activityGrid.Location = new Point(20, 110);
            activityGrid.Size = new Size(900, 450);

            activityGrid.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            activityGrid.ReadOnly = true;
            activityGrid.AllowUserToAddRows = false;
            activityGrid.AllowUserToDeleteRows = false;

            activityGrid.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            activityPage.Controls.AddRange(new Control[]
            {
                heading,
                refreshButton,
                activityGrid
            });
        }

        private void LoadActivity()
        {
            try
            {
                if (activityGrid != null)
                {
                    activityGrid.DataSource =
                        Database.GetRecentActivity(100);
                }
            }
            catch (Exception ex)
            {
                ShowDatabaseError(ex);
            }
        }

        // -------------------------------------------------
        // ORIGINAL BOPECHAT CHATBOT
        // -------------------------------------------------

        private void button2_Click(object? sender, EventArgs e)
        {
            PlayGreeting();
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            DisplayLogo();
        }

        private void PlayGreeting()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "voice",
                    "Bope_recording.wav");

                player?.Stop();
                player?.Dispose();

                player = new SoundPlayer(soundPath);
                player.Play();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Greeting audio could not be played.");
            }
        }

        private void DisplayLogo()
        {
            try
            {
                string imagePath = System.IO.Path.Combine(
                    Application.StartupPath,
                    "images",
                    "Bope.jpeg");

                Image? oldImage = pictureBox1.Image;

                pictureBox1.Image = Image.FromFile(imagePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                oldImage?.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Logo image could not be loaded.");
            }
        }

        private void button3_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            string message = ChatInput2.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please enter a message.");
                return;
            }

            ChatInput.AppendText(
                "You: " + message + Environment.NewLine);

            string? reply = ProcessPart3Command(message);

            if (reply == null)
            {
                reply = engine.GetResponse(message);
            }

            ChatInput.AppendText(
                "BopeChat: " + reply +
                Environment.NewLine + Environment.NewLine);

            try
            {
                Database.Log("Chat message", message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            ChatInput2.Clear();
            ChatInput2.Focus();
        }

        private string? ProcessPart3Command(string message)
        {
            string command = message.Trim();
            string lower = command.ToLowerInvariant();

            if (lower == "start quiz" ||
                lower == "take quiz" ||
                lower == "cybersecurity quiz")
            {
                part3Tabs.SelectedTab = quizPage;
                StartQuiz();

                return "Let's start your 12-question cybersecurity quiz! " +
                       "Choose an answer for each question.";
            }

            if (lower == "list tasks" ||
                lower == "show tasks")
            {
                part3Tabs.SelectedTab = tasksPage;
                LoadTasks();

                return "I've opened your Task Assistant. " +
                       "Your saved tasks are displayed there.";
            }

            if (lower == "show activity" ||
                lower == "activity log")
            {
                part3Tabs.SelectedTab = activityPage;
                LoadActivity();

                return "I've opened your Activity Log.";
            }

            const string addPrefix = "add task ";

            if (lower.StartsWith(addPrefix))
            {
                string title = command.Substring(addPrefix.Length).Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    return "Please type a task title after 'add task'.";
                }

                try
                {
                    Database.AddTask(
                        title,
                        "Task created through BopeChat.",
                        null);

                    LoadTasks();
                    LoadActivity();

                    return "Your task '" + title +
                           "' has been added successfully.";
                }
                catch (Exception ex)
                {
                    return "I could not add your task. " + ex.Message;
                }
            }

            return null;
        }

        private void ChatInput2_TextChanged(object? sender, EventArgs e)
        {
        }

        private void ChatInput_TextChanged(object? sender, EventArgs e)
        {
        }

        // Keep this method name to preserve your existing Designer event.
        private void ChatInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        // -------------------------------------------------
        // ERROR HANDLING AND CLEANUP
        // -------------------------------------------------

        private void ShowDatabaseError(Exception ex)
        {
            MessageBox.Show(
                "A database operation could not be completed.\n\n" +
                ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            reminderTimer?.Stop();
            reminderTimer?.Dispose();

            player?.Stop();
            player?.Dispose();

            if (pictureBox1 != null)
            {
                Image? oldImage = pictureBox1.Image;
                pictureBox1.Image = null;
                oldImage?.Dispose();
            }

            base.OnFormClosed(e);
        }
    }
}