using Spectre.Console;
using System.Globalization;

namespace TimeSessionTracker
{
    internal static class HelperFunctions
    {
        internal static string GetDateInput(string message)
        {
            AnsiConsole.Markup(message);
            string? dateInput = Console.ReadLine();
            while (!DateTime.TryParseExact(dateInput, "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                AnsiConsole.Markup($"[red]Invalid input[/] - {message}");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        internal static object GetIdInput(string message)
        {
            AnsiConsole.Markup(message);
            string? idInput = Console.ReadLine();
            while (!Int32.TryParse(idInput, out _) || Convert.ToInt32(idInput) < 0)
            {
                AnsiConsole.Markup($"[red]Invalid input[/] - {message}");
                idInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(idInput);
            return finalInput;
        }

        internal static string GetSessionTimes(string message)
        {
            AnsiConsole.Markup(message);
            string? timeInput = Console.ReadLine();
            while (!DateTime.TryParseExact(timeInput, "hh:mm tt".Trim().ToLower(), new CultureInfo("en-US"), DateTimeStyles .None, out _))
            {
                AnsiConsole.Markup($"[red]Invalid input[/] - {message}");
                timeInput = Console.ReadLine();
            }

            return timeInput;
        }

        internal static TimeSpan CalculateSessionDuration(string sessionStartTime, string sessionEndTime)
        {
            return DateTime.Parse(sessionEndTime).Subtract(DateTime.Parse(sessionStartTime));
        }

        internal static void PressEnter()
        {
            AnsiConsole.Markup("\n\nPress [blue]<enter>[/] to return to main menu");

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
            } while (key.Key != ConsoleKey.Enter);
        }
    }
}
