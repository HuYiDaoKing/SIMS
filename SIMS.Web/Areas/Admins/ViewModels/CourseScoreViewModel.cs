using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIMS.Web.EntityDataModel;

namespace SIMS.Web.Areas.Admins.ViewModels
{
    public class CourseScoreViewModel : CourseScore
    {
        public string CourseName { get; set; }
        public string CourseTeacherName { get; set; }
        public string MajorName { get; set; }
        public string StudentName { get; set; }
    }
}