using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cipher_Multitool
{
    static class VigenereCipher
    {
        static Dictionary<char, char> LetterMapping_lower = new Dictionary<char, char>();
        static Dictionary<char, char> LetterMapping_upper = new Dictionary<char, char>();

        static public string Encode(string input, string inputKey)
        {
            SetDictionaries();
            string output = "";
            Key key = new Key(inputKey);
            for (int i = 0; i < input.Length; ++i)
            {
                if (!char.IsLetter(input[i]))
                    output += input[i];
                else
                    output += Encrypt(input[i], key.GetShift());
            }
            return output;
        }

        static public string Decode(string input, string inputKey)
        {
            string output = "";
            Key key = new Key(inputKey);
            foreach (char i in input)
            {
                if (!char.IsLetter(i))
                    output += i;
                else
                    output += Encrypt(i, -key.GetShift());
            }
            return output;
        }

        static char Encrypt(char k, int shift)
        {
            bool wasUpper = char.IsUpper(k);
            bool wasLower = char.IsLower(k);
            // Add shift
            if (wasUpper)
            {
                k += (char)shift;
                k = LetterMapping_upper[k];
            }
            else if (wasLower)
            {
                k += (char)shift;
                k = LetterMapping_lower[k];
            }
            return k;
        }

        static void SetDictionaries()
        {
            int index = 0;
            //ASCII 71 = G; 71+26 = 97 = a
            for (char x = 'G'; x < 'a'; x++)
            {
                LetterMapping_lower[x] = (char)('a' + (char)index);
                index++;
            }

            index = 0;
            for (char x = 'a'; x <= 'z'; x++)
            {
                LetterMapping_lower[x] = x;
            }

            for (char x = '{'; x <= '{' + (char)(26); x++)
            {
                LetterMapping_lower[x] = (char)('a' + (char)index);
                index++;
            }

            index = 0;
            for (char x = (char)(39); x <= (char)(39) + (char)(26); x++)
            {
                LetterMapping_upper[x] = (char)('A' + (char)index);
                index++;
            }
            for (char x = 'A'; x <= 'Z'; x++)
            {
                LetterMapping_upper[x] = x;
            }
            index = 0;
            for (char x = '['; x <= 'u'; x++)
            {
                LetterMapping_upper[x] = (char)('A' + (char)index);
                index++;
            }
        }
    }

    class Key
    {
        int index = 0;
        List<int> shifts = new List<int>();
        void SetShifts(string key)
        {
            //reikia tikrint ar input yra raides ir pns
            foreach (char x in key)
            {
                /* Black magic? */
                if (char.IsLower(x))
                    shifts.Add(x - 97);
                else if (char.IsUpper(x))
                    shifts.Add(x - 65);
            }
        }
        public int GetShift()
        {
            int shift = shifts[index];
            index++;
            if (index == shifts.Count)
                index = 0;
            return shift;
        }

        public Key(string key)
        {
            SetShifts(key);
        }
    }
}
