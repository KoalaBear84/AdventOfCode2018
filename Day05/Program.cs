using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day05
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 05";
            Console.Title = title;
            ConsoleEx.WriteLine(title, ConsoleColor.Green);

            string input = (await File.ReadAllLinesAsync("input.txt")).First();

            ProcessInputStar1(input);
            ProcessInputStar2(input);

            ConsoleEx.WriteLine("END", ConsoleColor.Green);
            Console.ReadKey();
        }

        private static void ProcessInputStar1(string input)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            input = React(input);

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star 1. {stopwatch.ElapsedMilliseconds}ms. Answer: {input.Length}", ConsoleColor.Yellow);
        }

        private static string React(string input)
        {
            int removed = 0;

            do
            {
                removed = 0;

                for (int i = input.Length - 1; i >= 0; i--)
                {
                    if (input.Length >= i + 2)
                    {
                        string polymer = input.Substring(i, 2);
                        char first = polymer.First();
                        char last = polymer.Last();

                        if (first != last && char.ToLower(first) == char.ToLower(last))
                        {
                            removed++;
                            input = input.Remove(i, 2);
                        }
                    }
                }
            }
            while (removed > 0);
            return input;
        }

        private static void ProcessInputStar2(string input)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Dictionary<char, int> charStats = new Dictionary<char, int>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                string inputNew = input;
                inputNew = inputNew.Replace(c.ToString(), string.Empty);
                inputNew = inputNew.Replace(c.ToString().ToLower(), string.Empty);

                string output = React(inputNew);

                charStats[c] = output.Length;
            }

            stopwatch.Stop();

            int answer = charStats.OrderBy(cs => cs.Value).First().Value;

            ConsoleEx.WriteLine($"Star 2. {stopwatch.ElapsedMilliseconds}ms. Answer: {answer}", ConsoleColor.Yellow);
        }
    }
}
