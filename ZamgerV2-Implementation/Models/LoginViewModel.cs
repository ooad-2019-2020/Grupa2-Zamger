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
        [Required(ErrorMessage = "Username je obavezan")]
        public String username { get; set; }
        [Required(ErrorMessage = "Password je obavezan")]
        [DataType(DataType.Password)]
        public String password { get; set; }
    }
}
