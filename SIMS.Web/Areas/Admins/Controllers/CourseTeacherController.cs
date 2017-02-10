using SIMS.Web.Areas.Admins.ViewModels;
using SIMS.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIMS.Web.Services;
using System.Collections;
using SIMS.Web.EntityDataModel;
using SIMS.Common;

namespace SIMS.Web.Areas.Admins.Controllers
{
    public class CourseTeacherController : Controller
    {
        // GET: Admins/CourseTeacher
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(int courseId, int teacherId)
        {
            DataTableParams dtParams = new DataTableParams(this);

            List<CourseTeacherViewModel> list = CourseTeacherService.Instance.GetBySomeWhere(teacherId, courseId, dtParams.Start, dtParams.Length);
            int count = CourseTeacherService.Instance.GetCount(teacherId, courseId);

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(int courseId, int teacherId)
        {
            var oResult = new Object();
            try
            {
                if (courseId == 0 && teacherId == 0)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "给老师指派课程失败!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                CourseTeacher courseTeacher = new CourseTeacher
                {
                    CourseId = courseId,
                    TeacherId = teacherId,
                    CreateTime=DateTime.Now,
                    ModifyTime=DateTime.Now
                };

                //检查是否给当前教师分配过课程
                bool isExist = IsExist(0,teacherId);
                if (isExist)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "已经给当前老师指派过课程!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                bool bRet = CourseTeacherService.Instance.Create(courseTeacher);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "给{0}老师指派课程新增成功!" : "给{0}老师指派课程新增失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("给老师指派新增失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(int id, int courseId, int teacherId)
        {
            var oResult = new Object();
            try
            {
                if (courseId == 0 && teacherId == 0)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "您没有选择教师或者课程!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Course course = CourseService.Instance.GetById(courseId);
                if (course == null)
                {
                    LogHelper.Log(LogType.Info, "课程没有查到!");
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "课程没有查到!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                AdminUser user = AdminUserService.Instance.GetById(teacherId);
                if (user == null)
                {
                    LogHelper.Log(LogType.Info, "教师没有查到!");
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "教师没有查到!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                CourseTeacher courseTeacher = CourseTeacherService.Instance.GetById(id);
                if (courseTeacher != null)
                {
                    courseTeacher.TeacherId = teacherId;
                    courseTeacher.CourseId = courseId;
                    courseTeacher.ModifyTime = DateTime.Now;
                }

                //检查是否给当前教师分配过课程
                bool isExist = IsExist(id, teacherId);
                if (isExist)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "已经给当前老师指派过课程!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                bool bRet = CourseTeacherService.Instance.Update(courseTeacher);

                string msg = String.Empty;
                if (bRet)
                {
                    msg = String.Format("给{0}老师指派课程{1}成功!", user.Name, course.Name);
                }

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "修改成功!" : "课程修改失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("课程新增失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
        }

        public bool IsExist(int id, int teacherId)
        {
            return CourseTeacherService.Instance.Exist(id,teacherId);
        }

        #region Private

        private Object ToJson(CourseTeacher model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }
        
        #endregion

    }
}