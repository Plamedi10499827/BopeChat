using System;
using System.Collections.Generic;

namespace BopeChat.Services
{
    internal class ChatbotEngine
    {
        private readonly TaskDescriptions taskDescriptions = new TaskDescriptions();
        private readonly ReminderParser reminderParser = new ReminderParser();

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a message so I can assist you.";
            }

            string message = input.Trim().ToLowerInvariant();

            // Greetings
            if (message.Contains("hello") ||
                message.Contains("hi") ||
                message.Contains("hey"))
            {
                return "Hello! I'm BopeChat, your cybersecurity assistant. How can I help you today?";
            }

            // Help
            if (message.Contains("help"))
            {
                return GetHelpResponse();
            }

            // Cybersecurity topics
            if (message.Contains("password"))
            {
                return "Use strong, unique passwords containing a combination of uppercase letters, lowercase letters, numbers and symbols. Consider using a password manager.";
            }

            if (message.Contains("phishing"))
            {
                return "Phishing is a cyberattack where criminals use deceptive emails, messages or websites to trick people into revealing sensitive information. Always check links and verify senders.";
            }

            if (message.Contains("malware") ||
                message.Contains("virus"))
            {
                return "Malware is malicious software designed to damage systems, steal information or gain unauthorized access. Keep your antivirus software updated and avoid suspicious downloads.";
            }

            if (message.Contains("hacking"))
            {
                return "Hacking involves exploiting computer systems or networks. Ethical hacking helps identify vulnerabilities so they can be fixed before malicious attackers exploit them.";
            }

            if (message.Contains("firewall"))
            {
                return "A firewall monitors and filters network traffic according to security rules. It helps protect your computer or network from unauthorized access.";
            }

            if (message.Contains("encryption"))
            {
                return "Encryption converts readable information into encoded data so that only authorized people with the correct key can access it.";
            }

            if (message.Contains("two factor") ||
                message.Contains("2fa") ||
                message.Contains("multi factor"))
            {
                return "Two-factor authentication adds another layer of security by requiring an additional verification method besides your password.";
            }

            if (message.Contains("privacy"))
            {
                return "Protect your privacy by limiting the personal information you share online, reviewing app permissions and using secure websites.";
            }

            if (message.Contains("wifi"))
            {
                return "Avoid sensitive transactions on public Wi-Fi. Use secure networks, WPA2 or WPA3 encryption, and consider a trusted VPN when using public networks.";
            }

            if (message.Contains("scam"))
            {
                return "Be cautious of messages requesting money, personal information or urgent action. Verify requests through an official contact method before responding.";
            }

            // Task assistant
            if (message.Contains("task"))
            {
                return taskDescriptions.GetTaskHelp();
            }

            // Reminders
            if (message.Contains("reminder") ||
                message.Contains("remind"))
            {
                return "I can help you create reminders. Try adding a task with a date or time, for example: 'Remind me tomorrow at 14:00 to study cybersecurity.'";
            }

            // Quiz
            if (message.Contains("quiz"))
            {
                return "You can start the cybersecurity quiz by typing 'start quiz' in the chat.";
            }

            // Thank you
            if (message.Contains("thank"))
            {
                return "You're welcome! I'm always here to help you stay safe online.";
            }

            // Goodbye
            if (message.Contains("bye") ||
                message.Contains("goodbye"))
            {
                return "Goodbye! Stay safe online and remember to practice good cybersecurity habits.";
            }

            // Default response
            return "I'm not sure I understand. You can ask me about cybersecurity, passwords, phishing, malware, or tasks. Type 'help' to see what I can do.";
        }

        private string GetHelpResponse()
        {
            return
                "Here's what I can help you with:\n\n" +
                "• Cybersecurity information\n" +
                "• Password safety\n" +
                "• Phishing awareness\n" +
                "• Malware and viruses\n" +
                "• Firewalls and encryption\n" +
                "• Online privacy\n" +
                "• Task management\n" +
                "• Reminders\n" +
                "• Cybersecurity quizzes\n\n" +
                "Try asking me a question about cybersecurity or type 'start quiz'.";
        }
    }
}