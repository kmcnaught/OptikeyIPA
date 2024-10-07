using System;
using System.Linq;

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    class PhoneticCharacterSubstitution : ICharacterSubstitution
    {
        private static bool IsIPAShortVowel(char c)
        {
            char[] shortVowels = { 'æ', 'ɒ', 'ə', 'ɛ', 'ɪ', 'ʊ', 'ʌ', };
            return shortVowels.Contains(c);
        }

        private static bool IsIPALongVowel(char c)
        {
            char[] longVowels ={
                        'ɜ',
                        'Z', /* "eə" : */
                        'I', /* "iː" */
                        'U', /* "uː" */
                        'A', /* "ɑː" */
                        'C', /* "ɔː" */
            };

            return longVowels.Contains(c);
        }

        private static bool IsIPAVowel(char c)
        {
            return IsIPAShortVowel(c) || IsIPALongVowel(c);
        }

        private static bool IsIPADiphthong(char c)
        {
            char[] diphthongs ={
                         'Y', /* "aɪ" */
                         'V', /* "aʊ" */
                         'E', /* "eɪ" */
                         'O', /* "ɔɪ" */
                         'W', /* "əʊ" */
                         'J', /* "ɪə" */
                         'S',  /* "ɪʊ" */
                         'R', /* "ʊə" */
            };

            return diphthongs.Contains(c);
        }

        public double Cost(char c1, char c2)
        {
            return Math.Min(OneWayCost(c1, c2),
                            OneWayCost(c2, c1));
        }

        public double OneWayCost(char c1, char c2)
        {
            // schwa can sound like any vowel
            if (c1 == 'ə' && IsIPAVowel(c2))
                return 0.1;

            // forgive long/short vowel mixup
            // ask heather for more pairs

            if (c1 == IpaMappings.IpaToMapped["ɑː"] &&
                c2 == 'æ')
            {
                return 0.2;
            }
            if ((c1 == IpaMappings.IpaToMapped["eə"] && c2 == 'e') ||
                (c1 == IpaMappings.IpaToMapped["iː"] && c2 == 'i') ||                
                (c1 == IpaMappings.IpaToMapped["uː"] && c2 == 'u') ||
                (c1 == IpaMappings.IpaToMapped["ɑː"] && c2 == 'ɑ') ||
                (c1 == IpaMappings.IpaToMapped["ɑː"] && c2 == 'æ') ||
                (c1 == IpaMappings.IpaToMapped["ɔː"] && c2 == 'ɔ'))
            {
                return 0.1;
            }

            // scottish special - Loch - free conversion to "k" as we don't have "ch"
            if (c1 == 'x' && c2 == 'k')
            {
                return 0.0;
            }

            // Voiced vs. Voiceless Consonants
            if ((c1 == 'p' && c2 == 'b') ||
                (c1 == 't' && c2 == 'd') ||
                (c1 == 'k' && c2 == 'g') ||
                (c1 == 'f' && c2 == 'v') ||
                (c1 == 's' && c2 == 'z') ||
                (c1 == 'ʃ' && c2 == 'ʒ'))
            {
                return 0.1;
            }

            // Nasal Consonants
            if ((c1 == 'm' && c2 == 'n') ||
                (c1 == 'n' && c2 == 'ŋ') ||
                (c1 == 'm' && c2 == 'b'))
            {
                return 0.2;
            }

            // Fricatives and Affricates
            if ((c1 == 's' && c2 == 'ʃ') ||
                (c1 == IpaMappings.IpaToMapped["tʃ"] && c2 == IpaMappings.IpaToMapped["dʒ"]) ||
                (c1 == 'θ' && c2 == 'ð'))
            {
                return 0.2;
            }

            // Liquids and Glides
            if ((c1 == 'l' && c2 == 'r') ||
                (c1 == 'w' && c2 == 'r') ||
                (c1 == 'j' && c2 == 'ʒ'))
            {
                return 0.1;
            }

            // Misc
            if ((c1 == 'f' && c2 == 'θ') ||
                (c1 == IpaMappings.IpaToMapped["oʊ"] && c2 == IpaMappings.IpaToMapped["əʊ"]) || // we don't have oʊ but əʊ is v close
                (c1 == 'ɪ' && c2 == 'i')
                ) // i gets used as "ɪ or i:" for ambiguous pronunciations
            {
                return 0.1;
            }

            // Default cost is 1.0 (should be less than insertion I think)
            return 1.0;
        }

        // it would be nice to be able to have a cheaper insertion
        // within a cluster, e.g. "consonant after consonant = cheaper"
        // but this would require context in the WeightedLevenshtein algorithm
        public double InsertionCost(char c1)
        {
            switch (c1)
            {
                case 'ə':
                    return 0.5;
                default:
                    return 1.0;
            }
        }
        public double DeletionCost(char c1)
        {
            return 1.0;
        }
    }
}
