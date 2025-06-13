# Shizima Web API

_This API is deployed on our personnal server. It's easier to use, because some prerequisites are needed to run it locally !_

Deployed version of : [ShizimaWebAPI](https://shizima-api.baptiiiste.com/swagger)


**Prerequisites:**

- _PostgresSQL database_

- _EntityFrameworkCore_

- _MinioServer_

## Getting started

Clone repo:

```shell
git clone https://gitlab.isima.fr/shizima/servers/api.git
```

Edit config to fit with your SQL Server database & Minio Server: 

> Sources/Shizima.WebAPI/appsettings.json
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=AppDBName;Username=username;Password=password"
},
"Minio": {
  "Endpoint": "url.to.your.minioserver",
  "AccessKey": "accessKey",
  "SecretKey": "sevretKey",
  "UseSSL": "true"
},
```

You'll need also to need to subscribe to our NuGet feed to get the librairies we just developed.

```sh
dotnet nuget add source "https://gitlab.isima.fr/api/v4/groups/11770/-/packages/nuget/index.json" --name "gitlab.isima" --username youtGitLabUsername --password youtGitLabAccessToken 
```

Then, Restore & Build:

```shell
cd Sources/

dotnet restore

dotnet build --no-restore
```

Run the project using `https` or `IIS`. The Swagger doc should automatically open. 