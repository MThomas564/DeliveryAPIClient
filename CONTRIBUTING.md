# Contributing

Contributions are welcome — bug reports, feature requests, and pull requests all help.

## Getting started

1. Fork the repository and clone your fork
2. Create a feature branch: `git checkout -b feature/my-change`
3. Make your changes and add tests where relevant
4. Run the test suite: `dotnet test`
5. Push your branch and open a pull request against `main`

## Running tests

```bash
dotnet test
```

## Building the package locally

```bash
dotnet pack src/DeliveryAPIClient/DeliveryAPIClient.csproj --configuration Release --output ./nupkg
```

## Code style

- Follow the existing patterns in the codebase
- Keep public API surface minimal and intentional
- Add XML doc comments to any new public members
- Prefer `CancellationToken` on all async methods

## Reporting issues

Please open a GitHub issue with a clear description of the problem and, if possible, a minimal reproduction case.
