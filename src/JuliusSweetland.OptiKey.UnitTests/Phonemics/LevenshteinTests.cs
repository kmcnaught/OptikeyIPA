/*
* This file is used under
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

// Test adapted from https://github.com/glienard/StringSimilarity.NET/blob/26826a754f841748bf4d539019942c874117ca14/test/F23.StringSimilarity.Tests/WeightedLevenshteinTest.cs#L50


using System;
using JuliusSweetland.OptiKey.Services.Suggestions.Phonemics;
using NUnit.Framework;
using static JuliusSweetland.OptiKey.Services.Suggestions.Phonemics.LevenshteinResult;

namespace JuliusSweetland.OptiKey.UnitTests.Phonemics
{
    [TestFixture]
    public class LevenshteinTests
    {

        [Test]
        public void TestDistance()
        {
            var instance = new WeightedLevenshtein(new ExampleCharSub());

            Assert.That(instance.Distance("String1", "String1").Distance, Is.EqualTo(0.0));
            Assert.That(instance.Distance("String1", "Srring1").Distance, Is.EqualTo(0.5));
            Assert.That(instance.Distance("String1", "Srring2").Distance, Is.EqualTo(1.5));
            Assert.That(instance.Distance("Str ing1", "String1").Distance, Is.EqualTo(0.5));
            Assert.That(instance.Distance("String1", "Stri ng1").Distance, Is.EqualTo(0.5));
            Assert.That(instance.Distance("Str.ing1", "String1").Distance, Is.EqualTo(0.5));
            Assert.That(instance.Distance("String1", "Stri.ng1").Distance, Is.EqualTo(0.5));
            Assert.That(instance.Distance("String1", "Strixng1").Distance, Is.EqualTo(1.0));
            Assert.That(instance.Distance("String1", "Strig1").Distance, Is.EqualTo(1.0));

            //TODO: copy these over/adapt, or change contract so it treats nulls as empty strings
            //NullEmptyTests.TestDistance(instance);
        }



        [Test]
        public void TestResultReconstruction()
        {
            var instance = new WeightedLevenshtein(new ExampleCharSub());
            var word1 = "kɑːt";
            var word2 = "kəʊd";
            var result = instance.Distance(word1, word2);

            Assert.That(result.GetWord1().ToString(), Is.EqualTo(word1));
            Assert.That(result.GetWord2().ToString(), Is.EqualTo(word2));
        }

        [Test]
        public void TestOperations()
        {
             var instance = new WeightedLevenshtein(new ExampleCharSub());

            {
                // substitution
                var res = instance.Distance("DOG", "DIG").Operations;
                Assert.That(res.Count, Is.EqualTo(3));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Substitution));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Keep));

                Assert.That(op1.Item2, Is.EqualTo('D'));
                Assert.That(op2.Item2, Is.EqualTo('O'));
                Assert.That(op2.Item3, Is.EqualTo('I'));
                Assert.That(op3.Item2, Is.EqualTo('G'));

            }


            {
                // insertion
                var res = instance.Distance("DOG", "DOGE").Operations;
                Assert.That(res.Count, Is.EqualTo(4));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];
                var op4 = res[3];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Keep));

                Assert.That(op1.Item2, Is.EqualTo('D'));
                Assert.That(op2.Item2, Is.EqualTo('O'));
                Assert.That(op3.Item2, Is.EqualTo('G'));

                Assert.That(op4.Item1, Is.EqualTo(Operation.Insertion));
                Assert.That(op4.Item3, Is.EqualTo('E'));

            }

            // Single deletion
            {
                var res = instance.Distance("DOGE", "DOG").Operations;
                Assert.That(res.Count, Is.EqualTo(4));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];
                var op4 = res[3];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op4.Item1, Is.EqualTo(Operation.Deletion));

                Assert.That(op1.Item2, Is.EqualTo('D'));
                Assert.That(op2.Item2, Is.EqualTo('O'));
                Assert.That(op3.Item2, Is.EqualTo('G'));
                Assert.That(op4.Item2, Is.EqualTo('E'));
            }

            // Multiple differences
            {
                var res = instance.Distance("kitten", "sitting").Operations;
                Assert.That(res.Count, Is.EqualTo(7));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];
                var op4 = res[3];
                var op5 = res[4];
                var op6 = res[5];
                var op7 = res[6];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Substitution));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op4.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op5.Item1, Is.EqualTo(Operation.Substitution));
                Assert.That(op6.Item1, Is.EqualTo(Operation.Keep));
                Assert.That(op7.Item1, Is.EqualTo(Operation.Insertion));

                Assert.That(op1.Item2, Is.EqualTo('k')); // sub from
                Assert.That(op1.Item3, Is.EqualTo('s')); // sub to
                Assert.That(op2.Item2, Is.EqualTo('i')); // keep
                Assert.That(op3.Item2, Is.EqualTo('t')); // keep
                Assert.That(op4.Item2, Is.EqualTo('t')); // keep
                Assert.That(op5.Item2, Is.EqualTo('e')); // sub from
                Assert.That(op5.Item3, Is.EqualTo('i')); // sub to
                Assert.That(op6.Item2, Is.EqualTo('n')); // keep 
                Assert.That(op7.Item3, Is.EqualTo('g')); // insert
            }

            // Empty source string
            {
                var res = instance.Distance("", "abc").Operations;
                Assert.That(res.Count, Is.EqualTo(3));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Insertion));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Insertion));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Insertion));

                Assert.That(op1.Item3, Is.EqualTo('a'));
                Assert.That(op2.Item3, Is.EqualTo('b'));
                Assert.That(op3.Item3, Is.EqualTo('c'));
            }

            // Empty target string
            {
                var res = instance.Distance("abc", "").Operations;
                Assert.That(res.Count, Is.EqualTo(3));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Deletion));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Deletion));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Deletion));

                Assert.That(op1.Item2, Is.EqualTo('a'));
                Assert.That(op2.Item2, Is.EqualTo('b'));
                Assert.That(op3.Item2, Is.EqualTo('c'));
            }

            // Completely different strings
            {
                var res = instance.Distance("abc", "def").Operations;
                Assert.That(res.Count, Is.EqualTo(3));

                var op1 = res[0];
                var op2 = res[1];
                var op3 = res[2];

                Assert.That(op1.Item1, Is.EqualTo(Operation.Substitution));
                Assert.That(op2.Item1, Is.EqualTo(Operation.Substitution));
                Assert.That(op3.Item1, Is.EqualTo(Operation.Substitution));

                Assert.That(op1.Item2, Is.EqualTo('a'));
                Assert.That(op1.Item3, Is.EqualTo('d'));
                Assert.That(op2.Item2, Is.EqualTo('b'));
                Assert.That(op2.Item3, Is.EqualTo('e'));
                Assert.That(op3.Item2, Is.EqualTo('c'));
                Assert.That(op3.Item3, Is.EqualTo('f'));
            }

            // Both empty
            {
                var res = instance.Distance("", "").Operations;
                Assert.That(res.Count, Is.EqualTo(0));                
            }

            // Both equal
            {
                var res = instance.Distance("flurble", "flurble").Operations;
                Assert.That(res.Count, Is.EqualTo(0));
            }

        }        

        private class ExampleCharSub : ICharacterSubstitution
        {
            public double Cost(char c1, char c2)
            {
                // The cost for substituting 't' and 'r' is considered
                // smaller as these 2 are located next to each other
                // on a keyboard
                if (c1 == 't' && c2 == 'r')
                {
                    return 0.5;
                }

                // For most cases, the cost of substituting 2 characters
                // is 1.0
                return 1.0;
            }
            public double InsertionCost(char c1)
            {
                switch (c1)
                {
                    case ' ':
                        return 0.5;
                    case '.':
                        return 0.5;
                }
                return 1.0;
            }
            public double DeletionCost(char c1)
            {
                switch (c1)
                {
                    case ' ':
                        return 0.5;
                    case '.':
                        return 0.5;
                }
                return 1.0;
            }
        }
    }
}
