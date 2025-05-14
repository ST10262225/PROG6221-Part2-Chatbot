using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Figgle;
using System.Media;
using System.IO;

namespace Prog_Part1
{
    public class Program
    {
        // Instance of the FilterBot class to handle responses.
        static FilterBot bot = new FilterBot();
        // Random number generator for various tasks.
        static Random random = new Random();

        // Stores the name of the user interacting with the bot.
        static string userName = "";
        // Stores the cybersecurity topic the user is most interested in.
        static string favoriteTopic = "";
        // Stores the last response given by the bot to avoid repetition during elaboration.
        static string lastBotResponse = "";

        // Define personalization rules based on favorite topic
        static Dictionary<string, Dictionary<string, Func<string, string, string>>> personalizationRules =
            new Dictionary<string, Dictionary<string, Func<string, string, string>>>()
            {
                {
                    "privacy", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"you should", (response, topic) => $"As someone interested in {topic}, {userName}, you might want to {response.ToLower().Replace("you should", "review")} the security settings on your accounts."},
                        {"important to", (response, topic) => $"Given your interest in {topic}, {userName}, it's especially important to be mindful of what information you share online."},
                        {"be aware of", (response, topic) => $"Since you're focused on {topic}, {userName}, be aware of how websites track your online activity."}
                    }
                },
                {
                    "passwords", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"you should", (response, topic) => $"Since you're interested in {topic}, {userName}, you should always {response.ToLower().Replace("you should", "use strong and unique passwords for all")} your accounts."},
                        {"important to", (response, topic) => $"Considering your interest in {topic}, {userName}, it's important to never reuse passwords across different services."},
                        {"a good idea to", (response, topic) => $"Knowing your interest in {topic}, {userName}, it's a good idea to use a password manager."}
                    }
                },
                {
                    "phishing", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"be careful of", (response, topic) => $"Given your interest in {topic}, {userName}, be extra careful of suspicious emails and messages that might be {topic} attempts."},
                        {"never", (response, topic) => $"As someone interested in {topic}, {userName}, never click on unfamiliar links or share personal information through unsolicited requests."},
                        {"learn to identify", (response, topic) => $"Since you're curious about {topic}, {userName}, learn to identify the common tactics used in {topic} attacks."}
                    }
                },
                {
                    "malware", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"install", (response, topic) => $"Considering your interest in {topic}, {userName}, you should install and regularly update antivirus software to protect against {topic}."},
                        {"avoid downloading", (response, topic) => $"Given your focus on {topic}, {userName}, avoid downloading files from untrusted sources that could contain {topic}."},
                        {"important to scan", (response, topic) => $"Knowing your interest in {topic}, {userName}, it's important to regularly scan your system for {topic}."}
                    }
                },
                {
                    "firewall", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"ensure", (response, topic) => $"Since you're interested in {topic}, {userName}, ensure your {topic} is always enabled to protect your network."},
                        {"helps prevent", (response, topic) => $"Given your focus on {topic}, {userName}, understand that a {topic} helps prevent unauthorized access to your system."},
                        {"important for", (response, topic) => $"Considering your interest in {topic}, {userName}, a properly configured {topic} is important for your security."}
                    }
                },
                {
                    "encryption", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"helps protect", (response, topic) => $"Given your interest in {topic}, {userName}, understand that {topic} helps protect your sensitive data."},
                        {"always", (response, topic) => $"As someone interested in {topic}, {userName}, always encrypt sensitive information when transmitting it online."},
                        {"important for", (response, topic) => $"Since you're curious about {topic}, {userName}, {topic} is important for maintaining the confidentiality of your data."}
                    }
                },
                {
                    "social", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"be cautious of", (response, topic) => $"Given your interest in {topic}, {userName}, be cautious of unexpected requests for personal data, as this is a common tactic in {topic} engineering."},
                        {"verify the identity", (response, topic) => $"Considering your interest in {topic}, {userName}, always verify the identity of individuals asking for sensitive information to avoid {topic} engineering."},
                        {"remember that", (response, topic) => $"As someone interested in {topic}, {userName}, remember that {topic} engineering often relies on manipulating trust."}
                    }
                },
                {
                    "authentication", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"always enable", (response, topic) => $"Since you're interested in {topic}, {userName}, always enable multi-factor {topic} on important accounts for extra security."},
                        {"never share", (response, topic) => $"Given your focus on {topic}, {userName}, never share your {topic} credentials with anyone."},
                        {"important for", (response, topic) => $"Knowing your interest in {topic}, {userName}, strong {topic} is important for securing your accounts."}
                    }
                },
                {
                    "vpn", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"helps you stay secure", (response, topic) => $"Given your interest in {topic}, {userName}, using a {topic} helps you stay secure on public Wi-Fi networks."},
                        {"always consider using", (response, topic) => $"As someone interested in {topic}, {userName}, always consider using a {topic} when browsing on untrusted networks."},
                        {"important for", (response, topic) => $"Knowing your interest in {topic}, {userName}, a {topic} is important for encrypting your connection and protecting your data."}
                    }
                },
                {
                    "2fa", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"always enable", (response, topic) => $"Since you're interested in {topic}, {userName}, always enable {topic} on your important accounts for an extra layer of security."},
                        {"helps prevent", (response, topic) => $"Given your focus on {topic}, {userName}, understand that {topic} helps prevent unauthorized access even if your password is compromised."},
                        {"consider using", (response, topic) => $"As someone interested in {topic}, {userName}, consider using an authenticator app for {topic} instead of SMS."}
                    }
                },
                {
                    "wifi", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"avoid logging into", (response, topic) => $"Given your interest in {topic} security, {userName}, avoid logging into sensitive accounts on public {topic} networks."},
                        {"consider using a", (response, topic) => $"As someone interested in {topic} safety, {userName}, consider using a VPN on public {topic} to encrypt your data."},
                        {"be cautious on", (response, topic) => $"Since you're curious about {topic} security, {userName}, be cautious when using unsecured public {topic} networks."}
                    }
                },
                {
                    "browsing", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"always check", (response, topic) => $"Given your interest in safe {topic}, {userName}, always check the URL carefully before entering personal information."},
                        {"look for", (response, topic) => $"As someone interested in secure {topic}, {userName}, look for 'HTTPS' in the website address."},
                        {"avoid clicking on", (response, topic) => $"Since you're curious about safe {topic}, {userName}, avoid clicking on suspicious pop-ups or ads."}
                    }
                },
                {
                    "scam", new Dictionary<string, Func<string, string, string>>()
                    {
                        {"be wary of", (response, topic) => $"Given your interest in avoiding {topic}s, {userName}, be wary of online offers that seem too good to be true."},
                        {"never share", (response, topic) => $"As someone interested in {topic} prevention, {userName}, never share personal or financial information with unsolicited contacts."},
                        {"verify the legitimacy", (response, topic) => $"Since you're curious about {topic}s, {userName}, always verify the legitimacy of websites and emails."}
                    }
                }
            };

        // Define keywords that indicate the user might be confused or wants more detail
        static readonly string[] confusionKeywords = { "explain", "more", "confused", "mean", "elaborate", "further", "understand", "detail", "clarify" };

        // Main entry point of the program.
        static void Main(string[] args)
        {
            // Sets the title of the console window.
            Console.Title = "Cybersecurity Awareness Bot";
            // Clears the console screen.
            Console.Clear();
            // Ensures UTF-8 encoding for proper character display.
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Plays a greeting sound.
            PlayGreeting();
            // Displays the bot's name in ASCII art.
            DisplayAsciiArt();
            // Gets the user's name.
            GetUserName();
            // Gets the user's favorite cybersecurity topic.
            GetFavoriteTopic();
            // Enters the main chat loop for interaction.
            ChatLoop();
        }

        // Plays a greeting sound from an audio file.
        static void PlayGreeting()
        {
            // Relative path to the audio file.
            string relativePath = @"Audio\greeting.wav"; // Relative path to the audio file
            // Gets the absolute path to the audio file.
            string absolutePath = Path.GetFullPath(relativePath); // Get the absolute path
            // Sets the console text color to yellow for the greeting message
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Checks if the audio file exists.
            if (!File.Exists(absolutePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Warning: Audio file not found. Skipping greeting...");
                Console.ResetColor();
                return;
            }

            try
            {
                // Uses SoundPlayer to load and play the audio file synchronously.
                using (SoundPlayer player = new SoundPlayer(absolutePath))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error playing audio: {ex.Message}");
                Console.ResetColor();
            }
        }

        // Displays the bot's name in ASCII art using the Figgle library.
        static void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(FiggleFonts.Standard.Render("CyberSentinel"));
            Console.ResetColor();
        }

        // Prompts the user to enter their name and stores it.
        // Memory recall for the users name.
        static void GetUserName()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nPlease enter your name: ");
            Console.ResetColor();
            userName = Console.ReadLine()?.Trim();

            // Loops until a valid name (only letters, at least 3 characters) is entered.
            while (string.IsNullOrEmpty(userName) || userName.Length < 3 || !userName.All(char.IsLetter))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Chatbot: ");
                DisplayTypingEffect("Name must contain only letters and cannot be empty. Please enter your name: ");
                Console.ResetColor();
                userName = Console.ReadLine()?.Trim();
            }

            // Creates a welcome message and displays it with a typing effect and border.
            string welcomeMessage = $"Hello, {userName}! Welcome to the Cybersecurity Awareness Bot. I am here to help you stay safe online";
            DisplayTypingEffect(welcomeMessage);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(new string('*', welcomeMessage.Length + 4));
            string borderedWelcomeMessage = $"* {welcomeMessage} *";
            Console.WriteLine(borderedWelcomeMessage);
            Console.WriteLine(new string('*', welcomeMessage.Length + 4));
            Console.ResetColor();
        }

        // Prompts the user to enter their favorite cybersecurity topic and stores it.
        // Memory recall for storing users favourite topic.
        static void GetFavoriteTopic()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nWhat cybersecurity topic are you most interested in?");
            Console.ResetColor();
            string userInput = Console.ReadLine()?.Trim().ToLower();

            if (userInput.StartsWith("i am interested in "))
            {
                favoriteTopic = userInput.Substring("i am interested in ".Length).Trim();
            }
            else if (userInput.StartsWith("my favorite topic is "))
            {
                favoriteTopic = userInput.Substring("my favorite topic is ".Length).Trim();
            }
            else
            {
                favoriteTopic = userInput; // If they don't use the specific phrase, just take their input.
            }

            DisplayTypingEffect($"Great! I'll remember that you're interested in {favoriteTopic}.");
        }
    

        // Main loop for chatting with the user.
        static void ChatLoop()
        {
            Console.WriteLine("\nAsk me anything about cybersecurity, or type 'exit' to leave.\n");

            // Continues until the user types 'exit'.
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{userName}: ");
                Console.ResetColor();
                string userInput = Console.ReadLine()?.Trim().ToLower();

                // Input validation for length.
                if (userInput.Length > 500)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chatbot: Please enter a more concise question.");
                    Console.ResetColor();
                    continue;
                }
                // Input validation for empty or short input.
                if (string.IsNullOrEmpty(userInput) || userInput.Length < 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Chatbot: ");
                    DisplayTypingEffect("Please enter a valid question with at least 3 characters.");
                    Console.ResetColor();
                    continue;
                }

                // Handles the 'exit' command to end the chat.
                if (userInput == "exit")
                {
                    ExitChat();
                    break;
                }

                // Handles changes to the user's favorite topic during the conversation.
                if (userInput.StartsWith("i'm interested in ") || userInput.StartsWith("my favorite topic is "))
                {
                    HandleTopicChange(userInput);
                    continue;
                }

                // Checks if the user's input contains keywords indicating confusion.
                bool userIsConfused = confusionKeywords.Any(userInput.Contains);

                // If the user is confused, a last topic was identified, and the last response wasn't a "no information" message, elaborate on the last topic.
                if (userIsConfused && !string.IsNullOrEmpty(bot.LastTopic) && !lastBotResponse.Contains("I don't have specific information"))
                {
                    ElaborateOnLastTopic();
                    continue; // Skip getting a new response for this turn
                }

                // Cleans the user input by removing punctuation.
                string cleanedInput = new string(userInput.Where(c => !char.IsPunctuation(c)).ToArray());
                // Gets a response from the FilterBot based on the cleaned input.
                string response = bot.GetResponse(cleanedInput);

                // Resets the LastTopic if the bot couldn't find specific information.
                if (response.Contains("I don't have specific information"))
                {
                    bot.LastTopic = null;
                }

                // Detects the sentiment of the user's input.
                string sentiment = DetectSentiment(userInput);
                // Adjusts the bot's response based on the detected sentiment.
                if (!string.IsNullOrEmpty(sentiment))
                {
                    response = AdjustResponseBasedOnSentiment(response, sentiment);
                }
                // Applies personalization rules based on the user's favorite topic.
                if (!string.IsNullOrEmpty(favoriteTopic) && personalizationRules.TryGetValue(favoriteTopic, out var topicRules))
                {
                    foreach (var rule in topicRules)
                    {
                        if (response.ToLower().Contains(rule.Key))
                        {
                            response = rule.Value(response, favoriteTopic);
                            break;
                        }
                    }
                }
                // Displays the bot's response with a typing effect.
                DisplayBotResponse(response);
                // Stores the current bot response to check for repetition during elaboration.
                lastBotResponse = response;
            }
        }

        // Handles changes to the user's favorite cybersecurity topic.
        static void HandleTopicChange(string userInput)
        {
            // Extracts the stated topic from the user's input.
            string statedTopic = GetStatedTopic(userInput);
            if (!string.IsNullOrEmpty(statedTopic))
            {
                // Updates the favoriteTopic.
                favoriteTopic = statedTopic;
                // Acknowledges the change in topic.
                string acknowledgement = $"Great! I'll remember that you're interested in {favoriteTopic}. It's a crucial part of staying safe online.";
                DisplayBotResponse(acknowledgement);
                lastBotResponse = acknowledgement;
            }
        }

        // Handles what the user is intresed in
        static string GetStatedTopic(string userInput)
        {
            if (userInput.StartsWith("i'm interested in "))
            {
                return userInput.Substring("i'm interested in ".Length).Trim();
            }
            else if (userInput.StartsWith("my favorite topic is "))
            {
                return userInput.Substring("my favorite topic is ".Length).Trim();
            }
            return null;
        }
        
        // Detects words of concern from the user
        static string DetectSentiment(string userInput)
        {
            // Dictionary to store the words of concern 
            Dictionary<string, string> sentimentKeywords = new Dictionary<string, string>()
            {
                {@"\b(worried|concerned|anxious)\b", "worried"},
                {@"\b(curious|interested to know|wondering)\b", "curious"},
                {@"\b(frustrated|annoyed|difficult)\b", "frustrated"}
            };

            foreach (var pair in sentimentKeywords)
            {
                if (Regex.IsMatch(userInput, pair.Key))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        // Dictionary for concern responses
        static Dictionary<string, Func<string, string, string>> sentimentAdjustments =
            new Dictionary<string, Func<string, string, string>>()
            {
                {"worried", (response, sentiment) => $"It's completely understandable to feel that way, {userName}. {response} Let me share some tips to help you stay safe."},
                {"curious", (response, sentiment) => $"That's a great question, {userName}! {response} I'm happy to provide more information."},
                {"frustrated", (response, sentiment) => $"I understand this can be frustrating, {userName}. {response} Let's see if I can help clarify things."},
            };

        static string AdjustResponseBasedOnSentiment(string botResponse, string sentiment)
        {
            if (sentimentAdjustments.ContainsKey(sentiment))
            {
                return sentimentAdjustments[sentiment](botResponse, sentiment);
            }
            return botResponse;
        }

        static void ElaborateOnLastTopic()
        {
            if (!string.IsNullOrEmpty(bot.LastTopic) && bot.Responses.TryGetValue(bot.LastTopic, out var relatedResponses) && relatedResponses.Count > 1)
            {
                string additionalResponse = "";
                int attempts = 0;
                do
                {
                    additionalResponse = relatedResponses[random.Next(relatedResponses.Count)];
                    attempts++;

                    if (attempts > 5) break; // Avoid infinite loops if all responses have been used
                } while (string.Equals(additionalResponse, lastBotResponse, StringComparison.OrdinalIgnoreCase));

                if (!string.Equals(additionalResponse, lastBotResponse, StringComparison.OrdinalIgnoreCase))
                {
                    DisplayBotResponse($"Okay, {userName}, let me elaborate on {bot.LastTopic}: {additionalResponse}");
                    lastBotResponse = additionalResponse;
                }
                else
                {
                    DisplayBotResponse($"(I've already given you some details on {bot.LastTopic}. Do you have a more specific question?)");
                }
            }
            else if (!string.IsNullOrEmpty(bot.LastTopic) && !lastBotResponse.Contains("I don't have specific information"))
            {
                DisplayBotResponse($"(Do you have any other specific questions about {bot.LastTopic}, {userName}?)");
            }
        }
        // Method to display the respones
        static void DisplayBotResponse(string message, int delay = 50)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Chatbot: ");
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        // Exit chat
        static void ExitChat()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            DisplayTypingEffect($"\nChatbot: Thank you for chatting, {userName}! Stay safe online.");
            Console.ResetColor();
            Thread.Sleep(2000);
            Console.Clear();
        }
        // Display the typing effect
        static void DisplayTypingEffect(string message, int delay = 50)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
    }
}
