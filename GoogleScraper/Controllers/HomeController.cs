using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    // Z�kladn� metoda pro zobrazen� domovsk� str�nky
    public IActionResult Index()
    {
        return View();
    }

    // Akce pro zpracov�n� vyhled�vac�ho dotazu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(string query)
    {
        // Zavol�n� metody scraperu pro z�sk�n� v�sledk�
        var results = await Scraper.SearchResultsAsync(query);

        // Kontrola, zda byly nalezeny v�sledky
        if (results == null || results.Count == 0)
        {
            return Json(new { success = false, message = "No results found" }); // Vr�cen� zpr�vy, pokud nebyly v�sledky nalezeny
        }

        // Nastaven� mo�nost� pro serializaci JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Naform�tov�n� JSON s odsazen�m
            // Bez escapov�n� speci�ln�ch znak� a s diakritikou
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Serializace v�sledk� do form�tovan�ho JSON
        var formattedJson = JsonSerializer.Serialize(new { success = true, results }, options);

        // Z�sk�n� cesty na plochu u�ivatele
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var filePath = Path.Combine(desktopPath, "SearchResults.json");

        // Z�pis form�tovan�ho JSON do souboru na plo�e
        await System.IO.File.WriteAllTextAsync(filePath, formattedJson);

        // Vr�cen� v�sledk� ve form� JSON
        return Json(new { success = true, results }, options);
    }
}
