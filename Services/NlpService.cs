using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BopeChat.Services
{
    internal class NlpService
    {
        public string DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "unknown";
            }

            string message = input.Trim().ToLowerInvariant();

            // Greetings
            if (Regex.IsMatch(message, @"\b(hello|hi|hey|good morning|good afternoon|good evening)\b"))
            {
                return "greeting";
            }

            // Help
            if (Regex.IsMatch(message, @"\b(help|commands|what can you do)\b"))
            {
                return "help";
            }

            // Quiz
            if (Regex.IsMatch(message, @"\b(quiz|test my knowledge|test me)\b"))
            {
                return "quiz";
            }

            // Reminders
            if (Regex.IsMatch(message, @"\b(remind|reminder|reminders)\b"))
            {
                return "reminder";
            }

            // Tasks
            if (Regex.IsMatch(message, @"\b(task|tasks|todo|to-do|to do|add task|complete task|delete task)\b"))
            {
                return "task";
            }

            // Cybersecurity topics
            if (Regex.IsMatch(message, @"\b(phishing|scam|scams|fraud)\b"))
            {
                return "phishing";
            }

            if (Regex.IsMatch(message, @"\b(password|passwords|passcode)\b"))
            {
                return "password";
            }

            if (Regex.IsMatch(message, @"\b(malware|virus|ransomware|spyware)\b"))
            {
                return "malware";
            }

            if (Regex.IsMatch(message, @"\b(firewall|network security)\b"))
            {
                return "firewall";
            }

            if (Regex.IsMatch(message, @"\b(encryption|encrypted|decrypt)\b"))
            {
                return "encryption";
            }

            if (Regex.IsMatch(message, @"\b(privacy|personal information|data protection)\b"))
            {
                return "privacy";
            }

            if (Regex.IsMatch(message, @"\b(wifi|wi-fi|wireless network)\b"))
            {
                return "wifi";
            }

            if (Regex.IsMatch(message, @"\b(hack|hacking|hacker|ethical hacking)\b"))
            {
                return "hacking";
            }

            // Activity log
            if (Regex.IsMatch(message, @"\b(activity|log|history)\b"))
            {
                return "activity";
            }

            // Goodbye
            if (Regex.IsMatch(message, @"\b(bye|goodbye|see you)\b"))
            {
                return "goodbye";
            }

            // Gratitude
            if (Regex.IsMatch(message, @"\b(thanks|thank you|thx)\b"))
            {
                return "thanks";
            }

            return "unknown";
        }

        public string ExtractTaskDescription(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string text = input.Trim();

            // Remove common task command prefixes
            text = Regex.Replace(
                text,
                @"^(please\s+)?(add|create|make|remember|save)\s+(a\s+)?(new\s+)?task\s*[:\-]?\s*",
                "",
                RegexOptions.IgnoreCase);

            // Remove reminder prefixes
            text = Regex.Replace(
                text,
                @"^(please\s+)?remind\s+me\s+(to\s+)?",
                "",
                RegexOptions.IgnoreCase);

            // Remove unnecessary starting words
            text = Regex.Replace(
                text,
                @"^(please\s+)?(to\s+)?",
                "",
                RegexOptions.IgnoreCase);

            return text.Trim();
        }

        public string ExtractKeywords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            string[] stopWords =
            {
                "the", "a", "an", "is", "are", "was", "were",
                "to", "of", "and", "or", "in", "on", "at",
                "for", "with", "my", "me", "please", "can",
                "could", "would", "should", "what", "how"
            };

            string[] words = Regex
                .Replace(input.ToLowerInvariant(), @"[^\w\s]", "")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return string.Join(
                " ",
                words.Where(word => !stopWords.Contains(word))
            );
        }

        public bool ContainsDateOrTime(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string message = input.ToLowerInvariant();

            return Regex.IsMatch(
                message,
                @"\b(today|tomorrow|tonight|yesterday|"
                + @"monday|tuesday|wednesday|thursday|friday|saturday|sunday|"
                + @"minute|minutes|hour|hours|day|days|week|weeks|"
                + @"am|pm)\b")
                ||
                Regex.IsMatch(
                    message,
                    @"\b\d{1,2}:\d{2}\b");
        }

        public bool IsTaskCommand(string input)
        {
            return DetectIntent(input) == "task";
        }

        public bool IsReminderCommand(string input)
        {
            return DetectIntent(input) == "reminder";
        }

        public bool IsQuizCommand(string input)
        {
            return DetectIntent(input) == "quiz";
        }
    }
}