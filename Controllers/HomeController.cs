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
            .OrderBy(u => u.Name)
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
        _context.UsersInfo.RemoveRange(_context.UsersInfo);
        _context.SaveChanges();

        var users = new List<UsersInfoModel>();
        var random = new Random();

        for (int i = 1; i <= 1000; i++)
        {
            users.Add(new UsersInfoModel
            {
                Id = i,
                Name = $"{GetRandomName(random)} {i}",
                Surname = GetRandomSurname(random),
                Job = GetRandomJob(random)
            });
        }

        _context.UsersInfo.AddRange(users);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult ExportToExcel(string lang = "en")
    {
        _languageService.SetLanguage(lang);
        var users = _context.UsersInfo.OrderBy(u => u.Name).ToList();
        
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(_languageService.GetText("UsersList"));
            
            // Başlıkları dil dosyasından al
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = _languageService.GetText("Name");
            worksheet.Cell(1, 3).Value = _languageService.GetText("Surname");
            worksheet.Cell(1, 4).Value = _languageService.GetText("Job");
            
            // Başlık stilini ayarla
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            // Verileri ekle
            for (int i = 0; i < users.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = users[i].Id;
                worksheet.Cell(i + 2, 2).Value = users[i].Name?.ToUpper();
                worksheet.Cell(i + 2, 3).Value = users[i].Surname?.ToUpper();
                worksheet.Cell(i + 2, 4).Value = users[i].Job?.ToUpper();
            }
            
            worksheet.Columns().AdjustToContents();
            
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                
                var fileName = $"{_languageService.GetText("UsersList")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
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

