﻿using Microsoft.AspNetCore.Identity;

namespace appointments.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string Name { get; set; }
    }
}
