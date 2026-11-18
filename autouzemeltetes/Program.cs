using System;
using System.Collections.Generic;
using System.Globalization;

namespace TcoKalkulator
{
    // Segédosztály a jármű kategóriák adatainak tárolásához
    class JarmuKategoria
    {
        public string Nev { get; set; }
        public double EvFogyasztas { get; set; }
        public double IceFogyasztas { get; set; }

        public JarmuKategoria(string nev, double evFogyasztas, double iceFogyasztas)
        {
            Nev = nev;
            EvFogyasztas = evFogyasztas;
            IceFogyasztas = iceFogyasztas;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("   ELEKTROMOS VS. BELSŐ ÉGÉSŰ AUTÓ KALKULÁTOR");
            Console.WriteLine("==========================================================\n");

            int futamido = BekerEgeszSzam("Tervezett futamidő (év): ");
            int evesKm = BekerEgeszSzam("Éves futásteljesítmény (km): ");

            List<JarmuKategoria> kategoriak = new List<JarmuKategoria>
            {
                new JarmuKategoria("Kisautó (EV: 14 kWh/100km | Benzin/Dízel: 6.0 l/100km)", 14.0, 6.0),
                new JarmuKategoria("Kompakt/Szedán (EV: 17 kWh/100km | Benzin/Dízel: 7.5 l/100km)", 17.0, 7.5),
                new JarmuKategoria("SUV/Terepjáró (EV: 22 kWh/100km | Benzin/Dízel: 9.0 l/100km)", 22.0, 9.0)
            };

            List<string> kategoriaNevek = new List<string>();
            foreach (var kat in kategoriak)
            {
                kategoriaNevek.Add(kat.Nev);
            }

            int katIndex = BekerMenu("Válaszd ki a jármű kategóriáját:", kategoriaNevek);
            JarmuKategoria valasztottKategoria = kategoriak[katIndex];

            List<string> profilok = new List<string>
            {
                "Csak otthon",
                "Vegyesen",
                "Csak utcai villámtöltőkön"
            };

            int profilIndex = BekerMenu("Hol töltenéd leginkább az elektromos autót?", profilok);

            Console.WriteLine();
            bool vanNapelem = BekerIgenNem("Rendelkezik az ingatlan otthoni napelem rendszerrel?");
            bool kellWallbox = BekerIgenNem("Szükséges-e otthoni fali töltő (Wallbox) kiépítése?");

            Console.WriteLine("\n--- Aktuális energia- és üzemanyagárak ---");
            double uzemanyagAr = BekerTortSzam("1 liter üzemanyag ára (Ft): ");
            double alapAramAr = BekerTortSzam("1 kWh lakossági áram alapára (Ft): ");

            double otthoniEgysegar = vanNapelem ? 0.0 : alapAramAr;
            double utcaiAcEgysegar = alapAramAr * 1.50;
            double utcaiDcEgysegar = alapAramAr * 3.00;

            double sulyozottAramAr = 0.0;
            if (profilIndex == 0)
            {
                sulyozottAramAr = otthoniEgysegar;
            }
            else if (profilIndex == 1)
            {
                sulyozottAramAr = (0.5 * otthoniEgysegar) + (0.5 * utcaiAcEgysegar);
            }
            else
            {
                sulyozottAramAr = utcaiDcEgysegar;
            }
            double iceOsszKoltseg = (valasztottKategoria.IceFogyasztas / 100.0) * evesKm * futamido * uzemanyagAr;
            double evEnergiaKoltseg = (valasztottKategoria.EvFogyasztas / 100.0) * evesKm * futamido * sulyozottAramAr;
            double wallboxKoltseg = kellWallbox ? 350000.0 : 0.0;
            double evOsszKoltseg = evEnergiaKoltseg + wallboxKoltseg;

            long osszesKm = (long)evesKm * futamido;
            Console.WriteLine("\n==============================================================");
            Console.WriteLine($"KÖLTSÉGÖSSZESÍTŐ ({futamido} évre, összesen {osszesKm:N0} km-re)");
            Console.WriteLine("==============================================================");
            Console.WriteLine($"{"Költségtípus",-44} | {"Összeg (Ft)",15}");
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine($"{"Hagyományos autó üzemanyagköltsége",-44} | {iceOsszKoltseg,12:N0} Ft");
            Console.WriteLine($"{"Elektromos autó energiaköltsége",-44} | {evEnergiaKoltseg,12:N0} Ft");
            Console.WriteLine($"{"Elektromos autó beruházási felára (Wallbox)",-44} | {wallboxKoltseg,12:N0} Ft");
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine($"{"Elektromos autó MINDÖSSZESEN",-44} | {evOsszKoltseg,12:N0} Ft");
            Console.WriteLine("==============================================================");

            Console.WriteLine("\nVÉGEREDMÉNY ÉS JAVASLAT:");
            double kulonbseg = Math.Abs(iceOsszKoltseg - evOsszKoltseg);

            if (evOsszKoltseg < iceOsszKoltseg)
            {
                Console.WriteLine("-> Az ELEKTROMOS autó fenntartása éri meg jobban.");
                Console.WriteLine($"-> Megtakarítás a teljes futamidő alatt: {kulonbseg:N0} Ft");
            }
            else if (iceOsszKoltseg < evOsszKoltseg)
            {
                Console.WriteLine("-> A HAGYOMÁNYOS autó fenntartása éri meg jobban.");
                Console.WriteLine($"-> Megtakarítás a teljes futamidő alatt: {kulonbseg:N0} Ft");
            }
            else
            {
                Console.WriteLine("-> A két hajtáslánc fenntartási költsége pontosan megegyezik.");
            }
            Console.ReadKey();
        }

        static int BekerEgeszSzam(string uzenet)
        {
            while (true)
            {
                Console.Write(uzenet);
                string bemenet = Console.ReadLine();
                if (int.TryParse(bemenet, out int ertek) && ertek >= 0)
                {
                    return ertek;
                }
                Console.WriteLine($"Hiba: pozitív egész számot adj meg!");
            }
        }

        static double BekerTortSzam(string uzenet)
        {
            while (true)
            {
                Console.Write(uzenet);
                string bemenet = Console.ReadLine()?.Replace(',', '.');
                if (double.TryParse(bemenet, NumberStyles.Any, CultureInfo.InvariantCulture, out double ertek) && ertek > 0)
                {
                    return ertek;
                }
                Console.WriteLine("Hiba: Érvénytelen bemenet! Kérlek nullánál nagyobb számot adj meg.");
            }
        }

        static int BekerMenu(string cim, List<string> opciok)
        {
            Console.WriteLine($"\n{cim}");
            for (int i = 0; i < opciok.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {opciok[i]}");
            }

            while (true)
            {
                Console.Write($"Válassz egy sorszámot (1-{opciok.Count}): ");
                string bemenet = Console.ReadLine();
                if (int.TryParse(bemenet, out int valasztas) && valasztas >= 1 && valasztas <= opciok.Count)
                {
                    return valasztas - 1;
                }
                Console.WriteLine($"Hiba: Kérlek 1 és {opciok.Count} közötti számot adj meg!");
            }
        }

        static bool BekerIgenNem(string uzenet)
        {
            while (true)
            {
                Console.Write($"{uzenet} (I/N): ");
                string valasz = Console.ReadLine()?.Trim().ToUpper();
                if (valasz == "I" || valasz == "IGEN")
                {
                    return true;
                }
                if (valasz == "N" || valasz == "NEM")
                {
                    return false;
                }
                Console.WriteLine("Hiba: Kérlek 'I' (Igen) vagy 'N' (Nem) betűvel válaszolj!");
            }
        }
    }
}
