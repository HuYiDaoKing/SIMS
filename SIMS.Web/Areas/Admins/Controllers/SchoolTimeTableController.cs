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
    public class SchoolTimeTableController : Controller
    {
        private const string SCHOOL_TIME_TABLE = @"<table align='center' border=1 bordercolor='#6DA4EA' cellspacing=2px width=600px height=200px><tralign='center'>
                <td>星期</td><td>星期一</td><td>星期二</td><td>星期三</td><td>星期四</td><td>星期五</td><td>星期六</td><td>星期日</td></tr>
                <tr align='center'><td rowspan=4>上午</td><td>Mon1</td><td>Tue1</td><td>Wen1</td><td>Thu1</td><td>Fri1</td><td rowspan=4>休息</td><td rowspan=4>休息</td></tr>
                <tr align='center'><td>Mon2</td><td>Tue2</td><td>Wen2</td><td>Thu2</td><td>Fri2</td><td></td></tr>
                <tr align='center'><td>Mon3</td><td>Tue3</td><td>Wen3</td><td>Thu3</td><td>Fri3</td><td></td></tr>
                <tr align='center'><td>Mon4</td><td>Tue4</td><td>Wen4</td><td>Thu4</td><td>Fri4</td><td></td></tr>
                <tr align='center'><td rowspan=2>下午</td><td>Mon5</td><td>Tue5</td><td>Wen5</td><td>Thu5</td><td>Fri5</td><td rowspan=2>休息</td><td rowspan=2>休息</td></tr>
                <tr align='center'><td>Mon6</td><td>Tue6</td><td>Wen6</td><td>Thu6</td><td>Fri6</td><td></td></tr>
            </tralign></table>";

        // GET: Admins/SchoolTimeTable
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetSchoolTimeTable()
        {
            string schoolTimeTable = SCHOOL_TIME_TABLE;

            try
            {
                AdminUser userInfo = Session[AccountHashKeys.CurrentAdminUser] as AdminUser;
                if(userInfo.MajorId==0)
                {
                    return schoolTimeTable;
                }

                //List<CourseScheduleViewModel> courseschedules = CourseScheduleService.Instance.GetByMajorId(userInfo.MajorId,1);
                //foreach(var item in courseschedules)
                //{
                //    string mon = String.Format("Mon{0}",item.JieCi);
                //    schoolTimeTable.Replace(mon,item.CourseName);
                //}

                for(int i=1;i<=5;i++)
                {
                    _GetSchoolTimeTable(userInfo.MajorId,i,ref schoolTimeTable);
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("获取课程表失败,异常:{0}", ex.Message);
                LogHelper.Log(LogType.Error, msg);
            }
            return schoolTimeTable;
        }

        private void _GetSchoolTimeTable(int? majorId, int zhouci, ref  string schoolTimeTable)
        {
            string flag = String.Empty;
            switch(zhouci)
            {
                case 1:
                    flag = "Mon";
                    break;
                case 2:
                    flag = "Tue";
                    break;
                case 3:
                    flag = "Wen";
                    break;
                case 4:
                    flag = "Thu";
                    break;
                case 5:
                    flag = "Fri";
                    break;
            }

            List<CourseScheduleViewModel> courseschedules = CourseScheduleService.Instance.GetByMajorId(majorId, zhouci);
            foreach (var item in courseschedules)
            {
                string sflag = String.Format("{0}{1}",flag,item.JieCi);
                schoolTimeTable=schoolTimeTable.Replace(sflag, item.CourseName);
                //string ss = schoolTimeTable.Replace(sflag, item.CourseName);
            }
        }

    }
}