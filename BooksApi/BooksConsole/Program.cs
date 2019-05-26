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
                // TODO: replace hardcoded shelfID with a dynamic one
                LoadVolumesOnShelf("0");
            else if (answer == "0")
                Environment.Exit(0);
        }

        private static void LoadShelvesOfUser()
        {
            var booksShelves = Engine.ListMyShelves();
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

        private static void LoadVolumesOnShelf(string shelfId)
        {
            var volumes = Engine.ListVolumesOnShelf(shelfId);
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
    }
}
