# OmniRecipes API

**IPORTANT**: This project is currently in development and not yet ready for use.

This is an API for OmniRecipes, a platform that provides a wide variety of recipes. 
Currently it allows to retrieve all recipes and add new recipes.

The data is stored in a MongoDB.

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
To run the web app locally, you need to have MongoDB installed and running on your machine. Unfortunately currently it also needs a connection to an Azure storage account. I'm still planning to introduce a way to run it locally without this connection.

1. Clone the repository to your local machine.
3. Start your MongoDB server. Create a database and a table to store your recipes. Add your connection String, database name and collection name to `appsettings.json` for development.
4. Store an API key to access your app in the `appsettings.json`
5. Store your azure storage endpoint in an environment variable or in the secrets manager with the key `Azure:StorageEndpoint" 
6. Navigate to the project directory and install the necessary dependencies with `dotnet restore`.
7. Build the project with `dotnet build`.
8. Start the server with `dotnet run`.

Now, the web app should be running locally on your machine.

## Licence
This project is licensed under the terms of the MIT license.
