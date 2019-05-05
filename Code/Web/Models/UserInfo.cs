namespace MultiTenant.Web.Models
{
    public class UserInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool HasAdminRights { get; set; }
        public int TenantId { get;set;}
    }
}