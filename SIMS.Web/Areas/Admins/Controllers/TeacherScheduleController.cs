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
    public class TeacherScheduleController : Controller
    {
        // GET: Admins/TeacherSchedule
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(int majorId)
        {
            AdminUser currentUser = Session[AccountHashKeys.CurrentAdminUser] as AdminUser;

            DataTableParams dtParams = new DataTableParams(this);

            List<CourseScheduleViewModel> list = CourseScheduleService.Instance.GetBySomeWhere(majorId,currentUser.Id,dtParams.Start,dtParams.Length);
            int count = CourseScheduleService.Instance.GetCount(majorId,currentUser.Id);

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMajors()
        {
            AdminUser currentUser = Session[AccountHashKeys.CurrentAdminUser] as AdminUser;
            List<Major> list = CourseScheduleService.Instance.GetMajorsByCourseTeacherId(currentUser.Id);
            return Json(list, JsonRequestBehavior.AllowGet);
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