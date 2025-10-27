# Agent Guidelines for FinancialManagement

## Build/Lint/Test Commands

### Frontend (React/TypeScript)
- **Build**: `cd Frontend && npm run build`
- **Dev server**: `cd Frontend && npm run dev`
- **Lint**: `cd Frontend && npm run lint`
- **Test**: No test framework configured yet

### Backend (.NET 8)
- **Build**: `cd FinancialManagement && dotnet build`
- **Run**: `cd FinancialManagement && dotnet run`
- **Test**: `cd FinancialManagement && dotnet test` (if test project exists)

## Code Style Guidelines

### C# (.NET)
- **Naming**: PascalCase for classes/methods/properties, camelCase for parameters, _camelCase for private fields
- **Imports**: Using statements at top, grouped by namespace
- **Async**: Use async/await pattern consistently
- **Error handling**: Return appropriate HTTP status codes, use IActionResult
- **Architecture**: Follow repository pattern, dependency injection

### TypeScript/React
- **Strict mode**: Enabled - no implicit any, strict null checks
- **Components**: Functional components with hooks, named exports
- **Imports**: External libraries first, then internal imports (relative paths)
- **Styling**: Material-UI components with sx prop for styling
- **Error handling**: Try/catch blocks, console.error for logging
- **Types**: Explicit interfaces for API responses, Partial<T> for forms
- **Naming**: camelCase for variables/functions, PascalCase for components/types

### General
- **Formatting**: Follow existing patterns in codebase
- **Comments**: Minimal comments, self-documenting code preferred
- **Security**: Never log sensitive data, use environment variables for secrets