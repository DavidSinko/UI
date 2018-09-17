using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace produkcny_system
{
    class pravidlo
    {
        public List<podmienka> podmienky;
        public String nazov;
        public List<akcia> akcie;

        public pravidlo(string s)
        {
            nazov = s;
            podmienky = new List<podmienka>();
            akcie = new List<akcia>();
        }
    }
}
