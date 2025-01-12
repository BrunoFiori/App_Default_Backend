﻿using System.ComponentModel.DataAnnotations;

namespace App_Default_Backend.Core.Models
{
    public class InputUser : LoginUser
    {        
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}