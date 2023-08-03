using System.Reflection;
using System.Text.RegularExpressions;

namespace IwbrDaemon
{
    public class CLI
    {
        private static System.Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(thisType);

        private static string[] exitCommand = new string[] { "quit", "exit" };
        private static string[] parameterOn = new string[] { "on", "enable", "active" };
        private static string[] parameterOff = new string[] { "off", "disable", "inactive" };

        private static List<string> cInput;

        static CLI()
        {
            if (Config.CLI.IsEnabled)
            {
                log.Info("Loading CLI...");
            }
        }

        public static void Prompt() {
            if (!Config.CLI.IsEnabled)
                return;
            
            //------------------------

            Console.Write("Iwbr > ");

            cInput = Regex.Replace(Console.ReadLine()
                                  ,"'\\s+"," ")
                    .ToLower()
                    .Split(" ")
                    .Select(p => p.Trim())
                    .ToList()
                    ;

            try
            {
                if (cInput.Count() == 0 ||
                     (cInput.Count() == 1 && cInput[0] == String.Empty)
                    )
                    return;

                if (exitCommand.Contains(cInput[0]))
                    System.Environment.Exit(0);
                else if (cInput.ElementAt(0) == "scheduler")
                {
                    if (parameterOn.Contains(cInput[1]))
                        Context.SchedulerEnable = true;
                    else if (parameterOff.Contains(cInput[1]))
                        Context.SchedulerEnable = false;
                    else
                        CommandNotFound();
                }
                else if (cInput[0] == "print")
                {
                    if (cInput[0] == "assets")
                        Context.PrintAssets();
                    else
                        CommandNotFound();
                }
                else
                {
                    CommandNotFound();
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                CommandNotFound();
            }            
        }
        private static void CommandNotFound()
        {
            Console.WriteLine("Command not found");
        }
    }
}
