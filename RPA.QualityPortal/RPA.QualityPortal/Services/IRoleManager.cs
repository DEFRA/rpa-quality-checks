namespace RPA.QualityPortal.Services
{
    public interface IRoleManager
    {
        bool IsUserInRole(string roleName);
    }
}