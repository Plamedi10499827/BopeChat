using System;
using System.Collections.Generic;

namespace BopeChat.Services
{
    internal class FuzzyMatcher
    {
        /// <summary>
        /// Calculates how similar two strings are.
        /// Returns a score between 0 and 1.
        /// 1 means an exact match.
        /// 0 means completely different.
        /// </summary>
        public double Similarity(string first, string second)
        {
            if (string.IsNullOrEmpty(first) &&
                string.IsNullOrEmpty(second))
            {
                return 1.0;
            }

            if (string.IsNullOrEmpty(first) ||
                string.IsNullOrEmpty(second))
            {
                return 0.0;
            }

            first = first.ToLowerInvariant();
            second = second.ToLowerInvariant();

            int distance = LevenshteinDistance(first, second);

            int maxLength = Math.Max(first.Length, second.Length);

            return 1.0 - (double)distance / maxLength;
        }

        /// <summary>
        /// Calculates the Levenshtein edit distance.
        /// </summary>
        private int LevenshteinDistance(string first, string second)
        {
            int[,] matrix = new int[first.Length + 1, second.Length + 1];

            for (int i = 0; i <= first.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= second.Length; j++)
            {
                matrix[0, j] = j;
            }

            for (int i = 1; i <= first.Length; i++)
            {
                for (int j = 1; j <= second.Length; j++)
                {
                    int cost = first[i - 1] == second[j - 1] ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(
                            matrix[i - 1, j] + 1,
                            matrix[i, j - 1] + 1
                        ),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            return matrix[first.Length, second.Length];
        }

        /// <summary>
        /// Finds the closest matching word from a list.
        /// </summary>
        public string FindBestMatch(
            string input,
            IEnumerable<string> options,
            double threshold = 0.5)
        {
            if (string.IsNullOrWhiteSpace(input) ||
                options == null)
            {
                return string.Empty;
            }

            string bestMatch = string.Empty;
            double bestScore = 0;

            foreach (string option in options)
            {
                if (string.IsNullOrWhiteSpace(option))
                {
                    continue;
                }

                double score = Similarity(input, option);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = option;
                }
            }

            return bestScore >= threshold
                ? bestMatch
                : string.Empty;
        }

        /// <summary>
        /// Checks whether the input approximately matches a target.
        /// </summary>
        public bool IsMatch(
            string input,
            string target,
            double threshold = 0.7)
        {
            return Similarity(input, target) >= threshold;
        }
    }
}