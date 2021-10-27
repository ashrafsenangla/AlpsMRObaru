using AspNetGroupBasedPermissions.Filters;
using Core.Entities.Data;
using Core.Entities.Data.Admin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Security;
using System.Security.Claims;
using System;
using AspNetGroupBasedPermissions.Models;
using Infrastructures.Data;

namespace AspNetGroupBasedPermissions.Controllers
{
   
    public class DemoController : Controller
    {
        private List<TestModel> _item;

        public List<TestModel> Item
        {
            get
            {
                _item = new List<TestModel>
        {
            new TestModel {MyProperty1=1,MyProperty2="TestProperty11",MyProperty3="TestProperty12" },
            new TestModel {MyProperty1=2,MyProperty2="TestProperty21",MyProperty3="TestProperty22" },
            new TestModel {MyProperty1=3,MyProperty2="TestProperty31",MyProperty3="TestProperty32" }
        };
                return _item;
            }
        }
        public ActionResult Index()
        {
            return View(Item);
        }
        public ActionResult GetDetail(int id)
        {
            var model = Item.Where(x => x.MyProperty1 == id).FirstOrDefault();
            return PartialView("TestPartial", model);
        }

        [HttpPost]
        public JsonResult Save(TestModel model)
        {
            return Json(new { Data = model, success = ModelState.IsValid ? true : false }, JsonRequestBehavior.AllowGet);
        }
    }
}

