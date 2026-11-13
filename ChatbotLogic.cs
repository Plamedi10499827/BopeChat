namespace CybersecurityAwarenessChatbot;

public static class ChatbotLogic
{
    public static string DetectSentiment(string input)
    {
        string text = input.ToLowerInvariant();

        if (new[] { "worried", "scared", "afraid", "confused", "stressed", "help" }.Any(word => text.Contains(word)))
            return "supportive";

        if (new[] { "thank", "great", "good", "awesome", "happy" }.Any(word => text.Contains(word)))
            return "positive";

        return "neutral";
    }

    public static string Respond(string input)
    {
        string text = input.ToLowerInvariant();

        if (text.Contains("phishing") || text.Contains("suspicious email") || text.Contains("scam"))
            return "Cybersecurity tip: do not click suspicious links or attachments. Verify the sender through a trusted channel and report suspected phishing.";

        if (text.Contains("password") || text.Contains("passphrase"))
            return "Use a long, unique passphrase for every account. Store it in a reputable password manager and never share it.";

        if (text.Contains("2fa") || text.Contains("two-factor") || text.Contains("multi-factor") || text.Contains("mfa"))
            return "Enable multi-factor authentication wherever possible. An authenticator app or security key is generally safer than SMS alone.";

        if (text.Contains("privacy") || text.Contains("settings"))
            return "Review account privacy settings, limit public personal information, and check app permissions regularly.";

        if (text.Contains("safe browsing") || text.Contains("browser") || text.Contains("website"))
            return "Check the domain carefully, use HTTPS as one signal (not a guarantee), keep your browser updated, and avoid unexpected downloads.";

        if (text.Contains("worried") || text.Contains("scared") || text.Contains("confused"))
            return "It's understandable to feel unsure. Take one step at a time, and never rush into links or requests for sensitive information.";

        if (text.Contains("thank"))
            return "You're welcome! Staying alert and building safe habits helps protect your accounts.";

        return "I can help with passwords, phishing, privacy, safe browsing, tasks, reminders, quizzes, and the activity log. Try: “add a task to enable 2FA”.";
    }

    public static string? ExtractTaskTitle(string input)
    {
        string cleaned = input.Trim();
        string lower = cleaned.ToLowerInvariant();

        string[] prefixes =
        [
            "add a task to ", "add task to ", "create a task to ", "create task to ",
            "remind me to ", "set a reminder to ", "add a reminder to ",
            "remind me about ", "set reminder for "
        ];

        foreach (string prefix in prefixes)
        {
            if (lower.StartsWith(prefix))
            {
                string title = cleaned[prefix.Length..].Trim().TrimEnd('.', '?', '!');
                title = title.Replace("tomorrow", "", StringComparison.OrdinalIgnoreCase)
                             .Replace("today", "", StringComparison.OrdinalIgnoreCase).Trim();

                title = System.Text.RegularExpressions.Regex.Replace(
                    title,
                    @"\\s+in\\s+\\d{1,3}\\s+days?$",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                ).Trim();

                if (title.Length > 0)
                    return char.ToUpperInvariant(title[0]) + title[1..];
            }
        }

        if ((lower.Contains("task") || lower.Contains("remind")) &&
            (lower.Contains("password") || lower.Contains("2fa") ||
             lower.Contains("two-factor") || lower.Contains("privacy") ||
             lower.Contains("phishing") || lower.Contains("backup")))
        {
            if (lower.Contains("password")) return "Update my password";
            if (lower.Contains("2fa") || lower.Contains("two-factor")) return "Enable two-factor authentication";
            if (lower.Contains("privacy")) return "Review account privacy settings";
            if (lower.Contains("phishing")) return "Review how to identify phishing";
            if (lower.Contains("backup")) return "Check account backups";
        }

        return null;
    }

    public static DateTime? ExtractReminderDate(string input)
    {
        string lower = input.ToLowerInvariant();
        DateTime now = DateTime.Now;

        if (lower.Contains("tomorrow"))
            return now.Date.AddDays(1).AddHours(9);

        if (lower.Contains("today"))
            return now.Date.AddHours(17);

        var match = System.Text.RegularExpressions.Regex.Match(
            lower,
            @"in\s+(\d{1,3})\s+days?"
        );

        if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
            return now.AddDays(days);

        return null;
    }
}