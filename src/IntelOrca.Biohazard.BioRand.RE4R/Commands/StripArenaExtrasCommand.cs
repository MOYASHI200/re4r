using System;
using System.ComponentModel;
using System.IO;
using Spectre.Console.Cli;

namespace IntelOrca.Biohazard.BioRand.RE4R
{
    internal sealed class StripArenaExtrasCommand : Command<StripArenaExtrasCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("BioRand-generated patch PAK to clean up")]
            [CommandOption("-i|--input")]
            public string? InputPath { get; init; }

            [Description("Output PAK containing only the Knife Arena cleanup")]
            [CommandOption("-o|--output")]
            public string? OutputPath { get; init; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.InputPath))
            {
                Console.Error.WriteLine("Input PAK not specified.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(settings.OutputPath))
            {
                Console.Error.WriteLine("Output PAK not specified.");
                return 1;
            }

            var inputPath = Path.GetFullPath(settings.InputPath);
            var outputPath = Path.GetFullPath(settings.OutputPath);

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

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            var modBuilder = ExportedMods.ExportMod(inputPath, "Knife Arena Cleanup", gameVersion: 6);
            modBuilder.SavePakFile(outputPath);

            Console.WriteLine($"Knife Arena cleanup PAK written to: {outputPath}");
            return 0;
        }
    }
}
