using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EmailTestApp.Resources;

namespace EmailTestApp.Models
{
    public class EventModel
    {
        private string eventName;
        private string leagueRef;
        private string date;
        private string startTime;
        private string endTime;
        private string location;
        private string description;

        #region Properties
        [Required]
        [DisplayName("Event Name")]
        public string EventName { get { return eventName; } set { eventName = value; } }
        
        [Required]
        [DisplayName("League")]
        public string LeagueRef { get { return leagueRef; } set { leagueRef = value; } }
        
        [Required]
        [DisplayName("Event Date")]
        //[RegularExpression(@"((0?[1-9])|(1[012]))[/-]((0?[1-9])|([12][0-9])|(3[01]))[/-]\d{4}", ErrorMessage="Valid US calender date formate (mm/dd/yyyy) expected.")]
        public string Date { get { return date; } set { date = value; } }
		
        [Required]
        [DisplayName("Start Time")]
        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9] [AaPp][Mm]$", ErrorMessage = "Invalid format. Example: 12:34 am")]
        public string StartTime { get { return startTime; } set { startTime = value; } }
        
        [Required]
        [DisplayName("End Time")]
        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9] [AaPp][Mm]$", ErrorMessage = "Invalid format. Example: 12:34 am")]
        public string EndTime { get { return endTime; } set { endTime = value; } }
        
        [Required]
        [StringLength(20, ErrorMessageResourceName = "StringLength20", ErrorMessageResourceType = typeof(Validation))]
        public string Location { get { return location; } set { location = value; } }

        [StringLength(150, ErrorMessageResourceName = "StringLength150", ErrorMessageResourceType = typeof(Validation))]
        public string Description { get { return description; } set { description = value; } }
        #endregion

        #region constructors
        public EventModel() { }

        public EventModel(string leName, string eName, string eDate, string start, string end, string loc, string desc)
        {
            eventName = eName;
            leagueRef = leName;
            date = DateTime.Parse(eDate).ToShortDateString();
            startTime = start;
            endTime = end;
            location = loc;
            description = desc;
        }
        #endregion

        #region overrides
        public override string ToString()
        {           
            return "There is an event for the " + LeagueRef + " called " + EventName + ". The event will be held from: " + StartTime + " - " + EndTime + " on " + Date + " at " + Location;
        }
        public override bool Equals(object obj)
        {
            if (obj is EventModel)
            {
                EventModel em = (EventModel)obj;
                return (this.EventName.Equals(em.EventName) && this.Date.Equals(em.Date));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
		#endregion
    }
}