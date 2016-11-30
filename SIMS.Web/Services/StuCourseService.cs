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
    public class StuCourseService
    {
        public static BaseRepository<StuCourse> oBll = new BaseRepository<StuCourse>();
        private static StuCourseService _instance = null;
        public static StuCourseService Instance
        {
            get
            {
                return _instance ?? new StuCourseService(); ;
            }
        }
        private StuCourseService()
        {

        }
        
        #region Public

        #region Query

        public StuCourse GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.StuCourse.Find(id);
                return query;
            }
        }
        public StuCourse GetByStudentNo(string studentNo)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.StuCourse.FirstOrDefault(c=>c.StudentCode.Equals(studentNo));
                return query;
            }
        } 
        public List<StuCourse> GetBySomeWhere(string studentNo, int iStart, int iLimit, out int iTotal)
        {
            var filtered = GetSearchResult(studentNo);
            iTotal = filtered.Count;

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            return query.ToList<StuCourse>();
        }

        #endregion

        #region Create

        public bool Create(StuCourse oItem)
        {
            var oRet = new StuCourse();

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

        public bool Update(StuCourse oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.StuCourse.Attach(oIem);
                context.Entry<StuCourse>(oIem).State = EntityState.Modified;
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
                    StuCourse oItem = GetById(id);
                    db.StuCourse.Attach(oItem);
                    db.Entry<StuCourse>(oItem).State = EntityState.Deleted;
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

        private List<StuCourse> GetSearchResult(string studentNo)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.StuCourse.ToList()
                            select c;

                return query
               .Where(m => (string.IsNullOrEmpty(studentNo) || m.StudentCode.Contains(studentNo)))
               .OrderByDescending(m => m.CreateTime)
               .ToList<StuCourse>();
            }
        }

        #endregion
    }
}