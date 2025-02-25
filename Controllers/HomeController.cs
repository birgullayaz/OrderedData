using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderedData.Models;
using OrderedData.Data;
using Npgsql;
using System.Runtime.InteropServices;
using ClosedXML.Excel;
using System.IO;

namespace OrderedData.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
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
        return View(pagedUsers);
    }

    public IActionResult Privacy()
    {
        // Önce mevcut verileri temizleyelim
        _context.UsersInfo.RemoveRange(_context.UsersInfo);
        _context.SaveChanges();

        UsersInfoModel user = new UsersInfoModel
        {
            Id = 1,
            Name = "birgul",
            Surname = "ayaz", 
            Job = "software engineering"
        };

        UsersInfoModel users = new UsersInfoModel
        {       
            Id = 2,
            Name = "büşra", 
            Surname = "Gülnaz",
            Job = "economy"
        };

        UsersInfoModel usersss = new UsersInfoModel
        {        
            Id = 3,  
            Name = "esra",
            Surname = "ilknaz",
            Job = "teacher"
        };

        UsersInfoModel userss = new UsersInfoModel
        {      Id = 4, 
            Name = "ahmet",
            Surname = "ayaz", 
            Job = "freelancer"
        };

        List<UsersInfoModel> usersList = new List<UsersInfoModel>();
        usersList.Add(user);
        usersList.Add(users); 
        usersList.Add(usersss);
        usersList.Add(userss);

        _context.UsersInfo.AddRange(usersList);
        _context.SaveChanges();

        return View(usersList);
    }

    public IActionResult ExportToExcel()
    {
        var users = _context.UsersInfo.OrderBy(u => u.Name).ToList();
        
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Users");
            
            // Başlıkları ekle
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Surname";
            worksheet.Cell(1, 3).Value = "Job";
            
            // Verileri ekle
            for (int i = 0; i < users.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = users[i].Name;
                worksheet.Cell(i + 2, 2).Value = users[i].Surname;
                worksheet.Cell(i + 2, 3).Value = users[i].Job;
            }
            
            // Excel dosyasını oluştur
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                
                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Users.xlsx"
                );
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

