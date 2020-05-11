using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class LoginViewModel
    {
        [Required]
        private String username;
        [Required]
        [DataType(DataType.Password)]
        private String password;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }

        public LoginViewModel(String username, String password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
