using AracKiralamaPortal.Models;
using System.Text.Json;

public class SiteSettingsService
{
    private readonly string _filePath;

    public SiteSettingsService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.WebRootPath, "data", "siteSettings.json");
    }

    public SiteSettings GetSettings()
    {
        if (!File.Exists(_filePath))
            return new SiteSettings();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<SiteSettings>(json);
    }

    public void SaveSettings(SiteSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
