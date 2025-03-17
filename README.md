# Shizima Web API

**Prerequisites:**

- _PostgresSQL database_

- _EntityFrameworkCore_

## Getting started

Clone repo:

```shell
git clone https://gitlab.isima.fr/shizima/servers/api.git
```

Edit config to fit with your SQL Server database: 

> Sources/Shizima.WebAPI/appsettings.json
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=AppDBName;Username=username;Password=password"
},
```

Restore & Build:

```shell
cd Sources/

dotnet restore

dotnet build --no-restore
```

Run the project using `https` or `IIS`. The Swagger doc should automatically open. 