using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Services
{

    public class LockService : ILockService
    {
        IQualityContext db;
        IPeopleContext pdb;
        IUserHelper userHelper;

        public LockService(IQualityContext context, IPeopleContext peopleContext, IUserHelper userHelper)
        {
            this.db = context;
            this.pdb = peopleContext;
            this.userHelper = userHelper;
        }

        public void CreateCheckLock(int CheckId)
        {
            LockCheck lockCheck = new LockCheck();

            lockCheck.CheckId = CheckId;
            lockCheck.CheckLockedBy = userHelper.CurrentUser();

            db.LockCheck.Add(lockCheck);

            db.SaveChanges();
        }

        public bool IsCheckLocked(int CheckId)
        {
            DateTime timeMinusTwo = DateTime.Now.AddHours(-2);
            string user = userHelper.CurrentUser();

            List<LockCheck> lockChecks = db.LockCheck.Where(x => x.Timestamp > timeMinusTwo && x.CheckId == CheckId && x.CheckLockedBy != user).ToList();

            if (lockChecks.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void DeleteLock(int checkId)
        {
            string lockedBy = userHelper.CurrentUser();

            db.LockCheck.Remove(db.LockCheck.Where(x => x.CheckId == checkId && x.CheckLockedBy == lockedBy).FirstOrDefault());
        }
    }
}