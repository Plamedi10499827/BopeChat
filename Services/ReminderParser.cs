
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BopeChat.Services
{
    internal class ReminderParser
    {
        /// <summary>
        /// Parses reminder text and returns the reminder date/time.
        /// Returns null if no reminder time can be identified.
        /// </summary>
        public DateTime? ParseReminder(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            string text = input.Trim().ToLowerInvariant();
            DateTime now = DateTime.Now;

            // Tomorrow
            if (text.Contains("tomorrow"))
            {
                DateTime tomorrow = now.Date.AddDays(1);

                Match tomorrowTime = Regex.Match(
                    text,
                    @"(?:at\s*)?(\d{1,2}):(\d{2})");

                if (tomorrowTime.Success)
                {
                    int hour = int.Parse(tomorrowTime.Groups[1].Value);
                    int minute = int.Parse(tomorrowTime.Groups[2].Value);

                    if (hour >= 0 && hour <= 23 &&
                        minute >= 0 && minute <= 59)
                    {
                        return tomorrow.AddHours(hour).AddMinutes(minute);
                    }
                }

                return tomorrow;
            }

            // Today
            if (text.Contains("today"))
            {
                DateTime today = now.Date;

                Match todayTime = Regex.Match(
                    text,
                    @"(?:at\s*)?(\d{1,2}):(\d{2})");

                if (todayTime.Success)
                {
                    int hour = int.Parse(todayTime.Groups[1].Value);
                    int minute = int.Parse(todayTime.Groups[2].Value);

                    if (hour >= 0 && hour <= 23 &&
                        minute >= 0 && minute <= 59)
                    {
                        DateTime result = today
                            .AddHours(hour)
                            .AddMinutes(minute);

                        if (result > now)
                        {
                            return result;
                        }
                    }
                }

                return now.AddHours(1);
            }

            // In X minutes
            Match minutesMatch = Regex.Match(
                text,
                @"in\s+(\d+)\s+minutes?");

            if (minutesMatch.Success)
            {
                int minutes = int.Parse(
                    minutesMatch.Groups[1].Value);

                return now.AddMinutes(minutes);
            }

            // In X hours
            Match hoursMatch = Regex.Match(
                text,
                @"in\s+(\d+)\s+hours?");

            if (hoursMatch.Success)
            {
                int hours = int.Parse(
                    hoursMatch.Groups[1].Value);

                return now.AddHours(hours);
            }

            // In X days
            Match daysMatch = Regex.Match(
                text,
                @"in\s+(\d+)\s+days?");

            if (daysMatch.Success)
            {
                int days = int.Parse(
                    daysMatch.Groups[1].Value);

                return now.AddDays(days);
            }

            // In X weeks
            Match weeksMatch = Regex.Match(
                text,
                @"in\s+(\d+)\s+weeks?");

            if (weeksMatch.Success)
            {
                int weeks = int.Parse(
                    weeksMatch.Groups[1].Value);

                return now.AddDays(weeks * 7);
            }

            // Try parsing a specific date/time
            if (DateTime.TryParse(
                input,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                if (parsedDate > now)
                {
                    return parsedDate;
                }
            }

            // No reminder time found
            return null;
        }
    }
}
