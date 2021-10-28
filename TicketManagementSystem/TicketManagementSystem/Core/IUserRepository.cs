namespace TicketManagementSystem.Core
{
    public interface IUserRepository
    {
        public User GetUser(string username);
        public User GetAccountManager();
        public void Dispose();
    }
}
