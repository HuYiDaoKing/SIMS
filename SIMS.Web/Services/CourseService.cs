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
    public class CourseService
    {
        public static BaseRepository<Course> oBll = new BaseRepository<Course>();
        private static CourseService _instance = null;

        public static CourseService Instance
        {
            get
            {
                return _instance ?? new CourseService(); ;
            }
        }

        private CourseService()
        {

        }

        #region Public

        #region Query
        public Course GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Course.Find(id);
                return query;
            }
        }

        public List<Course> GetBySomeWhere(string strKeyWord, int iStart, int iLimit, out int iTotal)
        {
            var filtered = GetSearchResult(strKeyWord);
            iTotal = filtered.Count;

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            return query.ToList<Course>();
        }

        #endregion

        #region Create

        public bool Create(Course oItem)
        {
            var oRet = new Course();

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

        public bool Update(Course oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.Course.Attach(oIem);
                context.Entry<Course>(oIem).State = EntityState.Modified;
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
                    Course oItem = GetById(id);
                    db.Course.Attach(oItem);
                    db.Entry<Course>(oItem).State = EntityState.Deleted;
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

        private List<Course> GetSearchResult(string strKeyWord)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Course.ToList()
                            select c;

                return query
               .Where(m => (string.IsNullOrEmpty(strKeyWord) || m.Name.Contains(strKeyWord)))
               .OrderByDescending(m => m.CreateTime)
                    //.ThenByDescending(m => m.Id)
               .ToList<Course>();
            }
        }

        #endregion
    }
}