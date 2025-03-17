# Database migrations

_This API is written in code-first mode using EF. It's recommended to read the `README.md`, to configure your database._

Go to project sources:

```shell
cd ./Sources/
```

## Add migration

```shell
dotnet ef migrations add "MigrationName" --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj -c DatabaseContext
```

## Apply migration (update database)

```shell
dotnet ef database update --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj -c DatabaseContext
```

## Remove migration

```shell
dotnet ef migrations remove --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj -c DatabaseContext
```

## Drop database

```shell
dotnet ef database drop --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj
```

## List migrations

```shell
dotnet ef migrations list --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj
```

## Rollback to old migration

```shell
dotnet ef database update "OldMigrationName" --startup-project .\Shizima.WebAPI\Shizima.WebAPI.csproj -c DatabaseContext
```