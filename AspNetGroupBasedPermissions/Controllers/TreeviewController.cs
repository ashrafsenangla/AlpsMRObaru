using Core.Entities.Data;
using Core.Entities.Data.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetGroupBasedPermissions.Controllers
{
    public class TreeviewController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Simple()
        {
            List<Service> all = new List<Service>();
            Infrastructures.Data.ApplicationDbContext _db = new Infrastructures.Data.ApplicationDbContext();

            {
                all = _db.Services.OrderBy(a => a.ParentId).ToList();
            }
            return View(all);
        }
	}
}