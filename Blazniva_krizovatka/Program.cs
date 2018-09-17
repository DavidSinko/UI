using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blazniva_krizovatka
{
    class Program
    {

        static void Main(string[] args)
        {

            //var uzol = new uzol(vytvorPociatocnyStav());

            var uzol = new uzol(new List<auto> {
new auto(farba.cervene,2,3,2,orientacia.h),
new auto(farba.oranzove,2,1,1,orientacia.h),
new auto(farba.zlte,3,2,1,orientacia.v),
new auto(farba.fialove,2,5,1,orientacia.v),
new auto(farba.zelene,3,2,4,orientacia.v),
new auto(farba.smodre,3,6,3,orientacia.h),
new auto(farba.sive,2,5,5,orientacia.h),
new auto(farba.tmodre,3,1,6,orientacia.v),
//new auto(farba.biele,2,4,5,orientacia.h),
});

            /*var uzol = new uzol(new List<auto> {
new auto(farba.cervene,2,3,4,orientacia.h),
new auto(farba.smodre,2,1,1,orientacia.h),
new auto(farba.tmodre,3,1,3,orientacia.v),
new auto(farba.zlte,2,5,1,orientacia.v),
new auto(farba.oranzove,2,5,2,orientacia.h),
new auto(farba.zelene,3,4,4,orientacia.v),
new auto(farba.ruzove,2,1,5,orientacia.v),
new auto(farba.fialove,3,1,6,orientacia.v),
new auto(farba.biele,2,4,5,orientacia.h),
new auto(farba.sive,2,6,5,orientacia.h),
});*/

            /*var uzol = new uzol(new List<auto> {
new auto(farba.cervene,2,3,2,orientacia.h),
new auto(farba.smodre,2,1,1,orientacia.h),
new auto(farba.ruzove,2,1,3,orientacia.v),
new auto(farba.zlte,2,3,1,orientacia.v),
new auto(farba.oranzove,2,5,1,orientacia.v),
new auto(farba.fialove,2,5,2,orientacia.v),
new auto(farba.tmodre,3,1,4,orientacia.v),
new auto(farba.zelene,2,5,3,orientacia.h),
new auto(farba.sive,2,6,3,orientacia.h),
new auto(farba.biele,2,5,5,orientacia.v),
new auto(farba.cierne,2,3,6,orientacia.v),
new auto(farba.lososove,2,5,6,orientacia.v),
});*/

            /*var uzol = new uzol(new List<auto> {
new auto(farba.cervene,2,3,1,orientacia.h),
new auto(farba.cierne,2,1,2,orientacia.v),
new auto(farba.oranzove,2,2,3,orientacia.h),
new auto(farba.tmodre,2,4,1,orientacia.h),
new auto(farba.zelene,2,4,3,orientacia.h),
new auto(farba.ruzove,2,5,4,orientacia.v),
new auto(farba.smodre,2,1,5,orientacia.h),
new auto(farba.fialove,2,2,5,orientacia.h),
new auto(farba.sive,3,3,6,orientacia.v),
new auto(farba.zlte,2,6,5,orientacia.h),
});*/

            Console.WriteLine(uzol);

            doSirky(uzol);
            //doHlbky(uzol);

            Console.ReadKey();
        }

        static List<auto> vytvorPociatocnyStav()
        {
            var pociatocny = new List<auto>();

            while (true)
            {
                Console.WriteLine("Zadaj farbu auta (cervene, zelene, smodre, zlte, oranzove, fialove, sive, tmodre, ruzove, biele, cierne, lososove, hnede)\n" +
                    "alebo 'koniec' pre ukoncenie zadavania:");
                farba farbaEnum;
                var farb = Console.ReadLine().ToString();
                if (farb == "cervene")
                    farbaEnum = farba.cervene;
                else if (farb == "zelene")
                    farbaEnum = farba.zelene;
                else if (farb == "svetlomodre")
                    farbaEnum = farba.smodre;
                else if (farb == "zlte")
                    farbaEnum = farba.zlte;
                else if (farb == "oranzove")
                    farbaEnum = farba.oranzove;
                else if (farb == "fialove")
                    farbaEnum = farba.fialove;
                else if (farb == "sive")
                    farbaEnum = farba.sive;
                else if (farb == "tmavomodre")
                    farbaEnum = farba.tmodre;
                else if (farb == "ruzove")
                    farbaEnum = farba.ruzove;
                else if (farb == "biele")
                    farbaEnum = farba.biele;
                else if (farb == "cierne")
                    farbaEnum = farba.cierne;
                else if (farb == "lososove")
                    farbaEnum = farba.lososove;
                else if (farb == "hnede")
                    farbaEnum = farba.hnede;
                else if (farb.Equals("koniec"))
                    break;
                else
                    throw new ApplicationException("Nespravny vstup pre farbu.");
                Console.WriteLine("Zadaj dlzku auta:");
                var dl = Console.ReadLine();
                var dlzka = Convert.ToInt32(dl);
                Console.WriteLine("Zadaj riadok v ktorom sa nachadza:");
                var ri = Console.ReadLine();
                var riadok = Convert.ToInt32(ri);
                Console.WriteLine("Zadaj stlpec v ktorom sa nachadza:");
                var st = Console.ReadLine();
                var stlpec = Convert.ToInt32(st);
                Console.WriteLine("Zadaj orientaciu auta (v/h):");
                orientacia orientaciaEnum;
                var orient = Console.ReadLine().ToString();
                if (orient == "v")
                    orientaciaEnum = orientacia.v;
                else if (orient == "h")
                    orientaciaEnum = orientacia.h;
                else
                    throw new ApplicationException("Nespravny vstup pre orientaciu.");

                pociatocny.Add(new auto(farbaEnum, dlzka, riadok, stlpec, orientaciaEnum));
            }

            return pociatocny;
        }

        static void vypisKonecneRiesenie(uzol aktualny)
        {
            /* tu chcem vypisat cestu ako sa auticka posuvali a nakoniec dostali do cieloveho stavu */
            Console.WriteLine("Operatory akymi sa auticka dostali do cieloveho stavu:\n");
            var lifo = new Stack<uzol>();
            while(aktualny != null)
            {
                lifo.Push(aktualny);
                aktualny = aktualny.predchodca;
            }

            var posuny = 0;

            while(lifo.Count != 0)
            {
                var tento = lifo.Pop();
                uzol dalsi;
                if (lifo.Count > 0)
                {
                    dalsi = lifo.Peek();
                }
                else
                    break;

                posuny++;
                for (int i = 0; i < tento.auta.Count; i++)
                {
                    if (tento.auta[i].orientacia == orientacia.h)
                    {
                        var posun = tento.auta[i].x - dalsi.auta[i].x;
                        if (posun < 0)
                            Console.WriteLine("VPRAVO(" + tento.auta[i].farba + ", " + Math.Abs(posun) + ")");
                        else if (posun > 0)
                            Console.WriteLine("VLAVO(" + tento.auta[i].farba + ", " + Math.Abs(posun) + ")");
                    }
                    else
                    {
                        var posun = tento.auta[i].y - dalsi.auta[i].y;
                        if (posun < 0)
                            Console.WriteLine("DOLE(" + tento.auta[i].farba + ", " + Math.Abs(posun) + ")");
                        else if (posun > 0)
                            Console.WriteLine("HORE(" + tento.auta[i].farba + ", " + Math.Abs(posun) + ")");
                    }

                }

            }
            Console.WriteLine("\nPocet posunov: " + posuny);
        }

        static void doSirky(uzol uzol)
        {
            var navstivene = new HashSet<String>();

            var rad = new Queue<uzol>();
            rad.Enqueue(uzol);

            var zoznam = new List<uzol>();

            var counter = 1;
            var vsetkyUzly = 1;
            while (rad.Count > 0)   // pokial je nieco vo fronte
            {
                uzol aktualny = null;

                aktualny = rad.Dequeue();

                if (navstivene.Contains(aktualny.ToString()))
                {
                    continue;
                }

                navstivene.Add(aktualny.ToString());
//                Console.WriteLine(String.Join(",", navstivene));    // vypise co obsahuje hashset

                if (aktualny.vzdialenost == 0)   // ak je cervene auto v cielovej pozicii
                {
                    Console.WriteLine("Nasli sme riesenie:\n");
                    Console.WriteLine(aktualny);
                    Console.WriteLine("Pocet spracovanych uzlov: " + counter + " Pocet rozvitych uzlov: " + vsetkyUzly);
                    Console.WriteLine();

                    vypisKonecneRiesenie(aktualny);

                    return;
                }
                counter++;      // pripocitavaj stavy zakazdym, kolko operacii bolo urobenych

                zoznam.Clear();
                zoznam.AddRange(aktualny.dalsie());

                for (int i = 0; i < zoznam.Count; i++)    // pridavam do radu preskumane uzly
                {
                    var sused = zoznam.ElementAt(i);
                    if (!navstivene.Contains(sused.ToString()))
                    {
                        rad.Enqueue(sused);
                        vsetkyUzly++;
                    }
                }
                aktualny.spracovany = true;     // oznaci uzol za spracovany

            }
            Console.WriteLine("Riesenie neexistuje");

//            Console.WriteLine(String.Join("\n", navstivene));

        }

        static void doHlbky(uzol uzol)
        {
            var navstivene = new HashSet<string>();

            var zasobnik = new Stack<uzol>();
            zasobnik.Push(uzol);

            var zoznam = new List<uzol>();

            var counter = 1;
            var vsetkyUzly = 1;
            while (zasobnik.Count > 0) // pokial je nieco v zasobniku
            {
                uzol aktualny = null;

                aktualny = zasobnik.Pop();

                if (navstivene.Contains(aktualny.ToString()))
                {
                    continue;
                }

                navstivene.Add(aktualny.ToString());

                if (aktualny.vzdialenost == 0)   // ak je cervene auto v cielovej pozicii
                {
                    Console.WriteLine("Nasli sme riesenie:\n");
                    Console.WriteLine(aktualny);
                    Console.WriteLine("Pocet spracovanych uzlov: " + counter + " Pocet rozvitych uzlov: " + vsetkyUzly);
                    Console.WriteLine();

                    vypisKonecneRiesenie(aktualny);

                    return;
                }
                counter++;      // pripocitavaj stavy zakazdym, kolko operacii bolo urobenych

                zoznam.Clear();
                zoznam.AddRange(aktualny.dalsie());

                for (int i = 0; i < zoznam.Count; i++)    // pridavam do radu preskumane uzly
                {
                    var sused = zoznam.ElementAt(i);
                    if (!navstivene.Contains(sused.ToString()))
                    {
                        zasobnik.Push(sused);
                        vsetkyUzly++;
                    }
                }
                aktualny.spracovany = true;     // oznaci uzol za spracovany

            }
            Console.WriteLine("Riesenie neexistuje");

        }

    }
}
