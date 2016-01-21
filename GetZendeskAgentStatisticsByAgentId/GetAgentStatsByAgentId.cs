using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Tickets;

namespace GetZendeskAgentStatisticsByAgentId
{
    class GetAgentStatsByAgentId
    {

        private static ZendeskApi _api;
        private static PrtgXmlOutput _xml;
        private static List<long> agentIdList;


        static void Main(string[] args)
        {
            _api = Common.ZendeskApiManager.ZendeskApi;
            _xml = new PrtgXmlOutput();

            if (args == null || args.Length == 0)
            {
                Common.Tools.PringAgentHelp(_api);
                return;
            }

            agentIdList = new List<long>();
            foreach (var s in args)
            {
                long agentId;
                if (long.TryParse(args[0], out agentId))
                {
                    agentIdList.Add(agentId);
                }
                else
                {
                    Tools.PrintError();
                    break;
                }
            }
        }

        private static void GetResult()
        {
            foreach (var agentId in agentIdList)
            {
                var agent = _api.Users.GetUser(agentId).User;

                string[] solvedStatus = { "closed", "solved" };


                // Get the agents tickets
                if (agent.Id != null)
                {
                    var ticketPage = _api.Tickets.GetTicketsByUserID((long)agent.Id);
                    var agentTickets = ticketPage.Tickets.ToList();

                    // If more than 100 exist, import the rest as well
                    while (!string.IsNullOrEmpty(ticketPage.NextPage))
                    {
                        ticketPage = _api.Tickets.GetByPageUrl<GroupTicketResponse>(ticketPage.NextPage);
                        agentTickets.AddRange(ticketPage.Tickets.ToList());
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
