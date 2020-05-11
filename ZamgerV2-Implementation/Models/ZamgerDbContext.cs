using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class ZamgerDbContext : DbContext
    {
        public ZamgerDbContext(DbContextOptions<ZamgerDbContext> options): base(options)
        {

        }

        
    }
}
