using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailTestApp.DBA;

namespace EmailTestApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Admin()
        {
            if (User.Identity.IsAuthenticated)
            {
                // If the user is an admin, go to the admin view
                if(MemberDB.isAdmin(User.Identity.Name))
                {
                    return View();
                }
                else
                {
                    return Redirect("../Home/Index");
                }
            }
            else
            {
                //ViewBag.route = "../Member/MyLeagues";
                return Redirect("../MemberAccess/Login");
            }
        }
    }
}