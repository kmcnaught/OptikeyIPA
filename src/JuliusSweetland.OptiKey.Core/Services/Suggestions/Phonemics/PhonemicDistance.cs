using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    // This class wraps the WeightedLevenshtein distance metric
    // with typed distance functions, to avoid errors with incorrect
    // IPA mappings being used.
    class PhonemicDistance
    {
        WeightedLevenshtein wL = new WeightedLevenshtein(new PhoneticCharacterSubstitution());

        public PhonemicDistance()
        {

        }

        public LevenshteinResult Distance(ValidIPA ipa1, ValidIPA ipa2)
        {
            return Distance(ipa1.ToOneCharacterIPA(), ipa2.ToOneCharacterIPA());
        }

        public LevenshteinResult Distance(OneCharacterIPA ipa1, OneCharacterIPA ipa2)
        {
            return wL.Distance(ipa1.ToString(), ipa2.ToString());
        }
    }
}
