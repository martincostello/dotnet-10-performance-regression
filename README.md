# .NET 10 Performance Regression Repro

A benchmark demonstrating a performance regression with .NET 10

To run the benchmark, run the following commands from the root of the repository:

```pwsh
$env:PUZZLE_INPUT = Resolve-Path (Join-Path "." "input.txt")
dotnet run --configuration Release --framework net8.0 --runtimes net8.0 net9.0 net10.0
```

### Results

```text
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
13th Gen Intel Core i7-13700H 2.90GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.100-preview.6.25358.103
  [Host]     : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2
  Job-HFWDAV : .NET 10.0.0 (10.0.25.35903), X64 RyuJIT AVX2
  Job-XMDLXS : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2
  Job-NFJGQR : .NET 9.0.7 (9.0.725.31616), X64 RyuJIT AVX2
```

| Method | Runtime   | Mean     | Error     | StdDev    | Ratio | Gen0    | Gen1    | Allocated | Alloc Ratio |
|------- |---------- |---------:|----------:|----------:|------:|--------:|--------:|----------:|------------:|
| Solve  | .NET 8.0  | 2.918 ms | 0.0351 ms | 0.0293 ms |  1.00 | 46.8750 | 11.7188 | 600.94 KB |        1.00 |
| Solve  | .NET 9.0  | 1.139 ms | 0.0127 ms | 0.0113 ms |  0.39 | 44.9219 | 11.7188 | 562.14 KB |        0.94 |
| Solve  | .NET 10.0 | 1.775 ms | 0.0065 ms | 0.0058 ms |  0.61 | 44.9219 | 11.7188 | 562.14 KB |        0.94 |
