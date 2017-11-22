using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BookStore.Models;
using BookStore.Extensions;
using Newtonsoft.Json;
using BookStore.ViewModels;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private BookDbContext db;

        public HomeController()
        {
            db = new BookDbContext();
        }

        private List<Book> GetCartBooks()
        {
            var CartContents = new List<int>();
            if (HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                var cartJson = HttpContext.Request.Cookies["cart"].Value;
                CartContents = JsonConvert.DeserializeObject<List<int>>(cartJson);
            }
            var CartBooks = db.Books.Where(b => CartContents.Contains(b.Id)).ToList();

            return CartBooks;
        }

        public ActionResult Index(string id = "All")
        {
            if (id.Equals("All")) ViewBag.books = db.Books;
            else ViewBag.books = db.Books.Where(b => b.Genre.Equals(id));

            ViewBag.ordered = GetCartBooks().Select(b=>b.Id).ToList();

            return View();
        }

        [HttpGet]
        public ActionResult Search(string query)
        {
            ViewBag.Title = $"Searching for «{query}»";
            ViewBag.Books = db.Books.ToList().Where(b => b.Name.CaseInsensitiveContains(query) 
            || b.Author.CaseInsensitiveContains(query) 
            || b.Genre.CaseInsensitiveContains(query));
            ViewBag.ordered = GetCartBooks().Select(b => b.Id).ToList();
            ViewBag.query = query;

            return View();
        }

        [HttpGet]
        public ActionResult Order()
        {
            if (GetCartBooks().Count != 0)
            {
                ViewBag.names = GetCartBooks().Select(b => b.Name).Aggregate((f, n) => $"{f}, {n}");
                ViewBag.sum = GetCartBooks().Select(b => b.Price).Aggregate((f, p) => f + p);
            }
            else return RedirectToAction("Cart");
            return View();
        }

        [HttpPost]
        public ActionResult Order(PurchaseViewModel purchaseData)
        {
            var CartBooks = GetCartBooks();
            foreach (var book in CartBooks)
            {
                var purchase = new Purchase()
                {
                    Person = purchaseData.Person,
                    UserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "0",
                    Address = purchaseData.Address,
                    BookId = book.Id,
                    Date = DateTime.Now,
                    MyBook = book
                };
                db.Purchases.Add(purchase);
                db.SaveChanges();
            }

            if (HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                HttpCookie myCookie = new HttpCookie("cart");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }

            return RedirectToAction("Cart", new { invokeModal = true });
        }

        public ActionResult AddToCart(int id, string returnUrl)
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                var cartJson = HttpContext.Request.Cookies["cart"].Value;
                var cartContents = JsonConvert.DeserializeObject<List<int>>(cartJson);
                cartContents.Add(id);
                HttpContext.Response.Cookies["cart"].Value = JsonConvert.SerializeObject(cartContents);
            }
            else
            {
                HttpContext.Response.Cookies["cart"].Value = JsonConvert.SerializeObject(new List<int>() { id });
            }

            return RedirectToAction(returnUrl);
        }

        public ActionResult RemoveFromCart(int id, string returnUrl)
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                var cartJson = HttpContext.Request.Cookies["cart"].Value;
                var CartContents = JsonConvert.DeserializeObject<List<int>>(cartJson);
                CartContents.Remove(id);
                HttpContext.Response.Cookies["cart"].Value = JsonConvert.SerializeObject(CartContents);
            }

            return RedirectToAction(returnUrl);
        }

        public ActionResult Cart(bool invokeModal = false)
        {
            if (HttpContext.Request.Cookies.AllKeys.Contains("cart"))
            {
                var cartJson = HttpContext.Request.Cookies["cart"].Value;
                var CartContents = JsonConvert.DeserializeObject<List<int>>(cartJson);
                ViewBag.Purchases = db.Books.Where(b => CartContents.Contains(b.Id)).ToList();
            }
            else ViewBag.Purchases = new List<Book>();

            ViewBag.invokeModal = invokeModal;
            return View();
        }

        [Authorize]
        public ActionResult CheckoutHistory()
        {
            var userId = User.Identity.GetUserId();
            var UserPurchases = db.Purchases.Where(p => p.UserId.Equals(userId)).ToList();
            ViewBag.Purchases = UserPurchases;

            return View();
        }

        public ActionResult SalesInfo() => View();

        [Authorize(Roles = "Admin")]
        public ActionResult AddBook() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddBook(Book model)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(model);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult BookView(int id)
        {
            var book = db.Books.FirstOrDefault(b => b.Id == id);
            ViewBag.book = book;
            ViewBag.ordered = GetCartBooks().Select(b => b.Id).Contains(id);
            var Reviews = db.Reviews.Where(r => r.BookId == book.Id).ToList();
            ViewBag.reviews = Reviews;
            double averageRating;
            if (Reviews.Any()) averageRating = (double)Reviews.Select(r => r.Rating).Aggregate((x, y) => x + y) / Reviews.Count;
            else averageRating = 0;

            ViewBag.averageRating = averageRating.ToString("F1");
            ViewBag.ci = new CultureInfo("en-US");

            return View();
        }

        [Authorize]
        public ActionResult AddReview(Review model)
        {
            model.Date = DateTime.Now;
            db.Reviews.Add(model);
            db.SaveChanges();

            return RedirectToAction("BookView", new { id = model.BookId });
        }
    }
}