using AE.Users;
using AE.Users.Entity;
using AE.WebUI.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    [Authorize]
    public class AccountController : Controller
    {
        #region constructor
        public AccountController()
        {
            // noop
        }
        #endregion

        #region manager helpers
        private ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        #endregion

        /// <summary>
        /// redirect helper
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Index(string message = "")
        {
            AspNetUser user = UserManager.FindById(User.Identity.GetUserId());
            return View(new AccountViewModel
            {
                Message = message,
                Username = user.UserName,
                Email = user.Email
            });
        }

        /// <summary>
        /// login - get
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// login - post
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        /// <summary>
        /// logout - post
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// ChangePassword - get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// ChangePassword - post
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data.");
                return View(model);
            }

            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                ModelState.AddModelError("", "Password's do not match.");
                return View(model);
            }

            // silly thing but lets relog to see if password is ok
            var login = await SignInManager.PasswordSignInAsync(User.Identity.GetUserName(), model.Password, false, true);
            switch (login)
            {
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    ModelState.AddModelError("", "Verification missing.");
                    return View(model);
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Invalid password.");
                    return View(model);
                case SignInStatus.Success:
                    // request token for password change from manager
                    string token = await UserManager.GeneratePasswordResetTokenAsync(User.Identity.GetUserId());
                    // change it, via reset
                    var res = await UserManager.ResetPasswordAsync(User.Identity.GetUserId(), token, model.NewPassword);
                    if (res.Succeeded)
                    {
                        return RedirectToAction("Index", new { message = "Success." });
                    }
                    break;
            }
            return RedirectToAction("Index", new { message = "Failure." });
        }
    }
}