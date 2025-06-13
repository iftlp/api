# Shizima Web API

_This API is deployed on our personnal server. It's easier to use, because some prerequisites are needed to run it locally !_

Deployed version of : [ShizimaWebAPI](https://shizima-api.baptiiiste.com/swagger)

## Run the API in Docker

**Prerequisites:**

- _PostgresSQL database_

- _MinioServer_

You need to pull this docker image: `docker.isima.fr/axdelafuen/shizima_webapi@latest`. (You can also build the image your self, the Dockerfile is in `./Sources/Shizima.WebAPI/Dockerfile`)

Then setup some env variables:

- `ConnectionStrings__DefaultConnection` : "Host=localhost;Port=5432;Database=AppDBName;Username=username;Password=password"
- `Minio__Access__Key` : "accessKey"
- `Minio__Secret__Key` : "secretKey"

## Run the API locally

**Prerequisites:**

- _PostgresSQL database_

- _EntityFrameworkCore_

- _MinioServer_


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
  "SecretKey": "secretKey",
  "UseSSL": "true"
},
```

You'll need also to need to subscribe to our NuGet feed to get the librairies we just developed.

```sh
dotnet nuget add source "https://gitlab.isima.fr/api/v4/groups/11770/-/packages/nuget/index.json" --name "gitlab.isima" --username youtGitLabUsername --password youtGitLabAccessToken 
```

Finnaly, you'll have to migrate our databse. Please refer to the document `migrations.md`.

Then, Restore & Build:

```shell
cd Sources/

dotnet restore

dotnet build --no-restore
```

Run the project using `https` or `IIS`. The Swagger doc should automatically open. 