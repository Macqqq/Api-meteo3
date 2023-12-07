using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api_meteo2.Service
{
    public class Ville
    {

        List<string> LSville;
        public Ville() { 
            LSville = new List<string>();
        }

        public void AddVille(string ville)
        {
            LSville.Add(ville);
        }

      
    }
}
