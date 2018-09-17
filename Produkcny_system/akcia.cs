using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace produkcny_system
{

    class akcia
    {
        public string typ;
        public string fakt;

        public akcia(string typ, string fakt)
        {
            this.typ = typ;
            this.fakt = fakt;
        }

        public string replace(Dictionary<string, string> m)
        {
            return Regex.Replace(fakt, @"\?(\w)", x => m.ContainsKey(x.Groups[1].Value) ? m[x.Groups[1].Value] : "");
        }

    }
}
