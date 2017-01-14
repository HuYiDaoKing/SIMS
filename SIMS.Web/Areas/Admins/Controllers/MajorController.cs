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
    public class MajorController : Controller
    {
        // GET: Admins/Major
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(int departmentId)
        {
            DataTableParams dtParams = new DataTableParams(this);
            List<MajorViewModel> list = MajorService.Instance.GetBySomeWhere(departmentId, dtParams.Start, dtParams.Length);
            int count = MajorService.Instance.GetCount(departmentId);
            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(int departmemtId, string name, string description)
        {
            var oResult = new Object();
            try
            {
                string code = MajorService.Instance.GetMajorCode(departmemtId);//学院代码+MaxMajorId(两位)

                if (MajorService.Instance.IsExist(0, departmemtId, code))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "专业新增失败,已经存在该编号!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Major major = new Major
                {
                    Code = code,
                    DepartmentId = departmemtId,
                    Name = name,
                    Description = description,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };

                bool bRet = MajorService.Instance.Create(major);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "专业新增成功!" : "专业新增失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("专业新增失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(int id, int departmentId, string name, string description)
        {
            var oResult = new Object();
            try
            {
                if (departmentId == 0)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "请选择部门!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                if (String.IsNullOrEmpty(name))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "名称不能为空!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Major major = MajorService.Instance.GetById(id);
                if (major != null)
                {
                    major.DepartmentId = departmentId;
                    major.Name = name;
                    major.Description = description;
                    major.ModifyTime = DateTime.Now;
                }

                bool bRet = MajorService.Instance.Update(major);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "专业修改成功!" : "专业修改失败!"
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = String.Format("专业新增失败!异常:{0}", ex.Message)
                };
                return Json(oResult, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetAllMajor()
        {
            List<Department> list = DepartmentService.Instance.GetAll();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Private

        private Object ToJson(Major model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        #endregion

    }
}