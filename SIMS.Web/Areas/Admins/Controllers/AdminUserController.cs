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

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(string name, string grade, string sex, string nation, int department, int major, int profession)
        {
            var oResult = new Object();
            try
            {
                string code = AdminUserService.Instance.GetCode(grade, department, major, profession);
                if(AdminUserService.Instance.IsExist(0,code))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "院系新增用户失败,已经存在改编号!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                AdminUser user = new AdminUser { 
                    Code=code,
                    Name=name,
                    Sex=sex,
                    DepartmentId=department,
                    MajorId=major,
                    Passwordsalt = BCrypt.Net.BCrypt.HashPassword("123", BCrypt.Net.BCrypt.GenerateSalt()),
                    Profession=profession,
                    Grade=grade,
                    Nation=nation,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };

                bool bRet = AdminUserService.Instance.Create(user);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "院系新增成功!" : "院系新增失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("院系新增失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Update(int id, string name, string description)
        {
            var oResult = new Object();
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "名称不能为空!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Department originalDepartment = DepartmentService.Instance.GetById(id);
                if (originalDepartment == null)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "不存在该院系!请检查!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }
                originalDepartment.Name = name;
                originalDepartment.Description = description;
                originalDepartment.ModifyTime = DateTime.Now;

                bool bRet = DepartmentService.Instance.Create(originalDepartment);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "院系修改成功!" : "院系修改失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("院系修改失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
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