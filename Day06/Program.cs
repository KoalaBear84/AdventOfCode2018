using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Day06
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 06";
            Console.Title = title;
            ConsoleEx.WriteLine(title, ConsoleColor.Green);

            List<string> inputLines = (await File.ReadAllLinesAsync("input.txt")).ToList();

            List<Point> points = ParseInput(inputLines);

            ProcessInput(points, star: 1);
            ProcessInput(points, star: 2);

            ConsoleEx.WriteLine("END", ConsoleColor.Green);
            Console.ReadKey();
        }

        private static List<Point> ParseInput(List<string> inputLines)
        {
            List<Point> points = new List<Point>();

            foreach (string inputLine in inputLines)
            {
                string[] splittedLine = inputLine.Split(',');

                points.Add(new Point
                {
                    X = int.Parse(splittedLine.First().Trim()),
                    Y = int.Parse(splittedLine.Last().Trim())
                });
            }

            return points;
        }

        private static void ProcessInput(List<Point> points, int star = 1)
        {
            Point[] pointsArray = points.ToArray();

            Stopwatch stopwatch = Stopwatch.StartNew();

            FillGrid(points, pointsArray, out int maxX, out int maxY, out GridPoint[,] grid, out Dictionary<int, int> counts);

            SaveGridToHtmlFile(grid);

            List<GridPoint> allPoints = AllPoints(grid);

            if (star == 1)
            {
                KeyValuePair<int, int> pointIndexArea = counts.OrderByDescending(c => c.Value).First();

                // Get the largest area not touching the sides (largest area which is not infinite as required)
                while (true)
                {
                    var res = allPoints.Where(ap => ap?.PointIndex == pointIndexArea.Key);

                    if (!res.Any(gp => gp.Point.X == 0 || gp.Point.Y == 0 || gp.Point.X == maxX - 1 || gp.Point.Y == maxY - 1))
                    {
                        break;
                    }

                    counts.Remove(pointIndexArea.Key);

                    pointIndexArea = counts.OrderByDescending(c => c.Value).First();
                }

                stopwatch.Stop();

                ConsoleEx.WriteLine($"Star 1. {stopwatch.ElapsedMilliseconds}ms. Answer: {pointIndexArea.Value} [{pointIndexArea.Key}]", ConsoleColor.Yellow);
            }
            else
            {
                IEnumerable<GridPoint> distanceUpTo10000 = allPoints.Where(p => p?.TotalDistancesToAllPoints <= 10000);

                stopwatch.Stop();

                ConsoleEx.WriteLine($"Star 2. {stopwatch.ElapsedMilliseconds}ms. Answer: {distanceUpTo10000.Count()}", ConsoleColor.Yellow);
            }
        }

        private static void FillGrid(List<Point> points, Point[] pointsArray, out int maxX, out int maxY, out GridPoint[,] grid, out Dictionary<int, int> counts)
        {
            maxX = points.Max(p => p.X) + 1;
            maxY = points.Max(p => p.Y) + 1;
            grid = new GridPoint[maxX, maxY];
            counts = new Dictionary<int, int>();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Point currentPoint = new Point(x, y);

                    var groupedByDistance = pointsArray.GroupBy(
                        p => ManhattanDistance(p, currentPoint),
                        (key, g) => new
                        {
                            Distance = key,
                            Points = g.ToList()
                        }
                    ).OrderBy(p => p.Distance);

                    var closestGroup = groupedByDistance.First();

                    int pointIndex = -1;

                    if (closestGroup.Points.Count == 1)
                    {
                        pointIndex = points.IndexOf(closestGroup.Points.First());

                        if (!counts.ContainsKey(pointIndex))
                        {
                            counts.Add(pointIndex, 0);
                        }

                        counts[pointIndex]++;
                    }

                    GridPoint gridPoint = new GridPoint
                    {
                        PointIndex = pointIndex,
                        Point = new Point(x, y),
                        ExactPoint = closestGroup.Distance == 0,
                        TotalDistancesToAllPoints = groupedByDistance.Sum(g => g.Distance * g.Points.Count)
                    };

                    if (pointIndex != -1)
                    {
                        gridPoint.PointOriginal = pointsArray[pointIndex];
                    }

                    grid[x, y] = gridPoint;
                }
            }
        }

        private static List<GridPoint> AllPoints(GridPoint[,] grid)
        {
            List<GridPoint> allPoints = new List<GridPoint>();

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    allPoints.Add(grid[x, y]);
                }
            }

            return allPoints;
        }

        /// <summary>
        /// Draw grid to HTML (with real input not really 'viewable' :P
        /// </summary>
        /// <param name="grid">Grid</param>
        private static void SaveGridToHtmlFile(GridPoint[,] grid)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<style>");
            stringBuilder.AppendLine("* {");
            stringBuilder.AppendLine("\tfont-size: 10px;");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine(".point {");
            stringBuilder.AppendLine("\tfont-weight: bold;");
            stringBuilder.AppendLine("\tcolor: blue;");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("</style>");

            stringBuilder.AppendLine("<pre>");

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                string gridLine = string.Empty;

                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    GridPoint gridPoint = grid[x, y];
                    gridLine += $"{(gridPoint?.ExactPoint == true ? "<span class=\"point\">" : "")}{(gridPoint?.PointIndex == null ? "." : gridPoint.PointIndex.ToString()),3}{(gridPoint?.ExactPoint == true ? "</span>" : "")}";
                }

                stringBuilder.AppendLine($"{gridLine}<br />");
            }

            stringBuilder.AppendLine("</pre>");

            File.WriteAllText("Grid.html", stringBuilder.ToString());
        }

        public static int ManhattanDistance(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        [DebuggerDisplay("#{PointIndex} @ {Point.X}, {Point.Y}, {ExactPoint}")]
        public class GridPoint
        {
            public int PointIndex { get; set; }
            public Point Point { get; set; }
            public Point PointOriginal { get; set; }
            public bool ExactPoint { get; set; }
            public int TotalDistancesToAllPoints { get; set; }
        }
    }
}
