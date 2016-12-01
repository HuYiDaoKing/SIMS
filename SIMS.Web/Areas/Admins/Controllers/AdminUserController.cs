using SIMS.Web.Areas.Admins.ViewModels;
using SIMS.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIMS.Web.Services;
using System.Collections;

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
        public JsonResult GetDataTable(string code)
        {
            DataTableParams dtParams = new DataTableParams(this);
            List<AdminUserViewModel> list = AdminUserService.Instance.GetBySomeWhere(code, dtParams.Start, dtParams.Length);
            int count = AdminUserService.Instance.GetCount(code);

            //return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list }, JsonRequestBehavior.AllowGet);
            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        #region Private

        private Object ToJson(AdminUserViewModel model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        #endregion
    }
}