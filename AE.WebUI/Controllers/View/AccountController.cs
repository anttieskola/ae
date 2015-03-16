using AE.Users;
using AE.Users.Entity;
using AE.WebUI.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AE.WebUI.Controllers.View
{
    /// <summary>
    /// This controller handles login, logout and all account management
    /// </summary>
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
        /// Login - get
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
        /// Login - post
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
                    if (returnUrl != null)
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Details");
                case SignInStatus.LockedOut:
                    // Todo: We missing view when account is locked, as we don't use the feature atm.
                    return View("Logout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(model);
            }
        }

        /// <summary>
        /// Logout - post
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
        /// Details view - get, there is no post
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Details(string message = "")
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
        /// Change password - get view§
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        /// <summary>
        /// Change password - post
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data");
                return View(model);
            }

            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                ModelState.AddModelError("", "Password's do not match");
                return View(model);
            }

            // silly thing but lets relog to see if password is ok
            var login = await SignInManager.PasswordSignInAsync(User.Identity.GetUserName(), model.Password, false, true);
            switch (login)
            {
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    ModelState.AddModelError("", "Verification missing");
                    return View(model);
                case SignInStatus.Failure:
                    ModelState.AddModelError("", "Invalid password");
                    return View(model);
                case SignInStatus.Success:
                    // request token for password change from manager
                    string token = await UserManager.GeneratePasswordResetTokenAsync(User.Identity.GetUserId());
                    // change it, via reset
                    var res = await UserManager.ResetPasswordAsync(User.Identity.GetUserId(), token, model.NewPassword);
                    if (res.Succeeded)
                    {
                        return RedirectToAction("Details", new { message = "Success" });
                    }
                    break;
            }
            return RedirectToAction("Message", new { message = "Failure" });
        }

        /// <summary>
        /// Forgot password - get view
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword(string message = null)
        {
            return View(new ForgotPasswordViewModel { Message = message });
        }

        /// <summary>
        /// Forgot password - post form submit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            // check model
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Message", new { message = "Failure" });
            }
            // check user exist
            AspNetUser user = await UserManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return RedirectToAction("Message", new { message = "Failure" });
            }
            // create token, link and msg
            string token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            string link = Url.Action("ResetPassword", "Account", new { UserId = user.Id, token = token }, protocol: Request.Url.Scheme);
            string msg = String.Format("Please reset your password from clicking the following <a href=\"{0}\">link</a>", link);
            // send email
            await UserManager.SendEmailAsync(user.Id, "Password reset", msg);
            // success
            return RedirectToAction("Message", new { message = "Email send" });
        }

        /// <summary>
        /// Reset password - get thru link (email) view
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string token)
        {
            // check id and token are set
            if (userId == null || token == null)
            {
                return RedirectToAction("Message", new { message = "Failure" });
            }
            // check user exist
            AspNetUser user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Message", new { message = "Failure" });
            }
            // view to reset
            return View(new ResetPasswordViewModel { UserId = userId, Token = token});

        }

        /// <summary>
        /// Reset password - post from view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // check model
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data");
                return View(model);
            }

            // check that passwords match
            if (model.NewPassword != model.NewPasswordConfirmation)
            {
                ModelState.AddModelError("", "Password's do not match");
                return View(model);
            }

            // reset password
            var res = await UserManager.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);
            if (!res.Succeeded)
            {
                return RedirectToAction("Message", new { message = "Failure" });
            }
            return RedirectToAction("Message", new { message = "Success" });
        }

        /// <summary>
        /// Message - get helper view just to display message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Message(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}
