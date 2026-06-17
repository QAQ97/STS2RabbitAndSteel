// using Godot;
// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Context;
// using MegaCrit.Sts2.Core.Entities.Players;
// using RabbitAndSteel.Scripts.Relics;
// using MegaCrit.Sts2.Core.Combat;
// using MegaCrit.Sts2.Core.Context;
// using MegaCrit.Sts2.Core.Entities.Players;

// namespace RabbitAndSteel.Scripts;

// public class LevitationRingInputController : Node
// {
//     private const Key ActivateKey = Key.Space;

//     public override void _Ready()
//     {
//         ProcessMode = ProcessModeEnum.Always;
//         SetProcessInput(true);
//         GD.Print("[RabbitAndSteel] LevitationRingInputController ready.");
//     }

//     public override void _Input(InputEvent @event)
//     {
//         if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo && keyEvent.Keycode == ActivateKey)
//         {
//             GD.Print("[RabbitAndSteel] Activate key pressed.");
//             var player = GetLocalCombatPlayer();
//             if (player == null) return;

//             var ring = player.Relics.OfType<LevitationRing>().FirstOrDefault();
//             if (ring != null)
//             {
//                 ring.Activate();
//                 GD.Print("[RabbitAndSteel] LevitationRing invincibility activated.");
//             }
//             else
//             {
//                 GD.Print("[RabbitAndSteel] No LevitationRing found on player.");
//             }
//         }
//     }

//     private static Player? GetLocalCombatPlayer()
//     {
//         var combatState = CombatManager.Instance.DebugOnlyGetState();
//         if (combatState == null) return null;

//         return combatState.Players.FirstOrDefault(p =>
//             p?.Creature != null && LocalContext.IsMe(p.Creature));
//     }
// }