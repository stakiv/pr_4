using _4praktika.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace _4praktika.Controllers
{
    public class AccountController : Controller
    {
        private MyDbContext _context;
        public AccountController(MyDbContext ctx)
        {
            _context = ctx;
        }

        public IActionResult RegistrationPage()
        {
            return View("Registration");
        }

        public IActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            try
            {
                var newUser = new User { Login = user.Login, Password = user.Password, FullName = user.FullName };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("MainPage", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при сохранении пользователя в бд");
                Console.WriteLine(ex);

                return RedirectToAction("Error", "Home");
            }

        }
        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                return RedirectToAction("Profile", "Account", new { login = user.Login });
            }
            else
            {
                ViewBag.ErrorMessage = "Неверный логин или пароль";
                return View("Login");
            }
        }

        public IActionResult Profile(string login, string senderLogin, DateTime? startDate, DateTime? endDate, string status, string error)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            IQueryable<Message> messageQuery = _context.Messages
                                                .Where(m => m.ToUserId == user.Id)
                                                .OrderBy(m => m.Status)
                                                .ThenByDescending(m => m.SendDate);


            if (!string.IsNullOrEmpty(senderLogin))
            {
                messageQuery = from messages in _context.Messages
                                join sender in _context.Users on messages.FromUserId equals sender.Id
                                where sender.Login == senderLogin
                                select messages;

            }

            startDate = startDate?.ToUniversalTime();
            endDate = endDate?.ToUniversalTime();

            if (startDate != null)
            {
                messageQuery = messageQuery.Where(m => m.SendDate >= startDate);
            }
            if (endDate != null)
            {
                // Добавляем 1 день к конечной дате, чтобы включить все сообщения в этот день
                endDate = endDate.Value.AddDays(1).AddTicks(-1);
                messageQuery = messageQuery.Where(m => m.SendDate <= endDate);
            }

            if (status == "unread")
            {
                messageQuery = messageQuery.Where(m => m.Status == "new");
            }

            var message = messageQuery.ToList();

            ViewBag.User = user;
            ViewBag.Messages = message;
            ViewBag.ErrorMessage = error;

            return View("Profile", user);
        }


        [HttpPost]
        public IActionResult SendMessage(int currentId, string recipientLogin, string messageSubject, string messageText)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == currentId);
            var message = _context.Messages.Where(m => m.ToUserId == currentId);
            ViewBag.User = user;
            ViewBag.Messages = message;


            var recipient = _context.Users.FirstOrDefault(u => u.Login == recipientLogin);
            if (recipient == null)
            {
                ViewBag.ErrorMessage = "Пользователь с указанным логином получателя не существует";
                return RedirectToAction("Profile", "Account", new { login = user.Login, error = ViewBag.ErrorMessage });
            }

            var newMessage = new Message
            {
                FromUserId = currentId,
                ToUserId = recipient.Id,
                MessageSubject = messageSubject,
                MessageText = messageText,
                SendDate = DateTime.UtcNow,
                Status = "new"
            };

            _context.Messages.Add(newMessage);
            _context.SaveChanges();

            return RedirectToAction("Profile", "Account", new { login = user.Login });
        }

        [HttpPost]
        [Route("/Account/UpdateMessageStatus/{login}/{messageId}")]
        public IActionResult UpdateMessageStatus(int messageId, string login)
        {
            var message = _context.Messages.FirstOrDefault(m => m.Id == messageId);
            Console.WriteLine("login" + login);
            Console.WriteLine("messageId" + messageId);
            if (message != null)
            {
                message.Status = "read";
                _context.SaveChanges();
                return RedirectToAction("Profile", "Account", new { login = login });
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}