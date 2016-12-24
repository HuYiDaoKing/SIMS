using SIMS.Web.EntityDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS.Web.Areas.Admins.ViewModels
{
    public class MajorViewModel:Major
    {
        public string DepartmentName { get; set; }
    }
}