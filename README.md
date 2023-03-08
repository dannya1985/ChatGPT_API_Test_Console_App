# ChatGPT API Test Console App

A simple console app that lets you make two simple choices.
The first sends your question or statement to ChatGPT for processing and it then prints out the result.
The second uses a prompt to generate the specified number of images based on your prompt, these are saved to the executables folder and windows explorer is opened automatically so you may preview them since this is a console app.

OpenAi doesnt have an official C# SDK, however Betalgo.OpenAI.GPT3 is their suggested SDK, therefore it was leveraged in this console app.
[![Betalgo.OpenAI.GPT3](https://img.shields.io/nuget/v/Betalgo.OpenAI.GPT3?style=for-the-badge)](https://www.nuget.org/packages/Betalgo.OpenAI.GPT3/)
https://github.com/betalgo/openai


Your API Key comes from here --> https://platform.openai.com/account/api-keys

API overview --> https://platform.openai.com/docs/introduction/overview



The app expects a secrets.json with the following secret (Replace the API key with yours):

{
  "ChatGPT:ServiceApiKey": "KEY_GOES_HERE"
}