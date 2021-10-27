using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Core.Entities.Data;
using System.Web.UI;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using AspNetGroupBasedPermissions.ViewModels;
using Core.Entities.Data.Admin;


namespace AspNetGroupBasedPermissions.Controllers
{
    public class HomeController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();


        public ActionResult Index()
        {

                 
            //ViewData["SubTitle"] = "Alps Electric (Malaysia) Sdn Bhd";
            //ViewData["Message"] = "Welcome";

            return View();
        }

        public ActionResult Minor()
        {
            ViewData["SubTitle"] = "Simple example of second view";
            ViewData["Message"] = "Data are passing to view by ViewData from controller";

            return View();
        }

    
        [ChildActionOnly]
        public ActionResult _AlpsMenu()
        {

            string currentUserId = User.Identity.GetUserId();
         
            List<UserServices> model = new List<UserServices>();
            var innerJoinQuery = (from pro in db.Services
                                  join sup in db.ApplicationUserService
                                  on pro.Id equals sup.ServiceId
                                  where sup.UserId == currentUserId
                                  select new
                                  {
                                      Id = pro.Id,
                                      NameOption = pro.NameOption,
                                      ParentId = pro.ParentId,
                                      ImageClass = pro.ImageClass,
                                      IsParent = pro.IsParent,
                                      Action = pro.Action,
                                      IsActive = pro.IsActive,
                                      Controller = pro.Controller,
                                      Seqeunce = pro.Sequence
                                  }).ToList();

            foreach (var item in innerJoinQuery)
            {
                model.Add(new UserServices()
                {
                    Id = item.Id,
                    NameOption = item.NameOption,
                    ParentId = item.ParentId,
                    ImageClass = item.ImageClass,
                    IsParent = item.IsParent,
                    Action = item.Action,
                    IsActive = item.IsActive,
                    Controller = item.Controller,
                    Sequence = item.Seqeunce
                });
            }
            return PartialView(model);

        }


        public ActionResult _Simple()
        {
            List<Service> all = new List<Service>();
            {
                all = db.Services.OrderBy(a => a.ParentId).ToList();
            }
            return PartialView(all);
        }

        public ActionResult _LeftPartialView()
        {
            List<Service> all = new List<Service>();
            {
                all = db.Services.OrderBy(a => a.ParentId).ToList();
            }
            return PartialView(all);
        }



        public ActionResult _Treemenu()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Simple()
        {
            List<Service> all = new List<Service>();
            Infrastructures.Data.ApplicationDbContext _db = new Infrastructures.Data.ApplicationDbContext();

            {
                all = _db.Services.Where(a => a.ParentId.Equals(0)).ToList();
            }

            return View("Simple", all);
        }

        [ChildActionOnly]
        public ActionResult _TreeView()
        {
            List<Service> all = new List<Service>();
            Infrastructures.Data.ApplicationDbContext _db = new Infrastructures.Data.ApplicationDbContext();

            {
                all = _db.Services.Where(a => a.ParentId.Equals(0)).ToList();
            }

            return PartialView("_TreeView",all);
        }

      

        public JsonResult GetSubMenu(string pid)
        {
            // this action for Get Sub Menus from database and return as json data
            System.Threading.Thread.Sleep(50);
            List<Service> subMenus = new List<Service>();
            int pID = 0;
            int.TryParse(pid, out pID);
            Infrastructures.Data.ApplicationDbContext _db = new Infrastructures.Data.ApplicationDbContext();
            {
                subMenus = _db.Services.Where(a => a.ParentId.Equals(pID)).OrderBy(a => a.NameOption).ToList();
            }

            return new JsonResult { Data = subMenus, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       

        [ChildActionOnly]
        public ActionResult Test(string parentEls)
        {
            ViewBag.Chk = parentEls;
            return PartialView();
        }

    }
}