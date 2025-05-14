# PROG6221-Part2-Chatbot

A simple console-based chatbot designed to raise awareness about cybersecurity topics.

## Overview

This chatbot interacts with users to answer their questions about various cybersecurity concepts. It can:

* Respond to general cybersecurity inquiries.
* Remember the user's name and favorite cybersecurity topic for a more personalized experience.
* Personalize responses based on the user's stated interests.
* Attempt to elaborate on previous topics if the user seems confused or asks for more detail.
* Detect basic sentiment in user input to adjust responses accordingly.
* Play a greeting sound upon startup.
* Display the chatbot's name in ASCII art.

## Features

Here's a breakdown of the key features:

* **Interactive Chat:** Users can type questions and receive responses related to cybersecurity.
* **Personalization:** The chatbot greets the user by name and tailors some advice based on their favorite cybersecurity topic.
* **Memory:** The chatbot remembers the user's name and preferred topic throughout the current session.
* **Elaboration:** If the user asks for more information or expresses confusion, the bot tries to provide additional details on the last discussed topic.
* **Sentiment Detection:** Basic sentiment analysis helps the bot respond with appropriate tone.
* **ASCII Art:** A visual welcome message using ASCII art.
* **Greeting Sound:** An audio cue when the chatbot starts.

## How to Run

To run this chatbot, you will need:

* **.NET SDK (Software Development Kit):** Ensure you have the .NET SDK installed on your system. You can download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download).
* **A Compatible Operating System:** This is a console application and should run on Windows, macOS, or Linux with the .NET runtime.
* **(Optional) Audio File:** The project includes a feature to play a greeting sound. Ensure the `Audio` folder with `greeting.wav` is in the same directory as the compiled executable, or adjust the path in the `Program.cs` file.
