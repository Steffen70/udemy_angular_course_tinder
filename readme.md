# Project Setup

Execute the following commands to restore the required packages

```bash
/Udemy_Angular_Course
$ dotnet restore DatingApp.sln
```

Navigate to the `client` folder and run

```bash
/Udemy_Angular_Course/client
$ npm install
```

## Configuration

+ Create an `appsettings.json` file
+ Add your [Cloudinary](https://cloudinary.com/) configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  },
  "AllowedHosts": "*"
}
```