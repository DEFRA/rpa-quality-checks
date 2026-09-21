namespace RPA.QualityPortal.Services
{
    public interface ILockService
    {
        void CreateCheckLock(int CheckId);

        bool IsCheckLocked(int CheckId);

        void DeleteLock(int CheckId);
    }
}