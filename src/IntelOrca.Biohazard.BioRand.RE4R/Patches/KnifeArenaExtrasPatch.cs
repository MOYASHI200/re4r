using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using IntelOrca.Biohazard.BioRand.Extensions;
using IntelOrca.Biohazard.BioRand.REE;
using IntelOrca.Biohazard.BioRand.REE.Extensions;
using IntelOrca.Biohazard.REE.Rsz;

namespace IntelOrca.Biohazard.BioRand.RE4R.Patches
{
    [ExportMod(
        FileName = "knife-arena-cleanup",
        Name = "Knife Arena Cleanup",
        Description = "Removes BioRand's extra enemies from the Chapter 11 Knife Arena without rerolling the seed.",
        Version = "1.0",
        Author = "MOYASHI200")]
    internal class KnifeArenaExtrasPatch(IReeRandomizerContext context) : IPatch
    {
        private const string ScenePath =
            "natives/stm/_chainsaw/leveldesign/chapter/cp10_chp4_2/level_cp10_chp4_2.scn.20";

        private static readonly HashSet<Guid> s_knownGuids =
        [
            "Enemy_1214".GetGuidHash(),
            "Enemy_1215".GetGuidHash(),
            "Enemy_1216".GetGuidHash(),
            "Enemy_1217".GetGuidHash()
        ];

        private static readonly Vector3[] s_positions =
        [
            new(178, 0, 48),
            new(183, 0, 47),
            new(179, 0, 46),
            new(185, 0, 50)
        ];

        public void Apply()
        {
            context.ModifyScnFile(ScenePath, scene =>
            {
                var guidsToRemove = new List<Guid>();

                scene.VisitGameObjects(gameObject =>
                {
                    if (s_knownGuids.Contains(gameObject.Guid) || MatchesKnifeArenaSignature(gameObject))
                    {
                        guidsToRemove.Add(gameObject.Guid);
                    }
                });

                foreach (var guid in guidsToRemove.Distinct())
                {
                    scene = scene.RemoveGameObject(guid);
                }

                return scene;
            });
        }

        private static bool MatchesKnifeArenaSignature(RszGameObject gameObject)
        {
            var spawnParam = gameObject.Components
                .FirstOrDefault(x => x.Type.FindFieldIndex("_StageID") != -1);

            if (spawnParam == null || spawnParam.Get<int>("_StageID") != 55302)
                return false;

            var transform = gameObject.FindComponent("via.Transform");
            if (transform == null)
                return false;

            var position = transform.Get<Vector3>("Position");
            return s_positions.Any(target => Vector3.DistanceSquared(position, target) < 0.01f);
        }
    }
}
