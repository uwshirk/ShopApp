using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailTestApp.Models;
using System.Data.SqlClient;
using System.Configuration;
using EmailTestApp.DBA;

namespace EmailTestApp.Controllers
{
    public class HomeController : Controller
    {
        const int MONTHS_AHEAD = 1;
        private List<EventModel> events;
        private string results = "";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ContactFromLeague(object id)
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
            ViewBag.leagueName = league.LeagueName;

            if (league == null)
            {
                return HttpNotFound();
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactFromLeague(EmailFormModel model)
        //public ActionResult ContactFromLeague(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {
                var smtp = new SmtpClient();
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();

                foreach (string r in model.Recipients)
                {
                    
                    message.To.Add(r);
                }

                message.Subject = "Test";
                message.From = new MailAddress(model.FromEmail);
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                smtp.Send(message);
                return RedirectToAction("Sent");

            }
            return View(model);
        }

        public ActionResult Sent()
        {
            return View();
        }

        public ActionResult getUpcomingEvents()
        {
            events = DBA.EventDB.getCurrentEvents(MONTHS_AHEAD);

            results = buildUpcomingEventsString(events);

            return Content(results, "text/html");
        }
        [HttpPost]
         public ActionResult getUpcomingEventsByUser(String userName)
         {
            events = DBA.EventDB.getCurrentEventsByUser(MONTHS_AHEAD, userName);

            results = buildUpcomingEventsString(events);

            return Content(results, "text/html");
         }

        public String buildUpcomingEventsString(List<EventModel> eventResults)
        {
            string buildString = "";
            foreach (EventModel myEvent in eventResults)
            {
                buildString += "<p>" + myEvent.ToString() + "</p><br/>\n";
            }

            return buildString;
        }
    }
}