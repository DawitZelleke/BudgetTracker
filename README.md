# Budget Tracker

An ASP.NET Core MVC web application for logging personal income and expense
transactions against a set of spending categories. Built on Entity Framework
Core over a SQLite database.

## Project context

Academic ASP.NET Core MVC project. My work beyond the base CRUD: database-side sorting, server-controlled timestamps, and antiforgery-protected edit and delete flows.

Start with [TransactionsController.cs](Controllers/TransactionsController.cs) to inspect those decisions. The project is also called Ledgerline in its interface.

## Features

- **Transaction ledger** — create, edit and delete income and expense entries,
  each tagged with a category, date, amount and description.
- **Sortable columns** — the Date, Description and Amount headers toggle between
  ascending and descending. Sorting is applied to the `IQueryable` before it is
  materialised, so SQLite does the ordering rather than the view.
- **Inline delete** — deletes go through a `POST` with an antiforgery token and a
  client-side `confirm()` dialog, instead of the scaffolded two-step
  GET-confirmation page.
- **Audit timestamps** — `CreatedAt` is set once on insert and preserved on
  update; `UpdatedAt` is refreshed server-side on every edit. The edit action
  reloads the tracked entity and copies fields onto it, so a client cannot
  overwrite `CreatedAt` by posting it back.

## Stack

| | |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Data access | Entity Framework Core 8, SQLite provider |
| Views | Razor, Bootstrap 5, jQuery unobtrusive validation |
| Database | SQLite (`BudgetTracker.db`, committed with sample data) |

## Running it

```bash
git clone https://github.com/DawitZelleke/BudgetTracker.git
cd BudgetTracker
libman restore     # fetches Bootstrap and jQuery into wwwroot/lib
dotnet restore
dotnet run
```

Then open `https://localhost:7153`. The default route lands on
`Transactions/Index`.

The connection string lives in `appsettings.json` under
`ConnectionStrings:BudgetTrackerConnection` and points at the SQLite file in the
project root, so no database setup is needed.

## Data model

```
Category (Id, Name)
    └── Transaction (Id, Amount, TransactionDate, TransactionType,
                     CategoryId, Description, CreatedAt, UpdatedAt)
```

`TransactionsController.Index` eager-loads `Category` with `.Include()` so the
ledger renders category names without an N+1 query, and calls `.AsNoTracking()`
since the list is read-only.

## Project layout

```
Controllers/     TransactionsController (CRUD + sorting), HomeController
DataAccess/      BudgetTrackerContext, Transaction, Category entities
Views/           Transactions/ (Index, Create, Edit), Shared/ (_Layout)
wwwroot/         site.css (Ledgerline theme), site.js
```

## Evaluation and limitations

Use the .NET 10 SDK. If `libman` is unavailable, install its CLI with `dotnet tool install --global Microsoft.Web.LibraryManager.Cli` and ensure the .NET tools directory is on PATH. Follow the URL printed by `dotnet run`; launch-profile URLs can vary.

Known limitations: no user accounts or per-user data isolation yet.

To evaluate the behavior locally:

1. Create a transaction with a category and verify it appears in the list.
2. Sort by date, description, and amount in both directions.
3. Edit a record and verify its creation timestamp is unchanged.
4. Cancel a deletion, then confirm one, and verify the corresponding outcomes.
5. Submit a change without a valid antiforgery token and verify rejection.

Next engineering priorities: automate those checks, add user isolation, and replace the checked-in sample database with reproducible migrations and seed data.
