using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ApartmentNetwork.Models;

namespace ApartmentNetwork
{
    public class Building
    {
        [Key]
        public int BuildingId {get; set; }
        [Display(Name = "Building Name (optional): ")]
        public string BuildingName {get; set; }
        [Required]
        public string AddressLine1 {get; set; }
        [Required]
        [Display(Name = "City: ")]
        public string City {get; set; }
        [Required]
        [Display(Name = "State: ")]
        public string State {get; set; }
        [Required]
        [Display(Name = "Zip Code: ")]
        public int ZipCode {get; set; }
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
        public List<User> Residents {get; set; }
    }
}