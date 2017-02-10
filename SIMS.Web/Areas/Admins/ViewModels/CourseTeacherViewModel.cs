using SIMS.Web.EntityDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS.Web.Areas.Admins.ViewModels
{
    public class CourseTeacherViewModel:CourseTeacher
    {
        public string Course { get; set; }
        public string Teacher { get; set; }
    }
}