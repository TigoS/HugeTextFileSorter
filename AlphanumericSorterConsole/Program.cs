// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using MultiSorterLib;

Stopwatch sw = new();

do
{
    Console.WriteLine("Please select the TEXT file to sort or press [ESC] to Exit the app.");
    Console.Write("Full file path: ");

    var fileName = Console.ReadLine();

    if (File.Exists(fileName))
    {
        // The following `Path.GetDirectoryName()` possible nullable warning is suppressed
        //  as it's checked to be a valid directory path just a line above
        string outputFileName = Path.Combine(Path.GetDirectoryName(fileName)!,
            Path.GetFileNameWithoutExtension(fileName) +
            "_Output" + Path.GetExtension(fileName));

        Console.WriteLine("Starting file Loading and Sorting... ");

        sw.Reset();
        sw.Start();
        
        try
        {
            using AlphanumericSorterHelper sorterHelper = new AlphanumericSorterHelper(fileName);
            sorterHelper.Sort();
            sorterHelper.SaveToFile(outputFileName);
        }
        catch (Exception e)
        {
            Console.WriteLine("An exception was thrown during app execution.");
            Console.WriteLine($"Error details: {e}");
        }

        sw.Stop();

        Console.WriteLine($"The file was successfully Sorted and Saved to '{outputFileName}'");
        Console.WriteLine($"OVERALL TIME: {sw.Elapsed:c}");
    }
    else
    {
        Console.WriteLine("Error: Unable to find the specified file!");
        Console.WriteLine("\tPlease check if the file exists and if the current user has Read permission.");
    }
} while (Console.ReadKey().Key != ConsoleKey.Escape);
