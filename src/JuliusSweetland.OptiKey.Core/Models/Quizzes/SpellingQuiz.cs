using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JuliusSweetland.OptiKey.Models.Quizzes
{
    public class SpellingQuiz
    {
        public List<SpellingQuestion> Questions { get; set; }
        public int CurrentQuestionIndex { get; private set; }
        public int TotalQuestions => Questions.Count;
        public string filePath;

        public SpellingQuiz(string jsonFilePath)
        {
            filePath = jsonFilePath;
            LoadQuestions(filePath);
            CurrentQuestionIndex = 0;
        }

        private void LoadQuestions(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            var quizData = JsonConvert.DeserializeObject<QuizData>(json);
            Questions = quizData.Questions;
        }

        public SpellingQuestion GetCurrentQuestion()
        {
            if (CurrentQuestionIndex < TotalQuestions)
            {
                return Questions[CurrentQuestionIndex];
            }
            return null;
        }

        public void MoveToNextQuestion()
        {
            if (CurrentQuestionIndex < TotalQuestions - 1)
            {
                CurrentQuestionIndex++;
            }
        }

        public bool IsQuizComplete()
        {
            return CurrentQuestionIndex >= TotalQuestions;
        }
    }

    public class QuizData
    {
        public List<SpellingQuestion> Questions { get; set; }
    }

    public class SpellingQuestion
    {
        public string Word { get; set; }
        public string Context { get; set; }
        public List<string> Options { get; set; }
        public string Answer { get; set; }
    }

    // Example usage
    class Program
    {
        static void Main(string[] args)
        {
            var quiz = new SpellingQuiz("path_to_your_json_file.json");

            while (!quiz.IsQuizComplete())
            {
                var currentQuestion = quiz.GetCurrentQuestion();
                Console.WriteLine($"Word: {currentQuestion.Word}");
                Console.WriteLine($"Context: {currentQuestion.Context}");
                Console.WriteLine("Options:");
                foreach (var option in currentQuestion.Options)
                {
                    Console.WriteLine(option);
                }

                // Move to the next question
                quiz.MoveToNextQuestion();
            }

            Console.WriteLine("Quiz Complete!");
        }
    }

}
