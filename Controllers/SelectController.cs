using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderedData.Models;
using OrderedData.Data;
using Npgsql;
using System.Runtime.InteropServices;
using ClosedXML.Excel;
using System.IO;
using OrderedData.Helpers;
using OrderedData.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace OrderedData.Controllers
{
    public class SelectController : Controller
    {
        public SelectController(ApplicationDbContext context) 
            : base(context)
        {
        }

        [HttpGet]
        public JsonResult GetCities()
        {
            try 
            {
                var cities = _context.City
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new { id = c.Id, name = c.Name })
                    .ToList();

                return Json(cities);
            }
            catch 
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public JsonResult GetDistricts(int cityId)
        {
            try 
            {
                var districts = _context.District
                    .AsNoTracking()
                    .Where(d => d.CityId == cityId)
                    .OrderBy(d => d.Name)
                    .Select(d => new { id = d.Id, name = d.Name })
                    .ToList();

                return Json(districts);
            }
            catch 
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public JsonResult GetDistrictsByCityId(int cityId)
        {
            try 
            {
                var districts = _context.District
                    .AsNoTracking()
                    .Where(d => d.CityId == cityId)
                    .OrderBy(d => d.Name)
                    .Select(d => new { id = d.Id, name = d.Name })
                    .ToList();

                return Json(districts);
            }
            catch 
            {
                return Json(new List<object>());
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NewRegister()
        {
            try
            {
                var cities = _context.City
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new { Id = c.Id, Name = c.Name })
                    .ToList();
                
                ViewBag.Cities = cities;
                return View();
            }
            catch 
            {
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult NewRegister([FromBody] NewRegisterModel model)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Surname) || string.IsNullOrEmpty(model.Job))
                {
                    return Json(new { success = false, message = "Required fields are missing" });
                }

                var selectedCity = _context.City.FirstOrDefault(c => c.Id == model.CityId);
                var selectedDistrict = _context.District.FirstOrDefault(d => d.Id == model.DistrictId);

                if (selectedCity == null || selectedDistrict == null)
                {
                    return Json(new { success = false, message = "Invalid city or district selection" });
                }

                var usersInfo = new UsersInfoModel
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Job = model.Job,
                    City = model.CityId,
                    District = model.DistrictId
                };

                _context.UsersInfo.Add(usersInfo);
                var result = _context.SaveChanges();

                if (result > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = "Registration successful!",
                        cityName = selectedCity.Name,
                        districtName = selectedDistrict.Name
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save to database" });
                }
            }
            catch 
            {
                return Json(new { success = false, message = "Error" });
            }
        }
    }
}
