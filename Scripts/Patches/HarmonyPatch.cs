using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib.Patching.Models;

namespace RabbitAndSteel.Scripts.Patches
{
    public class MonsterModel_VisualsPatch : IPatchMethod
    {
        public static string PatchId => "monster_model_visuals_patch";
        public static string Description => "monster model visuals patch for modded monsters";
        public static bool IsCritical => true;
        public static ModPatchTarget[] GetTargets() =>
            [new(typeof(MonsterModel), nameof(MonsterModel.CreateVisuals))];
        public static bool Prefix(MonsterModel __instance, ref NCreatureVisuals __result)
        {
            if (__instance is ZangHuaMonster modMonster)
            {
                var customVisuals = modMonster.CreateCustomVisuals();
                if (customVisuals != null)
                {
                    __result = customVisuals;
                    return false;
                }
            }
            return true;
        }
    }
}