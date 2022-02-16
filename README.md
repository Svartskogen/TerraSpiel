# TerraSpiel
Interactive and persistent Discord Bot Game with PvP features

### Stack:
C#, .NET Core 3.1, Entity Framework Core, MySql (Current implementation, could use any EFCore supported SQL provider), DSharpPlus

### Build and Deploy:
#### Database Setup:
Terra Spiel is currently setup to work with **MySql**, but can easly change SQL provider, refer to the implementation on `Startup.cs`

> For the following EFCore commands, locate yourself on `\TerraSpiel\TerraSpiel`

- Database creation:
```
dotnet-ef migrations add InitialCreate -p ../TerraSpiel.DAL.Migrations/TerraSpiel.DAL.Migrations.csproj --context TerraSpiel.DAL.GameContext
```

- Database update/migrations:
```
dotnet-ef database update -p ../TerraSpiel.DAL.Migrations/TerraSpiel.DAL.Migrations.csproj --context TerraSpiel.DAL.GameContext
```

#### Build:
- Build TerraSpiel.Bot project
- config.json parameters: (located on build folder: `Json/config.json`)
  - `token:` Discord bot token, you must create your own on the Discord Developers site.
  - `prefix:` Bot commands prefix to listen to 
  - `connstring:` Database connection string
  - `enviroment:` No use for now, leave on Debug
  - The rest of the fields are useful parameters to change gameplay settings.

Finally, run `TerraSpiel.Bot.exe`

## Features:

WIP
