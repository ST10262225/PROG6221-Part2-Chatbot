using System;
using System.Collections.Generic;
using System.Linq;

namespace Prog_Part1
{
    class FilterBot
    {
        // Dictionary to store responses for various cybersecurity-related topics.
        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>();

        // A set of common words to ignore in the input.
        private HashSet<string> ignoreWords = new HashSet<string>
        {
            "is", "the", "a", "can", "i", "my", "what", "about", "tell", "me", "how"
        };

        // Random object to select random responses from the available ones.
        private Random random = new Random();

        // Property to store the last recognized cybersecurity topic
        public string LastTopic { get; set; } = null;

        // Public getter for the responses dictionary
        public Dictionary<string, List<string>> Responses
        {
            get { return responses; }
        }

        public FilterBot()
        {
            responses = new Dictionary<string, List<string>>();
            StoreResponses();
        }

        // Method to populate the 'responses' dictionary with cybersecurity-related information.
        private void StoreResponses()
        {
            // Adding responses related to 'password' keyword.
            responses["password"] = new List<string>
            {
                "Passwords need to be protected and kept safe.",
                "Use a mix of letters, numbers, and special characters for better security.",
                "Avoid using the same password for multiple accounts.",
                "Consider using a password manager to store complex passwords securely.",
                "Change your passwords regularly to minimize risk."
            };

            // Adding responses related to 'phishing' keyword.
            responses["phishing"] = new List<string>
            {
                "Phishing is an attempt to steal personal information by pretending to be a trustworthy source.",
                "Never click on suspicious links in emails or messages.",
                "Always verify the sender's identity through official channels.",
                "Be wary of emails asking for urgent action or personal details.",
                "Report suspicious emails to your email provider."
            };

            // Adding responses related to 'malware' keyword.
            responses["malware"] = new List<string>
            {
                "Malware is malicious software designed to harm or exploit your device.",
                "Use reliable antivirus software and keep it updated.",
                "Avoid downloading files from unknown sources.",
                "Be cautious of suspicious attachments in emails.",
                "Regularly scan your system for malware."
            };

            // Adding responses related to 'firewall' keyword.
            responses["firewall"] = new List<string>
            {
                "A firewall is a security system that monitors and controls network traffic.",
                "Firewalls help prevent unauthorized access to your computer or network.",
                "Ensure your firewall is always enabled for better security.",
                "Configure your firewall settings to allow only necessary connections.",
                "A hardware firewall can provide an additional layer of protection."
            };

            // Adding responses related to 'encryption' keyword.
            responses["encryption"] = new List<string>
            {
                "Encryption is the process of converting data into a secure format.",
                "It helps protect sensitive information from unauthorized access.",
                "Always encrypt important data before transmitting it online.",
                "Look for HTTPS when browsing websites to ensure encryption.",
                "Use encryption tools for sensitive files stored on your device."
            };

            // Adding responses related to 'social' engineering keyword.
            responses["social"] = new List<string>
            {
                "Social engineering is a manipulation technique used to trick people into revealing confidential information.",
                "Be cautious of unexpected requests for personal data.",
                "Verify the identity of people asking for sensitive information before sharing anything.",
                "Don't trust unsolicited phone calls or messages asking for personal details.",
                "Educate yourself on common social engineering tactics."
            };

            // Adding responses related to 'authentication' keyword.
            responses["authentication"] = new List<string>
            {
                "Authentication verifies your identity before granting access to a system.",
                "Multi-factor authentication (MFA) adds an extra layer of security.",
                "Never share your authentication credentials with anyone.",
                "Use strong and unique passwords for all your accounts.",
                "Consider using biometric authentication where available."
            };

            // Adding responses related to 'privacy' keyword.
            responses["privacy"] = new List<string>
            {
                "Data privacy ensures that personal information is protected from unauthorized access.",
                "Adjust your privacy settings on social media to limit who can see your data.",
                "Be mindful of the information you share online.",
                "Review the privacy policies of websites and apps you use.",
                "Use privacy-focused search engines and browsers."
            };

            // Adding responses related to 'vpn' keyword.
            responses["vpn"] = new List<string>
            {
                "A VPN (Virtual Private Network) encrypts your internet connection to protect your data and privacy.",
                "Using a VPN helps you stay secure on public Wi-Fi networks by hiding your IP address.",
                "A VPN allows you to access region-restricted content while keeping your online activity private.",
                "Choose a reputable VPN provider with a strong privacy policy.",
                "Be aware that free VPNs may have privacy limitations."
            };

            // Adding responses related to '2fa' keyword.
            responses["2fa"] = new List<string>
            {
                "Two-Factor Authentication (2FA) adds an extra security step by requiring a second verification method, like a text code.",
                "Always enable 2FA on important accounts to prevent unauthorized access, even if your password gets stolen.",
                "Authenticator apps like Google Authenticator or Authy are safer than SMS codes, which can be intercepted.",
                "Keep your recovery codes in a safe place in case you lose access to your 2FA device.",
                "Be wary of fake login pages asking for your 2FA code."
            };

            // Adding responses related to 'wifi' keyword.
            responses["wifi"] = new List<string>
            {
                "Public Wi-Fi networks are often unsecured, making it easy for hackers to intercept your data.",
                "Avoid logging into banking or sensitive accounts while connected to public Wi-Fi.",
                "Using a VPN on public Wi-Fi helps encrypt your data and keeps your browsing private.",
                "Disable automatic Wi-Fi connection when you're not using it.",
                "Consider using your mobile hotspot instead of public Wi-Fi for sensitive tasks."
            };

            // Adding responses related to 'safe browsing' keyword.
            responses["browsing"] = new List<string>
            {
                "Always check the URL before entering personal information—phishing sites often look identical to real ones.",
                "Look for 'HTTPS' in the website address—it means your connection is secure.",
                "Avoid clicking on pop-ups or ads claiming you won a prize—they are often scams.",
                "Keep your web browser updated to the latest version for security patches.",
                "Use browser extensions that enhance your privacy and security."
            };

            //Adding responses related to 'scam' keyword.
            responses["scam"] = new List<string>
            {
                "Be wary of online offers that seem too good to be true; they are often scams.",
                "Never share personal or financial information with unsolicited contacts.",
                "Verify the legitimacy of websites and emails before providing any details.",
                "Phishing is a common type of online scam used to steal your information.",
                "Report online scams to the relevant authorities."
            };
        }

        // Method to get a response based on the user's input.
        public string GetResponse(string input)
        {
            // Convert input to lowercase and split into words.
            char[] delimiters = new char[] { ' ', '.', ',', '?', '!', ';', ':', '-', '_' };
            string[] inputWords = input.ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            List<string> topicResponses = new List<string>();
            string currentTopic = null;
            bool foundMatch = false;

            // Loop through the user input and check for keyword matches.
            foreach (var keyword in responses.Keys)
            {
                if (inputWords.Contains(keyword) || inputWords.Any(word => word.StartsWith(keyword)))
                {
                    List<string> keywordResponses = responses[keyword];

                    if (keywordResponses.Count > 0)
                    {
                        int randomIndex = random.Next(keywordResponses.Count);
                        topicResponses.Add(keywordResponses[randomIndex]);
                        currentTopic = keyword;
                        foundMatch = true;
                    }
                }
            }

            // If responses were found, join and return them as a string
            if (topicResponses.Count > 0)
            {
                LastTopic = currentTopic; // Set LastTopic when a topic is successfully identified
                return string.Join("\n", topicResponses);
            }

            // If no responses were found, return a default message
            return "That's an interesting question! However, I don't have specific information on that right now. " +
                   "Could you try rephrasing it or asking about a different cybersecurity area?";
        }
    }
}




