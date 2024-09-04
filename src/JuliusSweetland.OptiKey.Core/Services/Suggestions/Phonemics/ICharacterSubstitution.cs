// Taken from https://github.com/glienard/StringSimilarity.NET with MIT License

namespace JuliusSweetland.OptiKey.Services.Suggestions.Phonemics
{
    public interface ICharacterSubstitution
    {
        /// <summary>
        /// Indicate the cost of substitution c1 and c2.
        /// </summary>
        /// <param name="c1">The first character of the substitution.</param>
        /// <param name="c2">The second character of the substitution.</param>
        /// <returns>The cost in the range [0, 1].</returns>
        double Cost(char c1, char c2);


        /// <summary>
        /// Indicate the cost of deleting c1.
        /// </summary>
        /// <param name="c1">The character to be deleted.</param>
        /// <returns>The cost in the range [0, 1].</returns>
        double DeletionCost(char c1);

        /// <summary>
        /// Indicate the cost of inserting c1.
        /// </summary>
        /// <param name="c1">The character to be inserted.</param>
        /// <returns>The cost in the range [0, 1].</returns>
        double InsertionCost(char c1);
    }
}
