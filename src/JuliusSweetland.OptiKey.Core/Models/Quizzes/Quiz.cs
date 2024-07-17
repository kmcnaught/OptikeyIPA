using System.Collections.Generic;

namespace JuliusSweetland.OptiKey.Models.Quizzes
{
    public class Quiz
    {
        public Quiz(string description,
                    List<Question> questions,
                    string quizPromptWritten=null,
                    string quizPromptSpoken=null,                    
                    bool immediateFeedback=true)
        {
            Description = description;
            Questions = questions;
            QuizPromptWritten = quizPromptWritten;
            QuizPromptSpoken = quizPromptSpoken;
            ImmediateFeedback = immediateFeedback;
        }

        public string Description { get; private set; }
        public List<Question> Questions { get; private set; }

        public string QuizPromptWritten { get; private set; }
        public string QuizPromptSpoken { get; private set; }
        public bool ImmediateFeedback { get; private set; }
    }

}