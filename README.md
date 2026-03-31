# HugeTextFileSorter

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![C# 14](https://img.shields.io/badge/C%23-14.0-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![NUnit](https://img.shields.io/badge/tests-NUnit%204-green)](https://nunit.org/)
[![BenchmarkDotNet](https://img.shields.io/badge/benchmarks-BenchmarkDotNet-blue)](https://benchmarkdotnet.org/)

A **.NET 10** solution for generating, sorting, and merging large alphanumeric text files. Designed for scenarios where files are too large to fit entirely in memory, it employs **external sorting** — splitting files into sorted chunks and merging them via a priority-queue-based k-way merge — to handle multi-gigabyte datasets efficiently.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Core Library — MultiSorterLib](#core-library--multisorterlib)
  - [AlphanumericEntity](#alphanumericentity)
  - [AlphanumericSorterHelper](#alphanumericsorterhelper)
  - [ExternalSorter](#externalsorter)
  - [Merger](#merger)
  - [FileSizeExtensions](#filesizeextensions)
  - [SystemMemoryHelper](#systemmemoryhelper)
- [File Format](#file-format)
- [Sorting Algorithm](#sorting-algorithm)
- [Applications](#applications)
  - [AlphanumericSorterConsole](#alphanumericsorterconsole)
  - [TestFileGenerator](#testfilegenerator)
  - [AlphanumericSorterBenchmark](#alphanumericsorterbenchmark)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build](#build)
  - [Run the Console Sorter](#run-the-console-sorter)
  - [Run the Test File Generator](#run-the-test-file-generator)
- [Testing](#testing)
  - [Test Coverage by Project](#test-coverage-by-project)
  - [Test Categories](#test-categories)
  - [Test Coverage Percentage](#test-coverage-percentage)
- [Benchmarks](#benchmarks)
  - [Environment](#environment)
  - [Results (100 MB test file)](#results-100-mb-test-file)
  - [Key Takeaways](#key-takeaways)
- [License](#license)

## Overview

Working with large text files (hundreds of MB to multiple GB) presents a fundamental challenge: the data cannot be loaded into memory all at once. **HugeTextFileSorter** solves this by implementing an external sort-merge strategy:

1. **Split** — the input file is divided into memory-manageable chunks (default 256 MB)
2. **Sort** — each chunk is parsed into alphanumeric entities and sorted in-memory
3. **Merge** — sorted chunks are merged into a single output file using a min-heap (`PriorityQueue`)

The solution also includes a **Windows Forms test file generator** for producing randomized input files with configurable size, word count, duplicate density, and other parameters.

## Features

- **External sorting** for files that exceed available RAM
- **k-way merge** using `PriorityQueue<T>` for efficient multi-file merging
- **Alphanumeric entity model** with ordinal string comparison, then numeric ordering
- **Configurable test file generation** with duplicate string density control
- **File size formatting utilities** with binary metric prefixes (KB, MB, GB, TB, …)
- **System memory detection** via P/Invoke (`GlobalMemoryStatusEx`) with non-Windows fallback
- **94 unit tests** including stress tests with NUnit 4 and Moq
- **BenchmarkDotNet integration** for performance measurement
- **Full XML documentation** for IntelliSense support

## Project Structure

```
HugeTextFileSorter/
├── MultiSorterLib/                      # Core sorting library
│   ├── AlphanumericEntity.cs            # Entity model (IComparable, IEquatable)
│   ├── AlphanumericSorterHelper.cs      # In-memory file sort orchestrator
│   ├── ExternalSorter.cs                # Chunk-based external sorting
│   ├── Merger.cs                        # Priority-queue k-way merge
│   ├── FileSizeExtensions.cs            # File size formatting & conversion
│   └── SystemMemoryHelper.cs            # Available memory detection (P/Invoke)
├── AlphanumericSorterConsole/           # Console application for sorting files
│   └── Program.cs
├── TestFileGenerator/                   # Windows Forms test file generator
│   ├── frmMain.cs                       # Main form with generation UI
│   └── RandomFileGenerator.cs           # Random line/file generation logic
├── AlphanumericSorterBenchmark/         # BenchmarkDotNet performance suite
│   ├── BenchmarkSorter.cs
│   └── Program.cs
├── MultiSorterLib.Tests/                # Unit & stress tests for core library
│   ├── AlphanumericEntityTests.cs       # 34 tests — entity model
│   ├── AlphanumericSorterHelperTests.cs # 14 tests — sorter helper
│   ├── ExternalSorterAndMergerTests.cs  # 8 tests — external sort + merge
│   ├── FileSizeExtensionsTests.cs       # 10 tests — formatting & conversion
│   ├── SystemMemoryHelperTests.cs       # 3 tests — memory detection
│   ├── StressTests.cs                   # 12 tests — large-scale & concurrent
│   └── TestSetup.cs                     # Assembly-level test configuration
└── TestFileGenerator.Tests/             # Unit tests for file generator
    ├── RandomFileGeneratorTests.cs      # 8 tests — generation logic
    └── TestSetup.cs
```

## Core Library — MultiSorterLib

### AlphanumericEntity

Represents an immutable entity composed of a string part and a numeric part. Implements `IComparable<AlphanumericEntity>` and `IEquatable<AlphanumericEntity>`.

| Member | Type | Description |
|---|---|---|
| `StringPart` | `string` | The text component of the entity |
| `NumericPart` | `int` | The numeric component of the entity |
| `EntityLine` | `string` | Formatted as `"{NumericPart}. {StringPart}"` |
| `Delimiter` | `const char` | `'.'` — separator between numeric and string parts |
| `CompareTo()` | `int` | Ordinal string comparison first, then numeric |
| `Equals()` | `bool` | True if both parts match |
| `GetHashCode()` | `int` | Based on `HashCode.Combine(StringPart, NumericPart)` |

**Sort order:** StringPart (ordinal) → NumericPart (ascending)

```csharp
// "1. Alpha" < "3. Alpha" < "2. Beta" < "10. Zeta"
//  (Alpha,1)   (Alpha,3)    (Beta,2)    (Zeta,10)
```

### AlphanumericSorterHelper

Orchestrates in-memory sorting of a single file. Reads input, parses entities, sorts, and writes the output with `_Sorted` appended to the filename.

```csharp
using var helper = new AlphanumericSorterHelper("input.txt");
helper.Sort();
helper.SaveOutputFile(); // Writes "input_Sorted.txt"
```

### ExternalSorter

Splits large input files into sorted temporary chunks (default 256 MB each). Each chunk is independently parsed and sorted in memory.

```csharp
List<string> tempFiles = ExternalSorter.CreateSortedChunks("huge_file.txt");
// Returns paths to sorted temp files
```

### Merger

Merges multiple pre-sorted files into a single sorted output using a `PriorityQueue`-based k-way merge. Blank lines are automatically skipped.

```csharp
Merger.MergeSortedFiles(tempFiles, "sorted_output.txt");
```

### FileSizeExtensions

Extension methods for formatting and converting file sizes using binary metric prefixes.

| Method | Description |
|---|---|
| `FormatFileSize()` | Formats bytes as human-readable string (e.g., `"1.5 GB"`) |
| `GetFileSizeInBytes()` | Converts a value + prefix to bytes (e.g., `2 MB → 2,097,152`) |
| `GetFileSizeInUnits()` | Converts bytes to a metric unit (e.g., `1,048,576 B → 1.0 MB`) |

```csharp
double size = 2.5;
string formatted = size.FormatFileSize(metricBenchmark: MetricPrefixes.Giga);
// "2.5 GB"

ulong bytes = 2.0.GetFileSizeInBytes(MetricPrefixes.Mega);
// 2,097,152
```

### SystemMemoryHelper

Retrieves available physical memory. Uses `GlobalMemoryStatusEx` via P/Invoke on Windows; falls back to `GC.GetGCMemoryInfo().TotalAvailableMemoryBytes` on other platforms.

```csharp
ulong available = SystemMemoryHelper.GetAvailablePhysicalMemoryBytes();
```

## File Format

Each line in an input/output file follows the pattern:

```
{number}. {string}
```

**Examples:**

```
42. Alpha Beta
1. Gamma
100. Delta Epsilon
```

Lines that do not match this pattern (missing delimiter, non-numeric prefix, multiple delimiters) are silently ignored during parsing.

## Sorting Algorithm

### In-Memory Sort (small files)

```
Input File → Parse Entities → Sort (StringPart ordinal, then NumericPart) → Write Output
```

### External Sort (large files)

```
Input File → Split into 256 MB chunks
           → Sort each chunk in-memory
           → Write sorted chunks to temp files
           → k-way merge via PriorityQueue
           → Write final sorted output
```

The k-way merge reads one line at a time from each sorted chunk, maintains a min-heap keyed by line content, and writes the smallest element to the output — ensuring O(N log k) merge complexity where N is total lines and k is chunk count.

## Applications

### AlphanumericSorterConsole

Interactive console application. Prompts for a file path, sorts it using `AlphanumericSorterHelper`, and reports elapsed time.

```
Please select the TEXT file to sort or press [ESC] to Exit the app.
Full file path: C:\data\huge_file.txt
Starting file Loading and Sorting...
The file was successfully Sorted and Saved to 'C:\data\huge_file_Sorted.txt'
OVERALL TIME: 00:00:12.3456789
```

### TestFileGenerator

Windows Forms application for generating test files with configurable parameters:

- **File size** (in MB)
- **Max number** — upper bound for the numeric part
- **Max words count** — maximum words per string part
- **Max word length** — maximum characters per word
- **Duplicate string density** — percentage (0–100%) of lines sharing duplicate string parts
- **Cancellation support** — cancel long-running generation at any time

### AlphanumericSorterBenchmark

BenchmarkDotNet suite measuring performance across the full pipeline:

| Benchmark | Description |
|---|---|
| `GenerateTestFile` | Generates a 100 MB test file |
| `InitializeSorter` | Creates `AlphanumericSorterHelper` and parses input |
| `Sort` | In-memory sort of all parsed entities |
| `SaveOutputFile` | Writes sorted output to disk |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows 10 (version 1809+) — required for Windows Forms projects and P/Invoke memory detection

### Build

```shell
dotnet build HugeTextFileSorter.sln
```

### Run the Console Sorter

```shell
dotnet run --project AlphanumericSorterConsole
```

### Run the Test File Generator

```shell
dotnet run --project TestFileGenerator
```

## Testing

The solution includes **94 unit tests** written with **NUnit 4**, covering all core library classes, the test file generator, edge cases, and large-scale stress tests.

```shell
dotnet test
```

### Test Coverage by Project

| Test Class | Tests | Target |
|---|---|---|
| `AlphanumericEntityTests` | 34 | Entity model: comparison, equality, hashing, formatting |
| `AlphanumericSorterHelperTests` | 14 | Sorter: constructor, sort, save, parse, split |
| `StressTests` | 12 | Large-scale: 100K sorts, concurrent instances, end-to-end |
| `FileSizeExtensionsTests` | 10 | Formatting, conversion, round-trips |
| `ExternalSorterAndMergerTests` | 8 | External sort chunks, k-way merge |
| `RandomFileGeneratorTests` | 8 | File generation, line formatting, cancellation |
| `SystemMemoryHelperTests` | 3 | Memory detection (Windows/non-Windows) |
| **MultiSorterLib.Tests subtotal** | **81** | |
| **TestFileGenerator.Tests subtotal** | **8** | |
| | | |
| **Total** | **94 tests** | |

### Test Categories

| Category | Description | Examples |
|---|---|---|
| **Unit** | Individual method/class behavior | Entity comparison, GetSplit parsing, format conversions |
| **Edge Cases** | Boundary conditions and error handling | Empty files, invalid lines, missing files, dispose idempotency |
| **Stress** | Large-scale correctness and performance | 100K entity sorts, 50K-line file sort, concurrent sorter instances |
| **Integration** | End-to-end pipeline verification | External sort → merge → verify sorted output |

### Test Coverage Percentage

Coverage collected with [coverlet](https://github.com/coverlet-coverage/coverlet) via `dotnet test --collect:"XPlat Code Coverage"`.

#### MultiSorterLib (core library) — 94.4% line / 81.9% branch overall

| Class | Line Coverage | Branch Coverage | Complexity |
|---|---|---|---|
| `AlphanumericEntity` | **100%** | **100%** | 13 |
| `AlphanumericSorterHelper` | 80.8% | 57.1% | 28 |
| ↳ `EnumerateEntities` (iterator) | **100%** | **100%** | 9 |
| ↳ `GetSplit` (iterator) | **100%** | **100%** | 8 |
| `ExternalSorter` | **100%** | 91.7% | 12 |
| `FileSizeExtensions` | **100%** | **100%** | 10 |
| `Merger` | **100%** | 88.9% | 18 |
| `SystemMemoryHelper` | 80.0% | 50.0% | 4 |
| | | |
| **Overall** | **94.4%** (203/215 lines) | **81.9%** (77/94 branches) | |

#### TestFileGenerator — RandomFileGenerator

| Class | Line Coverage | Branch Coverage |
|---|---|---|
| `RandomFileGenerator` | 77.6% | 73.5% |

> **Note:** `frmMain` (Windows Forms UI) and `Program` classes are excluded from meaningful coverage metrics as they contain platform-specific UI code not suitable for automated unit testing.

## Benchmarks

Run benchmarks with:

```shell
dotnet run --project AlphanumericSorterBenchmark -c Release
```

The benchmark suite uses **BenchmarkDotNet** with `MemoryDiagnoser` to measure allocation and throughput across the full generate → load → sort → save pipeline on a 100 MB test file.

### Environment

```
BenchmarkDotNet v0.13.5, Windows 11 (10.0.26200.7623)
13th Gen Intel Core i9-13900H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.2 (10.0.225.61305), X64 RyuJIT AVX2
```

### Results (100 MB test file)

| Method | Mean | Error | StdDev | Gen0 | Gen1 | Gen2 | Allocated |
|---|---:|---:|---:|---:|---:|---:|---:|
| `GenerateTestFile` | 6,303.5 ms | 123.0 ms | 151.1 ms | 8,537,000 | 966,000 | 1,000 | ~100.0 GB |
| `InitializeSorter` | 0.042 ms | 0.001 ms | 0.001 ms | 0.24 | 0.18 | — | 3.58 KB |
| `Sort` | 63.4 ms | 1.2 ms | 1.9 ms | — | — | — | 775.35 KB |
| `SaveToFile` | 147.1 ms | 2.7 ms | 4.1 ms | 16,667 | 333 | — | 201.33 MB |

### Key Takeaways

- **File generation** dominates total time — ~6.3 seconds for 100 MB, accounting for ~97% of the full pipeline duration due to heavy string allocation (~100 GB GC pressure across generations)
- **Initialization** is near-instant at **42 μs** — file handle setup and lazy entity enumeration create minimal overhead (3.58 KB)
- **Sorting** completes in **63 ms** for the full 100 MB parsed entity set, allocating only 775 KB (the sorted array copy)
- **Saving** takes **147 ms** with ~201 MB allocated for buffered file I/O writes
- **Sort + Save** together take **~210 ms** — the actual sort-and-persist pipeline is extremely fast relative to file generation

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Author

**TigoS** — [GitHub](https://github.com/TigoS)
