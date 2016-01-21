using System.Collections.Generic;
using System.Linq;
using ZendeskApi_v2;
using Common;
using ZendeskApi_v2.Models.Tickets;

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
                Common.Tools.PrintGroupHelp(_api);
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
                string[] solvedStatus = { "closed", "solved" };

                // Get the agents tickets
                if (agent.Id != null)
                {
                    var ticketPage = _api.Tickets.GetTicketsByUserID((long) agent.Id);
                    var agentTickets = ticketPage.Tickets.Where(t => t.GroupId == groupId).ToList();

                    // If more than 100 exist, import the rest as well
                    while (!string.IsNullOrEmpty(ticketPage.NextPage))
                    {
                        ticketPage = _api.Tickets.GetByPageUrl<GroupTicketResponse>(ticketPage.NextPage);
                        agentTickets.AddRange(ticketPage.Tickets.Where(t => t.GroupId == groupId).ToList());
                    }

                    // Create a channel for this agent with the number of open tickets.
                    var openTickets = agentTickets.Where(t => !solvedStatus.Contains(t.Status.ToLower())).ToList();
                    var problems = openTickets.Count(t => t.Type.Equals("problem"));
                    var incidents = openTickets.Count(t => t.Type.Equals("incident"));
                    var tasks = openTickets.Count(t => t.Type.Equals("task"));

                    _xml.AddChannel(agent.Name + "(Problem)", problems.ToString());
                    _xml.AddChannel(agent.Name + "(Incident)", incidents.ToString());
                    _xml.AddChannel(agent.Name + "(Tasks)", tasks.ToString());
                    _xml.AddChannel(agent.Name + "(Total open)", openTickets.Count.ToString());
                }
            }

            // Output the finished XML.
            _xml.PrintXml();
        }
    }
}
