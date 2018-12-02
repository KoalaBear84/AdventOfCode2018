using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 01";
            Console.Title = title;
            ConsoleEx.WriteLine(title, ConsoleColor.Green);

            List<string> inputLines = (await File.ReadAllLinesAsync("input.txt")).ToList();

            ProcessInput(inputLines, star: 1);
            ProcessInput(inputLines, star: 2);

            ConsoleEx.WriteLine("END", ConsoleColor.Green);
            Console.ReadKey();
        }

        private static void ProcessInput(List<string> inputLines, int star = 1)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            int frequency = 0;
            bool found = false;

            HashSet<int> visitedFrequencies = new HashSet<int>();

            while (true)
            {
                foreach (string inputLine in inputLines)
                {
                    Command command = ParseCommand(inputLine);

                    if (command.Operator == '+')
                    {
                        frequency += command.Amount;
                    }

                    if (command.Operator == '-')
                    {
                        frequency -= command.Amount;
                    }

                    if (star == 2)
                    {
                        if (visitedFrequencies.Contains(frequency))
                        {
                            found = true;
                            break;
                        }

                        visitedFrequencies.Add(frequency);
                    }
                }

                if (star == 1 || found)
                {
                    break;
                }
            }

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star {star}. {stopwatch.ElapsedMilliseconds}ms. Answer: {frequency}", ConsoleColor.Yellow);
        }

        private static Command ParseCommand(string command)
        {
            Regex regex = new Regex(@"(?<Operator>\S{1})(?<Amount>\d+)");

            Match regexMatch = regex.Match(command);

            Command parsedCommand = new Command();

            if (regexMatch.Success)
            {
                parsedCommand.Operator = regexMatch.Groups["Operator"].Value.First();
                parsedCommand.Amount = int.Parse(regexMatch.Groups["Amount"].Value);
            }

            return parsedCommand;
        }

        public class Command
        {
            public char Operator { get; set; }
            public int Amount { get; set; }
        }
    }
}
