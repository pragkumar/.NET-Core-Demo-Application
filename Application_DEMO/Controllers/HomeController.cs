using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Application_DEMO.Models;
using Application_DEMO.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using System.Data.Entity;

namespace Application_DEMO.Controllers
{
    
    [Authorize(Roles = "Admin,Users")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext appDbContext;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeRepository, IDataProtectionProvider dataProtectionProvider, AppDbContext appDbContext)
        {
            _logger = logger;
            _employeeRepository = employeeRepository;
            this.appDbContext = appDbContext;
        }

        public IActionResult Index(string searchString)
        {
            var movies = from m in appDbContext.Employees
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Name.Contains(searchString));
            }
            return View( movies.ToList());

            //var model = _employeeRepository.GetAllEmployees();
            //return View(model);
        }

        public IActionResult Details(int? id)
        {
            Trace.TraceInformation("In HomeController.Details");
           

            Employee employee = _employeeRepository.GetEmployee(id.Value);
           

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Details"
            };

            // Pass the ViewModel object to the View() helper method
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee newEmployee = _employeeRepository.Add(employee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EditViewModel employeeEditViewModel = new EditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(Employee model)
        {
            // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the employee being edited from the database
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                // Update the employee object with the data in the model object
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                Employee updatedEmployee = _employeeRepository.Update(employee);

                return RedirectToAction("index");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
           

            Employee employee =  _employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            return View(employee);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {


            _employeeRepository.Delete(id);
           


            return RedirectToAction(nameof(Index));
        }




    }
    }
