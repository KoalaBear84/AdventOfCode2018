using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day02
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 02";
            Console.Title = title;
            ConsoleEx.WriteLine(title, ConsoleColor.Green);

            List<string> inputLines = (await File.ReadAllLinesAsync("input.txt")).ToList();

            ProcessInputStar1(inputLines);
            ProcessInputStar2(inputLines);

            ConsoleEx.WriteLine("END", ConsoleColor.Green);
            Console.ReadKey();
        }

        private static void ProcessInputStar1(List<string> inputLines)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            int charactersTwoTimes = 0;
            int charactersThreeTimes = 0;

            foreach (string inputLine in inputLines)
            {
                if (inputLine.GroupBy(c => c).Where(g => g.Count() == 2).FirstOrDefault() != null)
                {
                    charactersTwoTimes++;
                }
                if (inputLine.GroupBy(c => c).Where(g => g.Count() == 3).FirstOrDefault() != null)
                {
                    charactersThreeTimes++;
                }
            }

            int checksum = charactersTwoTimes * charactersThreeTimes;

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star 1. {stopwatch.ElapsedMilliseconds}ms. Answer: {checksum}", ConsoleColor.Yellow);
        }

        private static void ProcessInputStar2(List<string> inputLines)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string closestMatch = string.Empty;

            foreach (string first in inputLines)
            {
                foreach (string second in inputLines.Where(il => il != first))
                {
                    string matchingCharacters = string.Empty;

                    for (int pos = 0; pos < first.Length; pos++)
                    {
                        if (first[pos] == second[pos])
                        {
                            matchingCharacters += first[pos];
                        }
                    }

                    if (matchingCharacters.Length > closestMatch.Length)
                    {
                        closestMatch = matchingCharacters;
                    }
                }
            }

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star 2. {stopwatch.ElapsedMilliseconds}ms. Answer: {closestMatch}", ConsoleColor.Yellow);
        }
    }
}
