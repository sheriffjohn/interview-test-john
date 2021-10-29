using System;
using EmailService;
using TicketManagementSystem.Core;
using TicketManagementSystem.Infrastructure;

namespace TicketManagementSystem
{
    public class TicketService
    {
        private IEmailService emailService;
        private IUserRepository userRepository;

        // Potential dependency injection goes here, but for now we instansiate in this consructor
        public TicketService()
        {
            this.emailService = new EmailServiceProxy();
            this.userRepository = new UserRepository();
        }


        // Main flow of the application service
        // However the main flow should be in the program class, this service should only have operating methods, but due to restrictions we cannot change the program class
        public int CreateTicket(string title, Priority priority, string assignedTo, string description, DateTime dateTime, bool isPayingCustomer)
        {
            // 1. Validate certain args and throw a known exception if they are invalid (do this first to prevent unnecessary roundtrips to database)
            ValidateTicket(title, description);

            // 2. Fetch data and map to class
            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = getUser(assignedTo),
                Priority = priority,
                Description = description,
                Created = dateTime,
                PriceDollars = getPrice(isPayingCustomer, priority),
                AccountManager = getAccountManager(isPayingCustomer)
            };

            // 3. Handle priority
            HandlePriority(ticket);

            // 4. Send email if priority is high
            HandleEmail(ticket.Priority, ticket.Title, ticket.AssignedUser.FirstName);

            // 5. Create ticket and return the ticketId
            return TicketRepository.CreateTicket(ticket);
        }


        private void ValidateTicket(string title, string description)
        {
            // Check if title or description are null or empty and throw exception
            if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(description))
            {
                throw new InvalidTicketException("Title or description were null");

            }
        }

        private User getUser(string assignedTo)
        {
            var user = userRepository.GetUser(assignedTo);
            return user is null ? throw new UnknownUserException("User " + assignedTo + " not found") : user;
        }

        private User getAccountManager(bool isPayingCustomer)
        {
            return isPayingCustomer ? userRepository.GetAccountManager() : null;
        }

        private double getPrice(bool isPayingCustomer, Priority priority)
        {
            return isPayingCustomer ? priority == Priority.High ? 100 : 50 : 0;
        }

        // We want to change ticket priority
        // It looks we want to change priority if either certain time has passed or ticket title contains key words. 
        // Since the both if statements can't happen at the same time we might shorten the code.
        private Priority HandlePriority(Ticket ticket)
        {
            var isTimeToServe = ticket.Created < DateTime.UtcNow - TimeSpan.FromHours(1);
            var isTitleSevere = (ticket.Title.Contains("Crash") || ticket.Title.Contains("Important") || ticket.Title.Contains("Failure"));    

            // If statement is true and priority is already high there is no need to check and if all conditions are false the ticket priority will not be changed
            if (ticket.Priority != Priority.High && (isTimeToServe || isTitleSevere))
            {
                if (ticket.Priority == Priority.Low)
                {
                    ticket.Priority = Priority.Medium;
                }
                else if (ticket.Priority == Priority.Medium)
                {
                    ticket.Priority = Priority.High;
                }
            }

            return ticket.Priority;
        }

        private void HandleEmail(Priority priority, String title, String assignedTo)
        {
            if (priority == Priority.High)
            {
                emailService.SendEmailToAdministrator(title, assignedTo);
            }
        }


        // The first part of this code where we fetching ticket by ticketId I think i unnecessary since we already created the ticket, and only need to check if we got the return id.
        // If the main flow of the program was moved upp to program class we could reuse the ticket without having to make another roundtrip to the database.
        // So the only call to the database would be to update.

        public void AssignTicket(int ticketId, string username)
        {
            var user = getUser(username);

            var ticket = TicketRepository.GetTicket(ticketId);

            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + ticketId);
            }

            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        // Not used, want to remove it because of YAGNI principle
        /*      private void WriteTicketToFile(Ticket ticket)
            {
                var ticketJson = JsonSerializer.Serialize(ticket);
                File.WriteAllText(Path.Combine(Path.GetTempPath(), $"ticket_{ticket.Id}.json"), ticketJson);
            }*/
    }


}
