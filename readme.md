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

+ Set the connection string in `appsettings.json` or add `DATINGAPP_CONNECTION_STRING` enviroment variable
+ Add your [Cloudinary](https://cloudinary.com/) configuration
+ Set the `ADMIN_PASSWORD` enviroment variable
+ Add a `TokenKey` to your configuration

```json
{
  "CloudinarySettings": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  },
  "TokenKey": ""
}
```