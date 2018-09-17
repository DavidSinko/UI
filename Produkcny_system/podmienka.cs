using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace produkcny_system
{
    class podmienka
    {
        public Regex regex;
        public bool rozne;
        public string rozne1;
        public string rozne2;

        public podmienka(string s)
        {
            var m = Regex.Match(s, @"<>\s+\?(\w)\s+\?(\w)");
            if (m.Success)
            {
                rozne = true;
                rozne1 = m.Groups[1].Value;
                rozne2 = m.Groups[2].Value;
            }
            //else
            {
                var pom = Regex.Match(s, @"-");
                if(pom.Success)
                {
                    s = Regex.Replace(s, @"-", @"_");
                }
                var t = Regex.Replace(s, @"\?([\w\-]+)", @"(?<$1>\w+)"); // \?(\w), (?<$1>\w+)
                //Console.WriteLine("Vypis: " + t);
                regex = new Regex(t);
            }
        }

        public vysledok matches(string fakt)
        {
            var result = new vysledok();
            var m = regex.Match(fakt);
            if (m.Success)
            {
                result.uspech = true;
                result.mapping = new Dictionary<string, string>();
                foreach (var groupName in regex.GetGroupNames())
                {
                    if (groupName == "0") continue;
                    result.mapping[groupName] = m.Groups[groupName].Value;
                }
            }
            return result;
        }
    }
}
