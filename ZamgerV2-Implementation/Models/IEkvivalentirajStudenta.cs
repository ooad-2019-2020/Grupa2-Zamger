using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    interface IEkvivalentirajStudenta
    {
        public MasterStudent ekvivalentirajStudenta(MasterStudent student, int odabirDržave);
    }
}
