using System;
using System.Reflection;
using IntelOrca.Biohazard.BioRand.REE;

namespace IntelOrca.Biohazard.BioRand.RE4R
{
    public class Re4rRandomizerApp
    {
        public int Run(string[] args)
        {
            if (args.Length > 0 &&
                args[0].Equals("strip-arena-extras", StringComparison.OrdinalIgnoreCase))
            {
                return StripArenaExtrasCommand.Run(args[1..]);
            }

            var randomizerApp = new ReeRandomizerApp();
            randomizerApp.ApplicationName = "biorand-re4r";
            randomizerApp.ApplicationVersion = VersionHelper.GetGitHashShort(Assembly.GetExecutingAssembly());
            randomizerApp.ExamplePakFilename = "re_chunk_000.pak.patch_007.pak";
            randomizerApp.ExampleInstallPath = @"C:\Program Files (x86)\Steam\steamapps\common\RESIDENT EVIL 4  BIOHAZARD RE4";
            randomizerApp.Randomizer = new Re4rRandomizer();
            return randomizerApp.Run(args);
        }
    }
}
