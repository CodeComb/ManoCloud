using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mano.Models;

namespace Mano.Controllers
{
    public class AccountController : BaseController
    {
        public override void Prepare()
        {
            base.Prepare();
            Cookies["ASPNET_TEMPLATE"] = "Default";
            var Config = Resolver.GetRequiredService<IConfiguration>();
            if (Request.Host.ToString() != Config["Host"])
                Response.Redirect("//" + Config["Host"]);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool remember, [FromHeader] string Referer)
        {
            if (username.Contains('@'))
                username = DB.Users.SingleOrDefault(x => x.Email == username)?.UserName ?? "";
            var result = await SignInManager.PasswordSignInAsync(username, password, remember, false);
            if (result.Succeeded)
                return Redirect(Referer ?? Url.Action("Index", "Home"));
            else
                return Prompt(x =>
                {
                    x.Title = "登录失败";
                    x.Details = "请检查用户名密码是否正确后返回上一页重试！";
                    x.RedirectText = "忘记密码";
                    x.RedirectUrl = Url.Action("Index", "Home");
                    x.StatusCode = 403;
                });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, [FromServices] IConfiguration Config)
        {
            // 判断该邮箱是否已经被注册
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
                });

            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            //var url = Url.Link("default", new { action = "RegisterDetail", controller = "Account", key = aes_email });
            var url = $"http://{Config["Host"]}/Account/RegisterDetail?key={WebUtility.UrlEncode(aes_email)}";
            await Mail.SendEmailAsync(email, "Mano Cloud 新用户注册验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""{url}"">点击继续注册</a></p>
            </body>
            </html>");

            return Prompt(x =>
            {
                x.Title = "请验证您的邮箱";
                x.Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成注册操作";
                x.RedirectText = "进入邮箱";
                x.RedirectUrl = "http://mail." + email.Split('@')[1];
            });
        }

        [HttpGet]
        public IActionResult RegisterDetail(string key)
        {
            // 此时仍然需要检测一遍邮箱是否被注册
            var email = Aes.Decrypt(key);
            ViewBag.Key = key;
            ViewBag.Email = email;
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
                });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDetail(string key, string username, string password)
        {
            // 此时仍然需要检测一遍邮箱是否被注册
            var email = Aes.Decrypt(key);
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
                });
            var user = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                RegisteryTime = DateTime.Now
            };
            var result = await UserManager.CreateAsync(user, password);
            await UserManager.AddToRoleAsync(user, "Member");
            await UserManager.AddClaimAsync(user, new System.Security.Claims.Claim("编辑个人资料", user.Id.ToString()));
            if (result.Succeeded)
                return Prompt(x =>
                {
                    x.Title = "注册成功";
                    x.Details = "现在您可以使用这个帐号登录Mano Cloud了！";
                    x.RedirectText = "现在登录";
                    x.RedirectUrl = Url.Action("Login", "Account");
                });
            else
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = result.Errors.First().Description;
                    x.StatusCode = 400;
                });
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(string email, [FromServices] IConfiguration Config)
        {
            // 判断该邮箱是否已经存在 不存在不能密码重置
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });

            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            var url = $"http://{Config["Host"]}/Account/ForgotDetail?key={WebUtility.UrlEncode(aes_email)}";
            await Mail.SendEmailAsync(email, "Mano Cloud  密码找回验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""{url}"">点击继续完成找回密码</a></p>
            </body>
            </html>");

            return Prompt(x =>
            {
                x.Title = "请验证您的邮箱";
                x.Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成密码找回操作";
                x.RedirectText = "进入邮箱";
                x.RedirectUrl = "http://mail." + email.Split('@')[1];
            });
        }

        [HttpGet]
        public IActionResult ForgotDetail(string key)
        {
            // 判断该邮箱是否已经存在 不存在不能密码重置
            var email = Aes.Decrypt(key);
            ViewBag.Key = key;
            ViewBag.Email = email;
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });
            return View(DB.Users.Single(x => x.Email == email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotDetail(string key, string password, string confirm)
        {
            if (password != confirm)
            {
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"两次输入密码不一致！";
                    x.StatusCode = 400;
                });
            }
            // 判断该邮箱是否已经存在 不存在不能密码重置
            var email = Aes.Decrypt(key);
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "密码重置失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });
            var user = await UserManager.FindByEmailAsync(email);
            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await UserManager.ResetPasswordAsync(user, resetToken, password);
            if (result.Succeeded)
                return Prompt(x =>
                {
                    x.Title = "密码重置成功";
                    x.Details = "现在您可以使用这个帐号登录Mano Cloud了！";
                    x.RedirectText = "现在登录";
                    x.RedirectUrl = Url.Action("Login", "Account");
                });
            else
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = result.Errors.First().Description;
                    x.StatusCode = 400;
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Prompt(x =>
            {
                x.Title = "您已注销";
                x.Details = "您已成功注销了登录状态。";
                x.RedirectText = "重新登录";
                x.RedirectUrl = Url.Action("Login", "Account");
                x.HideBack = true;
            });
        }

        [Route("/Profile/{id:long}")]
        public IActionResult Index(long id)
        {
            var user = DB.Users
                .Include(x => x.Domains)
                .Include(x => x.Emails)
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .Include(x => x.Certifications)
                .Include(x => x.Educations)
                .Include(x => x.Projects)
                .ThenInclude(x => x.Commits)
                .ThenInclude(x => x.Changes)
                .ThenInclude(x => x.Commit)
                .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到该用户";
                    x.Details = "没有找到指定的用户，或该用户设置了访问权限";
                    x.StatusCode = 404;
                });
            return View(user);
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Profile(long id)
        {
            var user = DB.Users
                .Include(x => x.Domains)
                .Include(x => x.Emails)
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .Include(x => x.Certifications)
                .Include(x => x.Educations)
                .Include(x => x.Projects)
                .ThenInclude(x => x.Commits)
                .ThenInclude(x => x.Changes)
                .ThenInclude(x => x.Commit)
                .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return Prompt(x => 
                {
                    x.Title = "没有找到该用户";
                    x.Details = "没有找到指定的用户，或该用户设置了访问权限";
                    x.StatusCode = 404;
                });
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Profile(long id, string city, string province,string address, Sex sex, string prcid, string qq, string wechat, DateTime? birthday, IFormFile avatar)
        {
            var user = DB.Users
                .Single(x => x.Id == id);
            user.City = city;
            user.Province = province;
            user.Address = address;
            user.Sex = sex;
            user.PRCID = prcid;
            user.QQ = qq;
            user.WeChat = wechat;
            if (birthday.HasValue)
                user.Birthday = birthday.Value;
            if (avatar != null)
            {
                if (user.AvatarId.HasValue)
                    DB.Files.Remove(user.Avatar);
                var file = new CodeComb.AspNet.Upload.Models.File
                {
                    Bytes = avatar.ReadAllBytes(),
                    ContentLength = avatar.Length,
                    Time = DateTime.Now,
                    ContentType = avatar.ContentType,
                    FileName = avatar.GetFileName()
                };
                DB.Files.Add(file);
                user.AvatarId = file.Id;
            }
            DB.SaveChanges();
            return Prompt(x => 
            {
                x.Title = "修改成功";
                x.Details = "您的个人资料已经成功修改！";
                x.RedirectText = "查看个人资料";
                x.RedirectUrl = Url.Action("Index", "Account", new { Id = id });
            });
        }

        [HttpGet]
        public IActionResult Password(long id)
        {
            var user = DB.Users
               .Include(x => x.Domains)
               .Include(x => x.Emails)
               .Include(x => x.Skills)
               .Include(x => x.Experiences)
               .Include(x => x.Certifications)
               .Include(x => x.Educations)
               .Include(x => x.Projects)
               .ThenInclude(x => x.Commits)
               .ThenInclude(x => x.Changes)
               .ThenInclude(x => x.Commit)
               .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到该用户";
                    x.Details = "没有找到指定的用户，或该用户设置了访问权限";
                    x.StatusCode = 404;
                });
            return View(user);
        }
    }
}
