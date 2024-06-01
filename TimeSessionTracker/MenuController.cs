using Spectre.Console;

namespace TimeSessionTracker
{
    internal static class MenuController
    {
        internal static void DisplayMainMenu()
        {
            bool appRunning = true;
            while (appRunning)
            {
                Console.Clear();
                var menuSelection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Navigate with up/down arrows and press [blue]<enter>[/] to confirm selection")
                    .AddChoices(new[]
                    {
                        "Add new time session", "Update existing record", "Delete a record", "View existing records", "Close application"
                    }));

                switch (menuSelection)
                {
                    case "Add new time session":
                        DatabaseFunctions.AddNewSession();
                        break;

                    case "Update existing record":
                        DatabaseFunctions.UpdateSession();
                        break;

                    case "Delete a record":
                        DatabaseFunctions.DeleteSession();
                        break;

                    case "View existing records":
                        DatabaseFunctions.ViewSessions();
                        break;

                    case "Close application":
                        appRunning = false;
                        break;
                }
            }
        }
    }
}
