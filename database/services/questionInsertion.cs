using LiteDB;
using System;
using System.IO; // For reading JSON file
using database.models;
using Newtonsoft.Json;

namespace database.services
{
    public class QuestionInserter
    {
        public static void InsertQuestion(string jsonFilePath)
        {
            try
            {
                // Load the JSON data from the file
                string jsonData = File.ReadAllText(jsonFilePath);

                // Deserialize the JSON into a Question object
                var question = JsonConvert.DeserializeObject<Question>(jsonData);
                if (question == null)
                {
                    Console.WriteLine("No question data found in JSON.");
                    return;
                }

                // Connect to LiteDB (replace with your database path)
                string dbPath = "Filename=questions.db;Connection=shared";
                using (var db = new LiteDatabase(dbPath))
                {
                    var questionsCollection = db.GetCollection<Question>("question_data"); // More descriptive collection name

                    // Insert the question into the collection
                    questionsCollection.Insert(question);

                    Console.WriteLine("Question inserted successfully!");
                }
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting question: {ex.Message}");
            }
        }
    }
}
