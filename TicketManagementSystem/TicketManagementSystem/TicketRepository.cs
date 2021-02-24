using System.Collections.Generic;
using System.Linq;

namespace TicketManagementSystem
{
    public static class TicketRepository
    {
        private static readonly List<Ticket> Tickets = new List<Ticket>();

        public static int CreateTicket(Ticket ticket)
        {
            // Assume that the implementation of this method does not need to change.
            var id = Tickets.Max(i => i.Id) + 1;
            ticket.Id = id;

            Tickets.Add(ticket);

            return id;
        }

        public static Ticket GetTicket(int id)
        {
            // Assume that the implementation of this method does not need to change.
            return Tickets.FirstOrDefault(a => a.Id == id);
        }
    }
}
