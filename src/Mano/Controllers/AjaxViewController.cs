using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mano.Models;

namespace Mano.Controllers
{
    [Authorize]
    public class AjaxViewController : BaseController
    {
        //返回添加教育经历
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult EducationTbody(long id){
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult CertificationTbody(long id)
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult DomainTbody(long id, [FromServices] IConfiguration Config)
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult ExperienceTbody(long id)
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult ProjectTbody(long id)
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        [HttpGet]
        [AnyRolesOrClaims("Root, Master", "编辑个人资料")]
        public IActionResult SkillTbody(long id)
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
                  {
                      return Content("没有找到指定的用户，或该用户设置了访问权限");
                  }
            return View(user);
        }
        
    }
}
