using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blazniva_krizovatka
{
    enum farba
    {
        cervene, zelene, smodre, zlte, oranzove, fialove, sive, tmodre, ruzove, biele, cierne, lososove, hnede
    }

    enum orientacia
    {
        h, v
    }

    class auto
    {
        public int dlzka;
        public int x;
        public int y;
        public farba farba;
        public orientacia orientacia;

        public auto(farba f, int d, int y, int x, orientacia o)
        {
            farba = f;
            dlzka = d;
            this.x = x;
            this.y = y;
            orientacia = o;
        }

        public bool jeVpravo => x + dlzka >= 7;
        public bool jeVlavo => x <= 1;
        public bool jeHore => y <= 1;
        public bool jeDole => y + dlzka >= 7;

        public int offset(int i)
        {
            return 6 * (y - 1) + (x - 1) + i*(orientacia == orientacia.v ? 6 : 1);
        }

        public auto doprava()
        {
            if (jeVpravo || orientacia == orientacia.v) throw new Exception("Chyba. Nemozem posunut auto doprava");
            return new auto(farba, dlzka, y, x + 1, orientacia);
        }

        public auto dolava()
        {
            if (jeVlavo || orientacia == orientacia.v) throw new Exception("Chyba. Nemozem posunut auto dolava");
            return new auto(farba, dlzka, y, x - 1, orientacia);
        }

        public auto dole()
        {
            if (jeDole || orientacia == orientacia.h) throw new Exception("Chyba. Nemozem posunut auto dole");
            return new auto(farba, dlzka, y + 1, x, orientacia);
        }

        public auto hore()
        {
            if (jeHore || orientacia == orientacia.h) throw new Exception("Chyba. Nemozem posunut auto hore");
            return new auto(farba, dlzka, y - 1, x, orientacia);
        }
    }
}
