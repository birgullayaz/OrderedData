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
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;

namespace OrderedData.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly LanguageService _languageService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, LanguageService languageService)
    {
        _logger = logger;
        _context = context;
        _languageService = languageService;
    }

    public IActionResult Index(int page = 1, string lang = "en")
    {
        _languageService.SetLanguage(lang);
        ViewBag.CurrentLanguage = lang;
        
        int pageSize = 3;
        var totalUsers = _context.UsersInfo.Count();
        var maxPage = (int)Math.Ceiling(totalUsers / (double)pageSize);
        
        if (page < 1) page = 1;
        if (page > maxPage && maxPage > 0) page = maxPage;

        var pagedUsers = _context.UsersInfo
            .AsNoTracking()
            .OrderByDescending(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        ViewBag.CurrentPage = page;
        ViewBag.MaxPage = maxPage;
        ViewBag.TotalUsers = totalUsers;
        return View(pagedUsers);
    }

    public IActionResult Privacy()
    {
        try
        {
            // Önce tüm kayıtları sil
            var allUsers = _context.UsersInfo.ToList();
            _context.UsersInfo.RemoveRange(allUsers);
            _context.SaveChanges();

            // Yeni kayıtları ekle
            var users = new List<UsersInfoModel>();
            var random = new Random();

            for (int i = 0; i < 3000; i++)
            {
                users.Add(new UsersInfoModel
                {
                    Name = $"{GetRandomName(random)}",
                    Surname = GetRandomSurname(random),
                    Job = GetRandomJob(random)
                });
            }

            _context.UsersInfo.AddRange(users);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Privacy action");
            TempData["Error"] = "An error occurred while resetting the database.";
            return RedirectToAction("Index");
        }
    }

    public IActionResult ExportToExcel(string lang = "en")
    {
        _languageService.SetLanguage(lang);
        var users = _context.UsersInfo.OrderBy(u => u.Name).ToList();
        
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(_languageService.GetText("UsersList"));
            
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = new UppercaseTagHelper { Text = _languageService.GetText("Name") }.GetUpperText();
            worksheet.Cell(1, 3).Value = new UppercaseTagHelper { Text = _languageService.GetText("Surname") }.GetUpperText();
            worksheet.Cell(1, 4).Value = new UppercaseTagHelper { Text = _languageService.GetText("Job") }.GetUpperText();
            
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            for (int i = 0; i < users.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = users[i].Id;
                worksheet.Cell(i + 2, 2).Value = new UppercaseTagHelper { Text = users[i].Name }.GetUpperText();
                worksheet.Cell(i + 2, 3).Value = new UppercaseTagHelper { Text = users[i].Surname }.GetUpperText();
                worksheet.Cell(i + 2, 4).Value = new UppercaseTagHelper { Text = users[i].Job }.GetUpperText();
            }
            
            worksheet.Columns().AdjustToContents();
            
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                
                var fileName = new UppercaseTagHelper { Text = _languageService.GetText("UsersList") }.GetUpperText() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                
                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
        }
    }

    public IActionResult ChangeLanguage(string lang, int page = 1)
    {
        return RedirectToAction("Index", new { page, lang });
    }

    [HttpGet]
    public IActionResult NewRegister()
    {
        var cities = new Dictionary<int, string>
        {
            {1, "Istanbul"},
            {2, "Ankara"},
            {3, "Izmir"},
            {4, "Bursa"},
            {5, "Antalya"}
        };
        
        ViewBag.Cities = cities;
        return View();
    }

    [HttpPost]
    public IActionResult NewRegister([FromBody] NewRegisterModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.Name) || 
                string.IsNullOrEmpty(model.Surname) || 
                string.IsNullOrEmpty(model.Job))
            {
                return Json(new { success = false, message = "Name, Surname and Job are required" });
            }

            var usersInfo = new UsersInfoModel
            {
                Name = model.Name,
                Surname = model.Surname,
                Job = model.Job,
                City = model.CityId,      // CityId'yi long olarak kaydediyoruz
                District = model.DistrictId  // DistrictId'yi long olarak kaydediyoruz
            };

            _context.UsersInfo.Add(usersInfo);
            var result = _context.SaveChanges();

            // Kayıt başarılı mı kontrol et
            if (result > 0)
            {
                _logger.LogInformation($"New user added: {model.Name} {model.Surname}");
                return Json(new { success = true, message = "Registration successful!", userId = usersInfo.Id });
            }
            else
            {
                return Json(new { success = false, message = "Failed to save to database" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NewRegister");
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            return Json(new { success = false, message = $"Error: {innerMessage}" });
        }
    }

    private static readonly string[] Names = { 
        "Ahmet", "Mehmet", "Ayşe", "Fatma", "Ali", "Veli", "Zeynep", "Elif", "Can", "Cem",
        "Deniz", "Ece", "Emre", "Gül", "Hakan", "İrem", "Kemal", "Leyla", "Murat", "Nur"
    };

    private static readonly string[] Surnames = { 
        "Yılmaz", "Demir", "Kaya", "Çelik", "Şahin", "Yıldız", "Özdemir", "Arslan", "Doğan", "Kılıç",
        "Aydın", "Bulut", "Çetin", "Erdoğan", "Güneş", "Koç", "Kurt", "Öztürk", "Şen", "Yalçın"
    };

    private static readonly string[] Jobs = { 
        "Software Engineer", "Teacher", "Doctor", "Lawyer", "Architect", 
        "Designer", "Manager", "Accountant", "Chef", "Writer",
        "Data Scientist", "Product Manager", "UX Designer", "DevOps Engineer", 
        "Business Analyst", "Marketing Specialist", "HR Manager", "Sales Executive",
        "System Administrator", "Quality Assurance"
    };

    private string GetRandomName(Random random) => Names[random.Next(Names.Length)];
    private string GetRandomSurname(Random random) => Surnames[random.Next(Surnames.Length)];
    private string GetRandomJob(Random random) => Jobs[random.Next(Jobs.Length)];
}

public static class UppercaseTagHelperExtensions
{
    public static string GetUpperText(this UppercaseTagHelper helper)
    {
        return helper.Text?.ToUpper() ?? string.Empty;
    }
}





