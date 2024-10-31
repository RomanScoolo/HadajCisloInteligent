using System;
using System.Data.SqlTypes;

internal static class ProgramHelpers
{
    // inteligentne hadanie cisiel - klasicka hra, ale okorenena zabavou simulujucou inteligenciu
    // algoritmy:
    // - komentovane hadanie voci idealnemu stavu - DONE
    // - komentovane ak hada na tesno, alebo chybne - DONE
    // - poradit ako hadat, idealny matematicky postup - DONE
    // - zdrbat ak hada nelogicky, alebo opakovane - DONE
    // - uhybanie, tj pocitac sa bude snazit schovat cislo - DONE
    // - urob radost, tj podsunutie akoze uhadol skor - DONE
    // - statistika hadania, komentare k tomu - DONE
    // - kontroly ci cisla su v intervale - DONE
    // - vizualizovat proces hadania + komentare - akoze vyukovy rezim - DONE
    // - typ hry - hadanie podla vzdialenosti - DONE
    // - kontroly zlych vstupov (pismeno miesto cisla) - DONE
    // - typ hry - cifrspion, davat ine napovedy, napr. ze uhadol si cifru, alebo pocet cifier, parne, atd.
    // - vtipna hra, ze dava nahodne komentare k hadaniu
    // - ukladat statistiku generovanych cisel, a davat cisla ktore neboli, resp. boli najmenej
    // - umoznit zadanie maximalneho cisla

    private static void Main(string[] args)
    {
        Console.WriteLine("Vitaj v Hadaj cislo, zelam ti prijemnu zabavu");

        string meno;
        Console.Write("Ako sa volas ? = ");
        meno = Console.ReadLine();
        if (meno[0] != System.Char.ToUpper(meno[0]))
            Console.WriteLine("... zaujimave meno, ze sa nezacina velkym pismenom");
        if (meno.Length < 3)
            Console.WriteLine("... to asi nebude cesko-slovenske meno, vsak ?");
        bool pismenka = true;
        for (int i = 0; i < meno.Length; i++)
            if ((System.Char.ToUpper(meno[i]) < 'A') || (System.Char.ToUpper(meno[i]) > 'Z'))
                pismenka = false;
        if (!pismenka)
            Console.WriteLine("... to robot so mnou hraje alebo pouzivas nickname ?");

        Random _random = new Random();
        const int maxcislo = 100;
        // Console.WriteLine("Najblizsi vecsi Log2 pre " + Convert.ToString(maxcislo) + " je " + Convert.ToString(Math.Round(Math.Log(maxcislo)/Math.Log(2) + 0.5)));
        // const int maxpokus = 7; // nahradene vypoctom ak by sa menilo max cislo
        int maxpokus = (int)Math.Round(Math.Log(maxcislo) / Math.Log(2) + 0.5);
        string odpoved;
        char[] odpovede = new char[maxcislo]; // odpovede
        int num, pocet, tip, spodok, vrsok, level, deltadole, deltahore, typhry, vzdialenost;
        int[] pokusy = new int[1000];
        int pocet_pokusov = 0;
        bool vyuka;
        float priemer;
        do // hlavna herna slucka
        {
            num = _random.Next(maxcislo) + 1; // vygeneruje cislo
            Console.WriteLine("Ahoj " + meno + " myslim si cislo od 1 do " + Convert.ToString(maxcislo) + " ...");
            pocet = 0;        // pocet hadani
            tip = 0;          // hadanie uzivatela
            spodok = 1;       // spodny interval hadania
            vrsok = maxcislo; // horny interval hadania
            vyuka = false;    // bez vyuky ako default
            for (int i = 0; i < maxcislo; i++) // vycisti hadane cisla
                odpovede[i] = '0';

            // vyber typu hry
            Console.Write("Akym sposobom chces hadat ? 0 - klasicky, 1 - vzdialenostou, 2 - cifrspion = ");
            typhry = Zadaj_cislo(0, 2);

            if (typhry == 0)
            {
                // obtiaznost hry 6 - schovava cisla, 1 - necha hned uhadnut, 2/3 - bude generovat 33% sancu na uhadnutie, 4/5 - normalna hra, 0 - nahodne vyberie
                Console.Write("Level hry [1 super stastie, 2-3 viac stastia, 4-5 normalna hra, 6 super tazke, 7 vyuka/komentare, 0 nahodny level] = ");  // zadat rucne kvoli odladeniu
                level = Zadaj_cislo(0, 7);
                if (level == 0)
                    level = _random.Next(6) + 1;
                if (level == 7)
                    vyuka = true;

                // hadanie cisiel - klasicka varianta
                while (num != tip)
                {
                    pocet++;
                    if (vyuka)
                    {
                        Console.WriteLine("Hadaj v intervale " + Convert.ToString(spodok) + " az " + Convert.ToString(vrsok));
                        Console.WriteLine("Navrhujem ti hadat " + Convert.ToString((int)(spodok + vrsok) / 2));
                    }

                    Console.Write("Skus uhadnut " + meno + " na ake cislo myslim = ");
                    tip = Zadaj_cislo(1, maxcislo);

                    if (level == 1)
                        num = tip;
                    else if (((level == 2) || (level == 3)) && (odpovede[tip - 1] == '0') && (_random.Next(3) == 2)) // sanca na trefu je tretinova
                        num = tip;
                    else if (level == 6)
                    {
                        deltadole = Math.Abs(tip - spodok);
                        deltahore = Math.Abs(vrsok - tip);
                        if ((deltadole * deltahore) <= 1)
                            num = tip;
                        else if ((Math.Abs(deltahore - deltadole) < 2) && ((deltadole * deltahore) > 2)) // ak je rozdiel maly, tak nechaj nahodu vybrat ktorym smerom ho posunie
                        {
                            if (_random.Next(2) == 1)
                                num = (vrsok + tip) / 2;
                            else
                                num = (tip + spodok) / 2;
                        }
                        else
                        {
                            if (deltahore > deltadole)
                                num = (vrsok + tip) / 2;
                            else
                                num = (tip + spodok) / 2;
                        }
                    }

                    if ((odpovede[tip - 1] == '1') && (vyuka))
                        Console.WriteLine("Toto cislo si uz hadal, skoda premrhaneho pokusu " + meno + " ...");
                    odpovede[tip - 1] = '1';

                    if ((Math.Abs(tip - num) == 1) && (vyuka))
                        Console.Write("Tesne, ale ");

                    if ((tip >= spodok) && (tip <= vrsok))
                    {
                        if (num > tip)
                        {
                            Console.WriteLine(meno + " hadaj viac");
                            spodok = tip;
                        }
                        else if (num < tip)
                        {
                            Console.WriteLine(meno + " hadaj menej");
                            vrsok = tip;
                        }
                    }
                    else if (vyuka)
                    {
                        Console.WriteLine("Preco hadas mimo interval " + Convert.ToString(spodok) + " az " + Convert.ToString(vrsok) + " ?");
                        Console.WriteLine("Hadane cislo je urcite v tomto intervale. " + meno + " popremyslaj a skus znovu.");
                    }

                }
            }
            else if (typhry == 1) // typ hry 1 - podla vzdialenosti
            {
                while (num != tip)
                {
                    pocet++;
                    Console.Write("Skus uhadnut " + meno + " na ake cislo myslim = ");
                    tip = Zadaj_cislo(1, maxcislo);

                    if (odpovede[tip - 1] == '1')
                        Console.WriteLine("Toto cislo si uz hadal, premrhany pokus ...");
                    odpovede[tip - 1] = '1';

                    if (num != tip)
                    {
                        // vypocet vzdialenosti
                        vzdialenost = (int)Math.Round(Math.Log(Math.Abs(tip - num)) / Math.Log(2) + 0.5);
                        // Console.WriteLine("Vzdialenost " + Convert.ToString(Math.Abs(tip - num)) + " a Log2 je " + Convert.ToString(vzdialenost)); // pre odladenie
                        if (Math.Abs(tip - num) == 1)
                            Console.WriteLine("Tesne vedla ...");
                        else if (vzdialenost <= 2)
                            Console.WriteLine("Si velmi blizko ...");
                        else if (vzdialenost <= 3)
                            Console.WriteLine("Si blizko ...");
                        else if (vzdialenost <= 4)
                            Console.WriteLine("Tak nejak stredne ...");
                        else if (vzdialenost <= 5)
                            Console.WriteLine("Si daleko ...");
                        else if (vzdialenost <= 6)
                            Console.WriteLine("Si velmi daleko ...");
                        else
                            Console.WriteLine("Si v nedohladne ...");
                    }
                }
            }
            else // typ hry 2 - cifrspion = rozny typy napoved
            {
                // Console.WriteLine("Toto este nie je implementovane, ale mozes sa uz tesit :-)");
                // tip = num;

                // priprava premennych pre analyzu a hadanie
                // - cifry
                int pocet_cifier_zadaneho;
                string zadane_cislo, hadane_cislo = Convert.ToString(num);
                int[] cifry_zadaneho = new int[10];
                int[] cifry_hadaneho = new int[10];
                for (int i = 0; i < 10; i++)
                    cifry_hadaneho[i] = 0;
                for (int i = 0; i < hadane_cislo.Length; i++)
                    cifry_hadaneho[(int)(hadane_cislo[i] - '0')] = 1;
                // - delitele
                // vsetky delitele cisla num
                int delitele = 0;
                int[] delitel = new int[(int)Math.Sqrt(num)];
                for (int i = 1; i <= Math.Sqrt(num); i++)
                    if ((num % i) == 0)
                    {
                        delitel[delitele] = i;
                        delitele++;
                        // Console.Write(i + " ");
                        if ((num / i) != i)
                        {
                            delitel[delitele] = num / i;
                            // Console.Write((num / i) + " ");
                            delitele++;
                        }
                    }
                // Console.WriteLine("/ pocet delitelov = " + Convert.ToString(delitele));
                // test prvociselnosti cisla n
                bool prvocislo = false;
                if ((num > 1) && (delitele == 2))
                    prvocislo = true;
                // Console.WriteLine(num + " je prvocislo");

                while (num != tip)
                {
                    pocet++;
                    Console.Write("Skus uhadnut " + meno + " na ake cislo myslim = ");
                    tip = Zadaj_cislo(1, maxcislo);

                    if (odpovede[tip - 1] == '1')
                        Console.WriteLine("Toto cislo si uz hadal, premrhany pokus ...");
                    odpovede[tip - 1] = '1';

                    if (num != tip)
                    {
                        // rozny typy napoved, vymysliet priority
                        // ak uhadne vsetky cifry - DONE
                        // ak uhadne aspon jednu cifru - DONE
                        // ak uhadne pocet cifier - DONE
                        // parne/neparne ? - DONE
                        // zadane je nasobok hadaneho cisla, a naopak - DONE
                        // ze zadane cislo je prvocislo - DONE
                        // pocet delitelov
                        // niektory z delitelov (okrem 1-cky a seba sameho)
                        // niektory zo spolocnych delitelov (okrem 1-cky a seba sameho)
                        // dat napovedu bez ohladu na zadane cislo ? (akoze nejaka charakteristika hadaneho cisla)
                        // ohodnotit silu napovedy (t.j. ako zuzi zvysne cisle)
                        // davkovat napovedy ? (akoze jednu na hadanie, bud nahodne alebo podla sily)
                        // pametat si poskytnute napovedy, nedavat stejnu napovedu 2x

                        pocet_cifier_zadaneho = 0;
                        for (int i = 0; i < 10; i++)
                            cifry_zadaneho[i] = 2;
                        zadane_cislo = Convert.ToString(tip);
                        for (int i = 0; i < zadane_cislo.Length; i++)
                            cifry_zadaneho[(int)(zadane_cislo[i] - '0')] = 1;
                        for (int i = 0; i < 10; i++)
                            if (cifry_zadaneho[i] == cifry_hadaneho[i])
                                pocet_cifier_zadaneho++;

                        if (pocet_cifier_zadaneho == 0)
                            Console.WriteLine("Bohuzial, neuhadol si ziadnu cifru hadaneho cisla");
                        else if (pocet_cifier_zadaneho == hadane_cislo.Length)
                            Console.WriteLine("Uhadol si vsetky cifry hadaneho cisla!");
                        else if (pocet_cifier_zadaneho == 1)
                            Console.WriteLine("Uhadol si 1 cifru z hadaneho cisla");
                        else if ((pocet_cifier_zadaneho > 1) && (pocet_cifier_zadaneho < 5))
                            Console.WriteLine("Uhadol si " + Convert.ToString(pocet_cifier_zadaneho) + " cifry z hadaneho cisla");
                        else
                            Console.WriteLine("Uhadol si " + Convert.ToString(pocet_cifier_zadaneho) + " cifier z hadaneho cisla");

                        if (zadane_cislo.Length == hadane_cislo.Length)
                            Console.WriteLine("Uhadol si, ze hadane cislo je " + Convert.ToString(zadane_cislo.Length) + "-ciferne");

                        if ((tip % num) == 0)
                            Console.WriteLine("Zadane cislo je delitelne hadanym");

                        if ((num % tip) == 0)
                            Console.WriteLine("Hadane cislo je delitelne zadanym");

                        if (((tip % 2) == 0) && ((num % 2) == 0))
                            Console.WriteLine("Hadane cislo je tiez parne.");
                        else if (((tip % 2) == 1) && ((num % 2) == 1))
                            Console.WriteLine("Hadane cislo je tiez neparne.");

                        if (prvocislo)
                            Console.WriteLine("Hadane cislo je prvocislo.");
                        else if ((delitele >= 3) && (delitele <= 4))
                            Console.WriteLine("Hadane cislo ma " + Convert.ToString(delitele) + " delitele");
                        else
                            Console.WriteLine("Hadane cislo ma " + Convert.ToString(delitele) + " delitelov");
                    }
                }
            }

            // zaverecne ohodnotenia
            Console.WriteLine("Uhadol si na " + Convert.ToString(pocet) + " pokus.");
            if (pocet > maxpokus)
            {
                Console.WriteLine("Slo by to aj rychlejsie, " + meno + ", nabuduce sa skus viac posnazit.");
            }
            else if (pocet == maxpokus)
            {
                Console.WriteLine(meno + ", uhadol si na poslednu chvilu.");
            }
            else if (pocet == 1)
            {
                Console.WriteLine("Wow, " + meno + ", ty musis byt telepat!");
            }
            else if (pocet < maxpokus)
            {
                Console.WriteLine("Sikovne, " + meno + ", ani ja by som asi neuhadol skor.");
            }

            priemer = 0;
            if (pocet_pokusov > 0)
            {
                if (pocet > pokusy[pocet_pokusov - 1])
                    Console.WriteLine("Hadanie bolo slabsie nez predoslu hru.");
                else if (pocet < pokusy[pocet_pokusov - 1])
                    Console.WriteLine("Hadanie bolo lepsie nez predoslu hru.");
                else
                    Console.WriteLine("Nahoda alebo sikovnost ? Uhadol si rovnako rychlo ako predoslu hru.");
                for (int i = 0; i < pocet_pokusov; i++)
                    priemer += pokusy[i];
                priemer /= pocet_pokusov;
                if (pocet > priemer)
                    Console.WriteLine("Hadanie bolo slabsie nez doterajsi priemer.");
                else if (pocet < priemer)
                    Console.WriteLine("Hadanie bolo lepsie nez doterajsi priemer.");
                else
                    Console.WriteLine("Drzis si standard!");
            }
            pokusy[pocet_pokusov] = pocet;
            pocet_pokusov++;

            Console.WriteLine("... tak co " + meno + ", hrajeme znovu ?");
            Console.Write("ano/nie ? = ");
            odpoved = Velke_pismena(Console.ReadLine());

        } while ((odpoved != "NIE") && (odpoved != "N") && (pocet_pokusov < 1000));

    }

    private static int Zadaj_cislo(int cislo_min, int cislo_max)
    {
        bool nespravne = true;
        int cislo = 0;
        string vstup;
        do
        {
            while (nespravne)
            {
                vstup = Console.ReadLine();
                if (!Int32.TryParse(vstup, out cislo))
                    Console.Write("Nespravny vstup, prosim zadaj ciselnu hodnotu = ");
                else
                    nespravne = false;
            }
            if ((cislo >= cislo_min) && (cislo <= cislo_max))
                nespravne = false;
            else
                Console.Write("Nespravny vstup, zadaj cislo v rozsahu " + Convert.ToString(cislo_min) + " az " + Convert.ToString(cislo_max) + " = ");
        } while (nespravne);

        return cislo;
    }

    // function to convert all characters in string to upper case
    static string Velke_pismena(string s)
    {
        string vysledok = "";
        for (int i = 0; i < s.Length; i++)
        {
            // only convert lower case characters
            if (s[i] > 95)
                vysledok += Convert.ToChar(s[i] - 32);
            else
                vysledok += s[i];
        }
        return vysledok;
    }
}