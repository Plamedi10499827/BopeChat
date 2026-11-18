using System;
using System.Collections.Generic;

namespace BopeChat.Services
{
    internal class QuizService
    {
        private readonly List<QuizQuestion> questions = new List<QuizQuestion>();

        private int currentQuestionIndex = 0;
        private int score = 0;
        private bool quizStarted = false;

        public int Score => score;

        public int TotalQuestions => questions.Count;

        public int CurrentQuestionNumber => currentQuestionIndex + 1;

        public bool IsQuizStarted => quizStarted;

        public bool IsQuizFinished =>
            quizStarted && currentQuestionIndex >= questions.Count;

        public QuizService()
        {
            LoadQuestions();
        }

        /// <summary>
        /// Loads the cybersecurity quiz questions.
        /// </summary>
        private void LoadQuestions()
        {
            questions.Clear();

            questions.Add(new QuizQuestion(
                "What is phishing?",
                new string[]
                {
                    "A type of computer hardware",
                    "A scam used to steal personal information",
                    "A programming language",
                    "A firewall setting"
                },
                1,
                "Phishing is a scam where attackers trick people into revealing sensitive information."
            ));

            questions.Add(new QuizQuestion(
                "Which password is the strongest?",
                new string[]
                {
                    "123456",
                    "password",
                    "Bope123",
                    "T!m3#R0ck$92"
                },
                3,
                "Strong passwords should be long, unique and difficult to guess."
            ));

            questions.Add(new QuizQuestion(
                "What should you do with a suspicious email link?",
                new string[]
                {
                    "Click it immediately",
                    "Forward it to everyone",
                    "Verify the sender and link before clicking",
                    "Enter your password"
                },
                2,
                "Always verify suspicious links before clicking them."
            ));

            questions.Add(new QuizQuestion(
                "What is malware?",
                new string[]
                {
                    "Software designed to harm or exploit systems",
                    "A secure password",
                    "A computer monitor",
                    "A backup system"
                },
                0,
                "Malware is malicious software designed to damage systems or steal information."
            ));

            questions.Add(new QuizQuestion(
                "What does a firewall do?",
                new string[]
                {
                    "Creates passwords",
                    "Monitors and filters network traffic",
                    "Deletes all computer files",
                    "Makes a computer faster"
                },
                1,
                "A firewall helps control network traffic and prevent unauthorized access."
            ));

            questions.Add(new QuizQuestion(
                "What is two-factor authentication?",
                new string[]
                {
                    "Using two different passwords",
                    "Sharing your password with two people",
                    "An additional verification step for login",
                    "Logging in twice"
                },
                2,
                "Two-factor authentication adds an additional layer of account security."
            ));

            questions.Add(new QuizQuestion(
                "What should you do when using public Wi-Fi?",
                new string[]
                {
                    "Access every website without checking",
                    "Share your passwords",
                    "Disable all security",
                    "Avoid sensitive transactions on unsecured networks"
                },
                3,
                "Public Wi-Fi can be risky. Avoid sensitive transactions on unsecured networks."
            ));

            questions.Add(new QuizQuestion(
                "What is encryption?",
                new string[]
                {
                    "Deleting computer data",
                    "Converting information into coded data",
                    "Installing an application",
                    "Sharing files publicly"
                },
                1,
                "Encryption protects information by converting it into a coded format."
            ));

            questions.Add(new QuizQuestion(
                "What is ransomware?",
                new string[]
                {
                    "Software that encrypts files and demands payment",
                    "A normal operating system update",
                    "A type of firewall",
                    "A password manager"
                },
                0,
                "Ransomware locks or encrypts files and demands payment from victims."
            ));

            questions.Add(new QuizQuestion(
                "Why should software be updated regularly?",
                new string[]
                {
                    "To remove all files",
                    "To make passwords public",
                    "To fix bugs and security vulnerabilities",
                    "To disable security features"
                },
                2,
                "Updates often fix vulnerabilities that attackers could exploit."
            ));

            questions.Add(new QuizQuestion(
                "What is social engineering?",
                new string[]
                {
                    "Building computer hardware",
                    "Manipulating people into revealing information or performing actions",
                    "Creating secure backups",
                    "Writing computer programs"
                },
                1,
                "Social engineering exploits human trust to obtain information or access."
            ));

            questions.Add(new QuizQuestion(
                "What is the safest way to protect important files?",
                new string[]
                {
                    "Keep only one copy",
                    "Share them publicly",
                    "Disable antivirus",
                    "Maintain secure backups"
                },
                3,
                "Regular secure backups help recover information if files are lost or attacked."
            ));
        }

        /// <summary>
        /// Starts or resets the quiz.
        /// </summary>
        public void StartQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            quizStarted = true;
        }

        /// <summary>
        /// Returns the current quiz question.
        /// </summary>
        public QuizQuestion GetCurrentQuestion()
        {
            if (!quizStarted)
            {
                throw new InvalidOperationException(
                    "The quiz has not been started.");
            }

            if (IsQuizFinished)
            {
                throw new InvalidOperationException(
                    "The quiz has already been completed.");
            }

            return questions[currentQuestionIndex];
        }

        /// <summary>
        /// Checks the user's answer.
        /// Answer indexes start at zero.
        /// </summary>
        public bool SubmitAnswer(int answerIndex)
        {
            if (!quizStarted || IsQuizFinished)
            {
                return false;
            }

            QuizQuestion question = questions[currentQuestionIndex];

            bool correct = answerIndex == question.CorrectAnswerIndex;

            if (correct)
            {
                score++;
            }

            currentQuestionIndex++;

            return correct;
        }

        /// <summary>
        /// Returns feedback for the current question.
        /// </summary>
        public string GetFeedback(int answerIndex)
        {
            if (currentQuestionIndex <= 0)
            {
                return string.Empty;
            }

            QuizQuestion question = questions[currentQuestionIndex - 1];

            if (answerIndex == question.CorrectAnswerIndex)
            {
                return "Correct! " + question.Explanation;
            }

            return "Incorrect. The correct answer is: " +
                   question.Answers[question.CorrectAnswerIndex] +
                   "\n" + question.Explanation;
        }

        /// <summary>
        /// Returns the final quiz results.
        /// </summary>
        public string GetFinalResults()
        {
            if (!quizStarted)
            {
                return "The quiz has not been started.";
            }

            if (!IsQuizFinished)
            {
                return "The quiz is not finished yet.";
            }

            double percentage =
                (double)score / questions.Count * 100;

            string performance;

            if (percentage >= 80)
            {
                performance = "Excellent work! You have strong cybersecurity knowledge.";
            }
            else if (percentage >= 60)
            {
                performance = "Good job! Keep learning and improving your cybersecurity knowledge.";
            }
            else
            {
                performance = "Keep practicing! Reviewing cybersecurity basics will help you improve.";
            }

            return
                "Cybersecurity Quiz Completed!\n\n" +
                "Your score: " + score + " / " + questions.Count + "\n" +
                "Percentage: " + percentage.ToString("0.0") + "%\n\n" +
                performance;
        }

        /// <summary>
        /// Resets the quiz.
        /// </summary>
        public void ResetQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            quizStarted = false;
        }
    }
}