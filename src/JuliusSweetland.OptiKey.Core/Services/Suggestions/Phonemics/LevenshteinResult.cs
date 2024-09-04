using System;
using System.Collections.Generic;

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
    }
}
