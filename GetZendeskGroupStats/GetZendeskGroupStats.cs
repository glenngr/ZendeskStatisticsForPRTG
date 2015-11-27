using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Tickets;

namespace GetZendeskGroupStats
{
    public class GetZendeskGroupStats
    {
        private static ZendeskApi _api;
        private static PrtgXmlOutput _xml;
        private static readonly string OPEN_TICKETS_IN_GROUP_TEXT = "Number of open tickets in group";
        private static readonly string NUM_UNASSIGNED_TICKETS_IN_GROUP = "Number of unassigned tickets in group";
        private static readonly string NUM_PENDING_TICKETS_IN_GROUP = "Number of pending tickets in group";
        private static readonly string NUM_ONHOLD_TICKETS_IN_GROUP = "Number of on-hold tickets in group";

        /// <summary>
        /// All the OPEN tickets in the group supplied as parameter
        /// </summary>
        private static List<ZendeskApi_v2.Models.Tickets.Ticket> _allOpenTicketsInGroup;

        /// <summary>
        /// All tickets in the group
        /// </summary>
        private static List<ZendeskApi_v2.Models.Tickets.Ticket> _allTicketsInGroup;

        /// <summary>
        /// The start of today in DateTimeOffset
        /// </summary>
        private static DateTimeOffset _dayStartDateTimeOffset;

        static void Main(string[] args)
        {
            _api = Common.ZendeskApiManager.ZendeskApi;
            _xml = new PrtgXmlOutput();

            var today = DateTime.Today;
            _dayStartDateTimeOffset = (DateTimeOffset)today;

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
                GetResult(args[0].ToString());
            }
        }

        /// <summary>
        /// Get number of open tickets
        /// </summary>
        private static void GetNumOpenTicketsInGroup()
        {
            var numTickets = _allOpenTicketsInGroup.Count;
            _xml.AddChannel(OPEN_TICKETS_IN_GROUP_TEXT, numTickets.ToString());
        }

        /// <summary>
        /// Get number of unassigned tickets
        /// </summary>
        private static void GetNumUnassignedTicketsInGroup()
        {
            var numUnassigned = _allOpenTicketsInGroup.Count(t => t.AssigneeId == null);
            _xml.AddChannel(NUM_UNASSIGNED_TICKETS_IN_GROUP, numUnassigned.ToString());

        }

        /// <summary>
        /// Get number of tickets with status pending
        /// </summary>
        private static void GetNumPendingTicketsInGroup()
        {
            var numPending = _allOpenTicketsInGroup.Count(t => t.Status.ToLower().Equals("pending"));
            _xml.AddChannel(NUM_PENDING_TICKETS_IN_GROUP, numPending.ToString());

        }

        /// <summary>
        /// Get number of tickets on hold
        /// </summary>
        private static void GetNumOnHOldTicketsInGroup()
        {
            var numPending = _allOpenTicketsInGroup.Count(t => t.Status.ToLower().Equals("hold"));
            _xml.AddChannel(NUM_ONHOLD_TICKETS_IN_GROUP, numPending.ToString());

        }

        /// <summary>
        /// Get result data and print to console out.
        /// </summary>
        /// <param name="groupId"></param>
        private static void GetResult(long groupId)
        {
            string[] unwantedStatuses = { "closed", "solved" };
            string[] ticketTypes = {"problem", "incident"};

            // Import first 100 tickets.
            var ticketPage = _api.Tickets.GetAllTickets();
            _allTicketsInGroup = new List<Ticket>();
            _allTicketsInGroup.AddRange(ticketPage.Tickets.Where(t => t.GroupId == groupId).ToList());

            // If more than 100 exist, import the rest as well
            while (!string.IsNullOrEmpty(ticketPage.NextPage))
            {
                ticketPage = _api.Tickets.GetByPageUrl<GroupTicketResponse>(ticketPage.NextPage);
                _allTicketsInGroup.AddRange(ticketPage.Tickets.Where(t => t.GroupId == groupId).ToList());
            }

            _allOpenTicketsInGroup = _allTicketsInGroup.Where(t => !unwantedStatuses.Contains(t.Status.ToLower()) && ticketTypes.Contains(t.Type.ToLower())).ToList();

            // Get number of open tickets
            GetNumOpenTicketsInGroup();

            // Get number of unassigned tickets
            GetNumUnassignedTicketsInGroup();

            // Get number of pending tickets
            GetNumPendingTicketsInGroup();

            // Get number of "on-hold" tickets
            GetNumOnHOldTicketsInGroup();

            // Get number of new tickets today
            GetNewTicketsToday();

            // Get number of solved tickets today
            GetNumSolvedToday();

            _xml.PrintXml();
        }

        /// <summary>
        /// Get number of tickets in group
        /// </summary>
        /// <param name="groupName"></param>
        private static void GetResult(string groupName)
        {
            var group = _api.Groups.GetGroups().Groups.FirstOrDefault(g => g.Name.ToLower().Equals(groupName.ToLower()));
            if (group == null)
            {
                Common.Tools.PrintError();
            }

            GetResult((long)group.Id);
        }

        /// <summary>
        /// Get number of solved tickets today
        /// </summary>
        private static void GetNumSolvedToday()
        {
            var todaysTickets = _allTicketsInGroup.Where(t => t.UpdatedAt >= _dayStartDateTimeOffset && t.UpdatedAt <= DateTimeOffset.Now).ToList();

            var numSolvedToday = todaysTickets.Count(t => t.Status.ToLower().Equals("solved"));
            _xml.AddChannel("Tickets solved today", numSolvedToday.ToString());
        }

        /// <summary>
        /// Get number of new tickets today
        /// </summary>
        private static void GetNewTicketsToday()
        {
            
            var numNewTicketsToday = _allTicketsInGroup.Count(t => t.CreatedAt >= _dayStartDateTimeOffset && t.CreatedAt <= DateTimeOffset.Now);
            _xml.AddChannel("New tickets today", numNewTicketsToday.ToString());
        }
    }
}