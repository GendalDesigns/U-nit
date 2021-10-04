using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApartmentNetwork.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ApartmentNetwork.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        //LOGIN REG PAGE
        [HttpGet("/")]
        public IActionResult Index()
        {
            HttpContext.Session.SetInt32("loggedIn", 0);

            return View();
        }

        //SUBMIT REGISTRATION INFO
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");

                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("LoggedIn", 1);
                User  activeUser= _context.Users.FirstOrDefault(u => u.Email == newUser.Email);
                HttpContext.Session.SetInt32("UserId", activeUser.UserId);

                return RedirectToAction("BuildingInput");
            }

            return View("Index");
        }

        //ENTER BUILDING INFO
        [HttpGet("register/building")]
        public IActionResult BuildingInput()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }
            return View();
        }

        //PROCESS BUILDING INFO
        [HttpPost("register/building/check")]
        public IActionResult BuildingCheck(Building submittedBuilding)
        {
            User  activeUser= _context.Users.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("UserId"));

            if(ModelState.IsValid){
                var building = _context.Buildings.FirstOrDefault(build => build.AddressLine1 == submittedBuilding.AddressLine1 && build.ZipCode == submittedBuilding.ZipCode);

                //If buliding does not exist, create and assign admin duties to user
                if(building == null){
                    
                    _context.Add(submittedBuilding);
                    _context.SaveChanges();
                    var createdBuilding = _context.Buildings.FirstOrDefault(build => build.AddressLine1 == submittedBuilding.AddressLine1 && build.ZipCode == submittedBuilding.ZipCode);

                    Console.WriteLine(createdBuilding.AddressLine1);

                    activeUser.BuildingId = createdBuilding.BuildingId;
                    activeUser.IsAdmin = true;
                    activeUser.ConfirmedByAdmin = true;

                    _context.SaveChanges();

                    return RedirectToAction("BuildCreateSuccess");
                }

                //If building already exists, await admin confirmation before entering site
                activeUser.BuildingId = building.BuildingId;
                _context.SaveChanges();
                
                HttpContext.Session.SetInt32("BuildingId", building.BuildingId);

                return RedirectToAction("Wait");
            }
            return View("BuildingInput");
        }

        //Landing page for new admins -- redirects to dashboard
        [HttpGet("register/success")]
        public IActionResult BuildCreateSuccess()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }
            return View();
        }


        //Waiting room for unconfirmed users
        [HttpGet("register/confirmationNeeded")]
        public IActionResult Wait()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }

            User activeUser = _context.Users
                .FirstOrDefault(usr => usr.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            ViewBag.BuildingAdmin = _context.Users
                .FirstOrDefault(us => us.BuildingId == activeUser.BuildingId && us.IsAdmin == true);
            return View();
        }


        //PROCESS LOGIN INFO
        [HttpPost("checkLogin")]
        public IActionResult CheckLogin(LoginUser login)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(user => user.Email == login.LoginEmail);

                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login");

                    return View("Index");
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);

                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login");
                    return View("Index");
                }

                Console.WriteLine("Logged in");
                HttpContext.Session.SetInt32("LoggedIn", 1);

                User  activeUser= _context.Users.FirstOrDefault(u => u.Email == login.LoginEmail);
                HttpContext.Session.SetInt32("UserId", activeUser.UserId);
                HttpContext.Session.SetInt32("BuildingId", activeUser.BuildingId);

                //If user's building ID is still set to the standin defaul, redirect to the enter-builing-info page
                if(activeUser.BuildingId == 1)
                {
                    return RedirectToAction("BuildingInput");
                }

                //If the user is not the admin and they haven't been confirmed, redirect to the waiting room
                if(activeUser.ConfirmedByAdmin != true)
                {
                    return RedirectToAction("Wait");
                }
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        //LOGOUT USER//
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Console.WriteLine("Session has been cleared!");
            return View("Index");
        }


        // DISPLAY DASHBOARD
        [HttpGet("Dashboard")]
            public IActionResult Dashboard()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }
            //Import User data from database
            User userInDb = _context.Users
                    // .Include(usr => usr.Residence)
                    .FirstOrDefault(u => u.UserId == sessionID);
            ViewBag.CurrentUser = userInDb;
            
            //Get count of unadmitted residents for admin admission button
            var BuildingResidents = _context.Buildings
                .Include(bld => bld.Residents)
                .FirstOrDefault(bld => bld.BuildingId == userInDb.BuildingId);
            
            var unadmittedCount = 0;
            foreach(var user in BuildingResidents.Residents)
            {
                if(user.ConfirmedByAdmin != true)
                {
                    unadmittedCount ++;
                }
            }

            ViewBag.PendingResidents = unadmittedCount;

            //Get all Events
            ViewBag.AllEvents = _context.Events
                .Include(evnt => evnt.Creator)
                .Where(evnt => evnt.Creator.BuildingId == (int)HttpContext.Session.GetInt32("BuildingId"))
                .ToList();

            //Get all Bulletins
            ViewBag.AllBulletins = _context.Bulletins
                .Include(bltn => bltn.Creator)
                .Where(bltn => bltn.Creator.BuildingId == (int)HttpContext.Session.GetInt32("BuildingId"))
                .ToList();
            

            // User usersInBuilding = _context.Users
            //         .Include(usr => usr.Residence)
            //         .Where(u => u.BuildingId == sessionID);
            //         .List();


            // Import  building data from database
            // ViewBag.AllBuildingResidents = _context.Buildings
            // .Include(pln => pln.Residents)
            // .Where(bldg => bldg.BuildingId == userInDb.BuildingId)
            // .ToList();
                Console.WriteLine("returning dashboard view");
            return View();
        }

        //ADMIN ACTION PENDING RESIDENTS
        [HttpGet("pendingResidents")]
        public IActionResult PendingResidents()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }

            //Confirm active user is Admin
            var activeUser = _context.Users.FirstOrDefault(usr => usr.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            if(activeUser.IsAdmin == true)
            {
                ViewBag.AllBuildingResidents = _context.Buildings
                    .Include(bld => bld.Residents)
                    .FirstOrDefault(bld => bld.BuildingId == activeUser.BuildingId);

                return View();
            }
            return RedirectToAction("Dashboard");
        }

        //ADMIT A PENDING RESIDENT
        [HttpGet("pendingResidents/admit/{id}")]
        public IActionResult AdmitResident(int id)
        {
            User updateMe = _context.Users.FirstOrDefault(usr => usr.UserId == id);
            updateMe.ConfirmedByAdmin = true;
            updateMe.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("PendingResidents");
        }

        //DECLINE A PENDING RESIDENT
        [HttpGet("pendingResidents/decline/{id}")]
        public IActionResult DeclineResident(int id)
        {
            var changeUser = _context.Users.FirstOrDefault(usr => usr.UserId == id);
            changeUser.BuildingId = 1;
            _context.SaveChanges();
            return RedirectToAction("PendingResidents");
        }

        [HttpGet("/event")]
        public IActionResult EventForm()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }
            return View();
        }

        [HttpPost("/event/submit")]
        public IActionResult EventSubmit(Event newEvent)
        {
            if(ModelState.IsValid)
            {
                newEvent.UserId = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(newEvent);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("EventForm");
        }

        [HttpGet("/bulletin")]
        public IActionResult BulletinForm()
        {
            //Check if User is logged in
            int? sessionID = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine("UserId is "+ sessionID);
            if (sessionID==null)
            {
                ModelState.AddModelError("LoginPassword", "Please Login First");
                ModelState.AddModelError("FirstName", "Please Register First");
                return View("Index");
            }
            return View();
        }

        [HttpPost("/bulletin/submit")]
        public IActionResult BulletinSubmit(Bulletin newBulletin)
        {
            if(ModelState.IsValid)
            {
                newBulletin.UserId = (int)HttpContext.Session.GetInt32("UserId");
                _context.Add(newBulletin);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("EventForm");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
