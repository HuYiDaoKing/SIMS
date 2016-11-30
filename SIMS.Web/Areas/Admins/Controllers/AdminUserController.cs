using SIMS.Web.Areas.Admins.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS.Web.Areas.Admins.Controllers
{
    public class AdminUserController : Controller
    {
        // GET: Admins/AdminUser
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetList(string code)
        {
            int draw = int.Parse(Request.Params["draw"].ToString());
            int start = int.Parse(Request.Params["start"].ToString());
            int length = int.Parse(Request.Params["length"].ToString());


            //ActivityService service = new ActivityService();
            //List<ActivityViewModel> list = service.GetActivity(ActivityName, BeginTime, EndTime, start, length);
            
            List<AdminUserViewModel> list

            int count = service.GetActivityCount(ActivityName, BeginTime, EndTime);
            return Json(new { draw = draw, recordsTotal = list.Count, recordsFiltered = count, data = list }, JsonRequestBehavior.AllowGet);

            // List<ActivityViewModel> _Filters = list.Skip(start).Take(length).ToList();
            //return Json(new { draw = draw, recordsTotal = _Filters.Count, recordsFiltered = list.Count, data = _Filters }, JsonRequestBehavior.AllowGet);
        }

    }
}