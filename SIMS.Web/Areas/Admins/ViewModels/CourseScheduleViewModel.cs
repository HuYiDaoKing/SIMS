using SIMS.Web.EntityDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS.Web.Areas.Admins.ViewModels
{
    public class CourseScheduleViewModel:CourseSchedule
    {
        public string CourseName { get; set; }
        public string CourseTeacherName { get; set; }
        public string MajorName { get; set; }
    }
}