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
    public class CourseScoreService
    {
        public static BaseRepository<CourseScore> oBll = new BaseRepository<CourseScore>();
        private static CourseScoreService _instance = null;

        public static CourseScoreService Instance
        {
            get
            {
                return _instance ?? new CourseScoreService();
            }
        }

        private CourseScoreService()
        {

        }

        #region Public

        #region Query

        public CourseScore GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.CourseScore.Find(id);
                return query;
            }
        }

        public List<CourseScoreViewModel> GetBySomeWhere(int courseTeacherId, int courseId, int majorId, int iStart, int iLimit)
        {
            List<CourseScoreViewModel> list = new List<CourseScoreViewModel>();

            var filtered = GetSearchResult(courseTeacherId,courseId,majorId);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<CourseScore> coursescores = query.ToList<CourseScore>();
            foreach (CourseScore item in coursescores)
            {
                string teachername = String.Empty;
                string coursename = String.Empty;
                string majorname = String.Empty;
                string studentname = String.Empty;

                Course course = CourseService.Instance.GetById(item.CourseId);
                AdminUser teacher = AdminUserService.Instance.GetById(item.CourseTeacherId);
                AdminUser student = AdminUserService.Instance.GetById(item.StudentId);
                Major major = MajorService.Instance.GetById(item.MajorId);

                if (course == null)
                    continue;

                if (teacher == null)
                    continue;

                if (student == null)
                    continue;

                if (major == null)
                    continue;

                teachername = teacher.Name;
                coursename = course.Name;
                majorname = major.Name;
                studentname = student.Name;

                list.Add(new CourseScoreViewModel
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseTeacherId = item.CourseTeacherId,
                    MajorId=item.MajorId,
                    StudentId=item.StudentId,
                    CourseName = coursename,
                    CourseTeacherName = teachername,
                    MajorName=majorname,
                    StudentName=studentname,
                    Score=item.Score,
                    CreateTime = item.CreateTime,
                    ModifyTime = item.ModifyTime
                });
            }
            return list;
        }
        public int GetCount(int courseTeacherId, int courseId, int majorId)
        {
            using (var context = new SIMSDbContext())
            {
                return context.CourseScore.Where(c => c.CourseTeacherId == courseTeacherId && c.CourseId == courseId && c.MajorId == majorId).Count();
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

        public bool Create(CourseScore oItem)
        {
            var oRet = new CourseScore();

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

        public bool Update(CourseScore oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.CourseScore.Attach(oIem);
                context.Entry<CourseScore>(oIem).State = EntityState.Modified;
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
                    CourseScore oItem = GetById(id);
                    db.CourseScore.Attach(oItem);
                    db.Entry<CourseScore>(oItem).State = EntityState.Deleted;
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

        private List<CourseScore> GetSearchResult(int courseTeacherId, int courseId,int majorId)
        {
            using (var context = new SIMSDbContext())
            {
                var q = context.CourseScore.Where(c => c.CourseTeacherId == courseTeacherId && c.CourseId == courseId && c.MajorId == majorId).ToList();
                return context.CourseScore.Where(c=>c.CourseTeacherId==courseTeacherId && c.CourseId==courseId && c.MajorId==majorId).ToList();
            }
        }

        #endregion
    }
}