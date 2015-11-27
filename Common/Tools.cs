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
        /// Print help message
        /// </summary>
        public static void PrintHelp(ZendeskApi api)
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
