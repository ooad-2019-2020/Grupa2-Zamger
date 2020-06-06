using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZamgerV2_Implementation.Models
{
    public class MasterStudentAdapter : IEkvivalentirajStudenta
    {
        public MasterStudent ekvivalentirajStudenta(MasterStudent student, int odabirDržave)
        {
            switch(odabirDržave)
            {
                case 1: //Njemačka
                    {
                        student.ProsjekNaBSC = student.ProsjekNaBSC * 1.2;
                        return student;
                    }
                case 2: //Velika Britanija
                    {
                        student.ProsjekNaBSC = student.ProsjekNaBSC * 1.25;
                        return student;
                    }
                case 3: //SAD
                    {
                        student.ProsjekNaBSC = student.ProsjekNaBSC * 1.3;
                        return student;
                    }
                default: //ako ne izabere državu, već ostane tamo ono izaberite nek to znači da je BSc završio negdje u BiH pa ne treba skalirat
                    {
                        return student;
                    }
            }
        }
    }
}
