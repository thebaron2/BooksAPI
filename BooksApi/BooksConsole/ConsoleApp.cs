﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BooksApiEngine;

namespace BooksConsole
{
    class Program
    {
        private static readonly string filePath = "C:/Users/owena/git/BooksAPI/BooksAPI/BooksApi/BooksConsole/config.txt";

        static void Main(string[] args)
        {
            try
            {
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

        //private static void WriteOptions()
        //{
        //    string answer = "";
        //    Console.WriteLine("Do you wish to write this to a JSON file?\n" +
        //        "Type 'y' (yes) or 'n' (no).");
        //    answer = Console.ReadLine();
        //    CheckAnswer(answer);
        //}

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

        private static string AskShelfId()
        {
            // Ask for a shelfId.
            Console.WriteLine("Please type the ID of the shelf you wish to use:");
            string answer = Console.ReadLine();
            // Parse it to an Int for easier comparison.
            int shelfId = Int32.Parse(answer);
            Console.WriteLine("Looking for shelf with ID {0}...", shelfId);
            if (CheckShelfId(shelfId))
            {
                return answer;
            }
            return "Couldn't find shelf with ID " + answer + "!\n";
        }

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

        private static void LoadVolumesOnShelf(string shelfId)
        {
            int x = 0;
            // Checks if a string is returned that can be converted to an int.
            if (Int32.TryParse(shelfId, out x))
            {
                Console.WriteLine("Found shelf with ID {0}.\n", shelfId);
                var volumes = Engine.RetrieveVolumesOnShelf(shelfId);
                try
                {
                    var books = volumes.Result.Items;
                    Console.WriteLine("------------------------------------------------------------------------");
                    foreach (var book in books)
                    {
                        Console.WriteLine("Book ID: {0}\n" +
                            "Book Name: {1}\n" +
                            "Authors:", book.Id, book.VolumeInfo.Title);
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
