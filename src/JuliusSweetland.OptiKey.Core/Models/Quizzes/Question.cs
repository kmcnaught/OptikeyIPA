using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuliusSweetland.OptiKey.Models.Quizzes
{
    public class Question
    {
        public Question(string word,
                    string context,
                    List<string> options,
                    string answer,
                    string image)
        {
            Word = word;
            Context = context;
            Options = options;
            Answer = answer;
            Image = image;
        }

        public string Word { get; private set; }
        public string Context { get; private set; }
        public List<string> Options { get; private set; }
        public string Answer { get; private set; }
        public string Image { get; private set; }

    }
}
