using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApiEngine;

namespace TestApp
{
    class Program
    {
        private static readonly string filePath = "C:/Users/owena/git/BooksAPI/BooksAPI/BooksApi/BooksApiEngine/config.txt";

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
            string bookId = "7Q4R3RHe8AQC";

            var output = Engine.RetrieveBookById(bookId);
            var book = output.Result;
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Book name: {0}", book.VolumeInfo.Title);
            Console.WriteLine("------------------------------------------------------------------------\n");

            var book2 = Engine.RetrieveBookByIdOnShelf("0", bookId).Result;
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Book ID: {0}\nBook title: {1}", book2.Id, book.VolumeInfo.Title);
            Console.WriteLine("------------------------------------------------------------------------\n");
        }
    }
}
