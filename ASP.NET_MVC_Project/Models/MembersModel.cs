using EmailTestApp.Helpers;
using EmailTestApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml.Serialization;
//this line was changed test line only
namespace EmailTestApp.Models
{
    public class MembersModel
    {
        private string firstName;
        private string lastName;
        private string memberEmail;
        private string passwordHash;
        private string confirmPass;

        public List<LeagueModel> joinedLeagues = new List<LeagueModel>();
        public List<LeagueModel> createdLeagues = new List<LeagueModel>();

        #region Properties

        [Required]
        [DisplayName("First Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength50", ErrorMessageResourceType = typeof(Validation))]
        public string FirstName { get { return firstName; } set { firstName = value; } }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength50", ErrorMessageResourceType = typeof(Validation))]
        public string LastName { get { return lastName; } set { lastName = value; } }

        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        [StringLength(50, ErrorMessageResourceName = "StringLength50", ErrorMessageResourceType = typeof(Validation))]
        public string MemberEmail { get { return memberEmail; } set { memberEmail = value; } }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string PasswordHash { get { return passwordHash; } set { passwordHash = value; } }

        [Required]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [CompareAttribute("PasswordHash", ErrorMessageResourceName = "PasswordMatch", ErrorMessageResourceType=typeof(Validation))]
        public string ConfirmPassword { get { return confirmPass; } set { confirmPass = value; } }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        #endregion

        #region Constructors

        public MembersModel()
        { }

        public MembersModel(string firstName, string lastName, string email, string passwordHash)
        {

            FirstName = firstName;
            LastName = lastName;
            MemberEmail = email;
            PasswordHash = passwordHash;
            createdLeagues = DBA.LeagueDB.getCreatedLeagues(email);
            joinedLeagues = DBA.LeagueDB.getJoinedLeagues(email);

        }
        #endregion

        #region Overrides

        public override string ToString()
        {
            return FirstName + " " + LastName + ". Contact eMail: " + MemberEmail;
        }

        public override bool Equals(object obj)
        {
            if (obj is MembersModel)
            {
                MembersModel mem = (MembersModel)obj;
                return (this.MemberEmail.Equals(mem.memberEmail));
            }
            return false;
        }
        #endregion

        public MailAddress getMailAddress()
        {
            return new MailAddress(memberEmail);
        }
    }
}