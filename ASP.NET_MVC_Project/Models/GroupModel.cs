using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EmailTestApp.Resources;

namespace EmailTestApp.Models
{
    public class GroupModel
    {
        private string groupName;
        private int score;
        private string start;
        private DateTime date;
        private string eventName;
        private int limit;

        private List<MembersModel> groupMembers;
        
        #region properties  
        [Required]
        [DisplayName("Group Name")]
        [StringLength(30, ErrorMessageResourceName = "StringLength30", ErrorMessageResourceType = typeof(Validation))]
        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }
        
        [DisplayName("Group Members")]
        public List<MembersModel> GroupMembers
        {
            set { groupMembers = value; }
            get { return groupMembers; }
        }
        
        [DisplayName("Score")]
        public int GroupScore
        {
            set { score = value; }
            get { return score; }
        }
        
        [DisplayName("Begin Time")]
        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9] [AaPp][Mm]$", ErrorMessage = "Invalid format. Example: 12:34 am")]
        public string StartTime
        {
            set { start = value; }
            get { return start; }
        }
        
        [Required]
        public String EventDate
        {
            set { date = DateTime.Parse(value); }
            get { return date.ToShortDateString(); }
        }
        
        [Required]
        [DisplayName("Event")]
        [StringLength(30, ErrorMessageResourceName = "StringLength30", ErrorMessageResourceType = typeof(Validation))]
        public string EventName
        {
            set { eventName = value; }
            get { return eventName; }
        }
        
        public int Limit 
        { 
            set { 
                limit = value;
                groupMembers = new List<MembersModel>(limit);
            }
            get { return limit; } }
        #endregion

        #region constructors
        public GroupModel() {
            groupMembers = new List<MembersModel>();
            score = 0;
            limit = 0;
        }

        public GroupModel(string gname, string eName, string eDate, string start)
        {
            groupMembers = new List<MembersModel>();
            groupName = gname;
            eventName = eName;
            EventDate = eDate;
            this.start = start;
            score = 0;
            limit = 0;
        }

        public GroupModel(string gname, string eName, string eDate, string start, int limit)
        {
            this.limit = limit;
            groupMembers = new List<MembersModel>(limit);
            groupName = gname;
            eventName = eName;
            EventDate = eDate;
            this.start = start;
            score = 0;
        }
        #endregion

        public void addMember(MembersModel me)
        {
            if (limit == 0 || groupMembers.Count < limit)
                groupMembers.Add(me);
        }

        public bool isFull()
        {
            return (limit != 0 && groupMembers.Count >= limit);
        }
    }
}