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
        public Department GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Department.Find(id);
                return query;
            }
        }

        public List<Department> GetBySomeWhere(string strKeyWord, int iStart, int iLimit, out int iTotal)
        {
            var filtered = GetSearchResult(strKeyWord);
            iTotal = filtered.Count;

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            return query.ToList<Department>();
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

        private List<Department> GetSearchResult(string strKeyWord)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Department.ToList()
                            select c;

                return query
               .Where(m => (string.IsNullOrEmpty(strKeyWord) || m.Name.Contains(strKeyWord)))
               .OrderByDescending(m => m.CreateTime)
                    //.ThenByDescending(m => m.Id)
               .ToList<Department>();
            }
        }

        #endregion
    }
}