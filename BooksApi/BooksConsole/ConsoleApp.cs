using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BooksApiEngine;

namespace BooksConsole
{
    class Program
    {
        // Change this path to the location of the config.txt file.
        private static readonly string filePath = "C:/Users/owena/git/BooksAPI/BooksAPI/BooksApi/BooksConsole/config.txt";

        static void Main(string[] args)
        {
            try
            {
                // Call authenticateServiceObject in the Engine
                new Engine().AuthenticateServiceObject(filePath).Wait();
                RunApi();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Runs the console application
        /// </summary>
        private static void RunApi()
        {
            Intro();
            bool temp = true;
            while (temp)
            {
                Options();
            }
        }

        private static void Intro()
        {
            Console.WriteLine("Welcome to {0}", Engine.service.ApplicationName);
        }

        private static void Options()
        {
            string answer = "";

            Console.WriteLine(
                "Press '1' to list my shelves.\n" +
                "Press '2' to list volumes on a shelf.\n" +
                "Press '0' to exit."
            );
            answer = Console.ReadLine();
            CheckAnswer(answer);
        }

        /// <summary>
        /// Function to check the answer, which determines what function to call
        /// </summary>
        /// <param name="answer">A string, representing what was answered.</param>
        private static void CheckAnswer(string answer)
        {
            if (answer == "1")
                LoadShelvesOfUser();
            else if (answer == "2")
            {
                string shelfId = AskShelfId();
                LoadVolumesOnShelf(shelfId);
            }
            else if (answer == "0")
                Environment.Exit(0);
        }

        /// <summary>
        /// Calls the function that calls the API to retrieve all the user's shelves.
        /// </summary>
        private static void LoadShelvesOfUser()
        {
            var booksShelves = Engine.RetrieveMyShelves();
            var shelves = booksShelves.Result.Items;
            Console.WriteLine("------------------------------------------------------------------------");
            foreach (var shelf in shelves)
            {
                Console.WriteLine("Shelf ID: {0}\n" +
                    "Shelf Name: {1}\n" +
                    "Books on shelf: {2}\n", shelf.Id, shelf.Title, shelf.VolumeCount);
            }
            Console.WriteLine("------------------------------------------------------------------------\n");
        }

        /// <summary>
        /// A function that asks for the shelf id, 
        /// is used when asking for books on a certain shelf.
        /// </summary>
        /// <returns>A string representing a shelf id.</returns>
        private static string AskShelfId()
        {
            // Ask for a shelfId.
            Console.WriteLine("Please type the ID of the shelf you wish to use:");
            string answer = Console.ReadLine();
            // Parse it to an Int for easier comparison.
            int shelfId = Int32.Parse(answer);
            Console.WriteLine("Looking for shelf with ID {0}...", shelfId);
            // If shelfId is present in the user's shelves, return that shelf id.
            if (CheckShelfId(shelfId))
            {
                return answer;
            }
            return "Couldn't find shelf with ID " + answer + "!\n";
        }

        /// <summary>
        /// Checks whether the shelf id is present in the user's shelves.
        /// </summary>
        /// <param name="shelfId">An integer representing the shelf id, 
        /// is an integer for easier comparison</param>
        /// <returns>A boolean</returns>
        private static bool CheckShelfId(int shelfId)
        {
            // Looking through my shelves to find the shelf with the specified ID.
            var shelves = Engine.RetrieveMyShelves().Result.Items;

            foreach (var shelf in shelves)
            {
                if (shelfId == shelf.Id)
                {
                    return true;
                }
                continue;
            }
            return false;
        }

        /// <summary>
        /// Retrieves volumes on a specific shelf.
        /// </summary>
        /// <param name="shelfId">The shelf ID from which volumes are to be retrieved.</param>
        private static void LoadVolumesOnShelf(string shelfId)
        {
            int x = 0;
            // Checks if a string is returned that can be converted to an int.
            if (Int32.TryParse(shelfId, out x))
            {
                Console.WriteLine("Found shelf with ID {0}.\n", shelfId);
                // Calls the function that calls the API
                var volumes = Engine.RetrieveVolumesOnShelf(shelfId);
                try
                {
                    // Create a list of what is returned
                    var books = volumes.Result.Items;
                    Console.WriteLine("------------------------------------------------------------------------");
                    foreach (var book in books)
                    {
                        Console.WriteLine("Book ID: {0}\n" +
                            "Book Name: {1}\n" +
                            "Page count: {2}\n" +
                            "Sub title: {3}\n" +
                            "Authors:", book.Id, book.VolumeInfo.Title, book.VolumeInfo.PageCount, book.VolumeInfo.Subtitle);
                        foreach (var author in book.VolumeInfo.Authors)
                        {
                            Console.WriteLine("\tAuthor: {0}", author);
                        }
                    }
                    Console.WriteLine("------------------------------------------------------------------------\n");
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("No books found!");
                }
            }
            else
            {
                Console.WriteLine(shelfId);
            }
        }
    }
}
