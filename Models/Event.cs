using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ApartmentNetwork.Models;

namespace ApartmentNetwork
{
    public class Event
    {
        [Key]
        public int EventId {get; set; }
        [Required]
        [Display(Name = "Title of Event: ")]
        public string Title {get; set; }
        [Required]
        [Display(Name = "Date and time of Event: ")]
        public DateTime EventDate {get; set; }
        [Required]
        [Display(Name = "Location of Event: ")]
        public string Location {get; set; }
        [Required]
        [Display(Name = "Event Description: ")]
        public string Description {get; set; }
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
        public int UserId {get; set; }
        public User Creator {get; set; }
    }
}