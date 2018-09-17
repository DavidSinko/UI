using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blazniva_krizovatka
{
    class uzol
    {
        public List<auto> auta;
        public bool spracovany;
        public int vzdialenost;
        public uzol predchodca;
        public string text;

        public void suradnice()
        {
            var cervene = auta.First(x => x.farba == farba.cervene);
            var rozdiel = cervene.x;
        }

        public int vypocitajVzdialenost()
        {
            var cervene = auta.First(x => x.farba == farba.cervene);
            var d = 7 - cervene.x - cervene.dlzka;
            
            var t = obsadene();
            var pocetObsadenych = 0;
            for (int i = 0; i < d; i++)
            {
                if (t.Contains(cervene.offset(cervene.dlzka + i))) pocetObsadenych++;
            }
            return d + pocetObsadenych;
        }

        public int index(int x, int y)
        {
            return 6 * (y - 1) + (x - 1);
        }

        public uzol(List<auto> auta)
        {
            this.auta = auta;
            vzdialenost = vypocitajVzdialenost();
            text = ToString();
        }

        public uzol(List<auto> stare, int index, auto nove, uzol predchodca)
        {
            this.predchodca = predchodca;
            spracovany = false;
            auta = new List<auto>(stare);
            auta[index] = nove;
            vzdialenost = vypocitajVzdialenost();
            text = ToString();
        }

        public bool uzBolo()
        {
            var rodic = predchodca;
            var s = text;
            while(rodic != null)
            {
                if (s == rodic.text) { return true; }
                rodic = rodic.predchodca;
            }
            return false;
        }

        public List<uzol> dalsie()
        {
            var vysledok = new List<uzol>();        // vytvori novy zoznam uzlov
            for (int i = 0; i < auta.Count; i++)        // iteruj aky je pocet aut (auta je zoznam aut, cize aky je pocet aut v zozname)
            {
                var a = auta[i];
                if (a.orientacia == orientacia.h)
                {
                    if(!a.jeVpravo)
                    {
                        var novyUzol = new uzol(auta, i, a.doprava(), this);
                        if(!novyUzol.jeKolizia() && !novyUzol.uzBolo()) vysledok.Add(novyUzol);
                    }

                    if (!a.jeVlavo)
                    {
                        var novyUzol = new uzol(auta, i, a.dolava(), this);
                        if (!novyUzol.jeKolizia() && !novyUzol.uzBolo()) vysledok.Add(novyUzol);
                    }
                    
                }
                else
                {
                    if (!a.jeHore)
                    {
                        var novyUzol = new uzol(auta, i, a.hore(), this);
                        if (!novyUzol.jeKolizia() && !novyUzol.uzBolo()) vysledok.Add(novyUzol);
                    }

                    if (!a.jeDole)
                    {
                        var novyUzol = new uzol(auta, i, a.dole(), this);
                        if (!novyUzol.jeKolizia() && !novyUzol.uzBolo()) vysledok.Add(novyUzol);
                    }
                }
            }
            return vysledok;
        }

        public bool jeKolizia()
        {
            var obsadene = new HashSet<int>();
            foreach (var a in auta)
            {
                var f = a.orientacia == orientacia.v ? 6 : 1;
                for (int i = 0; i < a.dlzka; i++)
                {
                    var novaPozicia = a.offset(i);
                    if (obsadene.Contains(novaPozicia)) { return true; }
                    obsadene.Add(novaPozicia);
                }
            }
            return false;
        }

        public HashSet<int> obsadene()
        {
            var obsadene = new HashSet<int>();
            foreach (var a in auta)
            {
                for (int i = 0; i < a.dlzka; i++)
                {
                    obsadene.Add(a.offset(i));
                }
            }
            return obsadene;
        }

        public void print()
        {
            foreach (var a in auta)
            {
                for (int i = 0; i < a.dlzka; i++)
                {
                    Console.SetCursorPosition(
                        a.x + i * (a.orientacia == orientacia.h ? 1 : 0),
                        a.y + i * (a.orientacia == orientacia.v ? 1 : 0));
                    Console.Write(a.farba.ToString()[0]);
                }
            }
        }

        public override string ToString()
        {
            var farby = new Dictionary<int, farba>();
            foreach (var a in auta)
            {
                for (int i = 0; i < a.dlzka; i++)
                {
                    var novaPozicia = a.offset(i);
                    farby[novaPozicia] = a.farba;
                }
            }
            var s = "";
            for (int i = 0; i < 36; i++)
            {
                if (farby.ContainsKey(i))
                {
                    var t = farby[i].ToString();
                    while (t.Length < 10) t += " ";
                    s += t;
                }
                else
                {
                    s += new string(' ', 10);
                }
                if (i % 6 == 5) s += "\n";
            }
            return s;
        }
    }
}
