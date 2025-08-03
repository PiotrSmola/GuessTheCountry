# Guess The Country 🌍

A fun console-based geography quiz game built in C# that tests your knowledge of world countries using data from the REST Countries API.

## Features

- **3 Rounds of Fun**: Each game consists of 3 different countries to guess
- **Progressive Hints**: Get up to 4 hints per country, from general to specific
- **Smart Scoring System**: Earn more points for guessing faster (4-3-2-1 points per hint)
- **Real-time Data**: Fetches live country information from REST Countries API
- **Intelligent Answer Checking**: Accepts various country name formats
- **Detailed Summary**: Get a complete game summary with performance rating

## How to Play

1. Run the application
2. The game loads country data from the REST Countries API
3. For each round, you'll receive hints gradually:
   - **Hint 1**: Region and subregion
   - **Hint 2**: Population
   - **Hint 3**: Currency
   - **Hint 4**: Capital city
4. Type your answer after each hint
5. The faster you guess, the more points you earn!
6. After 3 rounds, see your final score and rating

## Scoring System

- Guess after 1st hint: **4 points**
- Guess after 2nd hint: **3 points**
- Guess after 3rd hint: **2 points**
- Guess after 4th hint: **1 point**
- Don't guess at all: **0 points**

Maximum possible score: **12 points**

## Performance Ratings

- 🏆 **GEOGRAPHY MASTER!** (90%+)
- 🥇 **EXPERT!** (75-89%)
- 🥈 **VERY GOOD!** (60-74%)
- 🥉 **NOT BAD!** (40-59%)
- 📚 **TIME TO STUDY!** (20-39%)
- 🗺️ **EXPLORE THE WORLD!** (<20%)

## Requirements

- .NET 9.0 or later
- Internet connection (for REST Countries API)

## Installation & Running

1. Clone this repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```

## Game Commands

- Type your country guess and press Enter
- Type `quit` at any time to exit the game
- Press Enter between rounds to continue

## Example Gameplay

```
=== GUESS THE COUNTRY! ===

= ROUND 1/3 =
Puzzle #1:

Hint 1/4:
• This country is located in the region: Europe (Western Europe)

Your answer: France
✓ CORRECT! The answer is: France
You earned 4 points for guessing after 1 hint(s)!

Remaining hints you didn't need:
• This country has a population of approximately 67.4 million inhabitants
• The currency of this country is: Euro (€)
• The capital of this country is: Paris
```

## API Reference

This application uses the [REST Countries API](https://restcountries.com/) to fetch real-time country data including:
- Country names (common and official)
- Capital cities
- Population data
- Currency information
- Regional classifications

## Technologies Used

- **C# / .NET 9.0**: Core application framework
- **System.Text.Json**: JSON deserialization
- **HttpClient**: API communication
- **REST Countries API**: Country data source

## License

This project is open source and available under the MIT License.
