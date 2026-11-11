# Home Ledger API

Home Ledger is an ASP.NET Core 10 API. All API, business model, and persistence names are in English.

## Request flow

For a category insert, `CategoriesController` receives a `CategoryDto`, uses AutoMapper to create a Domain `Category` business model, and calls `CategoryUseCase`. The use case calls `ICategoryRepository`. Infrastructure's `CategoryRepository` implements that contract, starts a transaction, maps the business model to `CategoryEntity`, and calls `ICategoryData`. `CategoryData` inherits the EF operations from `DataFactory<CategoryEntity>`, which uses `HomeLedgerDbContext` to write through EF Core. The repository commits the transaction and maps the saved entity back to a business model; the controller maps it to the response DTO.

The same separation is used for groupings, locations, expenses, incomes, and payment types. The Domain owns business models and repository contracts. Application owns one use case per model. Infrastructure owns EF entities, EF Data interfaces and implementations, mapping profiles, and repository implementations. API owns controllers, DTOs, and the API mapping profile.

## Projects

- `API`: one HTTP controller per business model.
- `API/HomeLedger.ApiContract`: one DTO per business model.
- `Business/HomeLedger.Domain/BusinessModels`: business models, one class per entity; `Repositories` holds the repository contract for each model.
- `Business/HomeLedger.Application/UseCases`: one CRUD use case and its interface per model.
- `Infrastructure`: EF Core entities, one class per entity. Each repository implements its Domain contract and inherits CRUD operations from the EF `DataFactory` through the single `IData<T>` interface.
- `Tests`: unit and integration test projects.

## Run

On Windows with a local SQL Server instance, run `dotnet run --project API/HomeLedger.Api`. The API uses the `HomeLedger` connection string from `appsettings.json` and applies pending SQL Server Entity Framework migrations on startup. In other environments, set `ConnectionStrings__HomeLedger` to a SQL Server connection string before starting the API.

To run SQL Server and the API in Docker, set a strong SQL Server SA password and start Compose. For PowerShell:

```powershell
$env:MSSQL_SA_PASSWORD = "Choose_A_Strong_Password_123!"
docker compose up --build
```

Compose starts SQL Server 2022 with a named persistent volume, waits for its health check, then starts the API. The API applies pending migrations at startup. Do not commit production passwords. For cloud deployment, set `ConnectionStrings__HomeLedger` as a secret to the managed SQL Server connection string; migrations run automatically when the API starts.
## Endpoints

Each resource supports `GET /api/{resource}`, `GET /api/{resource}/{id}`, `POST /api/{resource}`, `PUT /api/{resource}/{id}`, and `DELETE /api/{resource}/{id}`.

Resources: `categories`, `groupings`, `locations`, `expenses`, `incomes`, and `paymenttypes`.

Categories reference a grouping. Expenses reference a location and a category. Amounts use decimal values with two fractional digits; dates use `YYYY-MM-DD`. EF entities remain in Infrastructure; repositories translate them to Domain business models before returning them through Domain repository interfaces to Application.

## Tests and coverage

Run `dotnet test --solution HomeLedger.slnx` to execute tests. Run `dotnet test --solution HomeLedger.slnx --coverlet --coverlet-output-format cobertura` to generate Cobertura coverage reports with Coverlet.MTP. Coverage settings are stored in each test project's `testconfig.json`.
