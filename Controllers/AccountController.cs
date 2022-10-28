using Microsoft.AspNetCore.Mvc;
using ProjectApp.Models;
using ProjectApp.Context;
using Microsoft.EntityFrameworkCore;
using ProjectApp.ViewModel;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ProjectApp.Controllers
{
    public class AccountController : Controller
    {

        MyContext myContext;

        public AccountController(MyContext myContext)
        {
            this.myContext = myContext;
        }
       

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {

            var data = myContext.Users
                .Include(x => x.Employee)
                .Include(x => x.Role)
                .SingleOrDefault(x => x.Employee.Email.Equals(email) && x.Password.Equals(password));
            if (data != null)
            {
                LoginResponse loginResponse = new LoginResponse()
                {
                    FullName = data.Employee.FullName,
                    Email = data.Employee.Email,
                    Role = data.Role.Name
                };
                return RedirectToAction("Index", "Home", loginResponse);
            }

            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, DateTime birthDate, string password)
        {
            Employee employee = new Employee()
            {
                FullName = fullName,
                Email = email,
                BirthDate = birthDate
            };
            myContext.Employees.Add(employee);
            var result = myContext.SaveChanges();
            if (result > 0)
            {
                var id = myContext.Employees.SingleOrDefault(x => x.Email.Equals(email)).Id;
                User user = new User()
                {
                    Id = id,
                    Password = password,
                    RoleId = 1
                };
                myContext.Users.Add(user);
                var resultUser = myContext.SaveChanges();
                if (resultUser > 0)
                    return RedirectToAction("Login", "Account");

            }
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string email, string password, string baru)
        {

            var data = myContext.Users
                .Include(x => x.Employee)
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefault(x => x.Employee.Email.Equals(email) && x.Password.Equals(password));
                myContext.SaveChanges();
            if (data != null)
            {
                User user = new User()
                {
                    Id = data.Id,
                    Password = baru,
                    RoleId = data.RoleId,
                };
                myContext.Entry(user).State = EntityState.Modified;
                var resultUser = myContext.SaveChanges();
                if (resultUser > 0)
                    return RedirectToAction("Login", "Account");
            }

            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email, string baru)
        {
            var data = myContext.Users
                .Include(x => x.Employee)
                .Include(x => x.Role)
                .AsNoTracking()
                .SingleOrDefault(x => x.Employee.Email.Equals(email));
            myContext.SaveChanges();
            if (data != null)
            {
                User user = new User()
                {
                    Id = data.Id,
                    Password = baru,
                    RoleId = data.RoleId,
                };
                myContext.Entry(user).State = EntityState.Modified;
                var resultUser = myContext.SaveChanges();
                if (resultUser > 0)
                    return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
