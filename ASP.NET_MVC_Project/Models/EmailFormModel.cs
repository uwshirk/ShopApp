using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

// This code is from the following URL:
// http://www.mikesdotnetting.com/article/268/how-to-send-email-in-asp-net-mvc

namespace EmailTestApp.Models
{
    public class EmailFormModel
    {
        [Required, Display(Name = "Your name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Your email"), EmailAddress]
        public string FromEmail { get; set; }
        public List<String> Recipients { get; set; }
        [Required]
        public string Message { get; set; }
        //EventDate and LeagueRef are used for passing league
        //and event details in an email
        public string EventDate { get; set; }
        public string LeagueRef { get; set; }

    }
}