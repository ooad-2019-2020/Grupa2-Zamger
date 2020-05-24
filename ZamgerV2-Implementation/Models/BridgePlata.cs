using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class BridgePlata : IPlataOsoblja
    {
        private IPlataOsoblja bridge;

        public BridgePlata(IPlataOsoblja bridge)
        {
            this.bridge = bridge;
        }

        public double dajPlatu()
        {
            return bridge.dajPlatu() + 1500;
        }
    }
}
