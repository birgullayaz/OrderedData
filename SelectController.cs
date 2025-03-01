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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SelectController> _logger;

        public SelectController(ApplicationDbContext context, ILogger<SelectController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("SelectController initialized");
        }

        [HttpGet]
        public JsonResult GetCities()
        {
            try 
            {
                _logger.LogInformation("GetCities method started - Fetching all cities");
                
                var cities = _context.City
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new { id = c.Id, name = c.Name })
                    .ToList();

                _logger.LogInformation("GetCities completed - Found {Count} cities", cities.Count);
                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCities method");
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

                _logger.LogInformation($"Found {districts.Count} districts for cityId: {cityId}");
                
                return Json(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDistricts: {ex.Message}, CityId: {cityId}");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public JsonResult GetDistrictsByCityId(int cityId)
        {
            try 
            {
                _logger.LogInformation("GetDistrictsByCityId started - Fetching districts for cityId: {CityId}", cityId);

                var districts = _context.District
                    .AsNoTracking()
                    .Where(d => d.CityId == cityId)
                    .OrderBy(d => d.Name)
                    .Select(d => new { 
                        id = d.Id, 
                        name = d.Name 
                    })
                    .ToList();

                _logger.LogInformation("GetDistrictsByCityId completed - Found {Count} districts for cityId: {CityId}", 
                    districts.Count, cityId);

                return Json(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDistrictsByCityId for cityId: {CityId}", cityId);
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
                _logger.LogInformation("NewRegister GET method started - Loading initial form");

                var cities = _context.City
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new { Id = c.Id, Name = c.Name })
                    .ToList();

                _logger.LogInformation("NewRegister GET completed - Loaded {Count} cities for dropdown", cities.Count);
                
                ViewBag.Cities = cities;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NewRegister GET method");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult NewRegister([FromBody] NewRegisterModel model)
        {
            try
            {
                _logger.LogInformation("NewRegister POST started - Processing registration for user: {Name} {Surname}", 
                    model.Name, model.Surname);

                // Validation
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Surname) || string.IsNullOrEmpty(model.Job))
                {
                    _logger.LogWarning("NewRegister validation failed - Required fields missing");
                    return Json(new { success = false, message = "Required fields are missing" });
                }

                var selectedCity = _context.City.FirstOrDefault(c => c.Id == model.CityId);
                var selectedDistrict = _context.District.FirstOrDefault(d => d.Id == model.DistrictId);

                if (selectedCity == null || selectedDistrict == null)
                {
                    _logger.LogWarning("NewRegister validation failed - Invalid city/district selection: CityId={CityId}, DistrictId={DistrictId}", 
                        model.CityId, model.DistrictId);
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

                _logger.LogInformation("Saving new user to database: {Name} {Surname}, City={CityName}, District={DistrictName}", 
                    model.Name, model.Surname, selectedCity.Name, selectedDistrict.Name);

                _context.UsersInfo.Add(usersInfo);
                var result = _context.SaveChanges();

                if (result > 0)
                {
                    _logger.LogInformation("NewRegister completed successfully - User saved: {Name} {Surname}", 
                        model.Name, model.Surname);
                        
                    return Json(new { 
                        success = true, 
                        message = "Registration successful!",
                        cityName = selectedCity.Name,
                        districtName = selectedDistrict.Name
                    });
                }
                else
                {
                    _logger.LogWarning("NewRegister failed - Database save returned 0 rows affected");
                    return Json(new { success = false, message = "Failed to save to database" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NewRegister POST method for user: {Name} {Surname}", 
                    model.Name, model.Surname);
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
