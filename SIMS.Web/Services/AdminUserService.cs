using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Services;
using SIMS.Web.Models;
using SIMS.Common;
using System.Data.SqlClient;
using SIMS.Web.Areas.Admins.ViewModels;

namespace SIMS.Web.Services
{
    //工号也是唯一的(当成账号即可)
    public class AdminUserService
    {
        public static BaseRepository<AdminUser> oBll = new BaseRepository<AdminUser>();
        private static AdminUserService _instance = null;
        public static AdminUserService Instance
        {
            get
            {
                return _instance ?? new AdminUserService(); ;
            }
        }
        private AdminUserService()
        {

        }

        #region Public

        #region Query
        public AdminUser GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.AdminUser.Find(id);
                return query;
            }
        }

        public AdminUser GetByCode(string code)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.AdminUser.FirstOrDefault(c => c.Code.Equals(code));
                return query;
            }
        }

        public List<AdminUserViewModel> GetBySomeWhere(string code, int iStart, int iLimit)
        {
            List<AdminUserViewModel> list = new List<AdminUserViewModel>();
            var filtered = GetSearchResult(code);

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<AdminUser> users = query.ToList<AdminUser>();

            foreach (AdminUser user in users)
            {
                string departmentname = string.Empty;
                string majorname = string.Empty;
                Department department = DepartmentService.Instance.GetById(user.DepartmentId);
                if (department != null)
                    departmentname = department.Name;

                Major major = MajorService.Instance.GetById(user.MajorId);
                if (major != null)
                    majorname = major.Name;

                list.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Code = user.Code,
                    Name = user.Name,
                    Sex = user.Sex,
                    DepartmentId = user.DepartmentId,
                    MajorId = user.MajorId,
                    Passwordsalt = user.Passwordsalt,
                    Profession = user.Profession,
                    CreateTime = user.CreateTime,
                    ModifyTime = user.ModifyTime,
                    DepartmentName = departmentname,
                    MajorName = majorname
                });
            }
            return list;
        }

        public int GetCount(string code)
        {
            using (var context = new SIMSDbContext())
            {
                return context.AdminUser
                    .Where(m => (String.IsNullOrEmpty(code) || m.Code.Equals(code)))
                    .Count();
            }
        }

        //public bool IsExist(string code)
        //{
        //    using (var context = new SIMSDbContext())
        //    {
        //        int count = context.AdminUser
        //            .Where(m => (m.Code.Equals(code)))
        //            .Count();

        //        return count > 0 ? true : false;
        //    }
        //}

        public bool IsExist(int id, string code)
        {
            bool isExist = false;
            using (var context = new SIMSDbContext())
            {
                if (id == 0)
                {
                    var query = context.AdminUser.FirstOrDefault(c => c.Code == code);
                    isExist = null == query ? false : true;
                }
                else
                {
                    var query = context.AdminUser.FirstOrDefault(c => c.Code == code && c.Id != id);
                    isExist = null == query ? false : true;
                }
                return isExist;
            }
        }


        public string GetCode(string grade, int department, int major, int profession)
        {
            using (var context = new SIMSDbContext())
            {
                string code = string.Empty;

                switch (profession)
                {
                    case 0:
                        //年级+学院(系)+专业+MaxId
                        //一个专业设置<=1000
                        Department originalDepartment = DepartmentService.Instance.GetById(department);
                        Major originalMajor = MajorService.Instance.GetById(major);

                        int maxMajorId = GetMaxId();
                        if (maxMajorId < 10)
                        {
                            if (maxMajorId == 0)
                                code = String.Format("{0}{1}{2}000{3}", grade, originalDepartment.Code, originalMajor.Code, maxMajorId + 1);
                            else
                                code = String.Format("{0}{1}{2}000{3}", grade, originalDepartment.Code, originalMajor.Code, maxMajorId);

                        } if (10 <= maxMajorId && maxMajorId < 100)
                        {
                            code = String.Format("{0}{1}{2}00{3}", grade, originalDepartment.Code, originalMajor.Code, maxMajorId);
                        }
                        else if (100 <= maxMajorId && maxMajorId < 1000)
                        {
                            code = String.Format("{0}{1}{2}0{3}", grade, originalDepartment.Code, originalMajor.Code, maxMajorId);
                        }
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }


                return code;
            }
        }

        /// <summary>
        /// 自增ID
        /// </summary>
        /// <returns></returns>
        public int GetMaxId()
        {
            using (var context = new SIMSDbContext())
            {
                int count = context.Department.Count();
                if (count > 0)
                {
                    var q = (from c in context.Department select c.Id).Max();
                    return (from c in context.Department select c.Id).Max();
                }
                return 0;
            }
        }

        #endregion

        #region Create

        public bool AddSystemAccount()
        {
            bool bRet = true;
            try
            {
                AdminUser user = new AdminUser
                {
                    Code = "001",
                    Name = "admin",
                    Sex = "M",
                    DepartmentId = 0,
                    MajorId = 0,
                    Passwordsalt = BCrypt.Net.BCrypt.HashPassword("123", BCrypt.Net.BCrypt.GenerateSalt()),
                    Profession = 0,
                    Nation = "汉",
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };

                bool isExist = IsExist(user.Code);
                if (!isExist)
                    bRet = Create(user);
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogType.Error, string.Format("AddSystemAccount异常:{0}", ex.Message));
            }
            return bRet;
        }

        public bool Create(AdminUser oItem)
        {
            var oRet = new AdminUser();

            try
            {
                oRet = oBll.AddEntities(oItem);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var bRet = oRet == null ? false : true;
            return bRet;
        }

        #endregion

        #region Update

        public bool Update(Major oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.Major.Attach(oIem);
                context.Entry<Major>(oIem).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// 注意在调用的时候还要删除对应的物理文件
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool bRet = false;

            try
            {
                using (var db = new SIMSDbContext())
                {
                    AdminUser oItem = GetById(id);
                    db.AdminUser.Attach(oItem);
                    db.Entry<AdminUser>(oItem).State = EntityState.Deleted;
                    bRet = db.SaveChanges() > 0;
                }
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        #endregion

        #endregion

        #region Private

        private List<AdminUser> GetSearchResult(string code)
        {
            using (var context = new SIMSDbContext())
            {
                return context.AdminUser
               .Where(m => (string.IsNullOrEmpty(code) || m.Code.Contains(code)))
               .OrderByDescending(m => m.CreateTime)
               .ToList<AdminUser>();
            }
        }

        private bool IsExist(string code, int id = 0)
        {
            bool isExist = false;
            try
            {
                int iCount = 0;
                using (var context = new SIMSDbContext())
                {
                    /*string sql = @"select * from AdminUser where 1=1 ";
                    if (id == 0)
                    {
                        sql += "and Code=@Code";
                        iCount = context.Database.ExecuteSqlCommand(sql, new SqlParameter("@Code", code));
                    }
                    else
                    {
                        sql += "and Code=@Code and Id <> @Id";
                        iCount = context.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@Code", code),
                            new SqlParameter("@Id", id));
                    }*/
                    if (id == 0)
                    {
                        iCount = context.AdminUser.Where(c => c.Code == code).Count();
                    }
                    else
                    {
                        iCount = context.AdminUser.Where(c=>c.Code==code && c.Id !=id).Count();
                    }
                    
                    isExist = iCount > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("检测用户工号异常:{0}", ex.Message);
                LogHelper.Log(LogType.Error, msg);
                //throw ex;
            }
            return isExist;
        }

        #endregion
    }
}