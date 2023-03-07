# ChatGPT_API_Test_Console_App

A simple console app that sends your question or statement to ChatGPT for processing and it then prints out the result.
OpenAi doesnt have an official C# SDK, however this is their suggested SDK, therefore it was leveraged in this console app.
https://github.com/betalgo/openai

The app expects a secrets.json with the following secret (Replace the API key with yours):

{
  "ChatGPT:ServiceApiKey": "KEY_GOES_HERE"
}