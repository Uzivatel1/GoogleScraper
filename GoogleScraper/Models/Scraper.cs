using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Communication;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Scraper
{
    // Metoda pro asynchronní získání výsledků vyhledávání
    public static async Task<List<Dictionary<string, string>>> SearchResultsAsync(string query)
    {
        // Nastavení možností prohlížeče Chrome
        var options = new ChromeOptions();

        options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);
        options.AddUserProfilePreference("profile.default_content_setting_values.popups", 2);
        options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);

        // WebDriver je rozhraní poskytované knihovnou Selenium pro interakci s webovými prohlížeči
        // Vytvoření instance třídy ChromeDriver, která je konkrétní implementací WebDriver pro prohlížeč Google Chrome
        WebDriver driver = new ChromeDriver(options);

        var results = new List<Dictionary<string, string>>(); // Seznam slovníků pro uložení všech článků

        try
        {
            // Navigace na stránku vyhledávání Google
            driver.Navigate().GoToUrl($"https://www.google.com/search?q={query}");

            // Hledání bloků výsledků na stránce pomocí XPath
            var resultBlocks = driver.FindElements(By.XPath("//div[@jscontroller='SC7lYd'] | //div[@class='eKjLze']"));

            foreach (var resultBlock in resultBlocks)
            {
                var resultData = new Dictionary<string, string>(); // Slovník pro uložení dat jednoho článku

                // Nalezení a uložení zdroje
                var headerElement = resultBlock.FindElement(By.ClassName("VuuXrf"));
                resultData["zdroj"] = headerElement.Text;

                // Nalezení a uložení webové adresy zdroje
                var sourceElement = resultBlock.FindElement(By.CssSelector("cite"));
                resultData["web"] = sourceElement.Text;

                // Nalezení a uložení záhlaví
                var titleElement = resultBlock.FindElement(By.ClassName("LC20lb"));
                resultData["záhlaví"] = titleElement.Text;

                // Pokus o nalezení a uložení popisu
                try
                {
                    var discriptionElement = resultBlock.FindElement(By.ClassName("VwiC3b"));
                    resultData["popis"] = discriptionElement.Text;
                }
                catch (NoSuchElementException) { } // Pokud element není nalezen, není potřeba dělat nic

                // Pokus o nalezení a uložení hodnocení
                try
                {
                    var ratingElement = resultBlock.FindElement(By.ClassName("yi40Hd"));
                    resultData["hodnocení"] = ratingElement.Text;
                }
                catch (NoSuchElementException) { }

                // Pokus o nalezení a uložení recenzí
                try
                {
                    var reviewsElement = resultBlock.FindElement(By.ClassName("RDApEe"));
                    resultData["recenze"] = reviewsElement.Text;
                }
                catch (NoSuchElementException) { }

                // Přidání dat výsledku do slovníku s výsledky
                results.Add(resultData);
            }

            return results; // Vrácení slovníku s výsledky
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during scraping: " + ex.Message); // Výpis chyby, pokud dojde k výjimce
            return null; // Vrácení null v případě chyby
        }
        finally
        {
            // Uzavření prohlížeče, ať už došlo k chybě, nebo ne
            driver.Close();
        }
    }
}