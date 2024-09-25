using System;
using System.Collections.Generic;
using System.Linq;

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    public class LevenshteinResult
    {
        public enum Operation
        {
            Keep,
            Insertion,
            Deletion,
            Substitution
        }

        public double Distance { get; set; }
        public List<Tuple<Operation, char, char>> Operations { get; set; }

        public LevenshteinResult(double distance, List<Tuple<Operation, char, char>> operations)
        {
            Distance = distance;
            Operations = operations;
        }

        public ValidIPA GetWord1()
        {
            // Use LINQ to filter, convert to OneCharacterIPA, and then to a ValidIPA string
            ValidIPA validIPA = new ValidIPA(
                string.Concat(
                    Operations
                        .Where(t => t.Item1 != Operation.Insertion) // Ignore insertions, these aren't present in string1
                        .Select(t => new OneCharacterIPA(t.Item2).ToIPA())  // Convert to ValidIPA via OneCharacterIPA
                        .Select(ipa => ipa.ToString())            
                )
            );

            return validIPA;
        }

        public ValidIPA GetWord2()
        {
            // Use LINQ to filter, convert to OneCharacterIPA, and then to a ValidIPA string
            ValidIPA validIPA = new ValidIPA(
                string.Concat(
                    Operations
                        .Where(t => t.Item1 != Operation.Deletion) // Ignore deletions, these aren't present in string1
                        .Select(t => new OneCharacterIPA(t.Item3).ToIPA()) // Convert to OneCharacterIPA
                        .Select(ipa => ipa.ToString())             
                )
            );

            return validIPA;
        }

        public double GetNormalisedDistance()
        {
            if (Operations.Count() == 0)
            {
                return 1.0;
            }
            else {
                return 1.0 - Distance / Operations.Count();
            }
        }
    }
}
