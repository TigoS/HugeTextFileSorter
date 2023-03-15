// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using AlphanumericSorterBenchmark;
using BenchmarkDotNet.Running;

Stopwatch sw = new();

do
{
    Console.WriteLine("Starting file Loading and Sorting... ");

    sw.Reset();
    sw.Start();

    try
    {
        BenchmarkRunner.Run<BenchmarkSorter>();
    }
    catch (Exception e)
    {
        Console.WriteLine("An exception was thrown during app execution.");
        Console.WriteLine($"Error details: {e}");
    }
    finally
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    sw.Stop();

    Console.WriteLine("The file was successfully Sorted and Saved!");
    Console.WriteLine($"OVERALL TIME: {sw.Elapsed:c}");
} while (Console.ReadKey().Key != ConsoleKey.Escape);