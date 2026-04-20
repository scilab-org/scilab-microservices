# Microservices — Unit Test Guide

## Table of Contents

1. [Overview](#overview)
2. [Tech Stack](#tech-stack)
3. [Project Structure](#project-structure)
4. [Code Style & Conventions](#code-style--conventions)
5. [Test Anatomy](#test-anatomy)
6. [Layers and What to Test](#layers-and-what-to-test)
7. [Mocking Guidelines](#mocking-guidelines)
8. [Test Data Helpers](#test-data-helpers)
9. [How to Run Tests](#how-to-run-tests)
10. [Success Criteria](#success-criteria)
11. [Common Pitfalls](#common-pitfalls)

---

## Overview

This repository follows **Clean Architecture** with four distinct layers per service. Each layer has its own dedicated test project that validates only the logic within that layer in isolation.

```
src/Services/{ServiceName}/
├── Core/
│   ├── {ServiceName}.Domain/       ← pure domain logic; no external deps
│   ├── {ServiceName}.Application/  ← CQRS handlers, validators, service contracts
│   └── {ServiceName}.Infrastructure/ ← Keycloak, MinIO, Marten, HTTP clients
├── Api/
│   └── {ServiceName}.Api/          ← Carter endpoints, middleware
└── Tests/
    ├── {ServiceName}.Domain.Tests/
    ├── {ServiceName}.Application.Tests/
    ├── {ServiceName}.Infrastructure.Tests/
    └── {ServiceName}.Api.Tests/
```

**Guiding principle:** A test in layer X may **not** depend on the real implementation of layer Y. Dependencies are always mocked at the boundary.

---

## Tech Stack

| Library | Version | Purpose |
|---|---|---|
| `xunit` | 2.9.x | Test runner and assertion framework |
| `Moq` | 4.20.x | Mock creation for interfaces |
| `FluentAssertions` | 6.12.x | Readable, English-like assertion syntax |
| `coverlet.collector` | 6.0.x | Code coverage data collection |
| `Microsoft.NET.Test.Sdk` | 17.11.x | .NET test host |
| `Microsoft.Extensions.Logging.Abstractions` | 8.0.x | `NullLogger<T>` — silent logging in tests |
| `Microsoft.Extensions.Configuration` | 8.0.x | In-memory `IConfiguration` for Infrastructure tests |
| `Microsoft.AspNetCore.Mvc.Testing` | 8.0.x | `WebApplicationFactory` + `TestServer` for API integration tests |

All versions are pinned centrally in [`src/Directory.Packages.props`](../src/Directory.Packages.props).  
**Never** specify a version number directly in a `.csproj` test project file.

---

## Project Structure

### Test project naming

```
{ServiceName}.{Layer}.Tests
```

Examples:
- `User.Application.Tests`
- `User.Domain.Tests`
- `User.Infrastructure.Tests`

### Internal folder layout

The folder hierarchy inside each test project **mirrors the production project** it tests. This makes it trivial to locate the test for any given class.

```
{ServiceName}.Application.Tests/
├── Common/
│   ├── BaseTest.cs                          # Shared helpers (CancellationToken, NullLogger factory)
│   └── TestData/
│       └── {Entity}TestData.cs              # Static factory methods for test fixtures
├── Exceptions/
│   └── ApplicationExceptionTests.cs
├── Features/
│   └── {Feature}/
│       ├── Commands/
│       │   └── {Command}HandlerTests.cs
│       ├── Queries/
│       │   └── {Query}HandlerTests.cs
│       └── Validators/
│           └── {Validator}Tests.cs
├── GlobalUsing.cs                           # Shared using directives for the test project
└── {ServiceName}.Application.Tests.csproj

{ServiceName}.Domain.Tests/
├── Abstractions/
│   └── AggregateTests.cs
├── Entities/
│   └── {Entity}Tests.cs
├── Exceptions/
│   └── DomainExceptionTests.cs
├── GlobalUsing.cs
└── {ServiceName}.Domain.Tests.csproj

{ServiceName}.Infrastructure.Tests/
├── Exceptions/
│   └── InfrastructureExceptionTests.cs
├── Services/
│   └── {Service}Tests.cs
├── Repositories/
│   └── {Repository}Tests.cs
├── GlobalUsing.cs
└── {ServiceName}.Infrastructure.Tests.csproj

{ServiceName}.Api.Tests/
├── Infrastructure/
│   ├── ApiTestFactory.cs            # WebApplicationFactory subclass with minimal in-process server
│   └── TestAuthHandler.cs           # Authentication handler that reads claims from request headers
├── Endpoints/
│   └── {Feature}EndpointTests.cs
├── GlobalUsing.cs
└── {ServiceName}.Api.Tests.csproj
```

### File naming rule

```
{ProductionClassName}Tests.cs
```

| Production class | Test file |
|---|---|
| `CreateUserCommandHandler` | `CreateUserCommandHandlerTests.cs` |
| `OutboxMessageEntity` | `OutboxMessageEntityTests.cs` |
| `KeycloakService` | `KeycloakServiceTests.cs` |
| User-facing endpoints (e.g. `/users`, `/groups`) | `UsersEndpointTests.cs`, `GroupsEndpointTests.cs` |

---

## Code Style & Conventions

### Test method naming

All test methods follow this pattern:

```
{MethodOrBehavior}_Should{ExpectedOutcome}_When{Condition}
```

Examples:

```csharp
Handle_ShouldReturnUserId_WhenUserIsCreatedSuccessfully()
Create_ShouldInitializeEntityWithCorrectValues()
GetGroupRolesAsync_ShouldThrowInfrastructureException_WithGroupNotFound_WhenApiReturns404()
Validate_ShouldFail_WhenEmailIsInvalidFormat()
```

### Test class structure

Each test class is `sealed`. Dependencies are declared as `readonly` fields and wired up in the **constructor**, not in a `[SetUp]` method.

```csharp
public sealed class CreateUserCommandHandlerTests : BaseTest
{
    private readonly Mock<IKeycloakService> _keycloakService = new();
    private readonly Mock<IMinIoCloudService> _minIoCloudService = new();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _sut = new CreateUserCommandHandler(
            _keycloakService.Object,
            _minIoCloudService.Object,
            CreateLogger<CreateUserCommandHandler>());
    }
}
```

### One concern per test

Every `[Fact]` validates exactly **one behaviour**. Do not combine unrelated assertions in a single test.

### `GlobalUsing.cs`

Each test project has a `GlobalUsing.cs` that declares all `global using` directives needed across that project, keeping individual test files free from boilerplate `using` statements.

---

## Test Anatomy

Every test follows **Arrange / Act / Assert (AAA)**. Each section must be marked with a comment.

```csharp
[Fact]
public async Task Handle_ShouldReturnTrue_WhenUserIsActivatedSuccessfully()
{
    // Arrange
    const string userId = "user-id-001";
    var command = new ActivateUserCommand(userId, actor: UserTestData.SystemActor());
    _keycloakService
        .Setup(s => s.ActivateUserAsync(userId, CancellationToken))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _sut.Handle(command, CancellationToken);

    // Assert
    result.Should().BeTrue();
    _keycloakService.Verify(s => s.ActivateUserAsync(userId, CancellationToken), Times.Once);
}
```

| Section | Responsibility |
|---|---|
| **Arrange** | Configure inputs, mocks, and expected values |
| **Act** | Invoke exactly one method on the SUT |
| **Assert** | Verify return value and any side effects |

---

## Layers and What to Test

### Domain layer — `{ServiceName}.Domain.Tests`

**Reference:** `{ServiceName}.Domain.csproj`  
**External dependencies mocked:** None — the Domain layer is pure C# with no infrastructure.

| Type | What to test |
|---|---|
| **Entities with behaviour** | Every public method: factory `Create`, state transitions, computed properties |
| **Aggregate root** | `AddDomainEvent`, `ClearDomainEvents` (returns copy, clears internal list), `DomainEvents` read-only contract |
| **Domain Exceptions** | Constructor sets `Message`, inherits from `Exception` |
| **Interfaces, abstract base classes, pure property bags** | Annotate with `[ExcludeFromCodeCoverage]` — no testable logic |

**Example — entity method test:**

```csharp
[Fact]
public void RecordFailedAttempt_ShouldIncrementAttemptCountAndSetNextAttempt_WhenBelowMaxAttempts()
{
    // Arrange
    var entity = OutboxMessageEntity.Create(Guid.NewGuid(), "UserCreated", "{}", DateTimeOffset.UtcNow);

    // Act
    entity.RecordFailedAttempt("network error", DateTimeOffset.UtcNow);

    // Assert
    entity.AttemptCount.Should().Be(1);
    entity.LastErrorMessage.Should().Be("network error");
    entity.NextAttemptOnUtc.Should().NotBeNull();
}
```

---

### Application layer — `{ServiceName}.Application.Tests`

**Reference:** `{ServiceName}.Application.csproj`  
**External dependencies mocked:** All service interfaces (`IKeycloakService`, `IMinIoCloudService`, etc.)

| Type | What to test |
|---|---|
| **Command handlers** | Happy path, error paths, service method invocation counts |
| **Query handlers** | Correct delegation to service and data mapping |
| **FluentValidation validators** | Each rule: valid input passes, invalid input fails with correct error message |
| **Application Exceptions** | Constructor message and inheritance from `Exception` |
| **DI wiring, AutoMapper profiles, pure DTOs** | Annotate with `[ExcludeFromCodeCoverage]` |

**Example — validator test:**

```csharp
[Fact]
public async Task Validate_ShouldFail_WhenEmailIsInvalidFormat()
{
    // Arrange
    var command = new CreateUserCommand { Email = "not-an-email" };
    var validator = new CreateUserCommandValidator();

    // Act
    var result = await validator.ValidateAsync(command);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Email));
}
```

**Handling ambiguous `ApplicationException` name:**

Because `System.ApplicationException` exists, always import the custom exception using a `using` alias at the top of the file:

```csharp
using AppException = {ServiceName}.Application.Exceptions.ApplicationException;
```

---

### API layer — `{ServiceName}.Api.Tests`

**Reference:** `{ServiceName}.Api.csproj`  
**External dependencies mocked:** `ISender` (MediatR) — the handler chain is replaced with a `Mock<ISender>`; authentication is replaced with `TestAuthHandler`.

Carter endpoint handlers are `private` methods, so they cannot be unit-tested directly. Instead, tests send real HTTP requests to an in-process `TestServer` and assert on the HTTP response.

**Test host setup** — `ApiTestFactory` creates a minimal `WebApplication` (bypassing `Program.cs` entirely) with:
- `AddAuthentication("Test")` + a custom `TestAuthHandler` that reads user claims from `X-Test-*` request headers
- `AddAuthorization()` with any custom policies the real API uses
- `AddCarter()` to register Carter modules from the `{ServiceName}.Api` assembly
- `services.AddSingleton(SenderMock.Object)` to inject the mock MediatR sender
- Exception middleware that maps `UnauthorizedAccessException` → `403` (because endpoints throw this exception directly as an authorization guard)

```csharp
// ApiTestFactory snippet
app.Use(async (context, next) =>
{
    try { await next(context); }
    catch (UnauthorizedAccessException)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();
```

**Injecting user context** — `TestAuthHandler` reads claims from request headers. `ApiTestFactory.CreateTestClient(params string[] groups)` sets all standard headers and appends a `X-Test-Groups` header for each group string passed in:

```csharp
// Admin client
var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

// Unauthenticated-ish client (no groups → auth guard throws)
var client = _factory.CreateTestClient(); // no groups
```

| Type | What to test |
|---|---|
| **Authorization guard** | Authorized role → `2xx`; unauthorized role → `4xx`; no claims → `4xx` |
| **ISender dispatch** | Correct command/query type sent; `It.Is<T>` matcher verifies all relevant properties |
| **Query parameter forwarding** | URL query params are correctly mapped into the query/command dispatched to `ISender` |
| **Route parameters** | `{userId}`, `{groupId}` are correctly extracted and passed into the command/query |
| **Carter module registration** | Covered implicitly — if Carter fails to register a module the route returns 404 |

**Example — authorization guard test:**

```csharp
[Fact]
public async Task ActivateUser_WhenSystemAdmin_Returns200()
{
    // Arrange
    _factory.SenderMock
        .Setup(s => s.Send(It.IsAny<ActivateUserCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    var client = _factory.CreateTestClient(AuthorizeConstants.SystemAdmin);

    // Act
    var response = await client.PutAsync("/users/user-123/activate", null);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    _factory.SenderMock.Verify(
        s => s.Send(It.Is<ActivateUserCommand>(cmd => cmd.UserId == "user-123"),
            It.IsAny<CancellationToken>()),
        Times.Once);
}

[Fact]
public async Task ActivateUser_WhenNotSystemAdmin_ReturnsError()
{
    // Arrange
    var client = _factory.CreateTestClient("app:user"); // wrong group

    // Act
    var response = await client.PutAsync("/users/user-123/activate", null);

    // Assert
    response.IsSuccessStatusCode.Should().BeFalse();
    _factory.SenderMock.Verify(
        s => s.Send(It.IsAny<ActivateUserCommand>(), It.IsAny<CancellationToken>()),
        Times.Never);
}
```

**`IClassFixture<ApiTestFactory>`** — each endpoint test class receives a single `ApiTestFactory` instance for the entire class. Call `factory.SenderMock.Reset()` in the constructor to clear state between test class runs when multiple test classes share the fixture.

---

### Infrastructure layer — `{ServiceName}.Infrastructure.Tests`

**Reference:** `{ServiceName}.Infrastructure.csproj`  
**External dependencies mocked:** `IKeycloakApi` (Refit), `IMinioClient`, `IDocumentSession` (Marten), `IConfiguration` (via in-memory builder)

**Important:** Build a real `IConfiguration` using `ConfigurationBuilder().AddInMemoryCollection(...)` rather than mocking it. Services typically call `GetRequiredSection(...)`, which is an extension method that cannot be mocked on a `Mock<IConfiguration>`.

```csharp
var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["ApiClients:Keycloak:Realm"] = "my-realm",
        ["ApiClients:Keycloak:ClientId"] = "svc-client",
        ["ApiClients:Keycloak:ClientSecret"] = "secret",
        ["ApiClients:Keycloak:GrantType"] = "client_credentials",
        ["ApiClients:Keycloak:Scopes:0"] = "openid",
    })
    .Build();
```

**Creating `Refit.ApiException` in tests:**

Refit's `ApiException` has a protected constructor. Use the async static factory:

```csharp
private static async Task<ApiException> CreateApiException(HttpStatusCode statusCode)
{
    var request = new HttpRequestMessage(HttpMethod.Get, "http://api.test");
    var response = new HttpResponseMessage(statusCode);
    return await ApiException.Create(request, HttpMethod.Get, response, new RefitSettings());
}
```

| Type | What to test |
|---|---|
| **Service classes** (e.g. `KeycloakService`) | Access token retrieval, success path, HTTP 404 → domain exception, HTTP 409 → conflict exception, generic failure → infrastructure exception |
| **Repository classes** | `Store` + `SaveChangesAsync` called on success; exception propagation |
| **Infrastructure Exceptions** | Constructor message, inheritance from `Exception` |
| **`DependencyInjection`, `ApiClientExtension`, `InitialData`** | Annotate with `[ExcludeFromCodeCoverage]` — pure wiring |

**Example — service error path:**

```csharp
[Fact]
public async Task GetUserByIdAsync_ShouldThrowInfrastructureException_WithUserNotFound_WhenApiReturns404()
{
    // Arrange
    const string userId = "missing-user";
    SetupGetAccessToken();
    var apiEx = await CreateApiException(HttpStatusCode.NotFound);
    _keycloakApiMock
        .Setup(x => x.GetUserByIdAsync(Realm, userId, AccessToken))
        .ThrowsAsync(apiEx);

    // Act
    var act = () => _sut.GetUserByIdAsync(userId, CancellationToken.None);

    // Assert
    var ex = await act.Should().ThrowAsync<InfrastructureException>();
    ex.Which.Message.Should().Be(MessageCode.UserNotFound);
}
```

---

## Mocking Guidelines

- Mock **interfaces**, never concrete classes
- Only configure setups that the test actually exercises
- Use `Times.Once` / `Times.Never` in `Verify` calls to confirm interaction counts
- Prefer `Returns(Task.CompletedTask)` over `.ReturnsAsync(default)` for `Task`-returning methods with no return value

```csharp
// Good — specific matcher, explicit verification
_keycloakService
    .Setup(s => s.ActivateUserAsync(userId, CancellationToken))
    .Returns(Task.CompletedTask);
_keycloakService.Verify(s => s.ActivateUserAsync(userId, CancellationToken), Times.Once);

// Bad — overly broad matcher hides bugs
_keycloakService.Verify(s => s.ActivateUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
```

---

## Test Data Helpers

Static factory classes in `Common/TestData/` create consistent, meaningful test fixtures. Prefer these over inline object initializers in test methods.

Each factory method has **sensible defaults** and accepts **named parameters** to override only what the test cares about:

```csharp
// Use defaults
var user = UserTestData.CreateUserDto();

// Override only the relevant field
var request = UserTestData.CreateUserDtoRequest(email: "invalid-email");
```

**Adding new factory methods:**

- Keep factory methods `static`
- Give them descriptive names: `CreateXxx`, `UpdateXxx`, `XxxWithNoGroups`, etc.
- Never embed test-specific assertions inside factory methods

**When `TestData` helpers are not needed:**

For Domain layer tests, entities are typically constructed via their own static factory (`OutboxMessageEntity.Create(...)`) or constructor — no additional test data helper is required.

---

## How to Run Tests

All commands are run from the **repository root** (`scilab-microservices/`).

### Run a single test project

```bash
# Domain tests
dotnet test src/Services/User/Tests/User.Domain.Tests/User.Domain.Tests.csproj

# Application tests
dotnet test src/Services/User/Tests/User.Application.Tests/User.Application.Tests.csproj

# Infrastructure tests
dotnet test src/Services/User/Tests/User.Infrastructure.Tests/User.Infrastructure.Tests.csproj

# API tests
dotnet test src/Services/User/Tests/User.Api.Tests/User.Api.Tests.csproj
```

### Run all tests for a service (filter by namespace)

```bash
dotnet test scilab-microservices.sln --filter "FullyQualifiedName~User"
```

### Run all tests in the solution

```bash
dotnet test scilab-microservices.sln
```

### Filter to a specific class or method

```bash
# All tests in a class
dotnet test ... --filter "FullyQualifiedName~KeycloakServiceTests"

# A single test method
dotnet test ... --filter "FullyQualifiedName=User.Infrastructure.Tests.Services.KeycloakServiceTests.GetUserByIdAsync_ShouldReturnMappedUser"
```

### Collect code coverage

```bash
dotnet test \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage-results
```

The coverage report is written to `./coverage-results/<guid>/coverage.cobertura.xml`.

### Generate HTML coverage report

Requires the `reportgenerator` global tool (install once):

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Generate and open:

```bash
reportgenerator \
  -reports:"coverage-results/**/coverage.cobertura.xml" \
  -targetdir:"coverage-results/html" \
  -reporttypes:Html \
  -assemblyfilters:"+User.*,+Management.*,+Lab.Management,+Lab.Infrastructure,+Lab.Domain"

open coverage-results/html/index.html
```

---

## Success Criteria

A test run is considered **fully successful** when all six gates pass.

### Gate 1 — All tests pass

```
Passed!  - Failed: 0, Passed: N, Skipped: 0
```

- Zero failures
- Zero skips — a skipped test requires a documented reason (link to issue) in the `Skip` message

### Gate 2 — Line coverage: 100% on Application layer

Every executable line in the `{ServiceName}.Application` assembly is hit by at least one test.

```
Package {ServiceName}.Application: 100.0%
```

### Gate 3 — Line coverage: 100% on Domain layer

Every executable line in the `{ServiceName}.Domain` assembly is hit by at least one test.

```
Package {ServiceName}.Domain: 100.0%
```

> **Note on Domain coverage:** Abstract base classes, interfaces, and pure property-bag entities are annotated with `[ExcludeFromCodeCoverage]` — they are intentionally excluded. Only entities with real behaviour (state transitions, computations) require tests.

### Gate 4 — API tests: all endpoints covered

Every Carter endpoint module must have at least:
- One test for the **authorized happy path** (correct group → `2xx`)
- One test for the **unauthorized path** (wrong/missing group → `4xx`)
- One test verifying the **correct command or query is dispatched** with expected properties

### Gate 5 — Branch coverage: 100% on all covered assemblies

Both sides of every `if`, `??`, `switch`, and ternary expression must be exercised.

| Metric | Target | Applies to |
|---|---|---|
| Line coverage | **100%** | Application + Domain |
| Branch coverage | **100%** | Application + Domain + Infrastructure |

### Gate 6 — No mocked business logic

Only mock **infrastructure boundaries** (external services, repositories, HTTP clients). Never mock the handler, validator, entity, or aggregate under test.

---

## Common Pitfalls

### `System.ApplicationException` name collision

The `System` namespace exports `ApplicationException`. If your service has its own `ApplicationException` in the Application layer, import it with a `using` alias to avoid compiler ambiguity:

```csharp
using AppException = {ServiceName}.Application.Exceptions.ApplicationException;
```

### Mocking `IConfiguration.GetRequiredSection`

`GetRequiredSection` is an extension method and cannot be mocked via `Mock<IConfiguration>`. Use a real in-memory configuration instead:

```csharp
var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?> { ... })
    .Build();
```

### Creating `Refit.ApiException` for error-path tests

Do not try to `new` up `ApiException` directly — the constructor is `protected`. Use the static factory:

```csharp
var ex = await ApiException.Create(
    new HttpRequestMessage(HttpMethod.Get, "http://api.test"),
    HttpMethod.Get,
    new HttpResponseMessage(HttpStatusCode.NotFound),
    new RefitSettings());
```

### Validator tests require direct instantiation

FluentValidation validators registered via `AddValidatorsFromAssembly` are not exercised by handler tests. Create dedicated validator test classes that instantiate the validator directly and call `ValidateAsync`.

### Unused mock setups

Moq in `Strict` mode will fail if a setup is not exercised. In `Loose` mode (the default), unused setups silently succeed. Always review setups to ensure they correspond to the code path under test; otherwise they provide false confidence.

### Domain test dependencies

The Domain test project should reference only `{ServiceName}.Domain.csproj`. If your test needs types from Application or Infrastructure, the test belongs in a different test project.

### `TestServer` propagates unhandled exceptions

`Microsoft.AspNetCore.TestHost` re-throws unhandled exceptions from the request pipeline onto the client thread. Endpoints that throw `UnauthorizedAccessException` as an authorization guard will cause `client.SendAsync` to throw rather than returning an error-status response.

Fix: register exception-mapping middleware **before** `UseAuthentication` in `ApiTestFactory`:

```csharp
app.Use(async (context, next) =>
{
    try { await next(context); }
    catch (UnauthorizedAccessException)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
});
```

### Carter module discovery in API tests

`AddCarter()` (no arguments) scans all loaded assemblies. If the `{ServiceName}.Api` assembly has not been loaded yet, Carter will not discover its modules and every route returns `404`. Force the assembly to load in `CreateHost`:

```csharp
_ = typeof(SomeEndpointType).Assembly;
```

### `DELETE` with a body

Some endpoints accept a request body on a `DELETE` verb (e.g. `RemoveRolesFromGroup`). `HttpClient.DeleteAsync` does not accept a content argument. Use `SendAsync` instead:

```csharp
var request = new HttpRequestMessage(HttpMethod.Delete, "/groups/g1/roles")
{
    Content = new StringContent(JsonSerializer.Serialize(roleNames), Encoding.UTF8, "application/json")
};
var response = await client.SendAsync(request);
```

### `WebApplicationFactory` generic type parameter

When bypassing `Program.cs`, use the factory class itself as the type parameter (`WebApplicationFactory<ApiTestFactory>`) rather than a class from the production project. This avoids loading the full production DI configuration.
