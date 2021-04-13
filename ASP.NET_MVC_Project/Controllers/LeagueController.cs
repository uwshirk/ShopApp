using EmailTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailTestApp.DBA;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailTestApp.Controllers
{
    public class LeagueController : Controller
    {
        // GET: League
        public ActionResult League()
        {
            ObjectFactory theData = new ObjectFactory("DefaultConnection");
            if (User.Identity.IsAuthenticated)
            {
                if (MemberDB.isAdmin(User.Identity.Name))
                {
                    return View(theData.TheLeagues);
                }
                else
                {
                    return RedirectToAction("MyLeagues", "Member");
                }
            }
            else
            {
                return RedirectToAction("Login", "MemberAccess");
            }

        }

        public ActionResult AllLeagues()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyLeagues", "Member");
            }
            else
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                return View(theData.TheLeagues);
            }

        }
        [HttpPost]
        public ActionResult AllLeagues(string league, bool join)
        {
            string email = User.Identity.Name;

            if (join)
            {
                if (LeagueDB.addMemberToLeague(email, league))
                {
                    return AllLeagues();
                }
            }
            else
            {
                if (LeagueDB.removeLeagueMember(email, league))
                {
                    return AllLeagues();
                }
            }

            return AllLeagues();
        }

        // GET: League/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: League/Create
        public ActionResult LeagueCreate()
        {
            if (User.Identity.IsAuthenticated)
            {
                LeagueModel league = new LeagueModel();
                league.Commissioner = User.Identity.Name;
                return View(league);
            }
            else
            {
                return View();
            }
        }

        // POST: League/Create
        [HttpPost]
        public ActionResult LeagueCreate(LeagueModel newLeague)
        {
            if (ModelState.IsValid)
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                theData.CreateLeague(newLeague, "DefaultConnection");

                return RedirectToAction("League");
            }
            else
            {
                return View(newLeague);
            }
        }

        [HttpPost]
        public ActionResult LeagueUpdate(LeagueModel league)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    ObjectFactory theData = new ObjectFactory("DefaultConnection");
                    theData.EditLeague(league, "DefaultConnection");

                    if (User.Identity.IsAuthenticated && LeagueDB.isCommissioner(User.Identity.Name, league.LeagueName))
                    {
                        return RedirectToAction("MyLeagues", "Member");
                    }
                    else
                    {
                        return RedirectToAction("League");
                    }
                }
                else
                {
                    return View(league);
                }
            }
            else
            {
                return RedirectToAction("Login", "MemberAccess");
            }

        }

        // GET: Member/Edit/5

        public ActionResult LeagueUpdate(object id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LeagueModel league = new LeagueModel();
                int num;
                if (Int32.TryParse(id.ToString(), out num))
                {
                    ObjectFactory theData = new ObjectFactory("DefaultConnection");
                    league = theData.TheLeagues[Convert.ToInt32(id)];
                }
                else if (id is string)
                {
                    league = LeagueDB.getLeague((string)id);
                }

                if (league == null)
                {
                    return HttpNotFound();
                }

                return View(league);
            }
            else
            {
                return RedirectToAction("Login", "MemberAccess");
            }

        }

        // GET: League/Delete/5
        public ActionResult DeleteLeague(string id)
        {
            LeagueModel league = LeagueDB.getLeague(id);
            return View(league);
        }

        // POST: League/Delete/5
        [HttpPost]
        public ActionResult DeleteLeague(LeagueModel gone)
        {
            try
            {
                // TODO: Add delete logic here
                if (ModelState.IsValid && User.Identity.IsAuthenticated
                    && (LeagueDB.isCommissioner(User.Identity.Name, gone.LeagueName)
                    || MemberDB.isAdmin(User.Identity.Name)))
                {
                    if (LeagueDB.deleteLeague(gone))
                    {
                        return RedirectToAction("AllLeagues");
                    }
                    else
                    {
                        return View(gone);
                    }
                }
                else
                {
                    return View(gone);
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult LeagueDetails(object id)
        {
            LeagueModel league = new LeagueModel();
            int num;
            if (Int32.TryParse(id.ToString(), out num))
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                league = theData.TheLeagues[Convert.ToInt32(id)];
            }
            else if (id is string)
            {
                league = LeagueDB.getLeague((string)id);
            }

            if (league.LeagueName == null)
            {
                return HttpNotFound();
            }

            return View(league);
        }

        public ActionResult MemberInvite(string id)
        {
            LeagueModel newLeague = new LeagueModel();
            EmailFormModel model = new EmailFormModel();
            if(id != null)
            {
                newLeague = LeagueDB.getLeague(id);
            }
            model.LeagueRef = newLeague.LeagueName;
            model.FromEmail = User.Identity.Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MemberInvite(EmailFormModel model)
        {
            LeagueModel newLeague = LeagueDB.getLeague(model.LeagueRef);
            if (ModelState.IsValid)
            {
                var leagueName = newLeague.LeagueName.Replace("'", "%27");
                var html = Request.Url.Host + "/League/LeagueDetails/" + leagueName;
                var smtp = new SmtpClient();
                var message = new MailMessage();
                message.Subject = "League Invite";
                message.From = new MailAddress(User.Identity.Name);
                message.IsBodyHtml = true;

                foreach (string r in model.Recipients)
                {
                    
                    MembersModel recipient = MemberDB.getMember(r);
                    if(recipient != null)
                    {
                        message.To.Add(r);
                        var body = "Dear {0}, \nYou have been invited to join {1}. Click the following link to view " +
                        "the league. <a href='" + html + "'>Details</a>";
                        message.Body = string.Format(body, recipient.FirstName, model.LeagueRef);
                        await smtp.SendMailAsync(message);
                    }                    
                }               
                return RedirectToAction("../Home/Sent");
            }
            return View("../Member/MyLeagues");
        }
    }
}
