using System.Text.Json;

namespace OrderedData.Services
{
    public class LanguageService
    {
        private readonly IWebHostEnvironment _environment;
        private Dictionary<string, string> _translations = new();

        public LanguageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void SetLanguage(string language)
        {
            var path = Path.Combine(_environment.ContentRootPath, "Resources", $"{language}.json");
            if (File.Exists(path))
            {
                var jsonString = File.ReadAllText(path);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ?? new();
            }
        }

        public string GetText(string key)
        {
            return _translations.TryGetValue(key, out var value) ? value : key;
        }
    }
} 