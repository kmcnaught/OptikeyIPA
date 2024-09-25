using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JuliusSweetland.OptiKey.Extensions;

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    // Represents a string that has been preformatted to be valid IPA. 
    public class ValidIPA : IEquatable<ValidIPA>
    {
        public string Value { get; }

        public ValidIPA(string value)
        {
            if (value == null)
                return;

            // Normalize the string to FormD (decomposed any diacritics)
            var normalizedString = value.Normalize(NormalizationForm.FormD);

            // Define a set of diacritics to remove
            var diacriticsToRemove = new HashSet<char>
            {
                '\u032C', // Voicing diacritic ̬ 
                '\u02B0', // Aspiration diacritic ʰ
                '\u0303', // Nasalization diacritic ̃ 
                '\u0329', // Syllabic diacritic  ̩ 
                '\u032A', // Dental diacritic ̪ 
                '\u031F', // Retracted diacritic  ̠ 
                '\u031F', // Advanced diacritic  ̟ 
                '\u0361', // tie bar like t͡ʃ
                '\u02DE', // rhotic accent "r coloured"
                '.', // Syllable separator "."
            };

            // Filter out the specified diacritics
            var filteredString = new string(
                normalizedString
                    .Where(c => !diacriticsToRemove.Contains(c))
                    .ToArray()
            );
            
            // Normalize back to FormC (composed form)
            value = filteredString.Normalize(NormalizationForm.FormC);

            Value = value.Trim()             // Remove whitespace at ends
                         .Trim('/')          // Remove '/' from start/end if present
                         .Replace(" ", "")   // Remove any spaces
                         .Replace("ˈ", "")   // Remove primary stress markers, we don't deal with them
                         .Replace("ˌ", "")   // Remove secondary stress markers, we don't deal with them
                         .Replace('.', '\0')   // Remove syllable boundaries, we don't deal with them
                         .Replace('\u0361'.ToString(), "") // Remove tie bars (this is done in normalisation filter I think)
                         .Replace(":", "ː"); // Ensure "long" markers are valid character (sometimes standard colon may be used by mistake)                                             
                         
        }

        public override string ToString()
        {
            return Value;
        }

        public OneCharacterIPA ToOneCharacterIPA()
        {
            return new OneCharacterIPA(this);
        }

        #region IEquatable (string comparison)
        public bool Equals(ValidIPA other)
        {
            if (other == null)
                return false;

            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((ValidIPA)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(ValidIPA left, ValidIPA right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(ValidIPA left, ValidIPA right)
        {
            return !(left == right);
        }
        #endregion //IEquatable
    }

    // Represents a string that is IPA mapped so that every phoneme
    // is a single char. 
    public class OneCharacterIPA : IEquatable<OneCharacterIPA>
    {
        public string Value { get; }

        public OneCharacterIPA(char singleChar)
        {
            Value = singleChar.ToString();
        }

        public OneCharacterIPA(ValidIPA ipaValue)
        {
            StringBuilder mappedWord = new StringBuilder();

            var ipaWord = ipaValue.Value;

            for (int i = 0; i < ipaWord.Length;)
            {
                bool matched = false;

                // Check for multi-character IPA units
                foreach (var kvp in IpaMappings.IpaToMapped)
                {
                    // Ensure the substring is long enough to match the multi-character IPA unit
                    if (i + kvp.Key.Length <= ipaWord.Length && ipaWord.Substring(i, kvp.Key.Length) == kvp.Key)
                    {
                        mappedWord.Append(kvp.Value);
                        i += kvp.Key.Length; // Move the index forward by the length of the matched unit
                        matched = true;
                        break;
                    }
                }

                // If no multi-character unit matched, add the single character
                if (!matched)
                {
                    mappedWord.Append(ipaWord[i]);
                    i++;
                }
            }

            Value = mappedWord.ToString();
        }

        public override string ToString()
        {
            return Value;
        }

        public ValidIPA ToIPA()
        {
            StringBuilder ipaWord = new StringBuilder();

            foreach (char c in Value)
            {
                if (IpaMappings.MappedToIpa.ContainsKey(c))
                {
                    ipaWord.Append(IpaMappings.MappedToIpa[c]);
                }
                else
                {
                    ipaWord.Append(c);
                }
            }

            return new ValidIPA(ipaWord.ToString());
        }

        #region IEquatable (string comparison) 
        public bool Equals(OneCharacterIPA other)
        {
            if (other == null)
                return false;

            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((OneCharacterIPA)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(OneCharacterIPA left, OneCharacterIPA right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(OneCharacterIPA left, OneCharacterIPA right)
        {
            return !(left == right);
        }
        #endregion //IEquatable
    }

    // To use string similarity metrics, we need single phonemes to be mapped to 
    // single characters. IPA sometimes uses 2 characters for either a long vowel
    // like 'iː' or a diphthong like 'əʊ'.
    // This class converts to and from this mapping. 
    // TODO: consider using types to represent strings in each case?
    class IpaMappings
    {
        // Use upper case substitutions for these 2-char phonemes. 
        // These were chosen somewhat arbitrarily, with a vague attempt
        // to be semantically meaningful
        public static readonly Dictionary<string, char> IpaToMapped = new Dictionary<string, char>
        {
            { "ɔɪ",  'O' },
            { "ɪʊ",  'S' },
            { "eə",  'Z' },
            { "iː",  'I' },
            { "uː",  'U' },
            { "ɑː",  'A' },
            { "ɔː",  'C' },
            { "eɪ", 'E' },
            { "əʊ", 'W' },
            { "dʒ", 'D' },
            { "ks", 'X' },
            { "kw", 'Q' },
            { "aɪ", 'Y' },
            { "aʊ", 'V' },
            { "ɪə", 'J' },
            { "ʊə", 'R' },
            { "tʃ", 'T' },
        };

        public static readonly Dictionary<char, string> MappedToIpa = new Dictionary<char, string>();

        static IpaMappings()
        {
            // Populate the reverse mapping dictionary
            foreach (var kvp in IpaToMapped)
            {
                MappedToIpa[kvp.Value] = kvp.Key;
            }
        }

    }
}
