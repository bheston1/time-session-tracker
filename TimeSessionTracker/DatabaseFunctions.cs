using System.Configuration;
using System.Data.SQLite;
using Dapper;
using Spectre.Console;
using System.Globalization;

namespace TimeSessionTracker
{
    internal static class DatabaseFunctions
    {
        private static readonly string? _connectionString = ConfigurationManager.ConnectionStrings["sqliteconn"].ConnectionString;
        private static bool _bypassViewConfirm = false;

        internal static void CreateLocalDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var createTable = "CREATE TABLE IF NOT EXISTS Sessions (Id INTEGER PRIMARY KEY AUTOINCREMENT, Date TEXT, Start TEXT, End TEXT, Duration TEXT)";
                connection.Execute(createTable);
            }
        }

        internal static void AddNewSession()
        {
            Console.Clear();
            string date = HelperFunctions.GetDateInput("Enter session date [yellow](format - mm/dd/yyyy)[/]: ");
            string sessionStartTime = HelperFunctions.GetSessionTimes("Enter session start time [yellow](format - hh:mm AM/PM)[/]: ");
            string sessionEndTime = HelperFunctions.GetSessionTimes("Enter session end time [yellow](format - hh:mm AM/PM)[/]: ");
            TimeSpan sessionDuration = HelperFunctions.CalculateSessionDuration(sessionStartTime, sessionEndTime);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var addSession = $"INSERT INTO Sessions (Date, Start, End, Duration) VALUES ('{date}', '{sessionStartTime}', '{sessionEndTime}', '{sessionDuration}')";
                connection.Execute(addSession);
            }
        }

        internal static void ViewSessions()
        {
            Console.Clear();
            if (!FoundDatabaseEntries()) return;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                List<TimeSession> tableData = new List<TimeSession>();

                var view = connection.ExecuteReader("SELECT * FROM Sessions ORDER BY Date DESC");

                while (view.Read())
                {
                    tableData.Add(new TimeSession
                    {
                        Id = view.GetInt32(0),
                        Date = DateTime.ParseExact(view.GetString(1), "MM/dd/yyyy", new CultureInfo("en-US")),
                        StartTime = DateTime.ParseExact(view.GetString(2), "hh:mm tt", new CultureInfo("en-US")),
                        EndTime = DateTime.ParseExact(view.GetString(3), "hh:mm tt", new CultureInfo("en-US")),
                        SessionDuration = TimeSpan.Parse(view.GetString(4)),
                    });
                }

                var table = new Table();
                table.Centered();
                table.Border(TableBorder.Square);
                table.Title("[yellow]TIME SESSIONS[/]");
                table.AddColumn("ID");
                table.AddColumn("Date");
                table.AddColumn("Start Time");
                table.AddColumn("End Time");
                table.AddColumn("Elapsed Time");

                foreach (var session in tableData)
                {
                    table.AddRow($"{session.Id}", $"{session.Date.ToString("MM/dd/yyyy")}", $"{session.StartTime.ToString("hh:mm tt")}", $"{session.EndTime.ToString("hh:mm tt")}", $"{session.SessionDuration.ToString(@"hh\:mm")}");
                }

                AnsiConsole.Write(table);
            }

            if (_bypassViewConfirm == false)
            {
                HelperFunctions.PressEnter();
            }
        }

        internal static void DeleteSession()
        {
            if (!FoundDatabaseEntries()) return;
            _bypassViewConfirm = true;
            ViewSessions();

            var recordId = HelperFunctions.GetIdInput("Enter [yellow]ID[/] of session to delete: ");

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var check = connection.ExecuteScalar($"SELECT 1 FROM Sessions WHERE Id = {recordId}");
                int query = Convert.ToInt32(check);
                if (query == 0)
                {
                    AnsiConsole.Markup("[red]Invalid ID[/]");
                    HelperFunctions.PressEnter();
                    _bypassViewConfirm = false;
                    return;
                }
                else
                {
                    var delete = connection.Execute($"DELETE FROM Sessions WHERE Id = '{recordId}'");
                }
            }

            AnsiConsole.Markup($"[red]Record with ID <{recordId}> was deleted[/]");
            HelperFunctions.PressEnter();
            _bypassViewConfirm = false;
        }

        internal static void UpdateSession()
        {
            if (!FoundDatabaseEntries()) return;
            _bypassViewConfirm = true;
            ViewSessions();

            var recordId = HelperFunctions.GetIdInput("Enter [yellow]ID[/] of session to update: ");

            using (var connection = new SQLiteConnection( _connectionString))
            {
                var check = connection.ExecuteScalar($"SELECT 1 FROM Sessions WHERE Id = {recordId}");
                int query = Convert.ToInt32(check);
                if (query == 0)
                {
                    AnsiConsole.Markup("[red]Invalid ID[/]");
                    HelperFunctions.PressEnter();
                    _bypassViewConfirm= false;
                    return;
                }

                string sessionStartTime = HelperFunctions.GetSessionTimes("Enter session start time [yellow](format - hh:mm AM/PM)[/]: ");
                string sessionEndTime = HelperFunctions.GetSessionTimes("Enter session end time [yellow](format - hh:mm AM/PM)[/]: ");
                TimeSpan sessionDuration = HelperFunctions.CalculateSessionDuration(sessionStartTime, sessionEndTime);
                var update = connection.Execute($"UPDATE Sessions SET Start = '{sessionStartTime}', End = '{sessionEndTime}', Duration = '{sessionDuration}' WHERE Id = {recordId}");
            }

            AnsiConsole.Markup($"[yellow]Record with ID <{recordId}> was updated[/]");
            HelperFunctions.PressEnter();
            _bypassViewConfirm = false;
        }

        private static bool FoundDatabaseEntries()
        {
            bool databaseEntriesFound = true;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var check = connection.ExecuteScalar("SELECT EXISTS(SELECT * FROM Sessions)");
                int query = Convert.ToInt32(check);
                if (query == 0)
                {
                    databaseEntriesFound = false;
                    AnsiConsole.Markup("[yellow]Nothing in database[/]");
                    HelperFunctions.PressEnter();
                }

                return databaseEntriesFound;
            }
        }
    }
}
