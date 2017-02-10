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

namespace SIMS.Web.Areas.Admins.Controllers
{
    public class CourseScheduleController : Controller
    {
        // GET: Admins/CourseSchedule
        public ActionResult Index()
        {
            //CourseScheduleService.Instance.GetDistinct();
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(int majorId)
        {
            DataTableParams dtParams = new DataTableParams(this);

            List<CourseScheduleViewModel> list = CourseScheduleService.Instance.GetBySomeWhere(majorId,dtParams.Start,dtParams.Length);
            int count = CourseScheduleService.Instance.GetCount(majorId);

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCourses()
        {
            List<Course> list = CourseService.Instance.GetAll();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMajors()
        {
            List<Major> list = MajorService.Instance.GetAll();
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Schedule()
        {
            var oResult = new Object();
            try
            {
                bool bRet = CourseScheduleService.Instance.Schedule();

                if (bRet)
                {
                    oResult = new
                    {
                        Bresult = true,
                        Notice = "自动排课成功!"
                    };
                }
                else
                {
                    oResult = new
                    {
                        Bresult = true,
                        Notice = "自动排课失败!"
                    };
                }
                
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("自动排课异常!{0}",ex.Message)
                };
            }
            return Json(oResult,JsonRequestBehavior.AllowGet);
        }

        #region Private

        private Object ToJson(CourseSchedule model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        #endregion
    }
}