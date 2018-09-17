using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace produkcny_system
{
    class Program
    {
        static void Main(string[] args)
        {
            var regexPodmienka = new Regex(@"\(([\w\s\?\-<>]+)\)");
            var podmienky = new Regex(@"^ak\s+\((.*)\)", RegexOptions.IgnoreCase);
            var pravidla = new List<pravidlo>();
            pravidlo aktualne = null;

            var nazov = new Regex(@"^(.*):$");
            foreach(var s in File.ReadLines("../../pravidlaR.txt"))
            {
                var m = nazov.Match(s);
                if(m.Success)
                {
                    //Console.WriteLine("nazov: " + m.Groups[1].Value);
                    aktualne = new pravidlo(m.Groups[1].Value);
                    pravidla.Add(aktualne);
                }

                m = podmienky.Match(s);
                if(m.Success)
                {
                    var zoznam = m.Groups[1].Value;
                    //Console.WriteLine("zoznam: " + zoznam);
                    foreach(Match p in regexPodmienka.Matches(zoznam))
                    {
                        var p3 = p.Groups[1].Value;
                        //Console.WriteLine("podmienka: " + p3);

                        aktualne.podmienky.Add(new podmienka(p3));
                    }
                }

                m = Regex.Match(s, @"^potom\s+\((.*)\)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var zoznam = m.Groups[1].Value;
                    //Console.WriteLine("akcia: " + zoznam);
                    var iter = 0;
                    foreach (Match p in Regex.Matches(zoznam, @"\((\w+)\s+([\(\!\:\w\s\?\-<>]+)\)"))
                    {
                        var op = p.Groups[1].Value;

                        iter++;
                        if (iter == 12) continue;
                        if (op == "otazka")
                        {
                            //Console.WriteLine("operacia: " + p.Groups[1].Value);
                            var p4 = Regex.Match(zoznam, @"\(otazka\s+\(([\(\!\:\w\s\?<>]+)\)\(([\w\s\!]+)\)", RegexOptions.IgnoreCase);
                            var vysl = "(" + p4.Groups[1].Value + ")(" + p4.Groups[2].Value + ")";
                            //Console.WriteLine("regex = " + vysl + "\n");
                            aktualne.akcie.Add(new akcia(op, vysl));
                            iter = 11;
                        }
                        else
                        {
                            //Console.WriteLine("operacia: " + p.Groups[1].Value);
                            //Console.WriteLine("regex = " + p.Groups[2].Value + "\n");
                            aktualne.akcie.Add(new akcia(op, p.Groups[2].Value));
                        }
                    }
                }
            }

            var fakty = new HashSet<string>();
            var vymazFakty = new HashSet<string>();
            var spravy = new HashSet<string>();

            foreach (var s in File.ReadLines("../../faktyR.txt"))
            {
                var m = Regex.Match(s, @"\((.*)\)");
                if (m.Success)
                {
                    fakty.Add(m.Groups[1].Value);
                }
            }

            bool changed;
            var pocitadlo = 0;
            do
            {
                changed = false;
                var noveFakty = new List<string>();
                
                foreach (var pravidlo in pravidla)
                {

                    if (pravidlo.podmienky.Count == 2)
                    {
                        for (int i = 0; i < fakty.Count; i++)
                        {
                            for (int j = 0; j < fakty.Count; j++)
                            {
                                if (fakty.ElementAt(i) == fakty.ElementAt(j)) continue;
                                var m1 = pravidlo.podmienky[0].matches(fakty.ElementAt(i));
                                var m2 = pravidlo.podmienky[1].matches(fakty.ElementAt(j));
                                if (m1.uspech && m2.uspech)
                                {
                                    var identical = true;
                                    var m = new Dictionary<string, string>();
                                    foreach (var item in m1.mapping)
                                    {
                                        if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                        {
                                            identical = false;
                                            break;
                                        }
                                        m[item.Key] = item.Value;
                                    }
                                    foreach (var item in m2.mapping)
                                    {
                                        if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                        {
                                            identical = false;
                                            break;
                                        }
                                        m[item.Key] = item.Value;
                                    }
                                    if (identical)
                                    {
                                        foreach (var akcia in pravidlo.akcie)
                                        {
                                            if (akcia.typ == "pridaj")
                                            {
                                                var novyFakt = akcia.replace(m);
                                                if (!fakty.Contains(novyFakt))
                                                {
                                                    noveFakty.Add(novyFakt);
                                                    //Console.WriteLine("Novy fakt: " + novyFakt);
                                                    fakty.UnionWith(noveFakty);
                                                    changed = true;
                                                }
                                            }
                                            if (akcia.typ == "sprava")
                                            {
                                                var sprava = akcia.replace(m);
                                                if (!spravy.Contains(sprava))
                                                {
                                                    spravy.Add(sprava);
                                                    Console.WriteLine("Sprava: " + sprava);
                                                }
                                            }
                                            if (akcia.typ == "vymaz")
                                            {
                                                var fakt = akcia.replace(m);
                                                if (!vymazFakty.Contains(fakt))
                                                {
                                                    vymazFakty.Add(fakt);
                                                    Console.WriteLine("Vymazanie " + fakt);
                                                    changed = true;
                                                }
                                            }
                                            if (akcia.typ == "otazka")
                                            {
                                                if (pocitadlo == 0)
                                                {
                                                    var cele = akcia.replace(m);
                                                    var o = Regex.Match(cele, @"\(([\w\s]+)\:\s+([\w\s]+)");
                                                    var otazka = o.Groups[1].Value;
                                                    var moznosti = o.Groups[2].Value;
                                                    Console.WriteLine("Otazka: " + otazka + "? " + moznosti);
                                                    var moznost = Console.ReadLine();

                                                    var f = Regex.Match(cele, @"\(([\w\s\!]+)\)");
                                                    var fakt = f.Groups[1].Value;
                                                    var novyFakt = Regex.Replace(fakt, @"\!", moznost);
                                                    if (!fakty.Contains(novyFakt))
                                                    {
                                                        noveFakty.Add(novyFakt);
                                                        changed = true;
                                                    }

                                                    pocitadlo++;
                                                }
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                    }

                    if (pravidlo.podmienky.Count == 1)
                    {
                        for (int i = 0; i < fakty.Count; i++)
                        {
                            var m1 = pravidlo.podmienky[0].matches(fakty.ElementAt(i));
                            if (m1.uspech)
                            {
                                var m = new Dictionary<string, string>();
                                foreach (var item in m1.mapping)
                                {
                                    m[item.Key] = item.Value;
                                }

                                foreach (var akcia in pravidlo.akcie)
                                {
                                    if (akcia.typ == "pridaj")
                                    {
                                        var novyFakt = akcia.replace(m);
                                        if (!fakty.Contains(novyFakt))
                                        {
                                            noveFakty.Add(novyFakt);
                                            //Console.WriteLine("Novy fakt: " + novyFakt);
                                            fakty.UnionWith(noveFakty);
                                            changed = true;
                                        }
                                    }
                                    if (akcia.typ == "sprava")
                                    {
                                        var sprava = akcia.replace(m);
                                        if (!spravy.Contains(sprava))
                                        {
                                            spravy.Add(sprava);
                                            Console.WriteLine("Sprava: " + sprava);
                                        }
                                    }
                                    if (akcia.typ == "vymaz")
                                    {
                                        var fakt = akcia.replace(m);
                                        if (!vymazFakty.Contains(fakt))
                                        {
                                            vymazFakty.Add(fakt);
                                            changed = true;
                                            Console.WriteLine("Vymazanie " + fakt);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (pravidlo.podmienky.Count == 3)
                    {
                        for (int i = 0; i < fakty.Count; i++)
                        {
                            for (int j = 0; j < fakty.Count; j++)
                            {
                                for (int k = 0; k < fakty.Count; k++)
                                {
                                    if (fakty.ElementAt(i) == fakty.ElementAt(j)) continue;
                                    if (fakty.ElementAt(i) == fakty.ElementAt(k)) continue;
                                    if (fakty.ElementAt(j) == fakty.ElementAt(k)) continue;
                                    var m1 = pravidlo.podmienky[0].matches(fakty.ElementAt(i));
                                    var m2 = pravidlo.podmienky[1].matches(fakty.ElementAt(j));
                                    var m3 = pravidlo.podmienky[2].matches(fakty.ElementAt(k));

                                    if ((m1.uspech && m2.uspech && m3.uspech) || (m1.uspech && m2.uspech && pravidlo.podmienky[2].rozne))
                                    {
                                        if (pravidlo.podmienky[2].rozne)
                                        {
                                            var identical = true;
                                            var m = new Dictionary<string, string>();
                                            foreach (var item in m1.mapping)
                                            {
                                                if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m2.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            if (identical)
                                            {
                                                foreach (var akcia in pravidlo.akcie)
                                                {
                                                    if (akcia.typ == "pridaj")
                                                    {
                                                        var novyFakt = akcia.replace(m);
                                                        if (!fakty.Contains(novyFakt))
                                                        {
                                                            noveFakty.Add(novyFakt);
                                                            fakty.UnionWith(noveFakty);
                                                            changed = true;
                                                        }
                                                    }
                                                    if (akcia.typ == "sprava")
                                                    {
                                                        var sprava = akcia.replace(m);
                                                        if (!spravy.Contains(sprava))
                                                        {
                                                            spravy.Add(sprava);
                                                            Console.WriteLine("Sprava: " + sprava);
                                                        }
                                                    }
                                                    if (akcia.typ == "vymaz")
                                                    {
                                                        var fakt = akcia.replace(m);
                                                        if (!vymazFakty.Contains(fakt))
                                                        {
                                                            vymazFakty.Add(fakt);
                                                            Console.WriteLine("Vymazanie " + fakt);
                                                            changed = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var identical = true;
                                            var m = new Dictionary<string, string>();
                                            foreach (var item in m1.mapping)
                                            {
                                                if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m3.mapping.ContainsKey(item.Key) && m3.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m2.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m3.mapping.ContainsKey(item.Key) && m3.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m3.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value &&
                                                    m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            if (identical)
                                            {
                                                foreach (var akcia in pravidlo.akcie)
                                                {
                                                    if (akcia.typ == "pridaj")
                                                    {
                                                        var novyFakt = akcia.replace(m);
                                                        if (!fakty.Contains(novyFakt))
                                                        {
                                                            noveFakty.Add(novyFakt);
                                                            //Console.WriteLine("Novy fakt: " + novyFakt);
                                                            fakty.UnionWith(noveFakty);
                                                            changed = true;
                                                        }
                                                    }
                                                    if (akcia.typ == "sprava")
                                                    {
                                                        var sprava = akcia.replace(m);
                                                        if (!spravy.Contains(sprava))
                                                        {
                                                            spravy.Add(sprava);
                                                            Console.WriteLine("Sprava: " + sprava);
                                                        }
                                                    }
                                                    if (akcia.typ == "vymaz")
                                                    {
                                                        var fakt = akcia.replace(m);
                                                        if (!vymazFakty.Contains(fakt))
                                                        {
                                                            vymazFakty.Add(fakt);
                                                            Console.WriteLine("Vymazanie " + fakt);
                                                            changed = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (pravidlo.podmienky.Count >= 4)
                    {
                        for (int i = 0; i < fakty.Count; i++)
                        {
                            for (int j = 0; j < fakty.Count; j++)
                            {
                                for (int k = 0; k < fakty.Count; k++)
                                {
                                    for (int l = 0; l < fakty.Count; l++)
                                    {
                                        if (fakty.ElementAt(i) == fakty.ElementAt(j)) continue;
                                        if (fakty.ElementAt(i) == fakty.ElementAt(k)) continue;
                                        if (fakty.ElementAt(i) == fakty.ElementAt(l)) continue;
                                        if (fakty.ElementAt(j) == fakty.ElementAt(k)) continue;
                                        if (fakty.ElementAt(j) == fakty.ElementAt(l)) continue;
                                        if (fakty.ElementAt(k) == fakty.ElementAt(l)) continue;
                                        var m1 = pravidlo.podmienky[0].matches(fakty.ElementAt(i));
                                        var m2 = pravidlo.podmienky[1].matches(fakty.ElementAt(j));
                                        var m3 = pravidlo.podmienky[2].matches(fakty.ElementAt(k));
                                        var m4 = pravidlo.podmienky[3].matches(fakty.ElementAt(l));
                                        if (m1.uspech && m2.uspech && m3.uspech && m4.uspech)
                                        {
                                            var identical = true;
                                            var m = new Dictionary<string, string>();
                                            foreach (var item in m1.mapping)
                                            {
                                                if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m3.mapping.ContainsKey(item.Key) && m3.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m4.mapping.ContainsKey(item.Key) && m4.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m2.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m3.mapping.ContainsKey(item.Key) && m3.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m4.mapping.ContainsKey(item.Key) && m4.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m3.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m4.mapping.ContainsKey(item.Key) && m4.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            foreach (var item in m4.mapping)
                                            {
                                                if (m1.mapping.ContainsKey(item.Key) && m1.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m2.mapping.ContainsKey(item.Key) && m2.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                if (m3.mapping.ContainsKey(item.Key) && m3.mapping[item.Key] != item.Value)
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                                m[item.Key] = item.Value;
                                            }
                                            if (identical)
                                            {
                                                foreach (var akcia in pravidlo.akcie)
                                                {
                                                    if (akcia.typ == "pridaj")
                                                    {
                                                        var novyFakt = akcia.replace(m);
                                                        if (!fakty.Contains(novyFakt))
                                                        {
                                                            noveFakty.Add(novyFakt);
                                                            //Console.WriteLine("Novy fakt: " + novyFakt);
                                                            fakty.UnionWith(noveFakty);
                                                            changed = true;
                                                        }
                                                    }
                                                    if (akcia.typ == "sprava")
                                                    {
                                                        var sprava = akcia.replace(m);
                                                        if (!spravy.Contains(sprava))
                                                        {
                                                            spravy.Add(sprava);
                                                            Console.WriteLine("Sprava: " + sprava);
                                                        }
                                                    }
                                                    if (akcia.typ == "vymaz")
                                                    {
                                                        var fakt = akcia.replace(m);
                                                        if (!vymazFakty.Contains(fakt))
                                                        {
                                                            vymazFakty.Add(fakt);
                                                            Console.WriteLine("Vymazanie " + fakt);
                                                            changed = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var vymaz in vymazFakty)
                    {
                        fakty.Remove(vymaz);
                    }
                    fakty.UnionWith(noveFakty);
                }
            }
            while (changed);

            Console.WriteLine();
            Console.WriteLine(string.Join("\n", fakty));
            Console.WriteLine("\nKoniec");

            Console.Read();
        }
    }
}
