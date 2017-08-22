using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cipher_Multitool
{
    // Text analysis. Only supprots ASCII characters 0-127
    class Analysis
    {
        private string initialData;
        private char[] characterCounts;

        public Analysis(string data)
        {
            initialData = data;
            characterCounts = new char[128];
            foreach (char ch in data)
            {
                if (ch < 127) // Only ASCII values are supported
                    characterCounts[ch]++;
            }
        }

        public List<KeyValuePair<int, float>> FindPeriodicIoC(int k)
        {
            string formatedInput = Regex.Replace(initialData, "[^a-zA-Z]+", string.Empty, RegexOptions.Compiled);
            float[] periodic_IoC = new float[k];
            for (int x = 0; x < k; x++)
            {
                string[] substrings = SplitStrings(formatedInput, x + 1);
                float sum = 0;
                for (int y = 0; y < x + 1; y++)
                {
                    Analysis tempAnal = new Analysis(substrings[y]);
                    sum += tempAnal.FindIoC();
                }
                periodic_IoC[x] = sum / (x + 1F);
            }

            List<KeyValuePair<int, float>> formattedIoC = new List<KeyValuePair<int, float>>();
            for (int i = 0; i < periodic_IoC.Length; ++i)
            {
                formattedIoC.Add(new KeyValuePair<int, float>(i, periodic_IoC[i]));
            }

            formattedIoC.Sort((pair1, pair2) => Math.Truncate(pair2.Value * 100) == Math.Truncate(pair1.Value * 100) ?
                                                    pair1.Key.CompareTo(pair2.Key) : pair2.Value.CompareTo(pair1.Value));

            return formattedIoC;
        }

        public float FindIoC()
        {
            var freqLetters = FreqLetter();
            int letterCount = CountLetters();
            float sum = 0;

            foreach (var pair in freqLetters)
            {
                sum += pair.Value * (pair.Value - 1);
            }
            return (sum / (letterCount * (letterCount - 1)));
        }

        // Ignores case ('A' == 'a')
        public Dictionary<char, int> FreqLetter()
        {
            var upperFreq = FreqUppercase();
            var lowerFreq = FreqLowercase();
            foreach (var pair in upperFreq)
            {
                char ch = char.ToLower(pair.Key);
                if (characterCounts[ch] > 0)
                    lowerFreq[ch] += pair.Value;
                else lowerFreq[ch] = pair.Value;
            }
            return lowerFreq.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public int CountLetters()
        {
            int output = 0;
            for (char ch = 'A'; ch <= 'z'; ++ch)
            {
                if (char.IsLetter(ch))
                    output += characterCounts[ch];
            }
            return output;
        }

        public Dictionary<char, int> FreqLowercase()
        {
            Dictionary<char, int> freq = new Dictionary<char, int>();
            for (char ch = 'a'; ch <= 'z'; ++ch)
            {
                if (characterCounts[ch] > 0)
                    freq[ch] = characterCounts[ch];
            }
            return freq;
        }

        public int CountLowercase()
        {
            int output = 0;
            for (char ch = 'a'; ch <= 'z'; ++ch)
            {
                output += characterCounts[ch];
            }
            return output;
        }

        public Dictionary<char, int> FreqUppercase()
        {
            Dictionary<char, int> freq = new Dictionary<char, int>();
            for (char ch = 'A'; ch <= 'Z'; ++ch)
            {
                if (characterCounts[ch] > 0)
                    freq[ch] = characterCounts[ch];
            }
            return freq;
        }

        public int CountUppercase()
        {
            int output = 0;
            for (char ch = 'A'; ch <= 'Z'; ++ch)
            {
                output += characterCounts[ch];
            }
            return output;
        }

        // Returns the frequency of digits from 0 to 9
        public Dictionary<char, int> FreqNumbers()
        {
            Dictionary<char, int> freq = new Dictionary<char, int>();
            for (char ch = '0'; ch < '9'; ++ch)
            {
                if (characterCounts[ch] > 0)
                    freq[ch] = characterCounts[ch];
            }
            return freq;
        }

        // Returns the total number of digits in the string
        public int CountNumbers()
        {
            int output = 0;
            for (char ch = '0'; ch <= '9'; ++ch)
            {
                output += characterCounts[ch];
            }
            return output;
        }

        public Dictionary<char, int> FreqSymbols()
        {
            Dictionary<char, int> freq = new Dictionary<char, int>();
            for (char ch = (char)0; ch < 128; ++ch)
            {
                if (!char.IsLetterOrDigit(ch) && characterCounts[ch] > 0)
                {
                    freq[ch] = characterCounts[ch];
                }
            }
            return freq;
        }

        // Returns the total number of NON alphanumerical symbols in the string
        public int CountSymbols()
        {
            int output = 0;
            for (char ch = (char)0; ch < 128; ++ch)
            {
                if (!char.IsLetterOrDigit(ch))
                    output += characterCounts[ch];
            }
            return output;
        }

        // A helper method
        static string[] SplitStrings(string input, int k)
        {
            string[] substrings = new string[k].Select(x => "").ToArray();
            for (int x = 0; x < input.Length; x++)
            {
                substrings[x % k] += input[x];
            }
            return substrings;
        }
    }
}
