using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Services;
using SIMS.Web.Models;

namespace SIMS.Web.Services
{
    public class DepartmentService
    {
        public static BaseRepository<Department> oBll = new BaseRepository<Department>();
        private static DepartmentService _instance = null;

        public static DepartmentService Instance
        {
            get
            {
                return _instance ?? new DepartmentService(); ;
            }
        }

        private DepartmentService()
        {

        }

        #region Public

        #region Query

        public bool IsExist(int id, string code)
        {
            bool isExist = false;
            using (var context = new SIMSDbContext())
            {
                if (id == 0)
                {
                    var query = context.Department.FirstOrDefault(c => c.Code == code);
                    isExist = null == query ? false : true;
                }
                else
                {
                    var query = context.Department.FirstOrDefault(c => c.Code == code && c.Id != id);
                    isExist = null == query ? false : true;
                }
                return isExist;
            }
        }

        public Department GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Department.Find(id);
                return query;
            }
        }

        public Department GetByName(string name)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Department.FirstOrDefault(c=>c.Name==name);
                return query;
            }
        }

        public List<Department> GetBySomeWhere(string name, int iStart, int iLimit)
        {
            var filtered = GetSearchResult(name);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;
            return query.ToList<Department>();
        }

        public int GetCount(string name)
        {
            using (var context = new SIMSDbContext())
            {
                return context.Department
                    .Where(m => (String.IsNullOrEmpty(name) || m.Code.Equals(name)))
                    .Count();
            }
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <returns></returns>
        public List<Department> GetAll()
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Department
                            select c;
                return query.ToList<Department>();
            }
        }

        public string GetDepartmentCode()
        {
            using (var context = new SIMSDbContext())
            {
                string code = string.Empty;
                int maxMajorId = GetMaxId();
                if (maxMajorId < 10)
                {
                    if (maxMajorId == 0)
                        code = string.Format("0{0}", maxMajorId + 1);
                    else
                        code = string.Format("0{0}", maxMajorId);
                }
                else
                {
                    code = maxMajorId.ToString();
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

        public bool Create(Department oItem)
        {
            var oRet = new Department();

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

        public bool Update(Department oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.Department.Attach(oIem);
                context.Entry<Department>(oIem).State = EntityState.Modified;
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
                    Department oItem = GetById(id);
                    db.Department.Attach(oItem);
                    db.Entry<Department>(oItem).State = EntityState.Deleted;
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

        private List<Department> GetSearchResult(string name)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Department.ToList()
                            select c;

                return query
               .Where(m => (string.IsNullOrEmpty(name) || m.Name.Contains(name)))
               .OrderByDescending(m => m.CreateTime)
               .ToList<Department>();
            }
        }

        #endregion
    }
}