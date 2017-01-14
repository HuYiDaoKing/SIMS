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
    public class CourseController : BaseController
    {
        // GET: Admins/Course
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDataTable(int departmentId)
        {
            DataTableParams dtParams = new DataTableParams(this);

            List<Course> list = CourseService.Instance.GetBySomeWhere(departmentId, dtParams.Start, dtParams.Length);
            int count = CourseService.Instance.GetCount(departmentId);

            return Json(new { draw = dtParams.Draw, recordsTotal = list.Count, recordsFiltered = count, data = list.Select(m => ToJson(m, dtParams)) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(int departmentId, string name, int score, string description)
        {
            var oResult = new Object();
            try
            {
                if (departmentId == 0)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "课程新增失败,没有选择院系!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                string code = CourseService.Instance.GetCourseCode(departmentId);
                //if (MajorService.Instance.IsExist(0, departmentId, code))
                if(CourseService.Instance.IsExist(0,departmentId,code))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "课程新增失败,已经存在该编号!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                Course course = new Course
                {
                    Code = code,
                    Name = name,
                    Score = score,
                    Hour = 10,
                    DepartmentId=departmentId,
                    Description = description
                };

                bool bRet = CourseService.Instance.Create(course);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? " 课程新增成功!" : "课程新增失败!"
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

        [HttpPost]
        public JsonResult Update(int id, int departmentId, string name, int score, string description)
        {
            var oResult = new Object();
            try
            {
                if (departmentId == 0)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "请选择学院!"
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

                Course course = CourseService.Instance.GetById(id);
                if (course != null)
                {
                    course.DepartmentId = departmentId;
                    course.Name = name;
                    course.Description = description;
                    course.ModifyTime = DateTime.Now;
                }
                bool bRet = CourseService.Instance.Update(course);

                oResult = new
                {
                    Bresult = bRet,
                    Notice = bRet ? "课程修改成功!" : "课程修改失败!"
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

        //[HttpPost]
        //public JsonResult GetDepartment()
        //{
        //    List<Department> list = DepartmentService.Instance.GetAll();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult GetMajorsByDepartmentId(int departmentId = 0)
        //{
        //    if (departmentId == 0)
        //    {
        //        List<Department> departments = DepartmentService.Instance.GetAll();
        //        return Json(departments, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        List<MajorViewModel> majors = MajorService.Instance.GetMajorsByDepartmentId(departmentId);
        //        return Json(majors, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #region Private

        private Object ToJson(Course model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        #endregion

    }
}