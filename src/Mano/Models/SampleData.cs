using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Mano.Models
{
    public static class SampleData
    {
        public static async Task InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<ManoContext>();
            var UserManager = services.GetRequiredService<UserManager<User>>();
            var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var env = services.GetRequiredService<IApplicationEnvironment>();

            if (DB.Database.EnsureCreated())
            {
                await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Root" });
                await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Master" });
                await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Member" });
                await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Banned" });

                var user = new User
                {
                    UserName = "admin",
                    Email = "1@1234.sh"
                };

                await UserManager.CreateAsync(user, "123456");
                await UserManager.AddToRoleAsync(user, "Root");

                DB.Extensions.Add(new Extension { Id = ".js", Technology = "JavaScript", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".cs", Technology = "C#.Net", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".vb", Technology = "Visual Basic", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".fs", Technology = "F#.Net", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".java", Technology = "Java", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".sh", Technology = "Unix Shell", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".cmd", Technology = "Windows Shell", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".ps1", Technology = "Power Shell", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".html", Technology = "HTML", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".less", Technology = "LESS", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".sass", Technology = "SASS", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".ts", Technology = "Type Script", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".as", Technology = "Action Script", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".c", Technology = "C", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".h", Technology = "C", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".cpp", Technology = "C++", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".hpp", Technology = "C++", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".swift", Technology = "Swift" , Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".m", Technology = "Objective C", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".py", Technology = "Python", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".rb", Technology = "Ruby", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".json", Technology = "JSON", Type = TechnologyType.序列化格式 });
                DB.Extensions.Add(new Extension { Id = ".xml", Technology = "XML", Type = TechnologyType.序列化格式 });
                DB.Extensions.Add(new Extension { Id = ".txt", Technology = "文本文档", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".rtf", Technology = "超文本文档", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".png", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".jpg", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".bmp", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".gif", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".svg", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".tiff", Technology = "图片", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".mp3", Technology = "音频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".wav", Technology = "音频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".m4a", Technology = "音频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".wma", Technology = "音频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".aiff", Technology = "音频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".wmv", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".avi", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".flv", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".mp4", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".mov", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".wmv", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".m4v", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".rm", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".rmvb", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".mpeg", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".3gp", Technology = "视频", Type = TechnologyType.其他 });
                DB.Extensions.Add(new Extension { Id = ".asp", Technology = "Active Server Page", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".aspx", Technology = "ASP.Net (Web Form)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".jsp", Technology = "Java Server Page", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".cshtml", Technology = "ASP.Net C# (Razor)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".vbhtml", Technology = "ASP.Net VB (Razor)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".jshtml", Technology = "jFlick (Razor)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".ejs", Technology = "Express JS (EJS)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".jade", Technology = "Express JS (Jade)", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".vbs", Technology = "Visual Basic Script", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".rdata", Technology = "R Language", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".php", Technology = "PHP", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".perl", Technology = "Perl", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".asm", Technology = "Assembly Language", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".pas", Technology = "Pascal / Delphi", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".groovy", Technology = "Groovy", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".d", Technology = "D Language", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".mdb", Technology = "Access", Type = TechnologyType.数据库 });
                DB.Extensions.Add(new Extension { Id = ".mdf", Technology = "SQL Server", Type = TechnologyType.数据库 });
                DB.Extensions.Add(new Extension { Id = ".accdb", Technology = "Access 2008", Type = TechnologyType.数据库 });
                DB.Extensions.Add(new Extension { Id = ".db", Technology = "SQLite", Type = TechnologyType.数据库 });
                DB.Extensions.Add(new Extension { Id = ".sqlite", Technology = "SQLite", Type = TechnologyType.数据库 });
                DB.Extensions.Add(new Extension { Id = ".e", Technology = "易语言", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".shtml", Technology = "Server Parsed HTML", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".xcodeproj", Technology = "X Code", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".sln", Technology = "Visual Studio", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".vbp", Technology = "Visual Basic 6.0", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".xproj", Technology = "ASP.Net Core 1.0", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".xaml", Technology = "Windows Presentation Foundation", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".axml", Technology = "Android Layout", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".storyboard", Technology = "Apple Storyboard", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".jsproj", Technology = "Cordova", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".frm", Technology = "Windows Form", Type = TechnologyType.工具或框架 });
                DB.Extensions.Add(new Extension { Id = ".coffee", Technology = "Coffee Script", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".f", Technology = "Fortran", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".for", Technology = "Fortran", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".sql", Technology = "SQL Script", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".lsp", Technology = "Lisp", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".lua", Technology = "Lua", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".go", Technology = "Go", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".dart", Technology = "Dart", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".ceylon", Technology = "Ceylon", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".md", Technology = "Markdown", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".fan", Technology = "Fantom", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".hx", Technology = "Haxe", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".ml", Technology = "OCaml", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".opa", Technology = "OPA", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".chpl", Technology = "Chapel", Type = TechnologyType.编程语言 });
                DB.Extensions.Add(new Extension { Id = ".tex", Technology = "Tex", Type = TechnologyType.编程语言 });
            }
        }
    }
}
