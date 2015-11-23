using System;
using ZendeskApi_v2;

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
            Console.WriteLine("List of groups:");
            PrintGroups(api);
        }

        /// <summary>
        /// Print list of groups
        /// </summary>
        public static void PrintGroups(ZendeskApi api)
        {
            foreach (var group in api.Groups.GetGroups().Groups)
            {
                Console.WriteLine(group.Id + ": " + group.Name);
            }
        }
    }
}
