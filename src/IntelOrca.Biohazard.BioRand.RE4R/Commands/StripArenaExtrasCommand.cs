using System;
using System.IO;

namespace IntelOrca.Biohazard.BioRand.RE4R
{
    internal static class StripArenaExtrasCommand
    {
        public static int Run(string[] args)
        {
            string? inputPath = null;
            string? outputPath = null;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if ((arg == "-i" || arg == "--input") && i + 1 < args.Length)
                {
                    inputPath = args[++i];
                }
                else if ((arg == "-o" || arg == "--output") && i + 1 < args.Length)
                {
                    outputPath = args[++i];
                }
                else if (arg == "-h" || arg == "--help")
                {
                    PrintUsage();
                    return 0;
                }
                else
                {
                    Console.Error.WriteLine($"Unknown or incomplete option: {arg}");
                    PrintUsage();
                    return 1;
                }
            }

            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
            {
                Console.Error.WriteLine("Both input and output PAK paths are required.");
                PrintUsage();
                return 1;
            }

            inputPath = Path.GetFullPath(inputPath);
            outputPath = Path.GetFullPath(outputPath);

            if (!File.Exists(inputPath))
            {
                Console.Error.WriteLine($"Input PAK not found: {inputPath}");
                return 1;
            }

            if (!inputPath.EndsWith(".pak", StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("Input must be a .pak file.");
                return 1;
            }

            if (!outputPath.EndsWith(".pak", StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("Output must be a .pak file.");
                return 1;
            }

            if (string.Equals(inputPath, outputPath, StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("Output must be a different file from the input PAK.");
                return 1;
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            try
            {
                var modBuilder = ExportedMods.ExportMod(
                    inputPath,
                    "Knife Arena Cleanup",
                    gameVersion: 6);

                modBuilder.SavePakFile(outputPath);
                Console.WriteLine($"Knife Arena cleanup PAK written to: {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to create Knife Arena cleanup PAK.");
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
                "Usage: biorand-re4r strip-arena-extras -i <generated.pak> -o <override.pak>");
        }
    }
}
