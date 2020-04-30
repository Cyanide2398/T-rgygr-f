using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szoftverfejlesztes
{
    public class Targy
    {
       public string kod { get; set; }
        public string nev { get; set; }
        public int ajanlottFelev { get; set; }
        public int kredit { get; set; }

        public string kategoria { get; set; }

        public bool teljesitett { get; set; }
        public int elofeltetel_db { get; set; }

        public List<string> elofeltetelek;

        public bool Green { get; set; }

        public Targy(string _kod, string _nev, int _ajanlottFelev, int _kredit, string _kategoria, bool _teljesitett = false)
        {
            this.kod = _kod;
            this.nev = _nev;
            this.ajanlottFelev = _ajanlottFelev;
            this.kredit = _kredit;
            this.elofeltetel_db = 0;
            this.elofeltetelek = new List<string>();
            this.Green = false;
            this.kategoria = _kategoria;

        }
    }
}
