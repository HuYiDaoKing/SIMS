using SIMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Services;

namespace SIMS.Web.Services
{
    public class CourseTeacherManager
    {
        private static object obj = new object();

        private static Dictionary<int, List<int>> _DicCourseTeacher;

        public static Dictionary<int, List<int>> DicCourseTeacher
        {
            get
            {
                Init();
                return _DicCourseTeacher;
            }
            set { _DicCourseTeacher = value; }
        }

        public static void Init()
        {
            try
            {
                lock (obj)
                {
                    _DicCourseTeacher = new Dictionary<int, List<int>>();

                    List<Course> courses = CourseService.Instance.GetAll();
                    foreach (Course course in courses)
                    {
                        if (course == null)
                            continue;

                        var teacherIds = CourseTeacherService.Instance.GetTeacherByCourseId(course.Id);
                        _DicCourseTeacher.Add(course.Id, teacherIds);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("初始化课程和教师关系失败:{0}", ex.Message);
                LogHelper.Log(LogType.Error, message);
            }
        }
    }
}