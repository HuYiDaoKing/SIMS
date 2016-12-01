using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS.Web.Helper
{
    public class DataTableParams
    {
        public DataTableParams(ControllerBase controller)
        {
            Controller = controller;

            Draw = int.Parse(controller.ControllerContext.HttpContext.Request["draw"]);
            Start = int.Parse(controller.ControllerContext.HttpContext.Request["start"]);
            Length = int.Parse(controller.ControllerContext.HttpContext.Request["length"]);
        }

        public ControllerBase Controller { get; set; }

        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }
}