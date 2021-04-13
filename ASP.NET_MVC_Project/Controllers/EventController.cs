using EmailTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailTestApp.DBA;
using System.Threading.Tasks;
using System.Net.Mail;

namespace EmailTestApp.Controllers
{
    public class EventController : Controller
    {
        // GET: Event
        public ActionResult Event()
        {
            // Get the connection to the database
            ObjectFactory theData = new ObjectFactory("DefaultConnection");

            // Check to see if user is logged in
            if (User.Identity.IsAuthenticated)
            {
                // Checks to see if the user is an admin
                if (MemberDB.isAdmin(User.Identity.Name))
                {
                    return View(theData.TheEvents);
                }
                else
                {
                    // Go to Leagues page if the user is not an admin
                    return RedirectToAction("MyLeagues", "Member");
                }
            }
            else
            {   // Go to login view if the user is not logged in 
                return RedirectToAction("Login", "MemberAccess");
            }
        }
        public ActionResult AllEvents()
        {
            // Gets the connection to the database
            ObjectFactory theData = new ObjectFactory("DefaultConnection");

            // Clear the dictionary of events
            theData.TheEvents.Clear();

            // Get events that are happening in one month or sooner
            List<EventModel> currentEvents = DBA.EventDB.getCurrentEvents(0);
            int index = 0;
            foreach(EventModel e in currentEvents)
            {
                theData.TheEvents.Add(index, e);
                index++;
            }
            return View(theData.TheEvents);
        }

        // GET: Event/Create
        public ActionResult EventCreate(string league)
        {
            if (league != null)
            {
                EventModel eve = new EventModel();
                eve.LeagueRef = league;
                return View(eve);
            }
            else 
            {
                return View();
            }
            
        }

        // POST: Event/Create
        [HttpPost]
        public ActionResult EventCreate(EventModel newEvent)
        {
            if (ModelState.IsValid)
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                theData.CreateEvent(newEvent, "DefaultConnection");

                if (User.Identity.IsAuthenticated && LeagueDB.isCommissioner(User.Identity.Name, newEvent.LeagueRef))
                {
                    return RedirectToAction("LeagueDetails", "League", new { id = newEvent.LeagueRef });
                }
                else
                {
                    return RedirectToAction("Event");
                }
            }
            else
            {
                return View(newEvent);
            }
        }

         [HttpPost]
        public ActionResult EventUpdate(EventModel newEvent)
        {
            if (ModelState.IsValid)
            {
                ObjectFactory theData = new ObjectFactory("DefaultConnection");
                theData.EditEvent(newEvent, "DefaultConnection");

                return RedirectToAction("Event");
            }
            else
            {
                return View(newEvent);
            }
        }

        public ActionResult EventUpdate(string league, string date)
        {
            EventModel newEvent = EventDB.getEvent(date, league);

            if (newEvent == null)
            {
                return HttpNotFound();
            }

            return View(newEvent);
        }

        // GET: Event/Delete/5
        public ActionResult DeleteEvent(string league, string date)
        {
            EventModel newEvent = EventDB.getEvent(date, league);
            if(User.Identity.IsAuthenticated && 
                (LeagueDB.isCommissioner(User.Identity.Name, league) || 
                MemberDB.isAdmin(User.Identity.Name)))
            {
                return View(newEvent);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Event/Delete/5
        [HttpPost]
        public ActionResult DeleteEvent(EventModel eve)
        {
            try
            {
                // TODO: Add delete logic here
                if (ModelState.IsValid)
                {
                    EventDB.deleteEvent(eve);
                    if (MemberDB.isAdmin(User.Identity.Name))
                    {
                        return RedirectToAction("Event");
                    }
                    else
                    {
                        return RedirectToAction("LeagueDetails", "League", new { id = eve.LeagueRef });
                    }
                }
                else
                {
                    return View(eve);
                }
            }
            catch
            {
                return View(eve);
            }
        }

        public ActionResult EventGroups(string league, string date)
        {
            EventModel newEvent = EventDB.getEvent(date, league);

            if (newEvent == null)
            {
                return HttpNotFound();
            }

            return View(newEvent);
        }
        
        [HttpPost]
        public ActionResult EventGroups(string league, string date, int groupKey, bool join)
        {
            string email = User.Identity.Name;

            if(join)
            {
                if(EventDB.addMemberToGroup(groupKey, email))
                {
                    return EventGroups(league, date);
                }
            }else
            {
                if(EventDB.removeMemberFromGroup(groupKey, email))
                {
                    return EventGroups(league, date);
                }
            }

            return EventGroups(league, date);
        }

        public ActionResult GroupMembers(int id)
        {
            GroupModel group = EventDB.getGroup(id);
            return View(group);
        }

        public ActionResult GroupCreate(string eName, string eDate)
        {
            GroupModel group = new GroupModel();
            group.EventName = eName;
            group.EventDate = eDate;
            return View(group);
        }

        [HttpPost]
        public ActionResult GroupCreate(GroupModel group)
        {
            if (ModelState.IsValid)
            {
                EventDB.createGroup(group);
                string league = EventDB.getEventLeague(group.EventName, group.EventDate);
                return RedirectToAction("EventGroups", new { league = league, date = group.EventDate });
            }
            else
            {
                return View(group);
            }
        }

        public ActionResult DeleteGroup(int id)
        {
            GroupModel group = EventDB.getGroup(id);
            if (group != null && User.Identity.IsAuthenticated &&
                LeagueDB.isCommissioner(User.Identity.Name,
                EventDB.getEventLeague(group.EventName, group.EventDate)))
            {
                return View(group);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult DeleteGroup(GroupModel rip)
        {
            if (ModelState.IsValid)
            {
                if (EventDB.deleteGroup(rip))
                {
                    string league = EventDB.getEventLeague(rip.EventName, rip.EventDate);
                    return RedirectToAction("EventGroups", new { league = league, date = rip.EventDate });
                }
                else
                {
                    return View(rip);
                }
            }
            else
            {
                return View(rip);
            }
        }

        public ActionResult GroupUpdate(int id)
        {
            GroupModel group = EventDB.getGroup(id);

            if (group == null || (User.Identity.IsAuthenticated && 
                !LeagueDB.isLeagueMember(EventDB.getEventLeague(group.EventName, group.EventDate), User.Identity.Name)))
            {
                return HttpNotFound();
            }

            return View(group);
        }

        [HttpPost]
        public ActionResult GroupUpdate(GroupModel g, int id)
        {
            if (ModelState.IsValid)
            {
                if (EventDB.updateGroup(g, id))
                {
                    string league = EventDB.getEventLeague(g.EventName, g.EventDate);
                    return RedirectToAction("EventDetails", new { league = league, date = g.EventDate });
                }
                else
                {
                    return View(g);
                }
            }
            else
            {
                return View(g);
            }
        }

        public ActionResult EventInvite(string name, string date, string league)
        {
            //we obtain a newEvent to make sure that there is an event tied to 
            //the passed values
            EventModel newEvent = new EventModel();
            EmailFormModel model = new EmailFormModel();
            
            //check to make sure the passed values are not null
            if (name != null && date != null && league != null)
            {
                //get the event tied to the passed values
                newEvent = EventDB.getEvent(date, league);
            }
            //Sets the FromEmail property of the model to the current user
            model.FromEmail = User.Identity.Name;
            //Sets the EventDate property of the model to the event's date
            model.EventDate = newEvent.Date;
            //Sets the LeagueRef property of the model to the event's league
            //Setting the models EventDate and LeagueRef property here allows
            //us to pass the data to the EventInvite POST method
            model.LeagueRef = newEvent.LeagueRef;
            ViewBag.leagueRef = newEvent.LeagueRef;
            if (name == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EventInvite(EmailFormModel model)
        {
            //Gets the event based on the passed values
            EventModel newEvent = EventDB.getEvent(model.EventDate, model.LeagueRef);
            if (ModelState.IsValid)
            {
                //Creates a var to hold a modified league reference that can be used with a link
                var leagueName = newEvent.LeagueRef.Replace("'", "%27");
                //This is the url that the user will click to go to the event
                var html = Request.Url.Host + "/Event/EventGroups?date=" + newEvent.Date + "&league=" + leagueName;
                //The structure of the body. 0 is the FromName, 1 is the FromEmail, and 2 is the message
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var mailMessage = new MailMessage();
                var message = "You have been invited to an event for " + newEvent.LeagueRef + 
                    " . Click the following link to view those who have joined and pick a group.";
                message += "<a href='" + html + "'>Click to view the details for this event.</a>";
                //This gets all of the recipients that were selected in the view
                //and adds them to the To section of the email
                foreach (string r in model.Recipients)
                {
                    mailMessage.To.Add(r);
                }

                mailMessage.Subject = "Event Invitation";
                mailMessage.From = new MailAddress(User.Identity.Name);
                mailMessage.Body = string.Format(body, model.FromName, model.FromEmail, message);
                //If IsBodyHtml is not true, then the link above will not work
                mailMessage.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    //This sends the mailMessage
                    await smtp.SendMailAsync(mailMessage);
                    return RedirectToAction("../Home/Sent");
                }
            }
            return View("../Member/MyLeagues");
        }
    }
}
