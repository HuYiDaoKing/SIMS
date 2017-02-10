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
    public class CourseScheduleService
    {
        public static BaseRepository<CourseSchedule> oBll = new BaseRepository<CourseSchedule>();
        private static CourseScheduleService _instance = null;

        private static int zhouMaxNum = 5;
        private static int jieMaxNum = 6;

        public static CourseScheduleService Instance
        {
            get
            {
                return _instance ?? new CourseScheduleService();
            }
        }

        private CourseScheduleService()
        {

        }

        #region Public

        #region Query

        public CourseSchedule GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.CourseSchedule.Find(id);
                return query;
            }
        }
        public List<CourseScheduleViewModel> GetBySomeWhere(int majorId, int iStart, int iLimit)
        {
            List<CourseScheduleViewModel> list = new List<CourseScheduleViewModel>();

            var filtered = GetSearchResult(majorId);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<CourseSchedule> courseschedules = query.ToList<CourseSchedule>();
            foreach (CourseSchedule item in courseschedules)
            {
                string courseteachername = String.Empty;
                string coursename = String.Empty;
                string majorname = String.Empty;

                Course course = CourseService.Instance.GetById(item.CourseId);
                AdminUser teacher = AdminUserService.Instance.GetById(item.CourseTeacherId);
                Major major = MajorService.Instance.GetById(item.MajorId);

                if (course == null)
                    continue;

                if (teacher == null)
                    continue;

                if (major == null)
                    continue;

                courseteachername = teacher.Name;
                coursename = course.Name;
                majorname = major.Name;

                list.Add(new CourseScheduleViewModel
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseTeacherId = item.CourseTeacherId,
                    MajorId = item.MajorId,
                    ZhouCi = item.ZhouCi,
                    JieCi = item.JieCi,
                    CourseTeacherName = courseteachername,
                    CourseName = coursename,
                    MajorName = majorname,
                    CreateTime = item.CreateTime,
                    ModifyTime = item.ModifyTime
                });
            }
            return list;
        }

        /// <summary>
        /// 教师查询课程
        /// </summary>
        /// <param name="majorId"></param>
        /// <param name="courseTeacherId"></param>
        /// <param name="iStart"></param>
        /// <param name="iLimit"></param>
        /// <returns></returns>
        public List<CourseScheduleViewModel> GetBySomeWhere(int majorId,int courseTeacherId, int iStart, int iLimit)
        {
            List<CourseScheduleViewModel> list = new List<CourseScheduleViewModel>();

            var filtered = GetSearchResult(majorId,courseTeacherId);
            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<CourseSchedule> courseschedules = query.ToList<CourseSchedule>();
            foreach (CourseSchedule item in courseschedules)
            {
                string courseteachername = String.Empty;
                string coursename = String.Empty;
                string majorname = String.Empty;

                Course course = CourseService.Instance.GetById(item.CourseId);
                AdminUser teacher = AdminUserService.Instance.GetById(item.CourseTeacherId);
                Major major = MajorService.Instance.GetById(item.MajorId);

                if (course == null)
                    continue;

                if (teacher == null)
                    continue;

                if (major == null)
                    continue;

                courseteachername = teacher.Name;
                coursename = course.Name;
                majorname = major.Name;

                list.Add(new CourseScheduleViewModel
                {
                    Id = item.Id,
                    CourseId = item.CourseId,
                    CourseTeacherId = item.CourseTeacherId,
                    MajorId = item.MajorId,
                    ZhouCi = item.ZhouCi,
                    JieCi = item.JieCi,
                    CourseTeacherName = courseteachername,
                    CourseName = coursename,
                    MajorName = majorname,
                    CreateTime = item.CreateTime,
                    ModifyTime = item.ModifyTime
                });
            }
            return list;
        }

        public int GetCount(int majorId)
        {
            using (var context = new SIMSDbContext())
            {
                if (majorId != 0)
                {
                    return context.CourseSchedule.Where(c => c.MajorId == majorId).Count();
                }
                return context.CourseSchedule.Count();
            }
        }

        /// <summary>
        /// 教师查询课程
        /// </summary>
        /// <param name="majorId"></param>
        /// <param name="courseTeacherId"></param>
        /// <returns></returns>
        public int GetCount(int majorId,int courseTeacherId)
        {
            using (var context = new SIMSDbContext())
            {
                if (majorId != 0)
                {
                    return context.CourseSchedule.Where(c => c.MajorId == majorId).Count();
                }
                return context.CourseSchedule.Where(c=>c.CourseTeacherId==courseTeacherId).Count();
            }
        }

        public Object GetDistinct()
        {
            using (var context = new SIMSDbContext())
            {
                var q = from c in context.CourseSchedule select new { CourseId=c.CourseId, CourseTeacherId=c.CourseTeacherId, MajorId=c.MajorId };
                var t = q.Distinct();
                return t;
            }
        }

        public List<Major> GetMajorsByCourseTeacherId(int courseTeacherId)
        {
            List<Major> majors = new List<Major>();
            using (var context = new SIMSDbContext())
            {
                if(courseTeacherId !=0)
                {
                    var q= from c in context.CourseSchedule.Where(c=>c.CourseTeacherId==courseTeacherId) select c.MajorId;
                    List<int> majorIds = q.Distinct().ToList();
                    for (int i = 0; i < majorIds.Count;i++ )
                    {
                        Major major = MajorService.Instance.GetById(majorIds[i]);
                        majors.Add(major);
                    }
                }
                return majors;
            }
        }

        public bool Exist(int courseId, int courseteacherId, int majorId, int zhouci, int jieci)
        {
            bool bRet = false;
            int count = 0;
            using (var context = new SIMSDbContext())
            {
                /*count = context.CourseSchedule.Where(
                    c => c.CourseId == courseId
                    //&& c.CourseTeacherId == courseteacherId
                    && c.MajorId == majorId
                    && c.ZhouCi == zhouci
                    && c.JieCi == jieci).Count();*/

                count = context.CourseSchedule.Where(
                    //c => c.CourseId == courseId
                    //&& c.CourseTeacherId == courseteacherId
                    c => c.MajorId == majorId
                    && c.ZhouCi == zhouci
                    && c.JieCi == jieci).Count();
            }
            bRet = count > 0 ? true : false;
            return bRet;
        }
        public bool ExistForMajor(int courseId, int courseteacherId, int majorId, int zhouci, int jieci)
        {
            bool bRet = false;
            int count = 0;
            using (var context = new SIMSDbContext())
            {
                /*count = context.CourseSchedule.Where(
                    c => c.CourseId == courseId
                    //&& c.CourseTeacherId == courseteacherId
                    && c.MajorId == majorId
                    && c.ZhouCi == zhouci
                    && c.JieCi == jieci).Count();*/

                count = context.CourseSchedule.Where(
                    c => c.CourseId == courseId
                    && c.CourseTeacherId == courseteacherId
                        //&& c.MajorId == majorId
                    && c.ZhouCi == zhouci
                    && c.JieCi == jieci).Count();
            }
            bRet = count > 0 ? true : false;
            return bRet;
        }
        public bool ExistForWeek(int courseId, int courseteacherId, int majorId, int zhouci, int jieci)
        {
            //同一专业一周内不能出现相同课程>2
            bool bRet = false;
            int count = 0;
            using (var context = new SIMSDbContext())
            {
                count = context.CourseSchedule.Where(
                c => c.CourseId == courseId
                    //&& c.CourseTeacherId == courseteacherId).Count();
                 && c.MajorId == majorId).Count();
                //&& c.ZhouCi == zhouci
                //&& c.JieCi == jieci).Count();
            }
            bRet = count > 2 ? true : false;
            return bRet;
        }
        public List<CourseScheduleViewModel> GetByMajorId(int? majorId,int zhouci)
        {
            List<CourseScheduleViewModel> list = new List<CourseScheduleViewModel>();
            using (var context = new SIMSDbContext())
            {
                if (majorId != 0)
                {
                    Major major = context.Major.Find(majorId);
                    List<CourseSchedule> couseschedules = context.CourseSchedule.Where(c => c.MajorId == majorId && c.ZhouCi==zhouci).ToList();

                    foreach (CourseSchedule item in couseschedules)
                    {
                        string courseteachername = String.Empty;
                        string coursename = String.Empty;
                        string majorname = String.Empty;

                        Course course = CourseService.Instance.GetById(item.CourseId);
                        AdminUser teacher = AdminUserService.Instance.GetById(item.CourseTeacherId);
                        //Major major = MajorService.Instance.GetById(item.MajorId);

                        if (course == null)
                            continue;

                        if (teacher == null)
                            continue;

                        if (major == null)
                            continue;

                        courseteachername = teacher.Name;
                        coursename = course.Name;
                        majorname = major.Name;

                        list.Add(new CourseScheduleViewModel
                        {
                            Id = item.Id,
                            CourseId = item.CourseId,
                            CourseTeacherId = item.CourseTeacherId,
                            MajorId = item.MajorId,
                            ZhouCi = item.ZhouCi,
                            JieCi = item.JieCi,
                            CourseTeacherName = courseteachername,
                            CourseName = coursename,
                            MajorName = majorname,
                            CreateTime = item.CreateTime,
                            ModifyTime = item.ModifyTime
                        });
                    }

                }
                return list;
            }
        }

        #endregion

        #region Create

        public bool Create(CourseSchedule oItem)
        {
            var oRet = new CourseSchedule();

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

        public bool Update(CourseSchedule oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.CourseSchedule.Attach(oIem);
                context.Entry<CourseSchedule>(oIem).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 第一层:课程
        /// 第二层:专业
        /// 说明:课程+教师唯一
        /// 课程+教师+周次+节次不能冲突
        /// </summary>
        /// <returns></returns>
        public bool Schedule()
        {
            bool bRet = true;
            try
            {
                Dictionary<int, List<int>> courseteachers = CourseTeacherManager.DicCourseTeacher;

                //专业
                List<Major> majors = MajorService.Instance.GetAll();

                foreach (Major major in majors)
                {
                    int majorId = major.Id;

                    //课程
                    foreach (var dic in courseteachers)
                    {
                        //周次[1,5]
                        //节次[1,6]
                        //1.检查当前课程+老师+周次+节次是否存在

                        //1.1 不存在,则插入
                        //1.2 存在,在递增节次或周次
                        int courseId = dic.Key;

                        foreach (var teacherId in dic.Value)
                        {
                            _Schedule(dic.Key, teacherId, majorId, 1, 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bRet = false;
                LogHelper.Log(LogType.Error, String.Format("排课异常:{0}", ex.Message));
            }
            return bRet;
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
                    CourseSchedule oItem = GetById(id);
                    db.CourseSchedule.Attach(oItem);
                    db.Entry<CourseSchedule>(oItem).State = EntityState.Deleted;
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

        private List<CourseSchedule> GetSearchResult(int majorId)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.CourseSchedule.ToList()
                            select c;

                if (majorId != 0)
                {
                    return query
                        .Where(m => m.MajorId == majorId)
                        .OrderByDescending(m => m.CreateTime)
                        .ToList<CourseSchedule>();
                }

                return query
               .OrderByDescending(m => m.CreateTime)
               .ToList<CourseSchedule>();
            }
        }

        private List<CourseSchedule> GetSearchResult(int majorId,int courseTeacherId)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.CourseSchedule.Where(c=>c.CourseTeacherId==courseTeacherId).ToList()
                            select c;

                if (majorId != 0)
                {
                    return query
                        .Where(m => m.MajorId == majorId &&  m.CourseTeacherId==courseTeacherId )
                        .OrderByDescending(m => m.CreateTime)
                        .ToList<CourseSchedule>();
                }

                return query
               .OrderByDescending(m => m.CreateTime)
               .ToList<CourseSchedule>();
            }
        }

        private void _Schedule(int courseId, int courseteacherId, int majorId, int zhouci, int jieci)
        {
            if (zhouci > zhouMaxNum)
            {
                string msg = String.Format("当前周次{0}已经超过最大值{1}", zhouci, zhouMaxNum);
                LogHelper.Log(LogType.Info, msg);
                return;
            }

            if (jieci > jieMaxNum)
            {
                string msg = String.Format("当前节次{0}已经超过最大值{1}", jieci, jieMaxNum);
                LogHelper.Log(LogType.Info, msg);
                return;
            }

            bool isExist = Exist(courseId, courseteacherId, majorId, zhouci, jieci);

            if (isExist)
            {
                if (jieci == jieMaxNum)
                    _Schedule(courseId, courseteacherId, majorId, zhouci + 1, jieci);
                else
                    _Schedule(courseId, courseteacherId, majorId, zhouci, jieci + 2);
            }
            else
            {
                //插入
                CourseScheduleService.Instance.Create(new CourseSchedule
                {
                    CourseId = courseId,
                    CourseTeacherId = courseteacherId,
                    MajorId = majorId,
                    ZhouCi = zhouci,
                    JieCi = jieci,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                });

                //插入
                CourseScheduleService.Instance.Create(new CourseSchedule
                {
                    CourseId = courseId,
                    CourseTeacherId = courseteacherId,
                    MajorId = majorId,
                    ZhouCi = zhouci,
                    JieCi = jieci + 1,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                });
            }

            if (zhouci < zhouMaxNum)
            {
                //不同周次的排课
                bool bExist = ExistForMajor(courseId, courseteacherId, majorId, zhouci, jieci);
                if (!bExist)
                {
                    _Schedule(courseId, courseteacherId, majorId, zhouci + 1, jieci);
                }

            }
        }

        #endregion
    }
}