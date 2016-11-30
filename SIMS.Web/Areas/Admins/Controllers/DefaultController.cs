using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS.Web.Areas.Admins.Controllers
{
    public class DefaultController : BaseController
    {
        // GET: Admins/Default
        public ActionResult Index()
        {
            return View();
        }
    }
}