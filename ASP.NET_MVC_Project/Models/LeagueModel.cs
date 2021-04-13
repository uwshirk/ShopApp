using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using EmailTestApp.Resources;

namespace EmailTestApp.Models
{
    [XmlRoot("Leagues"), XmlType("League")]
    public class LeagueModel
    {
        private string leagueName;
        private string commissioner;
        private string description;

        public List<MembersModel> Members;
        public List<EventModel> Events;

        #region Properties
        [Required]
        [DisplayName("League Name")]
        [StringLength(80, ErrorMessageResourceName="StringLength80", ErrorMessageResourceType= typeof(Validation))]
        public string LeagueName { get { return leagueName; } set { leagueName = value; } }

        [Required]
        [DisplayName("Commissioner Email")]
        [EmailAddress]
        [StringLength(50, ErrorMessageResourceName = "StringLength50", ErrorMessageResourceType = typeof(Validation))]
        public string Commissioner { get { return commissioner; } set { commissioner = value; } }

        [Required]
        [StringLength(150, ErrorMessageResourceName = "StringLength150", ErrorMessageResourceType = typeof(Validation))]
        public string Description { get { return description; } set { description = value; } }
        #endregion

        #region contstructors
        public LeagueModel() { }

        public LeagueModel(string leagueName, string commissioner, string desc)
        {
            this.leagueName = leagueName;
            this.commissioner = commissioner;
            description = desc;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return LeagueName + " " + Description + ". Contact: " + Commissioner;
        }
		#endregion
    }
}