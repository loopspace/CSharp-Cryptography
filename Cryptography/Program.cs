using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModularArithmetic;

namespace Cryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            string plaintext;
            string ciphertext;
            bool running = true;
            bool validChoice = false;
            ConsoleKeyInfo yn;
            ConsoleKeyInfo choice;
            string[] ciphers = { "Caesar", "Affine" };

            while (running)
            {
                validChoice = false;
                Console.WriteLine("Enter the plain text to be encrypted:");
                plaintext = Console.ReadLine();
                ciphertext = "";
                Console.WriteLine("Which cipher would you like to use?");
                for (int i = 0; i < ciphers.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, ciphers[i]);
                }
                while (!validChoice)
                {
                    choice = Console.ReadKey(true);
                    switch (choice.KeyChar)
                    {
                        case '0':
                            ciphertext = Caesar(plaintext);
                            validChoice = true;
                            break;
                        case '1':
                            ciphertext = Affine(plaintext);
                            validChoice = true;
                            break;
                        default:
                            Console.WriteLine("Please select from the list.");
                            break;
                    }
                }
                Console.WriteLine("The encrpted text is: {0}", ciphertext);
                Console.WriteLine("Encrypt/decrypt another? (y/n)");
                yn = Console.ReadKey(true);
                if (yn.KeyChar == 'N' || yn.KeyChar == 'n')
                {
                    running = false;
                }
            }
        }

        static string Caesar(string plaintext)
        {
            int shift = 0;
            string shiftResponse;
            bool validShift = false;

            Console.WriteLine("Enter the shift amount:");

            while (!validShift)
            {
                shiftResponse = Console.ReadLine();
                if (int.TryParse(shiftResponse, out shift))
                {
                    validShift = true;
                }
                else
                {
                    if (shiftResponse[0] >= 'A' && shiftResponse[0] <= 'Z')
                    {
                        shift = shiftResponse[0] - 'A';
                        validShift = true;
                    }
                    else if (shiftResponse[0] >= 'a' && shiftResponse[0] <= 'z')
                    {
                        shift = shiftResponse[0] - 'a';
                        validShift = true;
                    }
                    else
                    {
                        Console.WriteLine("Sorry, I couldn't figure out the shift.  Please try again:");
                    }
                }
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

        static string Affine(string plaintext)
        {
            int a = 1;
            int b = 0;
            int x;
            int y;
            int xx;
            int yy;
            string response;
            bool validResponse = false;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] toStrings = { "-", "->", ">" };
            string[] p;
            string[] aff;

            Console.WriteLine("Enter the parameters:");

            while (!validResponse)
            {
                response = Console.ReadLine();

                p = response.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);
                if (p.Length != 2)
                {
                    Console.WriteLine("I couldn't figure out two parameters, please try again.");
                }
                else
                {
                    if (int.TryParse(p[0], out a))
                    {
                        if (Tools.gcd(a, 26) != 1) {
                            Console.WriteLine("Sorry, that's an invalid multiplier.  Please try again:");
                        }
                        else if (int.TryParse(p[1], out b))
                        {
                            validResponse = true;
                        } else
                        {
                            Console.WriteLine("I couldn't figure out the second parameter.  Please try again:");
                        }
                    }
                    else
                    {
                        aff = p[0].Split(toStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (aff.Length != 2)
                        {
                            Console.WriteLine("I couldn't figure out the first parameter, please try again.");
                        }
                        else
                        {
                            x = aff[0][0];
                            xx = aff[1][0];
                            aff = p[1].Split(toStrings, System.StringSplitOptions.RemoveEmptyEntries);
                            if (aff.Length != 2)
                            {
                                Console.WriteLine("I couldn't figure out the second parameter, please try again.");
                            }
                            else
                            {
                                y = aff[0][0];
                                yy = aff[1][0];

                                if (Tools.gcd(x - y, 26) != 1)
                                {
                                    Console.WriteLine("Sorry, I can't figure out the multiplier from those letters, please try again.");
                                }
                                else
                                {
                                    a = (yy - xx) * Tools.modinverse(y - x, 26);
                                    b = xx - a * x;
                                    validResponse = true;
                                }

                            }
                        }
                    }
                }
            }
            var ciphertext = new StringBuilder();

            foreach (char c in plaintext)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    ciphertext.Append((char)((a * (c - 'A') + b) % 26 + 'A'));
                }
                else if (c >= 'a' && c <= 'z')
                {
                    ciphertext.Append((char)((a * (c - 'a') + b) % 26 + 'a'));
                }
                else
                {
                    ciphertext.Append(c);
                }
            }

            return ciphertext.ToString();

        }

    }
}

namespace ModularArithmetic
{
    public static class Tools
    {
        public static int gcd(int a, int b)
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
                return gcd(b, a);
            }
            return gcd(a - b, b);
        }

        public static int modinverse(int a, int m)
        {
            int c = 0;
            int b = 0;
            if (gcd(a, m) != 1)
            {
                return 0;
            }
            a = (a%m + m)%m;
            while (b != 1)
            {
                c++;
                b += a;
                b %= m;
            }
            return c;
        }
    }
}