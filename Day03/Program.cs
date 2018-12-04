using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day03
{
    class Program
    {
        private static Regex Regex = new Regex(@"#(?<Number>\d+) @ (?<X>\d+),(?<Y>\d+): (?<Width>\d+)x(?<Height>\d+)");

        static async Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 03";
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

            List<Claim> claims = ParseClaims(inputLines);

            int[,] fabric = new int[1000, 1000];

            foreach (Claim claim in claims)
            {
                for (int x = claim.Rectangle.X; x < claim.Rectangle.X + claim.Rectangle.Width; x++)
                {
                    for (int y = claim.Rectangle.Y; y < claim.Rectangle.Y + claim.Rectangle.Height; y++)
                    {
                        fabric[x, y]++;
                    }
                }
            }

            int intersectedInches = 0;

            for (int x = 0; x < fabric.GetLength(0); x++)
            {
                for (int y = 0; y < fabric.GetLength(1); y++)
                {
                    if (fabric[x, y] >= 2)
                    {
                        intersectedInches++;
                    }
                }
            }

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star 1. {stopwatch.ElapsedMilliseconds}ms. Answer: {intersectedInches}", ConsoleColor.Yellow);
        }

        private static void ProcessInputStar2(List<string> inputLines)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            List<Claim> claims = ParseClaims(inputLines);

            Claim uniqueClaim = null;

            foreach (Claim claimOuter in claims)
            {
                bool isUnique = true;

                foreach (Claim claimInner in claims.Where(c => c != claimOuter))
                {
                    if (claimInner.Rectangle.IntersectsWith(claimOuter.Rectangle))
                    {
                        isUnique = false;
                        break;
                    }
                }

                if (isUnique)
                {
                    uniqueClaim = claimOuter;
                    break;
                }
            }

            stopwatch.Stop();

            ConsoleEx.WriteLine($"Star 2. {stopwatch.ElapsedMilliseconds}ms. Answer: {uniqueClaim.Number}", ConsoleColor.Yellow);
        }

        private static List<Claim> ParseClaims(List<string> inputLines)
        {
            List<Claim> claims = new List<Claim>();

            foreach (string inputLine in inputLines)
            {
                Match regexMatch = Regex.Match(inputLine);

                claims.Add(new Claim
                {
                    Number = int.Parse(regexMatch.Groups["Number"].Value),
                    Rectangle = new Rectangle
                    {
                        X = int.Parse(regexMatch.Groups["X"].Value),
                        Y = int.Parse(regexMatch.Groups["Y"].Value),
                        Width = int.Parse(regexMatch.Groups["Width"].Value),
                        Height = int.Parse(regexMatch.Groups["Height"].Value)
                    }
                });
            }

            return claims;
        }

        [DebuggerDisplay("#{Number} @ {X},{Y}: {Width}x{Height}")]
        public class Claim
        {
            public int Number { get; set; }
            public Rectangle Rectangle { get; set; }
        }
    }
}
