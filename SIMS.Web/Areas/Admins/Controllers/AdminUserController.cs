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
using System.IO;
using SIMS.Common;
using System.Data;

namespace SIMS.Web.Areas.Admins.Controllers
{
    public class AdminUserController : Controller
    {
        private const string CACHE = @"~/Content/Admin/Files/";

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
                if (AdminUserService.Instance.IsExist(0, code))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "院系新增用户失败,已经存在改编号!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                AdminUser user = new AdminUser
                {
                    Code = code,
                    Name = name,
                    Sex = sex,
                    DepartmentId = department,
                    MajorId = major,
                    Passwordsalt = BCrypt.Net.BCrypt.HashPassword("123", BCrypt.Net.BCrypt.GenerateSalt()),
                    Profession = profession,
                    Grade = grade,
                    Nation = nation,
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

        [HttpPost]
        public JsonResult Upload()
        {
            var oResult = new Object();
            try
            {
                //注意:input name=file
                HttpPostedFileBase file = Request.Files["file"];

                //文件类型检测
                string fileExtension = Path.GetExtension(file.FileName);
                if (!string.Equals(fileExtension.ToLower(), ".xls") && !string.Equals(fileExtension.ToLower(), ".xlsx"))
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "文件格式不正确,请检查必须是Excel文件!"
                    };
                    return Json(oResult, JsonRequestBehavior.AllowGet);
                }

                if (file == null)
                {
                    oResult = new
                    {
                        Bresult = false,
                        Notice = "文件为空!"
                    };
                }
                else
                {
                    //1.文件上传
                    string fileName = String.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), file.FileName);
                    string filepath = Path.Combine(HttpContext.Server.MapPath(CACHE), fileName);
                    string fileDir = Path.GetDirectoryName(filepath);
                    if (!string.IsNullOrEmpty(fileDir))
                    {
                        if (!Directory.Exists(fileDir))
                            Directory.CreateDirectory(fileDir);
                        file.SaveAs(filepath);
                    }

                    //2.文件解析
                    using (ExcelHelper helper = new ExcelHelper(filepath))
                    {
                        DataTable dt1 = helper.ExcelToDataTable("2016级学生信息", true);
                        WriteToDB(dt1);

                        DataTable dt2 = helper.ExcelToDataTable("2015级学生信息", true);
                        WriteToDB(dt2);
                    }
                }

                oResult = new
                {
                    Bresult = false,
                    Notice = "文件上传成功!"
                };

            }
            catch (Exception ex)
            {
                oResult = new
                {
                    Bresult = false,
                    Notice = "文件上传失败,错误:" + ex.Message
                };
            }
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }

        #region Private

        private Object ToJson(AdminUserViewModel model, DataTableParams dtParams)
        {
            var json = model.AsJson() as Hashtable;
            json.Add("Actions", RenderViewHelper.RenderToString("_Actions", model, dtParams.Controller));

            return json;
        }

        private void WriteToDB(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string grade = row["年级"].ToString();
                string departmentname = row["院系"].ToString();
                string majorName = row["专业"].ToString();
                string sex = row["性别"].ToString();
                string age = row["年龄"].ToString();
                string name = row["名字"].ToString();
                string nation = row["民族"].ToString();

                int profession = 1;//学生
                Department department = DepartmentService.Instance.GetByName(departmentname);
                if (department == null)
                {
                    string msg = String.Format("当前院系{0},数据库中不存在!", departmentname);
                    LogHelper.Log(LogType.Error, msg);
                    continue;
                }

                Major major = MajorService.Instance.GetByName(majorName);
                if (major == null)
                {
                    string msg = String.Format("当前专业{0},数据库中不存在!", majorName);
                    LogHelper.Log(LogType.Error, msg);
                    continue;
                }

                string code = AdminUserService.Instance.GetCode(grade, department.Id, major.Id, profession);
                if (AdminUserService.Instance.IsExist(0, code))
                {
                    LogHelper.Log(LogType.Error, "院系新增用户失败,已经存在该编号!");
                    continue;
                }

                AdminUser user = new AdminUser
                {
                    Code = code,
                    Name = name,
                    Sex = sex,
                    DepartmentId = department.Id,
                    MajorId = major.Id,
                    Passwordsalt = BCrypt.Net.BCrypt.HashPassword("123", BCrypt.Net.BCrypt.GenerateSalt()),
                    Profession = profession,
                    Grade = grade,
                    Nation = nation,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };

                bool bRet = AdminUserService.Instance.Create(user);

                if (bRet)
                {
                    LogHelper.Log(LogType.Info, "用户新增成功!");
                }
                else
                {
                    LogHelper.Log(LogType.Info, "用户新增失败!");
                }
            }
        }
    }

        #endregion
}