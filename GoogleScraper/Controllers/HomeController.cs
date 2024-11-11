using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    // Základní metoda pro zobrazení domovské stránky
    public IActionResult Index()
    {
        return View();
    }

    // Akce pro zpracování vyhledávacího dotazu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(string query)
    {
        // Zavolání metody scraperu pro získání výsledkù
        var results = await Scraper.SearchResultsAsync(query);

        // Kontrola, zda byly nalezeny výsledky
        if (results == null || results.Count == 0)
        {
            return Json(new { success = false, message = "No results found" }); // Vrácení zprávy, pokud nebyly výsledky nalezeny
        }

        // Nastavení možností pro serializaci JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Naformátování JSON s odsazením
            // Bez escapování speciálních znakù a s diakritikou
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Serializace výsledkù do formátovaného JSON
        var formattedJson = JsonSerializer.Serialize(new { success = true, results }, options);

        // Získání cesty na plochu uživatele
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "SearchResults.json");

        // Zápis formátovaného JSON do souboru na ploše
        await System.IO.File.WriteAllTextAsync(filePath, formattedJson);

        // Vrácení výsledkù ve formì JSON
        return Json(new { success = true, results }, options);
    }
}
