using System;
using System.Collections.Generic;
using System.Linq;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.AccountsAndActivities;
using ZendeskApi_v2.Models.Tickets;

namespace Common
{
    public class Tools
    {
        /// <summary>
        /// Print error message
        /// </summary>
        public static void PrintError()
        {
            Console.WriteLine("Invalid argument");
        }

        /// <summary>
        /// Print help for assemblies requiring agent ids as parameter
        /// </summary>
        /// <param name="api"></param>
        public static void PringAgentHelp(ZendeskApi api)
        {
            Console.WriteLine("You have to specify at least one agent id as parameter separated by spaces");
            PrintAgentIds(api);
        }

        /// <summary>
        /// Prints agent ids
        /// </summary>
        /// <param name="api"></param>
        private static void PrintAgentIds(ZendeskApi api)
        {
            Console.WriteLine("List of agents");
            var users = api.Users.GetAllUsers();

            var agents = users.Users.Where(u => u.Role.ToLower().Equals("agent"));

            foreach (var agent in agents)
            {
                Console.WriteLine(agent.Id + ": " + agent.Name);
            }
        }

        /// <summary>
        /// Print help message
        /// </summary>
        public static void PrintGroupHelp(ZendeskApi api)
        {
            Console.WriteLine("You have to specify a group name or id as parameter");
            PrintGroups(api);
        }

        /// <summary>
        /// Print list of groups
        /// </summary>
        public static void PrintGroups(ZendeskApi api)
        {
            Console.WriteLine("List of groups:");
            foreach (var group in api.Groups.GetGroups().Groups)
            {
                Console.WriteLine(group.Id + ": " + group.Name);
            }
        }
    }
}
