using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class StudentskaSlužba
    {
        private String username;
        private String email;
        private int idStudentske;

        public StudentskaSlužba(string username, string email, int idStudentske)
        {
            this.Username = username;
            this.Email = email;
            this.IdStudentske = idStudentske;
        }

        public string Username { get => username; set => username = value; }
        public string Email { get => email; set => email = value; }
        public int IdStudentske { get => idStudentske; set => idStudentske = value; }
    }
}
