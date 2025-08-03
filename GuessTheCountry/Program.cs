using System.Text.Json;

namespace GuessTheCountry;

public class Country
{
    public Name? name { get; set; }
    public string[]? capital { get; set; }
    public long population { get; set; }
    public Dictionary<string, Currency>? currencies { get; set; }
    public string? region { get; set; }
    public string? subregion { get; set; }
}

public class Name
{
    public string? common { get; set; }
    public string? official { get; set; }
}

public class Currency
{
    public string? name { get; set; }
    public string? symbol { get; set; }
}

public class Game
{
    private readonly HttpClient httpClient;
    private readonly Random random;
    private List<Country> countries;
    private const int TOTAL_ROUNDS = 3;
    private const int MAX_HINTS = 4;

    public Game()
    {
        httpClient = new HttpClient();
        random = new Random();
        countries = new List<Country>();
    }

    public async Task LoadCountriesAsync()
    {
        try
        {
            Console.WriteLine("Loading country data...");
            string url = "https://restcountries.com/v3.1/all?fields=name,capital,population,currencies,region,subregion";
            string response = await httpClient.GetStringAsync(url);
            
            var loadedCountries = JsonSerializer.Deserialize<Country[]>(response);
            if (loadedCountries != null)
            {
                countries = loadedCountries.Where(c => 
                    c.name?.common != null && 
                    c.capital != null && 
                    c.capital.Length > 0 &&
                    c.population > 0
                ).ToList();
                
                Console.WriteLine($"Loaded {countries.Count} countries!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            throw;
        }
    }

    public async Task PlayAsync()
    {
        await LoadCountriesAsync();
        
        if (countries.Count == 0)
        {
            Console.WriteLine("Failed to load country data.");
            return;
        }

        Console.WriteLine("\n=== GUESS THE COUNTRY! ===");
        Console.WriteLine($"The game consists of {TOTAL_ROUNDS} rounds. In each round you will receive hints gradually.");
        Console.WriteLine("The faster you guess, the more points you earn!");
        Console.WriteLine("Scoring: 1st hint = 4 pts, 2nd hint = 3 pts, 3rd hint = 2 pts, 4th hint = 1 pt");
        Console.WriteLine("Type 'quit' to end the game at any time.\n");

        int totalScore = 0;
        var roundResults = new List<(int round, string country, int points)>();

        for (int round = 1; round <= TOTAL_ROUNDS; round++)
        {
            Console.WriteLine($"\n{'='} ROUND {round}/{TOTAL_ROUNDS} {'='}");
            
            var country = GetRandomCountry();
            var hints = GenerateHints(country);
            int roundScore = await PlayRound(country, hints, round);
            
            totalScore += roundScore;
            roundResults.Add((round, country.name?.common ?? "Unknown", roundScore));

            if (round < TOTAL_ROUNDS)
            {
                Console.WriteLine("\nPress Enter to continue to the next round...");
                string? input = Console.ReadLine();
                if (input?.ToLower() == "quit")
                {
                    break;
                }
            }
        }

        ShowGameSummary(totalScore, roundResults);
    }

    private async Task<int> PlayRound(Country country, List<string> hints, int roundNumber)
    {
        Console.WriteLine($"Puzzle #{roundNumber}:");
        
        for (int hintIndex = 0; hintIndex < hints.Count; hintIndex++)
        {
            Console.WriteLine($"\nHint {hintIndex + 1}/{hints.Count}:");
            Console.WriteLine($"• {hints[hintIndex]}");
            
            Console.Write("Your answer: ");
            string? userAnswer = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(userAnswer))
            {
                Console.WriteLine("No answer provided, moving to the next hint...");
                continue;
            }
            
            if (userAnswer.ToLower() == "quit")
            {
                return 0;
            }

            if (CheckAnswer(country, userAnswer))
            {
                int points = MAX_HINTS - hintIndex; // 4, 3, 2, 1 points
                Console.WriteLine($"✓ CORRECT! The answer is: {country.name?.common}");
                Console.WriteLine($"You earned {points} points for guessing after {hintIndex + 1} hint(s)!");
                
                if (hintIndex + 1 < hints.Count)
                {
                    Console.WriteLine("\nRemaining hints you didn't need:");
                    for (int i = hintIndex + 1; i < hints.Count; i++)
                    {
                        Console.WriteLine($"• {hints[i]}");
                    }
                }
                
                return points;
            }
            else
            {
                if (hintIndex < hints.Count - 1)
                {
                    Console.WriteLine("✗ Incorrect. Try again with the next hint!");
                }
            }
        }

        Console.WriteLine($"\n✗ You didn't guess it! The correct answer is: {country.name?.common}");
        Console.WriteLine("0 points for this round.");
        return 0;
    }

    private List<string> GenerateHints(Country country)
    {
        var hints = new List<string>();

        // Hint 1: Region
        if (!string.IsNullOrEmpty(country.region))
        {
            string regionHint = $"This country is located in the region: {country.region}";
            if (!string.IsNullOrEmpty(country.subregion))
            {
                regionHint += $" ({country.subregion})";
            }
            hints.Add(regionHint);
        }

        // Hint 2: Population
        hints.Add($"This country has a population of approximately {FormatPopulation(country.population)} inhabitants");

        // Hint 3: Currency
        hints.Add($"The currency of this country is: {GetCurrencyInfo(country)}");

        // Hint 4: Capital
        if (country.capital != null && country.capital.Length > 0)
        {
            hints.Add($"The capital of this country is: {country.capital[0]}");
        }

        while (hints.Count < MAX_HINTS)
        {
            if (!string.IsNullOrEmpty(country.name?.official) && 
                !hints.Any(h => h.Contains("official name")))
            {
                hints.Add($"The official name of this country starts with: {country.name.official[0]}");
            }
            else
            {
                hints.Add($"This country has {country.name?.common?.Length ?? 0} letters in its name");
                break;
            }
        }

        return hints.Take(MAX_HINTS).ToList();
    }

    private void ShowGameSummary(int totalScore, List<(int round, string country, int points)> results)
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("           GAME SUMMARY");
        Console.WriteLine(new string('=', 50));
        
        foreach (var result in results)
        {
            Console.WriteLine($"Round {result.round}: {result.country} - {result.points} pts");
        }
        
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"TOTAL SCORE: {totalScore}/{TOTAL_ROUNDS * MAX_HINTS} points");
        
        double percentage = (double)totalScore / (TOTAL_ROUNDS * MAX_HINTS) * 100;
        Console.WriteLine($"Percentage: {percentage:F1}%");
        
        string rating = percentage switch
        {
            >= 90 => "GEOGRAPHY MASTER!",
            >= 75 => "EXPERT!",
            >= 60 => "VERY GOOD!",
            >= 40 => "NOT BAD!",
            >= 20 => "TIME TO STUDY!",
            _ => "EXPLORE THE WORLD!"
        };
        
        Console.WriteLine($"Rating: {rating}");
        Console.WriteLine("\nThank you for playing!");
    }

    private Country GetRandomCountry()
    {
        return countries[random.Next(countries.Count)];
    }

    private string FormatPopulation(long population)
    {
        if (population >= 1_000_000_000)
            return $"{population / 1_000_000_000.0:F1} billion";
        else if (population >= 1_000_000)
            return $"{population / 1_000_000.0:F1} million";
        else if (population >= 1_000)
            return $"{population / 1_000.0:F0} thousand";
        else
            return population.ToString();
    }

    private string GetCurrencyInfo(Country country)
    {
        if (country.currencies == null || country.currencies.Count == 0)
            return "no currency information available";

        var currency = country.currencies.First().Value;
        return $"{currency.name} ({currency.symbol})";
    }

    private bool CheckAnswer(Country country, string userAnswer)
    {
        string correctAnswer = country.name?.common?.ToLower() ?? "";
        string userAnswerLower = userAnswer.ToLower();

        if (correctAnswer == userAnswerLower)
            return true;

        if (correctAnswer.Contains(userAnswerLower) || userAnswerLower.Contains(correctAnswer))
            return true;

        // Sprawdź oficjalną nazwę
        string officialAnswer = country.name?.official?.ToLower() ?? "";
        if (officialAnswer == userAnswerLower || 
            officialAnswer.Contains(userAnswerLower) || 
            userAnswerLower.Contains(officialAnswer))
            return true;

        return false;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var game = new Game();
            await game.PlayAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine("Check your internet connection and try again.");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
