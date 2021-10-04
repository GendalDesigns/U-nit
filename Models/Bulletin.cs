using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ApartmentNetwork.Models;

namespace ApartmentNetwork
{
    public class Bulletin
    {
        [Key]
        public int BulletinId {get; set; }
        [Required]
        [Display(Name = "Title: ")]
        public string Title {get; set; }
        [Required]
        [Display(Name = "Topic of Bulletin: ")]
        public string Topic {get; set; }
        [Display(Name = "Bulletin Message: ")]
        public string Content {get; set; }
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
        public int UserId {get; set; }
        public User Creator {get; set; }
    }
}