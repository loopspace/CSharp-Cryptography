using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModularArithmetic;


namespace ModularArithmetic
{
    public static class Tools
    {
        public static int Gcd(int a, int b)
        {
            if (a < 0)
            {
                a = -a;
            }
            if (b < 0)
            {
                b = -b;
            }
            if (b == 0)
            {
                return a;
            }
            if (a < b)
            {
                return Gcd(b, a);
            }
            return Gcd(a - b, b);
        }

        public static int ModInverse(int a, int m)
        {
            int c = 0;
            int b = 0;
            if (Gcd(a, m) != 1)
            {
                return 0;
            }
            a = (a % m + m) % m;
            while (b != 1)
            {
                c++;
                b += a;
                b %= m;
            }
            return c;
        }

        public static int Mod(int a, int m)
        {
            return ((a % m) + m) % m;
        }
    }
}

namespace Cryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            string plaintext;
            string ciphertext = "";
            bool encrypt;
            string[] keys = {
                "Specify the shift",
                "Specify the parameters",
                "Specify the keyword",
                "Specify the keyword",
                "Specify the keyword"
            };

            int encryptOpt = GetOptChoice(
                "What action do you want to take?",
                          'e',
                          0,
                          new string[] { "Encrypt", "Decrypt" },
                          args
                          );
            if (encryptOpt == 0)
            {
                encrypt = true;
            }
            else
            {
                encrypt = false;
            }
            int choice = GetOptChoice(
                       "Which cipher do you want to use?",
                       'c',
                       0,
                       new string[] { "Caesar", "Affine", "Substitution", "Vigenere", "Playfair" },
                       args
                          );

            string param = GetOptString(
                         keys[choice],
                         'p',
                         "",
                         args
                         );
            if (Console.IsInputRedirected)
            {
                using (StreamReader reader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding))
                {
                    plaintext = reader.ReadToEnd();
                }
            }
            else
            {
                plaintext = GetOptString(
                             "Enter the plaintext",
                             't',
                             "hello world",
                             args
                             );
            }

            switch (choice)
            {
                case 0:
                    ciphertext = Caesar(plaintext, param, encrypt);
                    break;
                case 1:
                    ciphertext = Affine(plaintext, param, encrypt);
                    break;
                case 2:
                    ciphertext = Substitution(plaintext, param, encrypt);
                    break;
                case 3:
                    ciphertext = Vigenere(plaintext, param, encrypt);
                    break;
                case 4:
                    ciphertext = Playfair(plaintext, param, encrypt);
                    break;
                default:
                    break;
            }
            Console.WriteLine(ciphertext);
            if (args.Length == 0)
            {
                Console.ReadKey();
            }
        }
        /*
            static bool getOptBool(string msg, char p, bool def, string[] args)
            {
                bool opt = def;
                if (args.Length > 0)
                {
                // Getting options from command line
                string optn = "";
                for (int i = 0; i < args.Length - 1; i++) {
                    if (args[i] == "-" + p)
                    {
                    optn = args[i+1];
                    }
                }
                if (optn != "") {
                    if (optn.ToLower() == "false")
                    {
                    opt = false;
                    }
                    else
                    {
                    opt = true;
                    }
                }
                }
                else
                {
                Console.WriteLine(msg);
                ConsoleKeyInfo response = Console.ReadKey(true);
                if (response.KeyChar == p)
                {
                    opt = true;
                }
                else
                {
                    opt = false;
                }
                }
                return opt;
            }
            */
        static string GetOptString(string msg, char p, string def, string[] args)
        {
            string opt = def;
            if (args.Length > 0)
            {
                // Getting options from command line
                string optn = "";
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-" + p)
                    {
                        optn = args[i + 1];
                    }
                }
                if (optn != "")
                {
                    opt = optn;
                }
            }
            else
            {
                Console.WriteLine(msg);
                opt = Console.ReadLine();
            }
            return opt;
        }

        static int GetOptChoice(string msg, char p, int def, string[] opts, string[] args)
        {
            int opt = def;
            if (args.Length > 0)
            {
                // Getting options from command line
                string optn = "";
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-" + p)
                    {
                        optn = args[i + 1];
                    }
                }
                if (optn != "")
                {
                    for (int i = 0; i < opts.Length; i++)
                    {
                        if (optn.ToLower() == opts[i].ToLower())
                        {
                            opt = i;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine(msg);
                for (int i = 0; i < opts.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, opts[i]);
                }
                ConsoleKeyInfo response = Console.ReadKey(true);
                if (response.KeyChar >= '0' && response.KeyChar < '0' + opts.Length)
                {
                    opt = response.KeyChar - '0';
                }
                Console.WriteLine("  {0}", opts[opt]);
            }
            return opt;
        }

        static string CleanText(string plaintext)
        {
            string cleantext = "";
            char c;
            for (int i = 0; i < plaintext.Length; i++)
            {
                c = plaintext[i];
                if (c >= 'A' && c <= 'Z')
                {
                    c -= 'A';
                    c += 'a';
                    cleantext += c;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    cleantext += c;
                }
            }

            return cleantext;
        }

        static string Caesar(string plaintext, string shiftResponse, bool encrypt)
        {
            int shift;

            if (!int.TryParse(shiftResponse, out shift))
            {
                if (shiftResponse[0] >= 'A' && shiftResponse[0] <= 'Z')
                {
                    shift = shiftResponse[0] - 'A';
                }
                else if (shiftResponse[0] >= 'a' && shiftResponse[0] <= 'z')
                {
                    shift = shiftResponse[0] - 'a';
                }
            }
            if (!encrypt)
            {
                shift = (26 - shift) % 26;
            }
            var ciphertext = new StringBuilder();

            foreach (char c in plaintext)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    ciphertext.Append((char)((c - 'A' + shift) % 26 + 'A'));
                }
                else if (c >= 'a' && c <= 'z')
                {
                    ciphertext.Append((char)((c - 'a' + shift) % 26 + 'a'));
                }
                else
                {
                    ciphertext.Append(c);
                }
            }

            return ciphertext.ToString();
        }

        static string Substitution(string plaintext, string keyword, bool encrypt)
        {
            keyword = CleanText(keyword);
            int[] keys = new int[26];
	    int[] rkeys = new int[26];
	    int a;
	    int j = 0;

	    for (int i = 0; i < 26; i++) {
		keys[i] = -1;
	    }
	    
	    foreach (char c in keyword)
	    {
		a = c - 'a';
		if (!keys.Contains(a)) {
		    keys[j] = a;
		    j++;
		}
	    }

	    int b = keys[j-1] + 1;
	    foreach (char c in "abcdefghijklmnopqrstuvwxyz")
	    {
		a = Tools.Mod( c - 'a' + b,26);
		if (!keys.Contains(a)) {
		    keys[j] = a;
		    j++;
		}
	    }

	    if (!encrypt)
	    {
		for (int i = 0; i < 26; i++)
		{
		    rkeys[keys[i]] = i;
		}
		for (int i = 0; i < 26; i++)
		{
		    keys[i] = rkeys[i];
		}

	    }

            var ciphertext = new StringBuilder();

            foreach (char c in plaintext)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    ciphertext.Append((char)('A' + keys[c - 'A']));
                }
                else if (c >= 'a' && c <= 'z')
                {
                    ciphertext.Append((char)('a' + keys[c - 'a']));
                }
                else
                {
                    ciphertext.Append(c);
                }
            }

            return ciphertext.ToString();
        }

        static string Affine(string plaintext, string response, bool encrypt)
        {
            int a = 1;
            int b = 0;
            int x;
            int y;
            int xx;
            int yy;
            bool validResponse = false;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] toStrings = { "-", "->", ">" };
            string[] p;
            string[] aff;

            p = response.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
            if (p.Length != 2)
            {
                validResponse = false;
            }
            else
            {
                if (int.TryParse(p[0], out a))
                {
                    if (Tools.Gcd(a, 26) != 1)
                    {
                        validResponse = false;
                    }
                    else if (int.TryParse(p[1], out b))
                    {
                        validResponse = true;
                    }
                    else
                    {
                        validResponse = false;
                    }
                }
                else
                {
                    aff = p[0].Split(toStrings, System.StringSplitOptions.RemoveEmptyEntries);
                    if (aff.Length != 2)
                    {
                        validResponse = false;
                    }
                    else
                    {
                        x = aff[0][0];
                        xx = aff[1][0];
                        aff = p[1].Split(toStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (aff.Length != 2)
                        {
                            validResponse = false;
                        }
                        else
                        {
                            y = aff[0][0];
                            yy = aff[1][0];

                            if (Tools.Gcd(x - y, 26) != 1)
                            {
                                validResponse = false;
                            }
                            else
                            {
                                a = Tools.Mod((yy - xx) * Tools.ModInverse(y - x, 26),26);
                                b = Tools.Mod(xx - a * x,26);
                                validResponse = true;
                            }

                        }
                    }
                }
            }
            if (!validResponse)
            {
                Console.WriteLine("Invalid parameters given for the affine cipher.");
                return "";
            }

            if (!encrypt)
            {
                a = Tools.ModInverse(a, 26);
                b = Tools.Mod(-a*b,26);
            }
            Console.WriteLine("Parameters: {0} x + {1}", a, b);


            var ciphertext = new StringBuilder();

            foreach (char c in plaintext)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    ciphertext.Append((char)(Tools.Mod(a * (c - 'A') + b, 26) + 'A'));
                }
                else if (c >= 'a' && c <= 'z')
                {
                    ciphertext.Append((char)(Tools.Mod(a * (c - 'a') + b, 26) + 'a'));
                }
                else
                {
                    ciphertext.Append(c);
                }
            }

            return ciphertext.ToString();

        }

        static string Vigenere(string plaintext, string keyword, bool encrypt)
        {
            string ciphertext = "";
            keyword = keyword.ToLower();
            int[] keys = new int[keyword.Length];

            if (encrypt)
            {
                for (int i = 0; i < keyword.Length; i++)
                {
                    keys[i] = keyword[i] - 'a';
                }
            }
            else
            {
                for (int i = 0; i < keyword.Length; i++)
                {
                    keys[i] = Tools.Mod(26 - (keyword[i] - 'a'), 26);
                }
            }

            int k = 0;
            char c;
            for (int i = 0; i < plaintext.Length; i++)
            {
                if (plaintext[i] >= 'A' && plaintext[i] <= 'Z')
                {
                    c = (char)(Tools.Mod(plaintext[i] - 'A' + keys[k], 26) + 'A');
                    k++;
                }
                else if (plaintext[i] >= 'a' && plaintext[i] <= 'z')
                {
                    c = (char)(Tools.Mod(plaintext[i] - 'a' + keys[k], 26) + 'a');
                    k++;
                }
                else
                {
                    c = plaintext[i];
                }
                ciphertext += c;
                k %= keyword.Length;
            }

            return ciphertext;
        }

        static string Playfair(string plaintext, string keyword, bool encrypt)
        {
            string cleantext = CleanText(plaintext);
            string ciphertext = "";
            int dir;
            if (encrypt)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }

            char[,] playfair = new char[5, 5];
            int[] invplayfair = new int[26];

            keyword = "j" + keyword.ToLower() + "abcdefghijklmnopqrstuvwxyz";
            keyword = new String(keyword.Distinct().ToArray());
            keyword = keyword.Substring(1);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    playfair[i, j] = keyword[5 * i + j];
                    invplayfair[keyword[5 * i + j] - 'a'] = 5 * i + j;
                }
            }
            invplayfair['j' - 'a'] = invplayfair['i' - 'a'];
            Console.WriteLine(string.Join(",", invplayfair));
            string output;
            for (int i = 0; i < 5; i++)
            {
                output = "";
                for (int j = 0; j < 5; j++)
                {
                    output += playfair[i, j];
                }
                Console.WriteLine(output);
            }
            int loc = 0;
            char xtra = 'x';
            char a;
            char b;
            while (loc < cleantext.Length)
            {
                a = cleantext[loc];
                if (a == 'j')
                {
                    a = 'i';
                }
                if (loc < cleantext.Length - 1)
                {
                    if (cleantext[loc] == cleantext[loc + 1])
                    {
                        b = xtra;
                    }
                    else
                    {
                        b = cleantext[loc + 1];
                        loc++;
                    }
                }
                else
                {
                    b = xtra;
                }
                if (b == 'j')
                {
                    b = 'i';
                }
                loc++;
                a -= 'a';
                b -= 'a';
                Console.WriteLine("{0} is at {1} and {2} is at {3}", a + 'a', invplayfair[a], b + 'a', invplayfair[b]);
                if (invplayfair[a] / 5 == invplayfair[b] / 5)
                {
                    // Same row
                    ciphertext += playfair[invplayfair[a] / 5, Tools.Mod(invplayfair[a] % 5 + dir, 5)];
                    ciphertext += playfair[invplayfair[b] / 5, Tools.Mod(invplayfair[b] % 5 + dir, 5)];
                }
                else if (invplayfair[a] % 5 == invplayfair[b] % 5)
                {
                    // Same column
                    ciphertext += playfair[Tools.Mod(invplayfair[a] / 5 + dir, 5), invplayfair[a] % 5];
                    ciphertext += playfair[Tools.Mod(invplayfair[b] / 5 + dir, 5), invplayfair[b] % 5];

                }
                else
                {
                    // All the rest
                    ciphertext += playfair[invplayfair[a] / 5, invplayfair[b] % 5];
                    ciphertext += playfair[invplayfair[b] / 5, invplayfair[a] % 5];
                }
            }

            return ciphertext;
        }

    }
}
