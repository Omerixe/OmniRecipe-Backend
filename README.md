# OmniRecipes API

**IMPORTANT**: This project is currently in development and not yet ready for use.

This is an API for OmniRecipes, a platform that provides a wide variety of recipes. 
Currently it allows to retrieve all recipes and add new recipes.

The data is stored in a MongoDB. The images are stored in an Azure Storage service if run in release mode.

I started this project to get a better understanding of writing and hosting my own backend. Currently the backend is hosted on Azure where my app [OmniRecipe-Android](https://github.com/Omerixe/OmniRecipe-Android) connects to it.

## Features
- Add new recipes which get stored in a database
  - Images get uploaded to Azure Storage
- Retrieve recipes from the database

**Outlook**
- Edit recipes
- Delete recipes

## Techstack
- .NET Core
- MongoDB
- Azure Storage
- Azure App Service

## Architecture
The webapp is not yet following any architecture pattern. It was created based on different tutorials, gut feeling and Copilot suggestions. I will put more time into this as soon as I get to a point where I have enough understanding of how .NET Core works.

## Installation
To run the web app locally, you need to have MongoDB installed and running on your machine. 

1. Clone the repository to your local machine.
3. Start your MongoDB server. Create a database and a table to store your recipes. Add your connection String, database name and collection name to `appsettings.json` for development.
4. Store an API key to access your app in the `appsettings.json`
5. Navigate to the project directory and install the necessary dependencies with `dotnet restore`.
6. Build the project with `dotnet build`.
7. Start the server with `dotnet run`.

Now, the web app should be running locally on your machine.

For a release version of the app, it needs to have `Azure:StorageEndpoint` set to the endpoint of your azure storage. This can either be set as environment variable or in your configuration on Azure.

## Licence
This project is licensed under the terms of the MIT license.
