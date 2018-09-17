using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HladaniePokladu
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new Random(DateTime.Now.Millisecond);
            var p = new pole();
            double sumaFitness = 0;
            double maxFitnessJedinca = 0;
            var descendingComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var generacia = 1;
            bool koniec = false;

            var elite = new List<jedinec>();
            var povodniJedinci = new List<jedinec>();
            var jedinci = new List<jedinec>();
            Console.WriteLine("Zadaj velkost populacie: (100)"); 
            var vp = Console.ReadLine();
            var velkostPopulacie = Convert.ToInt32(vp);
            var napln = velkostPopulacie;

            Console.WriteLine("Zadaj typ selekcie - 1 pre ruletu, 2 pre turnaj:");
            var vyb = Console.ReadLine();
            var typ = Convert.ToInt32(vyb);

            Console.WriteLine("Zadaj pocet jedincov, ktori sa maju vytvorit novi krizenim: (70)"); 
            var ko = Console.ReadLine();
            var kolko = Convert.ToInt32(ko);

            var pocetInstr = 500;
            Console.WriteLine("Chces zadat maximalny pocet instrukcii, ktore moze jedinec vykonat? Default je 500. Ano/Nie?:");
            var rozhodnutie = Console.ReadLine();
            if(rozhodnutie.ToLower() == "ano")
            {
                Console.WriteLine("Zadaj pocet:");
                var ins = Console.ReadLine();
                pocetInstr = Convert.ToInt32(ins);
            }

            while (napln-- > 0)
            {
                jedinci.Add(new jedinec(r));
            }
             

            var vypis = 100;

            while(!koniec)        // tu je cyklus v ktorom vsetko bezi, jedinci sa krizia
            {
                /*if (generacia == 1 || generacia % vypis == 0 || p.pocetPokladov == 5)
                {
                    Console.WriteLine("---------- Generacia: " + generacia + " ----------");
                }*/

                maxFitnessJedinca = 0;
                sumaFitness = 0;
                foreach (var j in jedinci)
                {
                    var steps = j.run(pocetInstr);
                    j.fitness = p.fitness(steps);
                    sumaFitness += j.fitness;
                    if (p.pocetPokladov == 5) koniec = true;

                    if (maxFitnessJedinca > j.fitness) maxFitnessJedinca *= 1;
                    else maxFitnessJedinca = j.fitness;

                    if (p.pocetPokladov == 5)
                    {
                        Console.WriteLine("Koniec! Nasli sme vsetky poklady.");
                        Console.WriteLine("Generacia: " + generacia);
                        Console.WriteLine("Suma fitness doterajsich jedincov: " + sumaFitness);
                        Console.WriteLine("Konecna fitness jedinca: " + j.fitness);       // vypisuje fitness
                        Console.WriteLine("Vysledne kroky:");
                        var s = string.Join(" ", steps.Select(x => x.ToString()));
                        Console.WriteLine(s);       // vypisuje postupnost krokov
                        Console.WriteLine("Pocet najdenych pokladov: " + p.pocetPokladov);
                        Console.WriteLine();
                    }

                }
                
                //jedinci.AddRange(elite);

                /*if (generacia == 1 || generacia % vypis == 0)
                {
                    Console.WriteLine("Suma fitness: " + sumaFitness + " Max fitness jedinca: " + maxFitnessJedinca);
                }*/

                if (generacia == 1 || generacia % 10000 == 0) Console.WriteLine("Hladam poklady...");


                //povodniJedinci = jedinci.ToList();
                povodniJedinci.AddRange(jedinci);
                jedinci.Clear();
                if(typ == 1)
                {
                    jedinci = ruleta(sumaFitness, povodniJedinci, kolko).ToList();
                }
                else if(typ == 2)
                {
                    jedinci = turnaj(povodniJedinci, kolko).ToList();
                }

                if (kolko < velkostPopulacie)
                {
                    for(int i = 0; i < (velkostPopulacie - kolko); i++)
                    {
                        jedinci.Add(new jedinec(r));
                    }
                }
                povodniJedinci.Clear();

                /*elite.Clear();
                for(int t = 0; t < 5; t++)
                {
                    elite.Add(jedinci[t]);
                }*/

                generacia++;
            }

            Console.Read();
        }



        public static List<jedinec> ruleta(double suma, List<jedinec> jedinci, int kolkoVytvor)
        {
            var terazVytvoreni = new List<jedinec>();
            var rodic1 = new jedinec();
            var rodic2 = new jedinec();
            bool prep = false;
            terazVytvoreni.Clear();

            // tu si zachovam pät najlepsich jedincov z povodnych
            var zachovaj = 5;
            jedinci = jedinci.OrderByDescending(x => x.fitness).ToList();
            for (int i = 0; i < zachovaj; i++)
            {
                terazVytvoreni.Add(jedinci[i]);
            }
            

            var pocet = 0;
            while (pocet < kolkoVytvor)
            {
                for (int i = 0; i < 2; i++)
                {
                    var random = new Random();
                    var cislo = random.NextDouble();

                    double sumaPravdepodobnosti = 0;
                    foreach (var j in jedinci)      // stanovenie velkosti usekov pre kazdeho jedinca
                    {
                        j.pravdepodobnost = sumaPravdepodobnosti + (j.fitness / suma);
                        sumaPravdepodobnosti = j.pravdepodobnost;
                    }

                    double current = 0;
                    double next = jedinci[0].pravdepodobnost;
                    for (int k = 0; k < jedinci.Count; k++)
                    {
                        if (cislo > current && cislo < next)
                        {
                            if (!prep)
                            {
                                rodic1 = jedinci[k];
                                jedinci.Remove(rodic1);
                                prep = true;
                            }
                            else
                            {
                                rodic2 = jedinci[k];
                                prep = false;
                            }
                            break;
                        }
                        if ((k + 1) < jedinci.Count)
                        {
                            current = jedinci[k].pravdepodobnost;
                            next = jedinci[k + 1].pravdepodobnost;
                        }
                        else
                        {
                            current = jedinci[k].pravdepodobnost;
                            next = 1;
                        }
                    }
                }

                var novy = new jedinec();

                novy = novy.krizenie(rodic1, rodic2);
                terazVytvoreni.Add(novy);

                novy = novy.invertujNahodnyBit(rodic1);
                terazVytvoreni.Add(novy);

                novy = novy.invertujPoslednyBit(rodic2);
                terazVytvoreni.Add(novy);

                novy = novy.invertujVsetkyBity(rodic1);
                terazVytvoreni.Add(novy);

                jedinci.Add(rodic1);

                pocet += 4;
            }

            return terazVytvoreni;
        }



        public static List<jedinec> turnaj(List<jedinec> jedinci, int kolkoVytvor)
        {
            var terazVytvoreni = new List<jedinec>();
            var rodic1 = new jedinec();
            var rodic2 = new jedinec();
            bool prep = false;
            var pocetJedincov = jedinci.Count;
            var random = new Random();
            var pocetK = random.Next((pocetJedincov/20), (pocetJedincov/10));
            terazVytvoreni.Clear();


            //  tu si zachovam pät najlepsich jedincov z povodnych
            var zachovaj = 5;
            jedinci = jedinci.OrderByDescending(x => x.fitness).ToList();
            for (int i = 0; i < zachovaj; i++)
            {
                terazVytvoreni.Add(jedinci[i]);
            }

            var pomocnyZoznam = new List<jedinec>();

            var pocet = 0;
            while (pocet < kolkoVytvor)
            {
                for (int j = 0; j < 2; j++)
                {
                    pomocnyZoznam.Clear();
                    var pridaj = new jedinec();
                    for (int i = 0; i < pocetK; i++)
                    {
                        pridaj = jedinci[random.Next(0, pocetJedincov)];
                        pomocnyZoznam.Add(pridaj);
                        jedinci.Remove(pridaj);
                        pocetJedincov--;
                    }

                    pomocnyZoznam = pomocnyZoznam.OrderByDescending(x => x.fitness).ToList();
                    if(!prep)
                    {
                        rodic1 = pomocnyZoznam.First();
                        prep = true;
                    }
                    else
                    {
                        rodic2 = pomocnyZoznam.First();
                        prep = false;
                    }

                    // vratim zoznam do povodneho stavu
                    jedinci.AddRange(pomocnyZoznam);
                    pocetJedincov = +pocetK;

                }

                var novy = new jedinec();

                novy = novy.krizenie(rodic1, rodic2);
                terazVytvoreni.Add(novy);

                novy = novy.invertujNahodnyBit(rodic1);
                terazVytvoreni.Add(novy);

                novy = novy.invertujPoslednyBit(rodic2);
                terazVytvoreni.Add(novy);

                pocet += 3;
            }

            return terazVytvoreni;
        }

    }
}
