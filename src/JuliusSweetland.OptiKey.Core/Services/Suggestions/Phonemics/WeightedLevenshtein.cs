/*
* This file is covered by
* The MIT License
*
* Copyright 2017 feature[23]
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

// This was modified from https://github.com/glienard/StringSimilarity.NET
// with added logic to keep track of operations.

using System;
using System.Collections.Generic;
using static JuliusSweetland.OptiKey.Services.Suggestions.Phonemics.LevenshteinResult;

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    /// Implementation of Levenshtein that allows to define different weights for
    /// different character substitutions.
    public class WeightedLevenshtein
    {
        private readonly ICharacterSubstitution _characterSubstitution;

        /// <summary>
        /// Create a new instance with provided character substitution.
        /// </summary>
        /// <param name="characterSubstitution">The strategy to determine character substitution weights.</param>
        public WeightedLevenshtein(ICharacterSubstitution characterSubstitution)
        {
            _characterSubstitution = characterSubstitution;
        }

        /// <summary>
        /// Compute Levenshtein distance using provided weights for substitution.
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>The computed weighted Levenshtein distance.</returns>
        /// <exception cref="ArgumentNullException">If s1 or s2 is null.</exception>
        public LevenshteinResult Distance(string s1, string s2)
        {
            if (s1 == null)
            {
                throw new ArgumentNullException(nameof(s1));
            }

            if (s2 == null)
            {
                throw new ArgumentNullException(nameof(s2));
            }

            if (s1.Equals(s2))
            {
                var ops = new List<Tuple<Operation, char, char>>();
                foreach (char c in s2)
                {
                    ops.Add(new Tuple<Operation, char, char>(Operation.Keep, c, c));
                }
                return new LevenshteinResult(0, ops);
            }

            if (s1.Length == 0)
            {
                var ops = new List<Tuple<Operation, char, char>>();
                foreach (char c in s2)
                {
                    ops.Add(new Tuple<Operation, char, char>(Operation.Insertion, '\0', c));
                }
                return new LevenshteinResult(s2.Length, ops);
            }

            if (s2.Length == 0)
            {
                var ops = new List<Tuple<Operation, char, char>>();
                foreach (char c in s1)
                {
                    ops.Add(new Tuple<Operation, char, char>(Operation.Deletion, c, '\0'));
                }
                return new LevenshteinResult(s1.Length, ops);
            }

            // create two work vectors of integer distances
            double[] v0 = new double[s2.Length + 1];
            double[] v1 = new double[s2.Length + 1];
            double[] vtemp;

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            Operation[,] operations = new Operation[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i < v0.Length; i++)
            {
                v0[i] = i;
                operations[0, i] = Operation.Insertion;
            }

            for (int i = 0; i < s1.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0
                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;
                operations[i + 1, 0] = Operation.Deletion;

                // use formula to fill in the rest of the row
                for (int j = 0; j < s2.Length; j++)
                {
                    double cost = 0;
                    double insertioncost = 0;
                    double deletioncost = 0;
                    if (s1[i] != s2[j])
                    {
                        cost = _characterSubstitution.Cost(s1[i], s2[j]);
                        insertioncost = _characterSubstitution.InsertionCost(s2[j]);
                        deletioncost = _characterSubstitution.DeletionCost(s1[i]);
                    }

                    double substitutionCost = v0[j] + cost;
                    double insertion = v1[j] + insertioncost;
                    double deletion = v0[j + 1] + deletioncost;

                    v1[j + 1] = Math.Min(insertion, Math.Min(deletion, substitutionCost));

                    if (v1[j + 1] == substitutionCost)
                    {
                        operations[i + 1, j + 1] = s1[i] == s2[j] ? Operation.Keep : Operation.Substitution;
                    }
                    else if (v1[j + 1] == insertion)
                    {
                        operations[i + 1, j + 1] = Operation.Insertion;
                    }
                    else
                    {
                        operations[i + 1, j + 1] = Operation.Deletion;
                    }
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                //System.arraycopy(v1, 0, v0, 0, v0.length);
                // Flip references to current and previous row
                vtemp = v0;
                v0 = v1;
                v1 = vtemp;
            }

            var operationList = new List<Tuple<Operation, char, char>>();
            int x = s1.Length;
            int y = s2.Length;

            while (x > 0 || y > 0)
            {
                // could optionally null a character if not relevant (e.g. insertion/deletion)
                char char1 = x > 0 ? s1[x - 1] : '\0';
                char char2 = y > 0 ? s2[y - 1] : '\0';

                operationList.Add(new Tuple<Operation, char, char>(
                    operations[x, y],
                    char1,
                    char2));

                switch (operations[x, y])
                {
                    case Operation.Substitution:
                        x--;
                        y--;
                        break;
                    case Operation.Insertion:
                        y--;
                        break;
                    case Operation.Deletion:
                        x--;
                        break;
                    case Operation.Keep:
                        x--;
                        y--;
                        break;
                }
            }

            operationList.Reverse();
            return new LevenshteinResult(v0[s2.Length], operationList);
        }
    }
}
