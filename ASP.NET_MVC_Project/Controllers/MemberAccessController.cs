using EmailTestApp.Helpers;
using EmailTestApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EmailTestApp.Controllers
{
    public class MemberAccessController : Controller
    {
        // GET: MemberLogin
        public ActionResult Index()
        {
            return View();
        }

        #region Login

        public ActionResult Login()
        {
            return View();
        }

        // POST: MemberAccess/Login
        [HttpPost]
        public ActionResult Login(string memberEmail, string passwordHash, bool rememberMe)
        {
            if (ModelState.IsValid)
            {
                if (DBA.MemberDB.memberExists(memberEmail))
                {
                    // Get the member from the database
                    MembersModel member = DBA.MemberDB.getMember(memberEmail);

                    // Create variables to use in the split
                    char[] separator = new char[1];
                    separator[0] = '$';

                    var removeEmptyStrings = System.StringSplitOptions.RemoveEmptyEntries;

                    // Split the hash from the database based on $
                    string[] dbHash = member.PasswordHash.Split(separator, removeEmptyStrings);

                    // Generate the hash
                    passwordHash = PasswordManager.GeneratePasswordHash(passwordHash, dbHash[1]);

                    // Split the hash given through the login
                    string[] userHash = passwordHash.Split(separator, removeEmptyStrings);

                    // Check to see if the password hash matches
                    if (dbHash[2].Equals(userHash[2]))
                    {
                        FormsAuthentication.SetAuthCookie(member.MemberEmail, rememberMe);

                        if (DBA.LeagueDB.getCreatedLeagues(member.MemberEmail).Count + DBA.LeagueDB.getJoinedLeagues(member.MemberEmail).Count > 0)
                        {
                            return RedirectToAction("MyLeagues", "Member");
                        }
                        else
                        {
                            return RedirectToAction("AllLeagues", "League");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login data is incorrect!");
                    }
                }
                
            }
            return View();
        }

        #endregion Login

        #region Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #endregion Logout

        #region Register

        // GET: MemberAccess/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: MemberAccess/Register
        [HttpPost]
        public ActionResult Register(MembersModel member)
        {
            if (ModelState.IsValid)
            {
                //!DBA.MemberDB.memberExists(member.MemberEmail)

                // Check to see if the member is in the database
                if (!DBA.MemberDB.memberExists(member.MemberEmail))
                {
                    //DBA.MemberDB.addMember(member);

                    // Hash the password
                    member.PasswordHash = Helpers.PasswordManager.GeneratePasswordHash(member.PasswordHash, "");

                    // Add the member to the database
                    DBA.MemberDB.addMember(member);

                    // Authenticate the form
                    FormsAuthentication.SetAuthCookie(member.MemberEmail, member.RememberMe);
                    if (DBA.LeagueDB.getCreatedLeagues(member.MemberEmail).Count + DBA.LeagueDB.getJoinedLeagues(member.MemberEmail).Count > 0)
                    {
                        return RedirectToAction("MyLeagues", "Member");
                    }
                    else
                    {
                        return RedirectToAction("AllLeagues", "League");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Member already exists!");
                }
            }
            return View();

        }

        #endregion Register
    }
}
