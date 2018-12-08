using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day04
{
    class Program
    {
        static Regex Regex1 = new Regex(@"\[(?<DateTime>\d+-\d+-\d+ \d+:\d+)\] (?<Description>.*)");
        static Regex Regex2 = new Regex(@"#(?<GuardNumber>\d+)");

        static async Task Main(string[] args)
        {
            string title = "AdventOfCode2018 - Day 04";
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

            List<Entry> entries = ParseInput(inputLines);

            File.WriteAllText("inputsorted.txt", string.Join(Environment.NewLine, entries.Select(e => e.Original)));

            DateTime date = entries.First().DateTime.Date;
            DateTime lastDate = entries.Last().DateTime.Date;

            Dictionary<DateTime, DaySchedule> daySchedules = new Dictionary<DateTime, DaySchedule>();

            do
            {
                daySchedules.Add(date, new DaySchedule { Date = date });

                date = date.AddDays(1);
            } while (date <= lastDate);

            date = entries.First().DateTime.Date;

            int guard = 0;

            do
            {
                DaySchedule daySchedule = daySchedules[date.Date];
                daySchedule.Guard = guard;
                List<Entry> todaysEntries = entries.Where(e => e.DateTime.Date == date).ToList();

                int sleeping = 0;

                for (int i = 0; i < 60; i++)
                {
                    Entry entry = todaysEntries.FirstOrDefault(e => e.DateTime.Minute == i);

                    if (entry?.Type == EntryType.GaurdBeginsShift)
                    {
                        guard = entry.Guard;
                        if (entry.DateTime.Hour == 0)
                        {
                            daySchedule.Guard = guard;
                        }
                        sleeping = 0;
                    }

                    if (entry?.Type == EntryType.Sleep)
                    {
                        sleeping = 1;
                    }

                    if (entry?.Type == EntryType.WakeUp)
                    {
                        sleeping = 0;
                    }

                    daySchedule.SleepingSchedule[i] = sleeping;
                }

                date = date.AddDays(1);
            } while (date <= lastDate);

            var schedulesPerGaurd = daySchedules.GroupBy(
                ds => ds.Value.Guard,
                (key, g) => new
                {
                    Guard = key,
                    Schedules = g.ToList(),
                    ScheduleMinutes = g.ToList().SelectMany(s => s.Value.SleepingSchedule),
                    SleepMinutes = g.ToList().Sum(s => s.Value.SleepingSchedule.Sum(sm => sm.Value))
                }
            );

            if (star == 1)
            {
                int guardMostAsleep = schedulesPerGaurd.OrderByDescending(spg => spg.SleepMinutes).First().Guard;
                IOrderedEnumerable<IGrouping<int, KeyValuePair<int, int>>> groupedByMinute =
                    schedulesPerGaurd
                        .First(spg => spg.Guard == guardMostAsleep)
                        .Schedules
                        .SelectMany(s => s.Value.SleepingSchedule)
                        .GroupBy(s => s.Key)
                        .OrderByDescending(s => s.Sum(b => b.Value));
                int minute = groupedByMinute.First().Key;

                stopwatch.Stop();

                ConsoleEx.WriteLine($"Star 1. {stopwatch.ElapsedMilliseconds}ms. Answer: {guardMostAsleep * minute}", ConsoleColor.Yellow);
            }
            else
            {
                IEnumerable<IGrouping<int, KeyValuePair<int, int>>> groupedByMinute = 
                    daySchedules
                        .SelectMany(s => s.Value.SleepingSchedule)
                        .GroupBy(s => s.Key);

                IOrderedEnumerable<IGrouping<int, KeyValuePair<int, int>>> orderedByMinute =
                    groupedByMinute
                        .OrderByDescending(s => s.Sum(b => b.Value));

                int minute = orderedByMinute.First().Key;

                var orderedEnumerable = schedulesPerGaurd.OrderByDescending(spg => spg.Schedules.Where(s => s.Value.SleepingSchedule[minute] == 1).Count());
                int guardMostSleptMinute = orderedEnumerable.First().Guard;

                stopwatch.Stop();

                ConsoleEx.WriteLine($"Star 2. {stopwatch.ElapsedMilliseconds}ms. Answer: {guardMostSleptMinute * minute}", ConsoleColor.Yellow);
            }

            //PrintSchedules(daySchedules);
        }

        private static void PrintSchedules(Dictionary<DateTime, DaySchedule> daySchedules)
        {
            Console.WriteLine(string.Join(Environment.NewLine, daySchedules.Select(ds => $"{ds.Value.Date:MM-dd} #{ds.Value.Guard,-4} {string.Join("", ds.Value.SleepingSchedule.Select(e => e.Value == 1 ? "." : "#"))}")));
        }

        private static List<Entry> ParseInput(List<string> inputLines)
        {
            List<Entry> entries = new List<Entry>();

            foreach (string inputLine in inputLines)
            {
                Match match1 = Regex1.Match(inputLine);

                string description = match1.Groups["Description"].Value;

                Entry entry = new Entry
                {
                    Original = inputLine,
                    DateTime = DateTime.Parse(match1.Groups["DateTime"].Value)
                };

                if (description.Contains("Guard"))
                {
                    entry.Guard = int.Parse(Regex2.Match(description).Groups["GuardNumber"].Value);
                    entry.Type = EntryType.GaurdBeginsShift;
                }

                if (description.Contains("wakes"))
                {
                    entry.Type = EntryType.WakeUp;
                }

                if (description.Contains("sleep"))
                {
                    entry.Type = EntryType.Sleep;
                }

                entries.Add(entry);
            }

            entries = entries.OrderBy(e => e.DateTime).ToList();

            return entries;
        }

        [DebuggerDisplay("{Original}")]
        public class Entry
        {
            public string Original { get; set; }
            public DateTime DateTime { get; set; }
            public EntryType Type { get; set; }
            public int Guard { get; set; }
        }

        public enum EntryType
        {
            GaurdBeginsShift,
            WakeUp,
            Sleep
        }

        public class DaySchedule
        {
            public DateTime Date { get; set; }
            public int Guard { get; set; }
            public Dictionary<int, int> SleepingSchedule { get; set; } = new Dictionary<int, int>(60);
        }
    }
}
