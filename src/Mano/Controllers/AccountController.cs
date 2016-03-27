using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mano.Models;

namespace Mano.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region Infrastructures
        public override void Prepare()
        {
            base.Prepare();
            Cookies["ASPNET_TEMPLATE"] = "Default";
            var Config = Resolver.GetRequiredService<IConfiguration>();
            if (Request.Cookies["ASPNET_TEMPLATE"] != "Default")
            {
                Response.Cookies.Append("ASPNET_TEMPLATE", "Default");
                Response.Redirect("//" + Config["Host"]);
            }
        }

        public static CommunityType GetCommunityType(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return CommunityType.None;
            if (url.IndexOf("github.com") >= 0)
                return CommunityType.GitHub;
            if (url.IndexOf("oschina.net") >= 0)
                return CommunityType.GitOSC;
            if (url.IndexOf("csdn.net") >= 0)
                return CommunityType.GitCSDN;
            if (url.IndexOf("codeplex.com") >= 0)
                return CommunityType.CodePlex;
            if (url.IndexOf("coding.net") >= 0)
                return CommunityType.CodingNet;
            return CommunityType.Git;
        }

        public Task Sync(User user)
        {
            return Sync(Resolver, user);
        }

        public static Task Sync(IServiceProvider services, User user)
        {
            var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = serviceScope.ServiceProvider.GetService<ManoContext>();
            return Task.Factory.StartNew(async () =>
            {
                var ret = new List<Community.Project>();
                if (!string.IsNullOrWhiteSpace(user.GitHub))
                    ret.AddRange(await Community.ProjectGetter.FromGitHub(user.GitHub));
                if (!string.IsNullOrWhiteSpace(user.CodePlex))
                    ret.AddRange(await Community.ProjectGetter.FromCodePlex(user.CodePlex));
                if (!string.IsNullOrWhiteSpace(user.GitOSC))
                    ret.AddRange(await Community.ProjectGetter.FromGitOSC(user.GitOSC));
                if (!string.IsNullOrWhiteSpace(user.GitCSDN))
                    ret.AddRange(await Community.ProjectGetter.FromCsdnCode(user.GitCSDN));
                if (!string.IsNullOrWhiteSpace(user.CodingNet))
                    ret.AddRange(await Community.ProjectGetter.FromCodingNet(user.CodingNet));
                ret = ret.DistinctBy(x => x.Git).ToList();
                var existed = user.Projects.Select(x => x.ThirdPartyUrl).ToList();
                foreach (var x in ret)
                {
                    if (!existed.Contains(x.Git))
                    {
                        db.Projects.Add(new Project
                        {
                            Title = x.Title,
                            ThirdPartyUrl = x.Git,
                            UserId = user.Id,
                            Verified = true,
                            Type = GetCommunityType(x.Git)
                        });
                    }
                }
                db.SaveChanges();
                serviceScope.Dispose();
            });
        }
        #endregion

        [HttpGet]
        [GuestOnly]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, bool remember, [FromHeader] string Referer)
        {
            if (username.Contains('@'))
                username = DB.Users.SingleOrDefault(x => x.Email == username)?.UserName ?? "";
            var result = await SignInManager.PasswordSignInAsync(username, password, remember, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Account");
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
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [GuestOnly]
        [AllowAnonymous]
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
            var url = $"http://{Config["Host"]}[controller]/RegisterDetail?key={WebUtility.UrlEncode(aes_email)}";
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
        [GuestOnly]
        [AllowAnonymous]
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
        [GuestOnly]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDetail(string key, string username, string password, [FromServices] IConfiguration Config)
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
            {
                user.EmailConfirmed = true;

                DB.Domains.Add(new Domain
                {
                    Default = true,
                    DomainName = Config["SLD"].Replace("*", user.UserName),
                    Verified = true,
                    UserId = user.Id
                });

                DB.Emails.Add(new Email
                {
                    EmailAddress = user.Email,
                    UserId = user.Id,
                    Verified = true
                });

                DB.SaveChanges();

                return Prompt(x =>
                {
                    x.Title = "注册成功";
                    x.Details = "现在您可以使用这个帐号登录Mano Cloud了！";
                    x.RedirectText = "现在登录";
                    x.RedirectUrl = Url.Action("Login", "Account");
                });
            }
            else
            {

                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = result.Errors.First().Description;
                    x.StatusCode = 400;
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [GuestOnly]
        [AllowAnonymous]
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
            var url = $"http://{Config["Host"]}[controller]/ForgotDetail?key={WebUtility.UrlEncode(aes_email)}";
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
        [GuestOnly]
        [AllowAnonymous]
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
        [GuestOnly]
        [AllowAnonymous]
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

        [Route("/Profile/{id:long?}")]
        public IActionResult Index(long? id)
        {
            if (!id.HasValue)
                id = User.Current.Id;
            var user = DB.Users
                .Include(x => x.Domains)
                .Include(x => x.Emails)
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .Include(x => x.Certifications)
                .Include(x => x.Educations)
                .Include(x => x.Projects)
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
        public async Task<IActionResult> Profile(long id, string position, string phonenumber, string city, string province, string address, Sex sex, string prcid, string qq, string wechat, string name, DateTime? birthday, IFormFile avatar)
        {
            var user = DB.Users
                .Single(x => x.Id == id);
            user.PhoneNumber = phonenumber;
            user.City = city;
            user.Province = province;
            user.Address = address;
            try
            {
                var client = new System.Net.Http.HttpClient();
                var bmapJson = await client.GetAsync($"http://api.map.baidu.com/geocoder/v2/?city={city}&address={address}&output=json&ak=lrMh687iqexo2F4V9bMYLmbX");
                var bmap = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(await bmapJson.Content.ReadAsStringAsync());
                user.Lon = bmap.result.location.lng;
                user.Lat = bmap.result.location.lat;
            }
            catch { }
            user.Sex = sex;
            user.PRCID = prcid;
            user.QQ = qq;
            user.WeChat = wechat;
            user.Name = name;
            user.Position = position;
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
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
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
        public async Task<IActionResult> Password(long id, string oldpwd, string password, string confirm)
        {
            var user = DB.Users
               .Single(x => x.Id == id);
            if (password != confirm)
            {
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"两次输入密码不一致！";
                    x.StatusCode = 400;
                });
            }
            var result = await UserManager.ChangePasswordAsync(user, oldpwd, password);
            if (!result.Succeeded)
            {
                return Prompt(x =>
                {
                    x.Title = "密码修改失败";
                    x.Details = $"原始密码输入错误，请检查后重试！";
                    x.StatusCode = 400;
                });
            }
            return Prompt(x =>
            {
                x.Title = "密码修改成功";
                x.Details = $"新密码已经生效，下次请使用新密码进行登录！";
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Email(long id)
        {
            var user = DB.Users
               .Include(x => x.Domains)
               .Include(x => x.Emails)
               .Include(x => x.Skills)
               .Include(x => x.Experiences)
               .Include(x => x.Certifications)
               .Include(x => x.Educations)
               .Include(x => x.Projects)
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
        public IActionResult EmailDelete(long id, Guid eid)
        {
            var user = DB.Users
               .Include(x => x.Emails)
               .SingleOrDefault(x => x.Id == id);
            if (!user.Emails.Any(x => x.Id == eid))
                return Prompt(x =>
                {
                    x.Title = "取消绑定失败";
                    x.Details = "没有找到这个电子邮箱地址";
                    x.StatusCode = 404;
                });
            var email = user.Emails.Single(x => x.Id == eid);
            if (email.EmailAddress == user.Email)
                return Prompt(x =>
                {
                    x.Title = "取消绑定失败";
                    x.Details = "您不能将主要的邮箱取消绑定！";
                });
            DB.Emails.Remove(email);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "取消绑定成功";
                x.Details = $"您已成功解除了{email.EmailAddress}与您Mano Cloud帐号的绑定！";
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult EmailPrimary(long id, Guid eid)
        {
            var user = DB.Users
               .Include(x => x.Emails)
               .SingleOrDefault(x => x.Id == id);
            if (!user.Emails.Any(x => x.Id == eid))
                return Prompt(x =>
                {
                    x.Title = "取消绑定失败";
                    x.Details = "没有找到这个电子邮箱地址";
                    x.StatusCode = 404;
                });
            var email = user.Emails.Single(x => x.Id == eid);
            if (!email.Verified)
                return Prompt(x =>
                {
                    x.Title = "设置失败";
                    x.Details = "您需要先验证该邮箱的有效性";
                    x.StatusCode = 400;
                });
            if (DB.Users.Where(x => x.Email == email.EmailAddress).Count() > 0)
                return Prompt(x =>
                {
                    x.Title = "设置失败";
                    x.Details = "已经有其他用户将该邮箱设置为主要邮箱，请您更换其他邮箱再试";
                    x.StatusCode = 400;
                });
            user.Email = email.EmailAddress;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "设置成功";
                x.Details = $"您现在可以使用{email.EmailAddress}作为登录凭据了";
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public async Task<IActionResult> EmailAdd(long id, string email, [FromServices] IConfiguration Config)
        {
            var reg = new Regex("[a-zA-Z0-9_.-]{0,}@[a-zA-Z0-9_.-]{0,}");
            if (!reg.IsMatch(email))
                return Prompt(x =>
                {
                    x.Title = "绑定失败";
                    x.Details = $"您欲绑定的电子邮箱{email}不合法！";
                    x.StatusCode = 400;
                });

            var user = DB.Users
               .Include(x => x.Emails)
               .SingleOrDefault(x => x.Id == id);
            if (user.Emails.Any(x => x.EmailAddress == email))
                return Prompt(x =>
                {
                    x.Title = "绑定失败";
                    x.Details = $"您已经绑定过邮箱{email}，请不要重复绑定！";
                    x.StatusCode = 400;
                });
            DB.Emails.Add(new Email
            {
                EmailAddress = email,
                Verified = false,
                UserId = user.Id
            });
            DB.SaveChanges();
            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            var aes_uid = Aes.Encrypt(user.Id.ToString());
            var url = $"http://{Config["Host"]}[controller]/EmailVerify?key={WebUtility.UrlEncode(aes_email)}&uid={WebUtility.UrlEncode(aes_uid)}";
            await Mail.SendEmailAsync(email, "Mano Cloud 绑定邮箱验证信", $@"<html>
                    <head></head>
                    <body>
                    <p><a href=""{url}"">点击继续绑定邮箱</a></p>
                    </body>
                </html>");
            return Prompt(x =>
            {
                x.Title = "绑定邮箱";
                x.Details = $"我们已经向电子邮箱{email}中发送了一封带有验证链接的电子邮件，请您按照电子邮件中的提示进行下一步操作";
                x.RedirectText = "进入邮箱";
                x.RedirectUrl = "http://mail." + email.Split('@')[1];
            });
        }

        public IActionResult EmailVerify(string key, string uid)
        {
            var d_uid = Convert.ToInt64(Aes.Decrypt(uid));
            var d_email = Aes.Decrypt(key);
            var eml = DB.Emails
                .Where(x => x.UserId == d_uid && x.EmailAddress == d_email && !x.Verified)
                .SingleOrDefault();
            if (eml == null)
                return Prompt(x =>
                {
                    x.Title = "绑定失败";
                    x.Details = $"没有找到与您的Mano Cloud帐号关联的电子邮箱{d_email}！";
                });
            eml.Verified = true;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "绑定成功";
                x.Details = $"电子邮箱{d_email}已经成功与您的Mano Cloud帐号绑定！";
                x.RedirectText = "返回绑定列表";
                x.RedirectUrl = Url.Action("Email", "Account", new { id = eml.UserId });
                x.HideBack = true;
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public async Task<IActionResult> EmailSend(long id, Guid eid, [FromServices] IConfiguration Config, [FromHeader]string Referer)
        {
            var user = DB.Users
               .Include(x => x.Emails)
               .SingleOrDefault(x => x.Id == id);
            var email = user.Emails.SingleOrDefault(x => x.Id == eid);
            if (email == null)
                return Prompt(x =>
                {
                    x.Title = "发送失败";
                    x.Details = "没有找到指定的电子邮箱地址";
                    x.StatusCode = 400;
                });
            // 发送激活信
            var aes_email = Aes.Encrypt(email.EmailAddress);
            var aes_uid = Aes.Encrypt(user.Id.ToString());
            var url = $"http://{Config["Host"]}[controller]/EmailVerify?key={WebUtility.UrlEncode(aes_email)}&uid={WebUtility.UrlEncode(aes_uid)}";
            await Mail.SendEmailAsync(email.EmailAddress, "Mano Cloud 绑定邮箱验证信", $@"<html>
                    <head></head>
                    <body>
                    <p><a href=""{url}"">点击继续绑定邮箱</a></p>
                    </body>
                </html>");
            return Redirect(Referer);
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Third(long id)
        {
            var user = DB.Users
               .Include(x => x.Domains)
               .Include(x => x.Emails)
               .Include(x => x.Skills)
               .Include(x => x.Experiences)
               .Include(x => x.Certifications)
               .Include(x => x.Educations)
               .Include(x => x.Projects)
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
        public IActionResult Third(long id, string github, string gitosc, string gitcsdn, string codeplex, string codingnet)
        {
            var user = DB.Users
               .SingleOrDefault(x => x.Id == id);
            user.GitHub = github;
            user.GitOSC = gitosc;
            user.GitCSDN = gitcsdn;
            user.CodePlex = codeplex;
            user.CodingNet = codingnet;
            user.LastPullTime = DateTime.Now;
            DB.SaveChanges();
            Sync(user);
            return Prompt(x =>
            {
                x.Title = "设置成功";
                x.Details = "第三方平台帐号绑定成功";
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Introduction(long id)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
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
        public IActionResult Introduction(long id, string Introduction)
        {
            var user = DB.Users
               .SingleOrDefault(x => x.Id == id);
            user.Introduction = Introduction;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "自我介绍修改成功，新的自我介绍内容将展现在您的云简历中。";
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Project(long id)
        {
            var user = DB.Users
               .Include(x => x.Domains)
               .Include(x => x.Emails)
               .Include(x => x.Skills)
               .Include(x => x.Experiences)
               .Include(x => x.Certifications)
               .Include(x => x.Educations)
               .Include(x => x.Projects)
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
        [Route("[controller]/Project/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ProjectAdd(long id, string title, string position, string thirdpartyurl, string projecturl, DateTime? begin, DateTime? end, string update, string hint)
        {
            if (update == "自动更新")
            {
                if (DB.Projects.Where(x => x.UserId == id && x.ThirdPartyUrl == thirdpartyurl).Count() > 0)
                {
                    return Prompt(x =>
                    {
                        x.Title = "添加失败";
                        x.Details = "您已经添加过这个项目了，请不要重复添加！";
                        x.StatusCode = 400;
                    });
                }
                if (string.IsNullOrEmpty(thirdpartyurl))
                {
                    return Prompt(x =>
                    {
                        x.Title = "添加失败";
                        x.Details = "请您填写Git源，Mano Cloud将根据此源Clone您项目的代码，以分析您所用技术及代码量。";
                        x.StatusCode = 400;
                    });
                }
            }
            else
            {

                if (!begin.HasValue)
                {
                    return Prompt(x =>
                    {
                        x.Title = "添加失败";
                        x.Details = "当您选择手动更新项目时，您必须填写项目开始时间。";
                        x.StatusCode = 400;
                    });
                }
            }
            var proj = new Project();
            proj.Title = title;
            proj.Position = position;
            proj.ProjectUrl = projecturl;
            proj.Size = 0;
            proj.Begin = begin ?? DateTime.Now;
            proj.End = end;
            proj.Hint = hint;
            proj.LastEditTime = DateTime.Now;
            proj.UserId = id;
            proj.ThirdPartyUrl = thirdpartyurl;
            proj.Type = GetCommunityType(thirdpartyurl);
            proj.Verified = false;
            DB.Projects.Add(proj);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = $"项目{proj.Title} 已经创建成功";
                x.HideBack = true;
                x.RedirectText = "返回项目列表";
                x.RedirectUrl = Url.Action("Project", "Account", new { id = id });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ProjectDelete(long id, Guid pid)
        {
            var project = DB.Projects.Where(x => x.Id == pid && x.UserId == id).SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "删除失败";
                    x.Details = "没有找到相关的项目";
                    x.StatusCode = 400;
                });
            DB.Projects.Remove(project);
            DB.SaveChanges();
            return RedirectToAction("Project", "Account", new { id = id });
        }

        [HttpGet]
        [Route("[controller]/Project/{id}/Edit/{pid}")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ProjectEdit(long id, Guid pid)
        {
            var user = DB.Users
               .Include(x => x.Domains)
               .Include(x => x.Emails)
               .Include(x => x.Skills)
               .Include(x => x.Experiences)
               .Include(x => x.Certifications)
               .Include(x => x.Educations)
               .Include(x => x.Projects)
               .SingleOrDefault(x => x.Id == id);
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "没有找到该用户";
                    x.Details = "没有找到指定的用户，或该用户设置了访问权限";
                    x.StatusCode = 404;
                });
            var project = DB.Projects.Where(x => x.Id == pid && x.UserId == id).SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "删除失败";
                    x.Details = "没有找到相关的项目";
                    x.StatusCode = 400;
                });
            ViewBag.Project = project;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Project/{id}/Edit/{pid}")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ProjectEdit(long id, Guid pid, string title, string position, string thirdpartyurl, string projecturl, DateTime? begin, DateTime? end, string update, string hint)
        {
            var project = DB.Projects.Where(x => x.Id == pid && x.UserId == id).Single();
            project.Title = title;
            project.ProjectUrl = projecturl;
            project.Position = position;
            project.Hint = hint;
            if (project.ThirdPartyUrl != thirdpartyurl)
            {
                project.ThirdPartyUrl = thirdpartyurl;
                project.Type = GetCommunityType(thirdpartyurl);
            }
            if (project.Type == CommunityType.None)
            {
                project.Verified = false;
                project.Begin = begin.Value;
                project.End = end;
            }
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "项目信息修改成功！";
                x.RedirectText = "返回项目列表";
                x.RedirectUrl = Url.Action("Project", "Account", new { id = id });
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Experience(long id)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
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
        [Route("[controller]/Experience/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ExperienceAdd(long id, Experience model)
        {
            model.UserId = id;
            DB.Experiences.Add(model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "添加成功";
                x.Details = $"您在{model.Company}的工作经历已经成功添加";
                x.HideBack = true;
                x.RedirectText = "返回工作经历";
                x.RedirectUrl = Url.Action("Experience", "Account", new { id = id });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult ExperienceDelete(long id, Guid eid, Experience model)
        {
            var exp = DB.Experiences
                .Single(x => x.Id == eid && x.UserId == id);
            DB.Experiences.Remove(exp);
            DB.SaveChanges();
            return Redirect(Url.Action("Experience", "Account", new { id = id }));
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Education(long id)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
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
        [Route("[controller]/Education/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult EducationAdd(long id, Education model)
        {
            model.UserId = id;
            DB.Educations.Add(model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "添加成功";
                x.Details = $"您在{model.School}的学习经历已经成功添加";
                x.HideBack = true;
                x.RedirectText = "返回教育经历";
                x.RedirectUrl = Url.Action("Education", "Account", new { id = id });
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Certification(long id)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
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
        [Route("[controller]/Certification/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult CertificationAdd(long id, Certification model, IFormFile Certification)
        {
            if (Certification == null)
            {
                return Prompt(x =>
                {
                    x.Title = "添加失败";
                    x.Details = "您必须上传证书的扫描件或照片，请返回重试！";
                });
            }
            var file = new CodeComb.AspNet.Upload.Models.File
            {
                Bytes = Certification.ReadAllBytes(),
                ContentLength = Certification.Length,
                FileName = Certification.GetFileName(),
                Time = DateTime.Now,
                ContentType = Certification.ContentType
            };
            DB.Files.Add(file);
            model.UserId = id;
            model.CertId = file.Id;
            DB.Certifications.Add(model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "添加成功";
                x.Details = $"{model.Title}已经成功添加！";
                x.HideBack = true;
                x.RedirectText = "返回我的证书";
                x.RedirectUrl = Url.Action("Certification", "Account", new { id = id });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult CertificationDelete(long id, Guid cid)
        {
            var cert = DB.Certifications
                .Where(x => x.UserId == id && x.Id == cid)
                .Single();
            DB.Certifications.Remove(cert);
            DB.SaveChanges();
            return RedirectToAction("Certification", "Account", new { id = id });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Skill(long id)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
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
        public IActionResult SkillDelete(long id, Guid sid)
        {
            var skill = DB.Skills
                .Single(x => x.Id == sid && x.UserId == id);
            DB.Skills.Remove(skill);
            DB.SaveChanges();
            return RedirectToAction("Skill", "Account", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/Skill/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult SkillAdd(long id, Skill model)
        {
            model.UserId = id;
            model.UpdateFromGit = false;
            DB.Skills.Add(model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "添加成功";
                x.Details = $"{model.Title}已经成功添加！";
                x.HideBack = true;
                x.RedirectText = "返回我的技能";
                x.RedirectUrl = Url.Action("Skill", "Account", new { id = id });
            });
        }

        [HttpGet]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult Domain(long id, [FromServices] IConfiguration Config)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .Include(x => x.Emails)
              .Include(x => x.Skills)
              .Include(x => x.Experiences)
              .Include(x => x.Certifications)
              .Include(x => x.Educations)
              .Include(x => x.Projects)
              .SingleOrDefault(x => x.Id == id);
            ViewBag.Regex = new Regex(Config["Host"].Replace("*.", "([a-zA-Z0-9-_]{0,}.|)"));
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
        [Route("[controller]/Domain/{id}/Add")]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult DomainAdd(long id, string domain, [FromServices] IConfiguration Config)
        {
            var regex = new Regex(Config["Host"].Replace("*.", "([a-zA-Z0-9-_]{0,}.|)"));
            if (regex.IsMatch(domain))
                return Prompt(x =>
                {
                    x.Title = "绑定失败";
                    x.Details = $"域名{domain}不合法，请检查后重试";
                });
            var user = DB.Users
              .Include(x => x.Domains)
              .SingleOrDefault(x => x.Id == id);
            lock (this)
            {
                if (user.Domains.Where(x => x.DomainName == domain).Count() > 0)
                {
                    return Prompt(x =>
                    {
                        x.Title = "绑定失败";
                        x.Details = $"你已经绑定过域名{domain}，请勿重复绑定！";
                    });
                }
                if (DB.Domains.Where(x => x.DomainName == domain).Count() > 0)
                {
                    return Prompt(x =>
                    {
                        x.Title = "绑定失败";
                        x.Details = $"域名{domain}已经被其他人绑定！";
                    });
                }
                DB.Domains.Add(new Domain
                {
                    Default = false,
                    DomainName = domain,
                    UserId = id
                });
                DB.SaveChanges();
            }

            return Prompt(x =>
            {
                x.Title = "绑定成功";
                x.Details = $"您已经成功绑定了域名{domain}";
                x.HideBack = true;
                x.RedirectText = "返回域名列表";
                x.RedirectUrl = Url.Action("Domain", "Account", new { id = id });
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult DomainDelete(long id, Guid did)
        {
            var domain = DB.Domains.Single(x => x.Id == did && x.UserId == id);
            DB.Domains.Remove(domain);
            DB.SaveChanges();
            return RedirectToAction("Domain", "Account", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimOrRolesAuthorize("Root, Master", "编辑个人资料")]
        public IActionResult DomainDefault(long id, Guid did)
        {
            var user = DB.Users
              .Include(x => x.Domains)
              .SingleOrDefault(x => x.Id == id);
            foreach (var x in user.Domains)
                x.Default = false;
            var domain = DB.Domains.Single(x => x.Id == did && x.UserId == id);
            domain.Default = true;
            DB.SaveChanges();
            return RedirectToAction("Domain", "Account", new { id = id });
        }
    }
}
