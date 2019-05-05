using MultiTenant.Web.Models;

namespace MultiTenant.Web.Helpers.Interfaces
{
    public interface IUserHelper
    {
        bool ValidateUser(string username, string password);
        User GetUser(string username);
    }
}