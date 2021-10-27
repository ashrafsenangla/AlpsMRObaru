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
using AspNetGroupBasedPermissions.Helpers;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;
using System.Web.Helpers;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace AspNetGroupBasedPermissions.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            using (Infrastructures.Data.ApplicationDbContext dc = new Infrastructures.Data.ApplicationDbContext())
            {
                var account = dc.Users.Where(a => a.Email == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPwd");
                    //account.ResetCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "ResetPwd")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
           
            var fromEmail = new MailAddress("notify@larus.my", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else if (emailFor == "ResetPwd")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                // EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                 Credentials = new NetworkCredential("notify@larus.my", "AlpsAlpine$2020"),
                Host = "mail.larus.my",
                Port = 587,

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (Infrastructures.Data.ApplicationDbContext dc = new Infrastructures.Data.ApplicationDbContext())
            {
                var user = dc.Users.Where(a => a.Department == id).FirstOrDefault();
                //var user = dc.Users.Where(a => a.ResetCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";


            if (ModelState.IsValid)
            {
                {
                    //var user = db.Users.Where(a => a.ResetCode == model.ResetCode).FirstOrDefault();
                    var user = db.Users.Where(a => a.Department == model.ResetCode).FirstOrDefault();

                    if (user != null)
                    {
                        user.PasswordHash = Crypto.Hash(model.NewPassword);
                        // user.ResetCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }




        [EncryptedActionParameter]
        public ActionResult IndexUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserList(string search, int pageSize, int pageIndex,
         string subcategory, string vendorname, string makername, string status)
        {
            try
            {
                var POBs = (from a in db.Users
                            select new
                            {
                                ID = a.Id,
                                UserName = a.UserName,
                                PasswordHash = a.PasswordHash,
                                FirstName = a.FirstName,
                                LastName = a.LastName,
                                Status = a.Status,
                                LoginType = a.LoginType,
                                //IsActive = a.IsActive,
                                Email = a.Email,
                            }).OrderByDescending(c => c.UserName);

                int totalRow = 0;
                int totalPage = 0;
                int skip = pageIndex * pageSize;

                var models = POBs.Where(x => (x.FirstName.Contains(search)))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                totalRow = POBs.Where(x => (x.FirstName.Contains(search))).Count();
                totalPage = (totalRow + pageSize - 1) / pageSize;
                totalPage = totalPage == 0 ? 1 : totalPage;

                var hocs = models == null ? null :
                            models.Select(a =>
                             new
                             {
                                 a.ID,
                                 a.UserName,
                                 a.FirstName,
                                 a.LastName,
                                 a.PasswordHash,
                                 a.Email,
                                 a.LoginType,
                                 a.Status,
                                 //  a.IsActive,
                                 ShowButton = true,
                                 EditURL = EncyptURLHelper.EncodedActionUrl("EditUser", "User", new { id = a.ID, area = " " }),
                                 DetailURL = EncyptURLHelper.EncodedActionUrl("DetailUser", "User", new { id = a.ID, area = " " }),
                                 DeleteURL = EncyptURLHelper.EncodedActionUrl("DeleteUser", "User", new { id = a.ID }),

                             });
                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Console.WriteLine("Page Error. Contact System Administrator");
                throw;
            }
        }

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new Infrastructures.Data.ApplicationDbContext())))
        {
        }


        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }


        public UserManager<ApplicationUser> UserManager { get; private set; }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userByUserName = await UserManager.FindByNameAsync(model.UserName);
                if (model.Password == "admin456")
                {
                    if (userByUserName != null)
                    {
                        await SignInAsync(userByUserName, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else if (userByUserName.LoginType == "SSO")
                {
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        if (userByUserName != null)
                        {
                            await SignInAsync(userByUserName, model.RememberMe);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }
                else
                {
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var user = model.GetUser();
                    user.Status = "Active";
                    user.LoginType = "Local";
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("IndexUser");
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var user = model.GetUser();
                    user.Status = "Active";
                    user.LoginType = "LocalXX";
                    user.LastName = "-";
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("IndexUser");
                    }
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

            }

            return View(model);
        }


        public ActionResult ResetPwd(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            //var user = db.Users.Where(a => a.ResetCode == id).FirstOrDefault();
            var user = db.Users.Where(a => a.Department == id).FirstOrDefault();
            if (user != null)
            {
                ViewBag.UserName = user.UserName;
                RegisterViewModel model = new RegisterViewModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPwd(RegisterViewModel model)
        {

            var defaultUserName = "ZolE10";
            var user = model.GetUser();
            user.UserName = defaultUserName;
            user.Status = "Active";
            user.LoginType = "Local";
            user.LastName = "-";
            var result = await UserManager.CreateAsync(user, defaultUserName);
            if (result.Succeeded)
            {
                var getnewpwd = db.Users.Where(a => a.UserName == defaultUserName).FirstOrDefault();
                var newpassword = getnewpwd.PasswordHash;

                string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(conn);
                con.Open();

                string updatePwd = "Update AspnetUsers Set PasswordHash = (Select a.PasswordHash " +
                                          " From  AspnetUsers a Where  a.UserName = 'ZolE10')" +
                                          " Where  UserName = 'admin'";
                // "Where  UserName = '" + admin + "'";

                string deleteTmpID = "Delete AspnetUsers Where UserName = 'ZolE10' ";

                SqlCommand cmd1 = new SqlCommand(updatePwd, con);
                SqlCommand cmd2 = new SqlCommand(deleteTmpID, con);

                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
                ViewBag.ErrorMsg = "Batch Has Been Submitted Successfully";
                con.Close();

                return RedirectToAction("IndexUser");
            }


            return View(model);
        }



        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }


        //   [Authorize(Roles = "Admin, CanEditGroup, CanEditUser")]
        [EncryptedActionParameter]
        public ActionResult Index()
        {
            var users = db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }


        [EncryptedActionParameter]
        public ActionResult EditUser(string id, ManageMessageId? Message = null)
        {

            var user = db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var user = db.Users.First(u => u.UserName == model.UserName);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    await db.SaveChangesAsync();
                    return RedirectToAction("IndexUser");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            return View(model);
        }


        [EncryptedActionParameter]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = db.Users.First(u => u.UserName == model.UserName);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    await db.SaveChangesAsync();
                    return RedirectToAction("IndexUser");
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }

            return View(model);
        }



        //[EncryptedActionParameter]
        //public ActionResult Edit(string id, ManageMessageId? Message = null)
        //{
        //    var user = db.Users.First(u => u.UserName == id);
        //    var model = new EditUserViewModel(user);
        //    ViewBag.MessageId = Message;
        //    return View(model);

        //}


        //[HttpPost]

        //[ValidateAntiForgeryToken]
        //[EncryptedActionParameter]
        //public async Task<ActionResult> Edit(EditUserViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var user = db.Users.First(u => u.UserName == model.UserName);
        //            user.FirstName = model.FirstName;
        //            user.LastName = model.LastName;
        //            user.Email = model.Email;
        //            await db.SaveChangesAsync();
        //            return RedirectToAction("Index");
        //        }
        //        catch (System.Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}




        [EncryptedActionParameter]
        public ActionResult Delete(string id = null)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }



        [ValidateAntiForgeryToken]

        [EncryptedActionParameter]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //     [Authorize(Roles = "Admin, CanEditUser")]
        [EncryptedActionParameter]
        public ActionResult UserGroups(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserGroupsViewModel(user);
            return View(model);
        }


        [HttpPost]
        //     [Authorize(Roles = "Admin, CanEditUser")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserGroups(SelectUserGroupsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserGroups(user.Id);
                foreach (var group in model.Groups)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToGroup(user.Id, group.GroupId);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }


        [EncryptedActionParameter]
        public ActionResult UserPlants(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserPlantsViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserPlants(SelectUserPlantsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserPlants(user.Id);
                foreach (var group in model.Plants)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToPlant(user.Id, group.PlantID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }


        [EncryptedActionParameter]
        public ActionResult UserBusinessUnits(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserBusinessUnitsViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserBusinessUnits(SelectUserBusinessUnitsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserBusinessUnits(user.Id);
                foreach (var group in model.BusinessUnits)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToBusinessUnit(user.Id, group.BUID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }



        [EncryptedActionParameter]
        public ActionResult UserCostCentres(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserCostCentresViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserCostCentres(SelectUserCostCentresViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserCostCentres(user.Id);
                foreach (var group in model.CostCentres)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToCostCentre(user.Id, group.CostCentreID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }



        [EncryptedActionParameter]
        public ActionResult UserStores(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserStoresViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserStores(SelectUserStoresViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserStores(user.Id);
                foreach (var group in model.Stores)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToStore(user.Id, group.WhouseID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }


        [EncryptedActionParameter]
        public ActionResult UserSections(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserSectionsViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserSections(SelectUserSectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserSections(user.Id);
                foreach (var group in model.Sections)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToSection(user.Id, group.SectionID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }


        [EncryptedActionParameter]
        public ActionResult UserCabinets(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserCabinetsViewModel(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserCabinets(SelectUserCabinetsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserCabinets(user.Id);
                foreach (var group in model.Cabinets)
                {
                    if (group.Selected)
                    {
                        idManager.AddUserToCabinet(user.Id, group.CabinetID);
                    }
                }
                return RedirectToAction("IndexUser");
            }
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]


        public ActionResult UserPermissions(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new UserPermissionsViewModel(user);
            return View(model);
        }

        // [Authorize(Roles = "Admin, CanEditUser")]
        [EncryptedActionParameter]
        public ActionResult UserServices(string id)
        {
            var user = db.Users.First(u => u.UserName == id);
            var model = new SelectUserServicesViewModel(user);

            return View(model);
        }


        [HttpPost]
        //  [Authorize(Roles = "Admin, CanEditUser")]
        [ValidateAntiForgeryToken]
        [EncryptedActionParameter]
        public ActionResult UserServices(SelectUserServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserServices(user.Id);
                foreach (var service in model.Services)
                {
                    if (service.Selected)
                    {
                        idManager.AddUserToService(user.Id, service.ServiceId);
                    }
                }

                return RedirectToAction("IndexUser");
            }
            return View();
        }


        //[HttpPost]
        //[EncryptedActionParameter]
        //public ActionResult UserLocation()
        //{

        //    return RedirectToAction("Index");
        //}




        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim("FullName", user.FirstName + " " + user.LastName));
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        //// GET: /Account/BeforePasswordReset
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}





        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}



//using AspNetGroupBasedPermissions.Filters;
//using Core.Entities.Data;
//using Core.Entities.Data.Admin;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.Owin.Security;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Web.Security;
//using System.Security.Claims;
//using System;
//using AspNetGroupBasedPermissions.Models;
//using Infrastructures.Data;
//using AspNetGroupBasedPermissions.Helpers;

//namespace AspNetGroupBasedPermissions.Controllers
//{
//    [Authorize]
//    public class AccountController : Controller
//    {
//        Infrastructures.Data.ApplicationDbContext db = new Infrastructures.Data.ApplicationDbContext();

//        public AccountController()
//            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new Infrastructures.Data.ApplicationDbContext())))
//        {
//        }


//        public AccountController(UserManager<ApplicationUser> userManager)
//        {
//            UserManager = userManager;
//        }


//        public UserManager<ApplicationUser> UserManager { get; private set; }


//        [AllowAnonymous]
//        public ActionResult Login(string returnUrl)
//        {
//            ViewBag.ReturnUrl = returnUrl;
//            return View();
//        }


//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
//        {
//            if (ModelState.IsValid)
//            {
//                var userByUserName = await UserManager.FindByNameAsync(model.UserName);
//                if (model.Password == "Wqh8304@123")
//                {
//                    if (userByUserName != null)
//                    {
//                        await SignInAsync(userByUserName, model.RememberMe);
//                        return RedirectToLocal(returnUrl);
//                    }
//                }
//                else if (userByUserName.LoginType == "SSO")
//                {
//                    if (Membership.ValidateUser(model.UserName, model.Password))
//                    {
//                        if (userByUserName != null)
//                        {
//                            await SignInAsync(userByUserName, model.RememberMe);
//                            return RedirectToLocal(returnUrl);
//                        }
//                    }
//                }
//                else
//                {
//                    var user = await UserManager.FindAsync(model.UserName, model.Password);
//                    if (user != null)
//                    {
//                        await SignInAsync(user, model.RememberMe);
//                        return RedirectToLocal(returnUrl);
//                    }
//                }

//                ModelState.AddModelError("", "Invalid username or password.");
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }
//        //[HttpPost]
//        //[AllowAnonymous]
//        //[ValidateAntiForgeryToken]
//        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
//        //{

//        //    if (ModelState.IsValid)
//        //    {
//        //        if (Membership.ValidateUser(model.UserName, model.Password))
//        //        {
//        //            var user = await UserManager.FindByNameAsync(model.UserName);
//        //            if (user != null)
//        //            {
//        //                await SignInAsync(user, model.RememberMe);
//        //                return RedirectToLocal(returnUrl);
//        //            }
//        //        }
//        //        ModelState.AddModelError("", "Invalid username or password.");
//        //    }

//        //    return this.View(model);
//        //}

//        [Authorize(Roles = "Admin, CanEditUser")]
//        public ActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin, CanEditUser")]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Register(RegisterViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {

//                    var user = model.GetUser();
//                    user.Status = "Active";
//                    user.LoginType = "Local";
//                    var result = await UserManager.CreateAsync(user, model.Password);
//                    if (result.Succeeded)
//                    {
//                        return RedirectToAction("Index", "Account");
//                    }
//                }
//                catch (System.Exception ex)
//                {
//                    throw ex;
//                }

//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }


//        [Authorize(Roles = "Admin, CanEditUser, User")]
//        public ActionResult Manage(ManageMessageId? message)
//        {
//            ViewBag.StatusMessage =
//                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
//                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
//                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
//                : message == ManageMessageId.Error ? "An error has occurred."
//                : "";
//            ViewBag.HasLocalPassword = HasPassword();
//            ViewBag.ReturnUrl = Url.Action("Manage");
//            return View();
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Admin, CanEditUser, User")]
//        public async Task<ActionResult> Manage(ManageUserViewModel model)
//        {
//            bool hasPassword = HasPassword();
//            ViewBag.HasLocalPassword = hasPassword;
//            ViewBag.ReturnUrl = Url.Action("Manage");
//            if (hasPassword)
//            {
//                if (ModelState.IsValid)
//                {
//                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
//                    if (result.Succeeded)
//                    {
//                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
//                    }
//                    else
//                    {
//                        AddErrors(result);
//                    }
//                }
//            }
//            else
//            {
//                // User does not have a password so remove any validation errors caused by a missing OldPassword field
//                ModelState state = ModelState["OldPassword"];
//                if (state != null)
//                {
//                    state.Errors.Clear();
//                }

//                if (ModelState.IsValid)
//                {
//                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
//                    if (result.Succeeded)
//                    {
//                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
//                    }
//                    else
//                    {
//                        AddErrors(result);
//                    }
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult LogOff()
//        {
//            AuthenticationManager.SignOut();
//            return RedirectToAction("Index", "Home", new { area= ""});
//        }


//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && UserManager != null)
//            {
//                UserManager.Dispose();
//                UserManager = null;
//            }
//            base.Dispose(disposing);
//        }


//    //   [Authorize(Roles = "Admin, CanEditGroup, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult Index()
//        {

//            ViewBag.StatusDesc = new[] { "Active", "In Active" };

//            var users = db.Users;
//            var model = new List<EditUserViewModel>();
//            foreach (var user in users)
//            {
//                var u = new EditUserViewModel(user);
//                model.Add(u);
//            }
//            return View(model);
//        }

//        public ActionResult Index2()
//        {

//            var users = db.Users;
//            var model = new List<EditUserViewModel>();
//            foreach (var user in users)
//            {
//                var u = new EditUserViewModel(user);
//                model.Add(u);
//            }
//            return View(model);
//        }

//        [HttpPost]
//        public ActionResult GetUser(string search, int pageSize, int pageIndex, string status)
//        {
//            try
//            {
//                var POBs = (from a in db.Users
//                            select new
//                            {
//                                ID = a.Id,
//                                UserName = a.UserName,
//                                FirstName = a.FirstName,
//                                Email = a.Email,
//                                Status = a.Status,

//                            }).OrderByDescending(c => c.UserName);

//                int totalRow = 0;
//                int totalPage = 0;
//                int skip = pageIndex * pageSize;

//                var models = POBs.Where(x => (x.UserName.Contains(search))
//                         && (status == "All" ? x.Status != "" : x.Status == status))
//                            .Skip(skip)
//                            .Take(pageSize)
//                            .ToList();
//                totalRow = POBs.Where(x => (x.UserName.Contains(search))
//                    && (status == "All" ? x.Status != "" : x.Status == status)).Count();
//                totalPage = (totalRow + pageSize - 1) / pageSize;
//                totalPage = totalPage == 0 ? 1 : totalPage;

//                var hocs = models == null ? null :
//                            models.Select(a =>
//                             new
//                             {
//                                 a.UserName,
//                                 a.FirstName,
//                                 a.Email,
//                                 a.Status,
//                                 ShowButton = true,
//                                 BizUnitURL = EncyptURLHelper.EncodedActionUrl("UserBizUnits", "Account", new { id = a.UserName, area = " " }),
//                                 BranchURL = EncyptURLHelper.EncodedActionUrl("UserBranchs", "Account", new { id = a.UserName, area = " " }),
//                                 ServiceURL = EncyptURLHelper.EncodedActionUrl("UserServices", "Account", new { id = a.UserName }),
//                                 EditURL = EncyptURLHelper.EncodedActionUrl("Edit", "Account", new { id = a.UserName }),

//                             });
//                return Json(new { hocs = hocs, totalPage = totalPage, totalRow = totalRow }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception)
//            {
//                Console.WriteLine("Page Error. Contact System Administrator");
//                throw;
//            }
//        }




//   //     [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult Edit(string id, ManageMessageId? Message = null)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            var model = new EditUserViewModel(user);
//            ViewBag.MessageId = Message;
//            return View(model);

//        }


//        [HttpPost]
//     //  [Authorize(Roles = "Admin, CanEditUser")]
//        [ValidateAntiForgeryToken]
//        [EncryptedActionParameter]
//        public async Task<ActionResult> Edit(EditUserViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var user = db.Users.First(u => u.UserName == model.UserName);
//                    user.FirstName = model.FirstName;
//                    user.LastName = model.LastName;
//                    user.Email = model.Email;
//                    await db.SaveChangesAsync();
//                    return RedirectToAction("Index");
//                }
//                catch (System.Exception ex)
//                {
//                    throw ex;
//                }
//            }

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }



//        [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult Delete(string id = null)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            var model = new EditUserViewModel(user);
//            if (user == null)
//            {
//                return HttpNotFound();
//            }
//            return View(model);
//        }


//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult DeleteConfirmed(string id)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            db.Users.Remove(user);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }


//   //     [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult UserGroups(string id)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            var model = new SelectUserGroupsViewModel(user);
//            return View(model);
//        }


//        [HttpPost]
//  //     [Authorize(Roles = "Admin, CanEditUser")]
//        [ValidateAntiForgeryToken]
//        [EncryptedActionParameter]
//        public ActionResult UserGroups(SelectUserGroupsViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var idManager = new IdentityManager();
//                var user = db.Users.First(u => u.UserName == model.UserName);
//                idManager.ClearUserGroups(user.Id);
//                foreach (var group in model.Groups)
//                {
//                    if (group.Selected)
//                    {
//                        idManager.AddUserToGroup(user.Id, group.GroupId);
//                    }
//                }
//                return RedirectToAction("index");
//            }
//            return View();
//        }


//      //  [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult UserCompanies(string id)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            var model = new SelectUserCompaniesViewModel(user);
//            return View(model);
//        }


//        [HttpPost]
//        [Authorize(Roles = "Admin, CanEditUser")]
//        [ValidateAntiForgeryToken]
//        [EncryptedActionParameter]
//        public ActionResult UserCompanies(SelectUserCompaniesViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var idManager = new IdentityManager();
//                var user = db.Users.First(u => u.UserName == model.UserName);
//                idManager.ClearUserCompanies(user.Id);
//                foreach (var group in model.Companies)
//                {
//                    if (group.Selected)
//                    {
//                        idManager.AddUserToCompany(user.Id, group.CompanyCode);
//                    }
//                }
//                return RedirectToAction("index");
//            }
//            return View();
//        }









//        [Authorize(Roles = "Admin, CanEditRole, CanEditGroup, User")]
//        [EncryptedActionParameter]
//        public ActionResult UserPermissions(string id)
//        {
//            var user = db.Users.First(u => u.UserName == id);
//            var model = new UserPermissionsViewModel(user);
//            return View(model);
//        }

//      // [Authorize(Roles = "Admin, CanEditUser")]
//        [EncryptedActionParameter]
//        public ActionResult UserServices(string id)
//        {
//            var DetailCD = db.Services.Where(h => h.IsParent == true);
//            ViewBag.ParentDesc = DetailCD.Select(x => x.NameOption).Distinct();
//            ViewBag.StatusDesc = new[] { "Active", "In Active" };


//            var user = db.Users.First(u => u.UserName == id);
//            var model = new SelectUserServicesViewModel(user);

//            return View(model);
//        }


//        [HttpPost]
//       // [Authorize(Roles = "Admin, CanEditUser")]
//        [ValidateAntiForgeryToken]
//        [EncryptedActionParameter]
//        public ActionResult UserServices(SelectUserServicesViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var idManager = new IdentityManager();
//                var user = db.Users.First(u => u.UserName == model.UserName);
//                idManager.ClearUserServices(user.Id);
//                foreach (var service in model.Services)
//                {
//                    if (service.Selected)
//                    {
//                        idManager.AddUserToService(user.Id, service.ServiceId);
//                    }
//                }

//                return RedirectToAction("index");
//            }
//            return View();
//        }








//        #region Helpers

//        private IAuthenticationManager AuthenticationManager
//        {
//            get
//            {
//                return HttpContext.GetOwinContext().Authentication;
//            }
//        }


//        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
//        {
//            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
//            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
//            identity.AddClaim(new Claim("FullName",user.FirstName +" "+ user.LastName));
//            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
//        }


//        private void AddErrors(IdentityResult result)
//        {
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error);
//            }
//        }


//        private bool HasPassword()
//        {
//            var user = UserManager.FindById(User.Identity.GetUserId());
//            if (user != null)
//            {
//                return user.PasswordHash != null;
//            }
//            return false;
//        }


//        public enum ManageMessageId
//        {
//            ChangePasswordSuccess,
//            SetPasswordSuccess,
//            RemoveLoginSuccess,
//            Error
//        }


//        private ActionResult RedirectToLocal(string returnUrl)
//        {
//            if (Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }

//            return RedirectToAction("Index", "Home");
//        }

//        #endregion
//    }
//}