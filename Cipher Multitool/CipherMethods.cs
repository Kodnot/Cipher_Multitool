using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cipher_Multitool
{
    static class CipherMethods
    {        
        static public string CaesarShift(string input, int shift)
        {
            // Remove useless shift overflow
            shift = shift % 26;
            char[] buffer = input.ToCharArray();
            for (int i = 0; i < buffer.Length; ++i)
            {
                char letter = buffer[i];
                if (!char.IsLetter(letter))
                {
                    continue;
                }
                bool wasUpper = char.IsUpper(letter);
                // Add shift
                letter = (char)(letter + shift);

                if (wasUpper && letter > 'Z' || !wasUpper && letter > 'z')
                {
                    letter = (char)(letter - 26);
                }
                else if (wasUpper && letter < 'A' || !wasUpper && letter < 'a')
                {
                    letter = (char)(letter + 26);
                }
                buffer[i] = letter;
            }
            return new string(buffer);
        }

        static public string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0') + " ");
            }
            return sb.ToString();
        }

        static public string BinaryToString(string data)
        {
            data = Regex.Replace(data, @"\s+", ""); // Remove all whitespace
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        static public string VigenereEncode(string data, string key)
        {
            return VigenereCipher.Encode(data, key);
        }
        
        static public string VigenereDecode(string data, string key)
        {
            return VigenereCipher.Decode(data, key);
        }

        static public string SimpleSubstitutionEncode(string data, string key)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            key = new string(key.ToLower().Distinct().ToArray());
            foreach (char c in key)
            {
                alphabet = Regex.Replace(alphabet, c + "+", "");
            }
            alphabet = key + alphabet;

            Dictionary<char, char> substitution = new Dictionary<char, char>();
            for (char c = 'a'; c <= 'z'; ++c)
            {
                substitution[c] = alphabet[c - 'a'];
            }

            char[] buffer = data.ToCharArray();
            for (int i = 0; i < buffer.Length; ++i)
            {
                char curChar = buffer[i];
                if (char.IsLetter(curChar))
                {
                    if (char.IsUpper(curChar))
                    {
                        buffer[i] = char.ToUpper(substitution[char.ToLower(curChar)]);
                    }
                    else
                    {
                        buffer[i] = substitution[curChar];
                    }
                }
            }
            return new string(buffer);
        }

        static public string SimpleSubstitutionDecode(string data, string key)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            key = new string(key.ToLower().Distinct().ToArray());
            foreach (char c in key)
            {
                alphabet = Regex.Replace(alphabet, c + "+", "");
            }
            alphabet = key + alphabet;

            Dictionary<char, char> substitution = new Dictionary<char, char>();
            for (char c = 'a'; c <= 'z'; ++c)
            {
                substitution[alphabet[c - 'a']] = c;
            }

            char[] buffer = data.ToCharArray();
            for (int i = 0; i < buffer.Length; ++i)
            {
                char curChar = buffer[i];
                if (char.IsLetter(curChar))
                {
                    if (char.IsUpper(curChar))
                    {
                        buffer[i] = char.ToUpper(substitution[char.ToLower(curChar)]);
                    }
                    else
                    {
                        buffer[i] = substitution[curChar];
                    }
                }
            }
            return new string(buffer);
        }
    }
}
