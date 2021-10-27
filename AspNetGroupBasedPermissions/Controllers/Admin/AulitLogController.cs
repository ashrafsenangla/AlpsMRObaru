using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Core.Entities.Data.Admin;
using Core.Entities.Data;

namespace AspNetGroupBasedPermissions.Controllers.Admin
{
    public class AulitLogController : Controller
    {
        private Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

        // GET: /AulitLog/
        public async Task<ActionResult> Index()
        {
            return View(await db.AuditLogs.ToListAsync());
        }

        // GET: /AulitLog/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditLog auditlog = await db.AuditLogs.FindAsync(id);
            if (auditlog == null)
            {
                return HttpNotFound();
            }
            return View(auditlog);
        }

        // GET: /AulitLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AulitLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="auditlogid,userid,eventdateutc,eventtype,tablename,recordid,columnname,originalvalue,newvalue")] AuditLog auditlog)
        {
            if (ModelState.IsValid)
            {
                auditlog.auditlogid = Guid.NewGuid();
                db.AuditLogs.Add(auditlog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(auditlog);
        }

        // GET: /AulitLog/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditLog auditlog = await db.AuditLogs.FindAsync(id);
            if (auditlog == null)
            {
                return HttpNotFound();
            }
            return View(auditlog);
        }

        // POST: /AulitLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="auditlogid,userid,eventdateutc,eventtype,tablename,recordid,columnname,originalvalue,newvalue")] AuditLog auditlog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditlog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(auditlog);
        }

        // GET: /AulitLog/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuditLog auditlog = await db.AuditLogs.FindAsync(id);
            if (auditlog == null)
            {
                return HttpNotFound();
            }
            return View(auditlog);
        }

        // POST: /AulitLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            AuditLog auditlog = await db.AuditLogs.FindAsync(id);
            db.AuditLogs.Remove(auditlog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
