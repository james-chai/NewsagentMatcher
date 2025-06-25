NewsagentMatcher.Console - Setup Guide

A .NET 8 console application used to validate and match newsagent data using customizable strategies.

---

📦 REQUIREMENTS

- ✅ .NET 8 SDK (8.0.0 or later)
  Download: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

To check if it's installed, run:

    dotnet --version

If the version is not 8.0 or later, download and install from the link above.

---

🛠 SETUP INSTRUCTIONS

Step 1: Open Terminal (PowerShell or CMD)

Step 2: Navigate to the solution root folder

Step 3: Restore, Build, and Test the Solution

    dotnet restore
    dotnet build
    dotnet test

These commands will install dependencies, compile the solution, and run all unit tests.

---

🌱 SET DOTNET_ENVIRONMENT (Optional)

To enable appsettings.Development.json, set the environment to Development.

PowerShell:

    $env:DOTNET_ENVIRONMENT = "Development"

Command Prompt:

    set DOTNET_ENVIRONMENT=Development

To Check the Value:

PowerShell:

    $env:DOTNET_ENVIRONMENT

Command Prompt:

    echo %DOTNET_ENVIRONMENT%

---

🚀 RUNNING THE CONSOLE APP

From Console Project Folder:

    cd NewsagentMatcher.Console
    dotnet run

---

🧪 RUNNING TESTS

You can run tests from the solution root using:

    dotnet test

This will run all unit tests in the NewsagentMatcher.Tests project.

---

▶️ RUNNING THE APP IN VISUAL STUDIO 2022

1. Open the solution NewsagentMatcher.sln.
2. Set NewsagentMatcher.Console as the startup project.
3. (Optional) In launchSettings.json, add:

       "environmentVariables": {
           "DOTNET_ENVIRONMENT": "Development"
       }

Then press F5 to start debugging.

---

🧯 TROUBLESHOOTING

Environment variable doesn't appear to be set?

Check it again:

PowerShell:

    $env:DOTNET_ENVIRONMENT

CMD:

    echo %DOTNET_ENVIRONMENT%

Note: Environment variables set via terminal only apply to that session. You must re-set them each time.

---

📝 NOTES

- Logging is handled using Serilog.
- Matching logic is implemented per chain:
- Supports configuration via:
  - appsettings.json
  - appsettings.Development.json (if DOTNET_ENVIRONMENT=Development is set)

