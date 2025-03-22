---
sidebar_position: 2
---

# Quick Start

## TODOS
- **Add rest of code for setting up.**
- **Do with AI once everything else is finished**

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

## Prerequisites

<Tabs groupId="ide-choice">
  <TabItem value="visual-studio" label="Visual Studio">
    * .NET 8.0 SDK installed on your system
    * Visual Studio 2022 with the web development components installed
  </TabItem>
  <TabItem value="vscode" label="VS Code">
    * Current version of the .NET SDK
    * VS Code installed on your system
    * C# DevKit extension installed
  </TabItem>
</Tabs>

## Installation

<Tabs groupId="ide-choice">
  <TabItem value="visual-studio" label="Visual Studio">
    1. Launch Visual Studio 2022 and click the **Create a new project** option
    2. From the project templates, locate and choose **ASP.NET Core Web API**, then click **Next**
    3. When prompted for project details, type `OrleansURLShortener` as your project name and proceed with **Next**
    4. In the final setup screen, ensure **.NET 8.0 (Long Term Support)** is selected, deselect the **Use controllers** option, and finish by clicking **Create**
  </TabItem>
  <TabItem value="vscode" label="VS Code">
    1. Open VS Code and access its integrated terminal
    2. Navigate to your preferred project directory
    3. Execute these commands in sequence:
    ```
    dotnet new webapi -o OrleansURLShortener
    code -r OrleansURLShortener
    ```
    These commands will generate a new Minimal API project and open it in VS Code.

    When VS Code prompts you about trusting the workspace:
    * Select the option to trust all files in the parent folder
    * Confirm by clicking **Yes, I trust the authors** (this is safe as you created the project)
  </TabItem>
</Tabs>

## Configuration

<Tabs groupId="ide-choice">
  <TabItem value="visual-studio" label="Visual Studio">
    1. In Solution Explorer, locate your project and open the NuGet Package Manager
    2. Use the search functionality to find the Orleans packages
    3. Locate and install the **Microsoft.Orleans.Server** package
  </TabItem>
  <TabItem value="vscode" label="VS Code">
    Add the Orleans server package by running this command in the terminal:
    ```
    dotnet add package Microsoft.Orleans.Server
    ```
  </TabItem>
</Tabs>

## Running Your First Project

<Tabs groupId="ide-choice">
  <TabItem value="visual-studio" label="Visual Studio">
    1. Click the run button in Visual Studio to launch the application. You should see a welcome message in your browser
    2. To test the URL shortener, append `/shorten?url=https://learn.microsoft.com` to your localhost address in the browser. This will generate a shortened URL
    3. Test the shortened URL by copying it and opening it in your browser - it should redirect you to the Microsoft Learn website
  </TabItem>
  <TabItem value="vscode" label="VS Code">
    1. Launch the application using the following terminal command:
    ```
    dotnet run
    ```
    2. Once running, navigate to your localhost URL and add `/shorten?url=https://learn.microsoft.com` to create a shortened link
    3. Copy and use the generated short URL - it will automatically redirect to the Microsoft Learn portal
  </TabItem>
</Tabs>