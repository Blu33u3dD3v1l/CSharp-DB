namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System;

    public class StartUp
    {
        public static void Main()
        {

            using var context = new BookShopContext();
            //DbInitializer.ResetDatabase(context);


            //var st = int.Parse(Console.ReadLine());
            IncreasePrices(context);
            //Console.WriteLine(result);
             
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction  AgeEnum = Enum.Parse<AgeRestriction> (command, ignoreCase: true);

            var st = context.Books
                .Include(x => x.BookId)
                .Where(x => x.AgeRestriction == AgeEnum)
                .Select(x => x.Title)                              
                .ToList();

             return String.Join(Environment.NewLine, st);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var sb = new StringBuilder();
            var st = context.Books                
               .Where(x => x.EditionType == Models.Enums.EditionType.Gold && x.Copies <= 5000)
               .Select(x => new
               {

                   titleName = x.Title


               })              
               .ToList();
            

            foreach (var item in st)
            {
               
                sb.AppendLine($"{item.titleName}");
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var sb = new StringBuilder();

            var allBooks = context.Books                             
                .Where(x => x.Price > 40)
                .OrderByDescending(x => x.Price)
                .ToList();


           
            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.Title} - ${item.Price:f2}");
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {



            var sb = new StringBuilder();

            var allBooks = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.Title}");
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var sb = new StringBuilder();
            var allBooks = context.Books
            .Join(context.Categories, e => e.BookId, d => d.CategoryId, (e, d) => new
            {

                e.Title,
                d.Name

            })
            .Where(x => x.Name == input)
            .ToList();





            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.Title}");
            }
            return sb.ToString().TrimEnd();
              

        }

    

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var sb = new StringBuilder();

            var allBooks = context.Authors
               .Where(x => x.FirstName.EndsWith(input))
               .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
               .ToList();

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName}");
            }        
            return sb.ToString().TrimEnd();

        }


        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {

            var sb = new StringBuilder();
            var allBooks = context.Books
                 .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                 .Select(x => new
                 {
                     thisTitle = x.Title
                 })
                 .OrderBy(x => x.thisTitle)
                 .ToList();

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.thisTitle}");
                
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {

            var sb = new StringBuilder();
            var allBooks = context.Books
                .Select(x => new
                {
                    ThisTitle = x.Title,                  
                    Id = x.BookId,
                    FirstName = x.Author.FirstName,
                    LastName = x.Author.LastName
                })
                .Where(x => x.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.Id)               
                .ToList();
        

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.ThisTitle} ({item.FirstName} {item.LastName})");
            }

            return sb.ToString().TrimEnd();

        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                 .Where(x => x.Title.Length > lengthCheck)
                 .ToList().Count();
        }


        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var sb = new StringBuilder();
            var allBooks = context.Authors
                 .Select(x => new
                 {
                     FullName = $"{x.FirstName} {x.LastName}",
                     TotalCopies = x.Books.Sum(x => x.Copies)

                 })
                 .OrderByDescending(x => x.TotalCopies)
                 .ToList();

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.FullName} - {item.TotalCopies}");
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var sb = new StringBuilder();
            var allBooks = context.Categories
                .Select(x => new
                {
                    Name = x.Name,
                    TotalProfit = x.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)

                })
                .OrderByDescending(x => x.TotalProfit)
                .ThenBy(x => x.Name)
                .ToList();

            foreach (var item in allBooks)
            {
                sb.AppendLine($"{item.Name} ${item.TotalProfit:f2}");
            }
            return sb.ToString().TrimEnd();

            
        }

        public static void IncreasePrices(BookShopContext context)
        {
          

            var increasePriceOfBooks = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)              
                .ToArray();
            foreach (var item in increasePriceOfBooks)
            {
                item.Price += 5;
            }
            context.SaveChanges();
        }

    }
}


