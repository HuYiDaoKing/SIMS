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
    public class DepartmentController : Controller
    {
        // GET: Admins/Department
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(string name)
        {
            DataTableParams dtParams = new DataTableParams(this);
            List<Department> list = DepartmentService.Instance.GetBySomeWhere(name, dtParams.Start, dtParams.Length);
            int count = DepartmentService.Instance.GetCount(name);

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(string name, string description)
        {
            var oResult = new Object();
            try
            {
                string code = DepartmentService.Instance.GetDepartmentCode();
                if (DepartmentService.Instance.IsExist(0, code))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "院系新增失败,已经存在改编号!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Department department = new Department
                {
                    Code = code,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };

                bool bRet = DepartmentService.Instance.Create(department);

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

        public JsonResult Update(int id,string name, string description)
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

        [HttpPost]
        public JsonResult GetDepartment()
        {
            List<Department> list = DepartmentService.Instance.GetAll();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Private

        private Object ToJson(Department model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        #endregion
    }
}