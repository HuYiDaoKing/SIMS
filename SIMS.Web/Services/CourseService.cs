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

        public bool IsExist(int id, int departmentId, string code)
        {
            bool isExist = false;
            using (var context = new SIMSDbContext())
            {
                if (id == 0)
                {
                    var query = context.Course.FirstOrDefault(c => c.DepartmentId == departmentId && c.Code == code);
                    isExist = null == query ? false : true;
                }
                else
                {
                    var query = context.Course.FirstOrDefault(c => c.DepartmentId == departmentId && c.Code == code && c.Id != id);
                    isExist = null == query ? false : true;
                }
                return isExist;
            }
        }

        public Course GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Course.Find(id);
                return query;
            }
        }

        public List<Course> GetBySomeWhere(int departmentId, int iStart, int iLimit)
        {
            var filtered = GetSearchResult(departmentId);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            return query.ToList<Course>();
        }

        public int GetCount(int departmentId)
        {
            using (var context = new SIMSDbContext())
            {
                if (departmentId == 0)
                    return context.Course.Count();
                else
                    return context.Course
                        .Where(m => m.DepartmentId == departmentId)
                        .Count();
            }
        }

        public string GetCourseCode(int departmentId)
        {
            //课程编号(院系代码2位+顺序编号2位)
            using (var context = new SIMSDbContext())
            {
                string code = string.Empty;
                Department department = DepartmentService.Instance.GetById(departmentId);
                string _code = string.Empty;
                int maxMajorId = GetMaxId();
                if (maxMajorId < 10)
                {
                    _code = string.Format("0{0}", maxMajorId + 1);
                }
                else
                {
                    _code = maxMajorId.ToString();
                }
                code = String.Format("{0}{1}", department.Code, _code);
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
                int count = context.Course.Count();
                if (count > 0)
                    return (from c in context.Course select c.Id).Max();
                return 0;
            }
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

        private List<Course> GetSearchResult(int departmentId)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Course.ToList()
                            select c;

                if (departmentId != 0)
                {
                    return query
               .Where(m => m.DepartmentId == departmentId)
               .OrderByDescending(m => m.CreateTime)
               .ToList<Course>();
                }

                return query
               .OrderByDescending(m => m.CreateTime)
               .ToList<Course>();
            }
        }

        #endregion
    }
}