using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZendeskConnector
{
    internal class Program
    {
        private static ZenDeskApiManager api;

        private static Dictionary<string, string> _commandsDictionary = new Dictionary<string, string>()
        {
            {"-getOpenTickets", ""}
        };

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            api = new ZenDeskApiManager();

            foreach (var grp in api.customApi.Groups.GetAssignableGroups().Groups)
            {
                Console.WriteLine(grp.Id + ": " + grp.Name);
            }
        }

        /// <summary>
        /// Prints the help.
        /// </summary>
        private static void PrintHelp()
        {
            StringBuilder help = new StringBuilder();
            help.AppendLine("Please specify one of the following parameters:");
        }

        private static void GetOpenTickets(long groupId)
        {
            //api.customApi.Tickets.GetAllTickets().Tickets.Where(t => t.GroupId = groupId);
        }
    }
}