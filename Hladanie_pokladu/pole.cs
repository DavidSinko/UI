using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HladaniePokladu
{
    class pole
    {
        const int N = 7;
        bool[,] poklad = new bool[N, N];

        public int pocetPokladov = 0;
        public int startX = 3;
        public int startY = 6;

        public int[] suradnice = new int[2];

        public pole()       // zakaldne rozlozenie pokladov a startovna pozicia
        {
            poklad[4, 1] = true;
            poklad[2, 2] = true;
            poklad[6, 3] = true;
            poklad[1, 4] = true;
            poklad[4, 5] = true;
            startX = 3;
            startY = 6;
        }

        public void resetPoklady()
        {
            poklad[4, 1] = true;
            poklad[2, 2] = true;
            poklad[6, 3] = true;
            poklad[1, 4] = true;
            poklad[4, 5] = true;
        }

        public void startovnaPozicia()      // ulozenie startovnej pozicie do pola
        {
            suradnice[0] = startX;
            suradnice[1] = startY;
        }

        public bool testJeMimo(pohyb pohyb)     // testovanie na vybocenie jedinca mimo mapky
        {
            switch(pohyb)
            {
                case pohyb.hore:
                    suradnice[1]--;
                    break;

                case pohyb.dole:
                    suradnice[1]++;
                    break;

                case pohyb.doprava:
                    suradnice[0]++;
                    break;

                case pohyb.dolava:
                    suradnice[0]--;
                    break;
            }

            if(suradnice[0] < 0 || suradnice[1] < 0 || suradnice[0] >= N || suradnice[1] >= N)
            {
                return true;
            }

            return false;
        }

        public double fitness(List<pohyb> pohyby)      // vypocet fitness funkcie jedinca
        {
            resetPoklady();
            pocetPokladov = 0;
            double score = 0;
            var x = startX;
            var y = startY;
            foreach (var p in pohyby)
            {
                switch (p)
                {
                    case pohyb.hore:
                        y--;
                        score -= 0.3;
                        break;

                    case pohyb.dole:
                        y++;
                        score -= 0.3;
                        break;

                    case pohyb.doprava:
                        x++;
                        score -= 0.3;
                        break;

                    case pohyb.dolava:
                        x--;
                        score -= 0.3;
                        break;
                }
                //Console.WriteLine($"pole = {x}, {y}");
                if (x < 0 || y < 0 || x >= N || y >= N)
                {
                    return Math.Abs(score);
                }
                if (poklad[x, y])       // ak jedinec najde poklad, prirataj mu do fitness vyssie cislo
                {
                    poklad[x, y] = false;
                    score += 1000;
                    pocetPokladov++;
                }
            }
            return Math.Abs(score);
        }
    }
}
