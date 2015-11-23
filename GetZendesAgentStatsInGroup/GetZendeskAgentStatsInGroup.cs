using System.Linq;
using ZendeskApi_v2;
using Common;

namespace GetZendeskAgentStatsInGroup
{
    class GetZendeskAgentStatsInGroup
    {
        private static ZendeskApi _api;
        private static PrtgXmlOutput _xml;

        static void Main(string[] args)
        {
            _api = Common.ZendeskApiManager.ZendeskApi;
            _xml = new PrtgXmlOutput();

            if (args == null || args.Length == 0)
            {
                Common.Tools.PrintHelp(_api);
                return;
            }

            long groupId;
            if (long.TryParse(args[0], out groupId))
            {
                GetResult(groupId);
            }
            else
            {
                Tools.PrintError();
            }
        }

        /// <summary>
        /// Get number of tickets assigned to each support agent in specified group
        /// </summary>
        /// <param name="groupId">Group id to generate stats for</param>
        private static void GetResult(long groupId)
        {
            // Get all agents in group
            var agentsInGroup = _api.Users.GetUsersInGroup(groupId).Users;

            // Loop through each agent in group
            foreach (var agent in agentsInGroup)
            {
                // Get the agents tickets
                var agentTickets = _api.Tickets.GetTicketsByUserID((long)agent.Id).Tickets.Where(t => t.GroupId == groupId).ToList();
                string[] solvedStatus = { "closed", "solved" };

                // Create a channel for this agent with the number of open tickets.
                var openTickets = agentTickets.Count(t => !solvedStatus.Contains(t.Status.ToLower()));
                _xml.AddChannel(agent.Name + "(Open)", openTickets.ToString());
            }

            // Output the finished XML.
            _xml.PrintXml();
        }
    }
}
