using KhielsSkincare.Areas.Admin.Repository;
using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace KhielsSkincare.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public AccountController(SignInManager<AppUser> signInManger, UserManager<AppUser> userManager, IEmailSender sender, IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManger;
            _userManager = userManager;
            _emailSender = sender;
            _webHostEnvironment = webHostEnvironment; 
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl=returnUrl});
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginViewModel.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid User or Pass");
            }
            return View(loginViewModel);
        }
        [HttpGet]
        public IActionResult IsLoggedIn()
        {
            bool isLoggedIn = User.Identity.IsAuthenticated;
            return Json(new { isLoggedIn });
        }
        public IActionResult Register()
        {
            return View();
        }      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser newUser = new AppUser { UserName = user.UserName, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    // Tạo mã xác nhận
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    var userId = await _userManager.GetUserIdAsync(newUser);

                    // Tạo ConfirmationLink
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, Request.Scheme);

                    // Đọc nội dung file HTML
                    var emailTemplatePath = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "EmailTemplate.html");
                    var emailTemplate = await System.IO.File.ReadAllTextAsync(emailTemplatePath);

                    // Thay thế các biến trong template
                    emailTemplate = emailTemplate.Replace("{{UserName}}", user.UserName);
                    emailTemplate = emailTemplate.Replace("{{ConfirmationLink}}", confirmationLink); // Thay thế link xác nhận

                    // Gửi email xác thực
                    await _emailSender.SendEmailAsync(newUser.Email, "Xác thực tài khoản", emailTemplate);

                    TempData["success"] = "Tạo tài khoản thành công. Hãy kiểm tra email để xác nhận.";
                    return Redirect("/home/index");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();

            
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Error", "Home"); // Trang lỗi nếu thiếu tham số
            }

            // Tìm người dùng theo userId
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error", "Home"); // Trang lỗi nếu không tìm thấy user
            }

            // Xác thực tài khoản
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["success"] = "Xác thực tài khoản thành công. Bây giờ bạn có thể đăng nhập.";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = "Xác thực tài khoản thất bại. Vui lòng thử lại.";
            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login","Account");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                        $"Nhấn vào liên kết để đặt lại mật khẩu: <a href='{resetLink}'>Link đặt lại mật khẩu</a>");

                    TempData["Message"] = "Email đặt lại mật khẩu đã được gửi.";
                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                ModelState.AddModelError(string.Empty, "Email không tồn tại hoặc chưa được xác nhận.");
            }
            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Hiển thị trang đặt lại mật khẩu
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Thực hiện đặt lại mật khẩu
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    // Kiểm tra nếu token không hợp lệ hoặc đã hết hạn
                    if (result.Succeeded)
                    {
                        TempData["Message"] = "Đặt lại mật khẩu thành công.";
                        return RedirectToAction("Login");
                    }
                    if (result.Errors.Any(e => e.Code == "InvalidToken"))
                    {
                        ModelState.AddModelError("", "Yêu không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu đặt lại mật khẩu một lần nữa.");
                        return RedirectToAction("ForgotPassword");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email không tồn tại.");
                }
            }
            return View(model);
        }
    }
}