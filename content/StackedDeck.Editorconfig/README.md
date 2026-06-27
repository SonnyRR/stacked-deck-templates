# Stacked Deck Editorconfig Item Template

Adds a comprehensive `.editorconfig` file with C# coding conventions, formatting
rules, and naming styles for consistent code across your project.

Identifier: `sd-editorconfig`

```sh
dotnet new sd-editorconfig
```

## 🧩 Features

> [!NOTE]
> Below are mentioned some of the key features of this template. The
> `.editorconfig` document is based on the same conventions used across
> all Stacked Deck templates.

### 📐 Coding Conventions

The generated `.editorconfig` enforces a consistent set of .NET and C# coding
conventions including:

- **Organize usings** - System directives first, separated groups
- **Qualification preferences** - Configure when `this.` qualifiers are required
- **Predefined types** - Enforce `int` over `Int32`, `string` over `String`, etc.
- **Parentheses clarity** - Explicit parentheses in binary and arithmetic operators
- **Expression-level preferences** - Null propagation, coalescing, collection/object
  initializers, compound assignment, and more
- **Modifier preferences** - Accessibility modifier requirements, readonly fields

### 🎨 Naming Styles

Built-in naming rules for consistent identifier naming across the codebase:

| Rule | Style | Severity |
| ---- | ----- | -------- |
| Interfaces | `I` prefix + PascalCase | Warning |
| Types (classes, structs, enums) | PascalCase | Error |
| Non-field members (methods, properties, events) | PascalCase | Error |
| Constants | UPPER_CASE | Error |
| Fields | camelCase | Error |
| Readonly static fields | PascalCase | Error |

### ✨ C# Style Preferences

Modern C# language features are encouraged with sensible defaults:

- File-scoped namespaces
- Pattern matching over type checks and casts
- Switch expressions
- Null checks over reference equality
- Collection expressions
- Target-typed `new` expressions
- Discard variables for unused values

### 🔧 Formatting Rules

Consistent code formatting enforced at build time:

- **Braces** - Allman style (new line before open brace)
- **Indentation** - 4 spaces (2 spaces for `json`, `xml`, `yml`, `yaml`,
  `csproj`, `props`, and `slnx`)
- **Spacing** - Around binary operators, commas, control flow keywords, cast
  expressions, and more
- **Wrapping** - Preserve single-line blocks and statements

### ⚠️ Diagnostic Severities

Pre-configured severity overrides for Roslyn and third-party analyzers:

- `CA1309` / `CA1311` - Ordinal string comparison (suggestion)
- `IDE1006` - Naming rule violations (error)
- `S1144` - Unused private members (error)
- `RCS1213` - Unused member (error)
- `S3241` - SonarAnalyzer rule (silent, pending upstream fix)
