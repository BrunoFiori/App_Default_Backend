﻿using System.ComponentModel.DataAnnotations;

namespace App_Default_Backend.Core.Models
{
    public class InputTodo
    {
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}