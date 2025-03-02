using Microsoft.AspNetCore.Mvc;
using OrderedData.Data;
using Microsoft.Extensions.Logging;

namespace OrderedData.Controllers
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<Controller> _logger;

        protected Controller(ApplicationDbContext context, ILogger<Controller> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Sadece sisteme giriş logu
            _logger.LogInformation("System accessed at: {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
} 