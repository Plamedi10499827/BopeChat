using System;

namespace BopeChat.Services
{
    internal class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;

        public string[] Answers { get; set; } = Array.Empty<string>();

        public int CorrectAnswerIndex { get; set; }

        public string Explanation { get; set; } = string.Empty;

        public QuizQuestion()
        {
        }

        public QuizQuestion(
            string question,
            string[] answers,
            int correctAnswerIndex,
            string explanation)
        {
            Question = question;
            Answers = answers;
            CorrectAnswerIndex = correctAnswerIndex;
            Explanation = explanation;
        }
    }
}