using EmailTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailTestApp.DBA;
using System.Globalization;

namespace EmailTestApp.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Member()
        {
            ObjectFactory theData = new ObjectFactory("DefaultConnection");

            // Check if user is logged in
            if (User.Identity.IsAuthenticated)
            {
                if (MemberDB.isAdmin(User.Identity.Name))
                {
                    return View(theData.TheMembers);
                }
                else
                {
                    return RedirectToAction("MyLeagues");
                }
            }
            else
            {
                return RedirectToAction("Login", "MemberAccess");
            }
        }

        // GET: Member/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Member/Create
        public ActionResult MemberCreate()
        {
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        public ActionResult MemberCreate(MembersModel newMember)
        {
            if (ModelState.IsValid)
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");

                // Hash the password
                newMember.PasswordHash = Helpers.PasswordManager.GeneratePasswordHash(newMember.PasswordHash, "");
                theData.CreateMember(newMember, "DefaultConnection");

                return RedirectToAction("Member");
            }
            else
            {
                return View(newMember);
            }
        }

        // POST: Member/Edit/5
        [HttpPost]
        public ActionResult MemberUpdate(MembersModel member)
        {
            if (ModelState.IsValid)
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                theData.EditMember(member, "DefaultConnection");

                return RedirectToAction("Member");
            }
            else
            {
                return View(member);
            }
        }

        // GET: Member/Edit/5

        public ActionResult MemberUpdate(object id)
        {
            MembersModel newMember = new MembersModel();
            int num;
            if (Int32.TryParse(id.ToString(), out num))
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                newMember = theData.TheMembers[Convert.ToInt32(id)];
            }
            else if (id is string)
            {
                newMember = MemberDB.getMember(id.ToString());
            }

            if (newMember == null)
            {
                return HttpNotFound();
            }

            return View(newMember);
        }

        // GET: Member/Delete/5
        public ActionResult DeleteMember(object id)
        {
            MembersModel deadMember = new MembersModel();
            int num;
            if (Int32.TryParse(id.ToString(), out num))
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                deadMember = theData.TheMembers[Convert.ToInt32(id)];
            }
            else if (id is string)
            {
                deadMember = MemberDB.getMember(id.ToString());
            }

            if (deadMember == null || !MemberDB.isAdmin(User.Identity.Name))
            {
                return HttpNotFound();
            }

            return View(deadMember);
        }

        // POST: Member/Delete/5
        [HttpPost]
        public ActionResult DeleteMember(MembersModel rip)
        {
            try
            {
                // TODO: Add delete logic here
                if (ModelState.IsValid && User.Identity.IsAuthenticated && 
                    MemberDB.isAdmin(User.Identity.Name))
                {
                    MemberDB.deleteMember(rip.MemberEmail);
                    return RedirectToAction("Member");
                }
                else
                {
                    return View(rip);
                }
            }
            catch
            {
                return View(rip);
            }
        }

        public ActionResult MyLeagues()
        {

            if(User.Identity.IsAuthenticated)
            {
                MembersModel me = MemberDB.getMember(User.Identity.Name);
                return View(me);
            }
            else
            {
                //ViewBag.route = "../Member/MyLeagues";
                return Redirect("../MemberAccess/Login");
            }
        }

        [HttpPost]
        public ActionResult MyLeagues(string league, bool join)
        {
            string email = User.Identity.Name;

            // Redirect based on whether the user has joined a league
            if (join)
            {
                if (LeagueDB.addMemberToLeague(email, league))
                {
                    return MyLeagues();
                }
            }
            else
            {
                if (LeagueDB.removeLeagueMember(email, league))
                {
                    return MyLeagues();
                }
            }

            return MyLeagues();
        }
    }
}
