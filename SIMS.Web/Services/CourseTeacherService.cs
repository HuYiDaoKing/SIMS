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
    public class CourseTeacherService
    {
        public static BaseRepository<CourseTeacher> oBll = new BaseRepository<CourseTeacher>();
        private static CourseTeacherService _instance = null;

        public static CourseTeacherService Instance
        {
            get
            {
                return _instance ?? new CourseTeacherService();
            }
        }

        private CourseTeacherService()
        {

        }

        #region Public

        #region Query

        public CourseTeacher GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.CourseTeacher.Find(id);
                return query;
            }
        }
        public List<CourseTeacherViewModel> GetBySomeWhere(int teacherId, int courseId, int iStart, int iLimit)
        {
            List<CourseTeacherViewModel> list = new List<CourseTeacherViewModel>();

            var filtered = GetSearchResult(teacherId, courseId);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<CourseTeacher> courseteachers = query.ToList<CourseTeacher>();
            foreach(CourseTeacher item in courseteachers)
            {
                string teachername = String.Empty;
                string coursename = String.Empty;

                Course course = CourseService.Instance.GetById(item.CourseId);
                AdminUser teacher = AdminUserService.Instance.GetById(item.TeacherId);

                if (course == null)
                    continue;

                if (teacher == null)
                    continue;

                teachername = teacher.Name;
                coursename = course.Name;

                list.Add(new CourseTeacherViewModel { 
                    Id=item.Id,
                    CourseId=item.CourseId,
                    TeacherId=item.TeacherId,
                    Course=coursename,
                    Teacher=teachername,
                    CreateTime=item.CreateTime,
                    ModifyTime=item.ModifyTime
                });  
            }
            return list;
        }
        public int GetCount(int teacherId, int courseId)
        {
            using (var context = new SIMSDbContext())
            {
                if (teacherId != 0)
                {
                    return context.CourseTeacher.Where(c => c.TeacherId == teacherId).Count();
                }
                else if (courseId != 0)
                {
                    return context.CourseTeacher.Where(c => c.CourseId == courseId).Count();
                }
                return context.CourseTeacher.Count();
            }
        }
        public bool Exist(int id, int teacherId)
        {
            bool bRet = false;
            int count = 0;
            using (var context = new SIMSDbContext())
            {
                if (id == 0)
                {
                    //新增
                    count = context.CourseTeacher.Where(c => c.TeacherId == teacherId).Count();
                }
                else
                {
                    //修改
                    count = context.CourseTeacher.Where(c => c.TeacherId == teacherId && c.Id != id).Count();
                }
            }
            bRet = count > 0 ? true : false;
            return bRet;
        }
        public List<int> GetTeacherByCourseId(int courseId)
        {
            List<int> courseTeacherIds = new List<int>();
            using (var context = new SIMSDbContext())
            {
                if (courseId == 0)
                    return courseTeacherIds;

                var q = from c in context.CourseTeacher.Where(c => c.CourseId == courseId) select c.TeacherId;
                courseTeacherIds = q.ToList();
           }
            return courseTeacherIds;
        }

        #endregion

        #region Create

        public bool Create(CourseTeacher oItem)
        {
            var oRet = new CourseTeacher();

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

        public bool Update(CourseTeacher oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.CourseTeacher.Attach(oIem);
                context.Entry<CourseTeacher>(oIem).State = EntityState.Modified;
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
                    CourseTeacher oItem = GetById(id);
                    db.CourseTeacher.Attach(oItem);
                    db.Entry<CourseTeacher>(oItem).State = EntityState.Deleted;
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

        private List<CourseTeacher> GetSearchResult(int teacherId, int courseId)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.CourseTeacher.ToList()
                            select c;

                if (teacherId != 0)
                {
                    return query
                        .Where(m => m.TeacherId == teacherId)
                        .OrderByDescending(m => m.CreateTime)
                        .ToList<CourseTeacher>();
                }

                if (courseId != 0)
                {
                    return query
                        .Where(m => m.CourseId == courseId)
                        .OrderByDescending(m => m.CreateTime)
                        .ToList<CourseTeacher>();
                }

                return query
               .OrderByDescending(m => m.CreateTime)
               .ToList<CourseTeacher>();
            }
        }

        #endregion
    }
}