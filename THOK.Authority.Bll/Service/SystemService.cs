﻿using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class SystemService : ServiceBase<AUTH_SYSTEM>, ISystemService
    {
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }
        [Dependency]
        public IModuleRepository ModuleRepository { get; set; }
        [Dependency]
        public IRoleSystemRepository RoleSystemRepository { get; set; }
        [Dependency]
        public IUserSystemRepository UserSystemRepository { get; set; }
        [Dependency]
        public IUserModuleRepository UserModuleRepository { get; set; }
        [Dependency]
        public IUserFunctionRepository UserFunctionRepository { get; set; }
        [Dependency]
        public ILoginLogRepository LoginLogRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string system_Name, string description, string status)
        {
            IQueryable<AUTH_SYSTEM> query = SystemRepository.GetQueryable();
            var systems = query.Where(i => i.SYSTEM_NAME.Contains(system_Name) && i.DESCRIPTION.Contains(description))
                .OrderBy(i => i.SYSTEM_ID)
                .Select(i => new { i.SYSTEM_ID, i.SYSTEM_NAME, i.DESCRIPTION, STATUS = i.STATUS == "1" ? "启用" : "禁用" });
            if (status != "")
            {
               // bool bStatus = Convert.ToBoolean(status);
                string bStatus = status;
                systems = query.Where(i => i.SYSTEM_NAME.Contains(system_Name) && i.DESCRIPTION.Contains(description) && i.STATUS == bStatus)
                    .OrderBy(i => i.SYSTEM_ID)
                    .Select(i => new { i.SYSTEM_ID, i.SYSTEM_NAME, i.DESCRIPTION, STATUS = i.STATUS == "1" ? "启用" : "禁用" });
            }
            int total = systems.Count();
            systems = systems.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = systems.ToArray() };
        }

        public bool Add(string systemName, string description, string status)
        {
            var system = new AUTH_SYSTEM()
            {
               // SYSTEM_ID = Guid.NewGuid().ToString(),
               SYSTEM_ID=SystemRepository.GetNewID("AUTH_SYSTEM","SYSTEM_ID"),
                SYSTEM_NAME = systemName,
                DESCRIPTION = description,
                STATUS = status
            };
            SystemRepository.Add(system);
            SystemRepository.SaveChanges();
            return true;
        }

        public bool Delete(string systemId)
        {
            //Guid gsystemId = new Guid(systemId);
            var system = SystemRepository.GetQueryable()
                .FirstOrDefault(i => i.SYSTEM_ID == systemId);
            if (system != null)
            {
                Del(ModuleRepository, system.AUTH_MODULE);
                Del(RoleSystemRepository, system.AUTH_ROLE_SYSTEM);
                Del(UserSystemRepository, system.AUTH_USER_SYSTEM);
                Del(LoginLogRepository, system.AUTH_LOGIN_LOG);
                SystemRepository.Delete(system);
                SystemRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string systemId, string systemName, string description, string status)
        {
            //Guid sid = new Guid(systemId);
            var system = SystemRepository.GetQueryable()
                .FirstOrDefault(i => i.SYSTEM_ID == systemId);
            system.SYSTEM_NAME = systemName;
            system.DESCRIPTION = description;
            system.STATUS = status;
            SystemRepository.SaveChanges();
            return true;
        }

        public object GetSystemById(string systemID)
        {
            //Guid sid = new Guid(systemID);
            var sysytem = SystemRepository.GetQueryable().FirstOrDefault(s => s.SYSTEM_ID.Trim() == systemID);
            return sysytem.SYSTEM_NAME;
        }

        public object GetDetails(string userName, string systemID, string cityID)
        {
            //Guid cityid = new Guid(cityID);
            //Guid systemid = new Guid(systemID);
            //var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME == userName);
            //var userSystemId = UserSystemRepository.GetQueryable().Where(us => us.USER_USER_ID == user.USER_ID
            //    && us.AUTH_SYSTEM.SYSTEM_ID == systemID && us.CITY_CITY_ID == cityID).Select(us => us.USER_SYSTEM_ID);
            ////string userSystemId = userSystemIds.Single().Trim();
            //var userSystems = UserSystemRepository.GetQueryable().Where(us => !userSystemId.Any(uid => uid == us.USER_SYSTEM_ID)
            //    && us.USER_USER_ID == user.USER_ID && us.AUTH_CITY.CITY_ID == cityID);
            //var userSystem = userSystems.Where(u => userSystems.Any(us => us.AUTH_USER_MODULE.Any(um => um.AUTH_USER_FUNCTION.Any(uf => 
            //    uf.USER_MODULE_USER_MODULE_ID== um.USER_MODULE_ID && uf.IS_ACTIVE == "1") || um.IS_ACTIVE == "1") || us.IS_ACTIVE == "1"))
            //    .Select(us => new {us.AUTH_SYSTEM.SYSTEM_ID, us.AUTH_SYSTEM.SYSTEM_NAME, us.AUTH_SYSTEM.DESCRIPTION, Status =us.AUTH_CITY.IS_ACTIVE=="0" ? "启用" : "禁用" });
            //return userSystem.ToArray();

            //       var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME == userName);
            //       var userSystem = UserSystemRepository.GetQueryable();
            //       var system = SystemRepository.GetQueryable();
            //var qq=from a in system 
            //       from b in userSystem 
            //       where (a.SYSTEM_ID==b.SYSTEM_SYSTEM_ID)
            //       select new{a.SYSTEM_ID, a.SYSTEM_NAME,a.DESCRIPTION,STATUS=a.STATUS=="1"?"启用":"未启用",b.CITY_CITY_ID,b.SYSTEM_SYSTEM_ID,b.USER_USER_ID,b.IS_ACTIVE};
            //var userUseSysem = qq.Where(p => p.CITY_CITY_ID == cityID && !p.SYSTEM_SYSTEM_ID.Contains(systemID)  && p.IS_ACTIVE=="1"  && p.USER_USER_ID == user.USER_ID);
            //return userUseSysem.Distinct().ToArray();
            var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME == userName);
            var userSystems = UserSystemRepository.GetQueryable().Where(usR => usR.SYSTEM_SYSTEM_ID != systemID && usR.USER_USER_ID == user.USER_ID && usR.CITY_CITY_ID == cityID);
            var userSystem = userSystems.Where(u => u.AUTH_USER_MODULE.Any(um => um.IS_ACTIVE == "1") || u.IS_ACTIVE == "1")
                .Select(us => new { us.AUTH_SYSTEM.SYSTEM_ID, us.AUTH_SYSTEM.SYSTEM_NAME, us.AUTH_SYSTEM.DESCRIPTION, STATUS = us.AUTH_SYSTEM.STATUS == "1" ? "启用" : "未启用" });
            return userSystem.ToArray();
        }

    }
}
