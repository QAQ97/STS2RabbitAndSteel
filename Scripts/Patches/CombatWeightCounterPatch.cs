using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using RabbitAndSteel.Scripts.Relics;
using STS2RitsuLib.Patching.Models;

namespace RabbitAndSteel.Scripts.Patches
{
    public class CombatWeightCounterPatch : IPatchMethod
    {
        public static string PatchId => "combat_weight_counter_patch";
        public static string Description => "增加EternalPartner遗物计数基于战斗权重";
        public static bool IsCritical => true;

        public static ModPatchTarget[] GetTargets() =>
            [new(typeof(Hook), nameof(Hook.AfterCombatVictory))];

        public static void Postfix(CombatRoom room)
        {
            if (room?.CombatState?.Players == null) return;

            foreach (var player in room.CombatState.Players)
            {
                var relic = player.Relics?.OfType<EternalPartner>().FirstOrDefault();
                if (relic == null) continue;

                int stacksToAdd = room.RoomType switch
                {
                    RoomType.Monster => 1,
                    RoomType.Elite  => 3,
                    RoomType.Boss   => 10,
                    _ => 0
                };

                if (stacksToAdd > 0)
                {
                    relic.AddStacks(stacksToAdd);
                }
            }
        }
    }
}