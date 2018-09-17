using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HladaniePokladu
{
    enum pohyb
    {
        hore = 0,
        dole = 1,
        doprava = 2,
        dolava = 3
    }

    class jedinec
    {
        public byte[] mem = new byte[64];       // pole bytov velkosti 64 (64 pamatovych buniek)
        public double fitness;
        public double pravdepodobnost;
        
        public jedinec()
        {
            
        }

        public jedinec(Random r)
        {
            r.NextBytes(mem);
        }

        public List<pohyb> run(int steps)     // funkcia pre beh programu pre jedinca, pocet krokov je nastavenych premennou steps
        {
            var p = new pole();
            p.startovnaPozicia();

            var result = new List<pohyb>();
            var ip = 0;
            while (steps-- > 0)     // pokial nepresiahne urceny pocet krokov
            {
                var inst = mem[ip];     // premenna pre instrukciu, ukazuje na pamatovu bunku
                var op = inst >> 6;     // premenna pre operaciu, co ma program vykonat (inkrementacia, dekrementacia, skok alebo vypis)
                var adr = (byte)(inst & 0x3f);      // premenna pre adresu (0011 1111)

                /*if (ip == 0 || ip == 1)
                {
                    Console.WriteLine("inst: " + Convert.ToString(inst, 2));
                    Console.WriteLine("op: " + Convert.ToString(op, 2));
                    Console.WriteLine("adr: " + Convert.ToString(adr, 2));
                }*/

                switch (op)     // podla operacie vykonaj konkretnu akciu
                {
                    case 0:         // inkrementacia
                        mem[adr]++;
                        ip++;
                        break;

                    case 1:         // dekrementacia
                        mem[adr]--;
                        ip++;
                        break;

                    case 2:         // skok
                        ip = adr;
                        break;

                    case 3:         // vypis
                        var pocet = bitcount(mem[adr]);
                        var prem = 0;
                        if (pocet <= 2) prem = 0;
                        else if (pocet <= 4) prem = 1;
                        else if (pocet <= 6) prem = 2;
                        else prem = 3;

                        result.Add((pohyb)(prem));
                        ip++;
                        if (p.testJeMimo((pohyb)(prem)))      // skontroluj, ci je mimo, ak ano, skonci program pre daneho jedinca
                        {
                            return result; 
                        }
                        break;

                }
                if (ip >= 64) ip = 0;       // ak sa dostane na koniec pola, tak chod od zaciatku
            }
            //Console.WriteLine("Dosiahnuty limit poctu krokov");
            return result;
        }

        static int bitcount(byte n)
        {
            int count = 0;
            while (n > 0)
            {
                count += (n & 1);
                n >>= 1;
            }
            return count;
        }

        public jedinec turnaj()
        {
            var novy = new jedinec();

            return novy;
        }

        public jedinec krizenie(jedinec rodic1, jedinec rodic2)
        {
            var novy = new jedinec();
            var random = new Random();
            var rozdelovaciBod = random.Next(0, 64);
            var prvy = random.Next(0, rozdelovaciBod);
            var druhy = random.Next(rozdelovaciBod, 64);

            for(int i = 0; i < 64; i++)
            {
                if (i < rozdelovaciBod) novy.mem[i] = rodic1.mem[i];
                else novy.mem[i] = rodic2.mem[i];
            }

            var ktoryBit = random.Next(0, 8);
            novy.mem[rozdelovaciBod] ^= (byte)(1 << ktoryBit);

            return novy;
        }

        public jedinec invertujNahodnyBit(jedinec jedinec)
        {
            var novy = new jedinec();
            var random = new Random();
            var ktoraBunka = random.Next(0, 64);
            var ktoryBit = random.Next(0, 8);

            novy = jedinec;
            novy.mem[ktoraBunka] ^= (byte)(1 << ktoryBit);

            return novy;
        }

        public jedinec invertujPoslednyBit(jedinec jedinec)
        {
            var novy = new jedinec();
            var random = new Random();
            var ktoraBunka = random.Next(0, 64);

            novy = jedinec;
            novy.mem[ktoraBunka] ^= 1;

            return novy;
        }

        public jedinec invertujVsetkyBity(jedinec jedinec)
        {
            var novy = new jedinec();
            var random = new Random();
            var ktoraBunka = random.Next(0, 64);

            novy = jedinec;
            novy.mem[ktoraBunka] = (byte)(~novy.mem[ktoraBunka]);

            return novy;
        }

    }
}
