# Connect the course tutor

The global data analytics chatbot uses the Gemini REST API from the ASP.NET server. It is available from **Ask Question** in the top navigation. Course forums are separate.

1. Create a Gemini API key in [Google AI Studio](https://aistudio.google.com/app/apikey).
2. Add a **server environment variable** named `GEMINI_API_KEY` with that key. On Windows, use **System Properties → Environment Variables** for the account running Visual Studio or IIS Express. Restart Visual Studio/IIS Express after adding it. In deployment, set the variable on the web server.
3. Open **Ask Question** from the top navigation. The header should show **AI ready**. Ask a data analytics question to verify the API connection.

Do not put the key in `Web.config`, a `.aspx` page, JavaScript, or Git. The app checks the process, Windows User, and Machine environment scopes, so a User variable is visible even when IIS Express still has an older process environment. The chatbot uses Gemini 3.5 Flash-Lite and retries Gemini 3.8 Flash when the first model returns HTTP 503. The endpoints are in `App_Code/DojoAi.cs`.

The chatbot covers general data analytics topics, regardless of which course the student is viewing. Recent messages are kept in the student's session for context. The system instruction asks the model to decline unrelated questions; a prompt is not a perfect guarantee, so review behavior with real student questions before public release.
