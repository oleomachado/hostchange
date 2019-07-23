using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace HostChange
{
    class Program
    {
        public const string CfgStart = "#CFG_START>";
        public const string CfgEnd = "#END";

        static void Main(string[] args)
        {
            Console.WriteLine("HostChange v0.1\n");

            if (!IsAdministrator())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HostChange requires ADMIN privileges to be able to change hosts.");
                Console.ForegroundColor = ConsoleColor.White;
                Environment.Exit(-1);
            }

            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage:   C:\HostChange [<operation>] <sectionName>");
                Console.WriteLine("   -list = list all available section names ");
                Console.WriteLine("   -fulllist = list all available sections values ");
                Environment.Exit(-2);
            }

            var opList = args[0].ToLower().Equals("-list");
            var opFullList = args[0].ToLower().Equals("-fulllist");

            var hostsContent = File.ReadAllLines(Path.Combine(Environment.SystemDirectory, @"Drivers\Etc\Hosts."));

            if (opList || opFullList)
            {
                ListSections(hostsContent, opFullList);
                Environment.Exit(0);
            }

            var sectionName = args[0];

            var outputLines = new StringBuilder();
            var uncomment = false;

            for (int line = 0; line < hostsContent.Length; line++)
            {
                if (hostsContent[line].ToUpper().StartsWith(CfgStart))
                {
                    var (sName, _) = GetSectionName(hostsContent[line]);

                    if (sName.ToLower().Equals(sectionName.ToLower()))
                    {
                        uncomment = true;
                        outputLines.AppendLine(hostsContent[line]);
                        continue;
                    }
                }

                if (uncomment)
                {
                    if (hostsContent[line].ToUpper().StartsWith(CfgEnd))
                    {
                        uncomment = false;
                        outputLines.AppendLine(hostsContent[line]);
                        continue;
                    }

                    while (hostsContent[line].StartsWith("#"))
                    {
                        hostsContent[line] = hostsContent[line].Substring(1);
                    }
                    outputLines.AppendLine(hostsContent[line]);
                }
                else
                {
                    if (!hostsContent[line].StartsWith("#"))
                    {
                        outputLines.Append("#");
                    }

                    outputLines.AppendLine(hostsContent[line]);
                }
            }

            File.WriteAllText(Path.Combine(Environment.SystemDirectory, @"Drivers\Etc\Hosts."), outputLines.ToString());

            FlushDns();
        }

        private static void ListSections(string[] hostEntries, bool showFullList)
        {
            var found = false;

            var previousConsoleColor = Console.ForegroundColor;

            for (int line = 0; line < hostEntries.Length - 1; line++)
            {
                if (hostEntries[line].StartsWith(CfgStart))
                {
                    var (sName, sDesc) = GetSectionName(hostEntries[line]);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"#{sName} -> ");
                    Console.ForegroundColor = previousConsoleColor;
                    Console.WriteLine(sDesc);

                    found = true;
                }
                else
                {
                    if (showFullList)
                    {
                        if (!hostEntries[line].StartsWith("#"))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(hostEntries[line]);
                            Console.ForegroundColor = previousConsoleColor;
                        }
                        else
                        {
                            Console.WriteLine(hostEntries[line]);
                        }
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("No sections found.");
            }
        }


        private static (string, string) GetSectionName(string text)
        {
            string sectionName = "";
            string descriptionName = "";

            var content = text.Replace(CfgStart, "");

            if (content.IndexOf("[", StringComparison.Ordinal) > 0)
            {
                if (content.IndexOf("]", StringComparison.Ordinal) > 0)
                {
                    sectionName = content.Substring(0, content.IndexOf("[", StringComparison.Ordinal));
                    descriptionName = content.Substring(content.IndexOf("[", StringComparison.Ordinal) + 1);
                    descriptionName = descriptionName.Remove(descriptionName.Length - 1);
                }
                else
                {
                    sectionName = content;
                }
            }

            return (sectionName.Trim(), descriptionName.Trim());
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void FlushDns()
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, @"Ipconfig.exe");
                myProcess.StartInfo.Arguments = "/flushdns";
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
