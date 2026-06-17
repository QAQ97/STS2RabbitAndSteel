// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Creatures;
// using MegaCrit.Sts2.Core.Entities.Relics;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Helpers;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.ValueProps;
// using RabbitAndSteel.Scripts.Characters;
// using STS2RitsuLib.Interop.AutoRegistration;
// using STS2RitsuLib.Scaffolding.Content;
// using System.Threading.Tasks;

// namespace RabbitAndSteel.Scripts.Relics;

// [RegisterRelic(typeof(AncientRelicPool))]
// [RegisterCharacterStarterRelic(typeof(AncientCharacter))]
// public class LevitationRing : ModRelicTemplate
// {
//     public override RelicRarity Rarity => RelicRarity.Starter;
//     private readonly float _durationSeconds = 1f;
//     private bool _isInvincible;

//     public override RelicAssetProfile AssetProfile => new(
//         IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
//         IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
//         BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
//     );

//     // 伤害修正：只有无敌期间且伤害来源有效时才免疫
//     public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
//     {
//         // 只保护遗物持有者本人（需通过 Owner.Creature 获取战斗生物）
//         if (target != base.Owner?.Creature)
//             return 1m;

//         // 没有伤害来源时不免疫（可根据需求修改）
//         if (dealer == null)
//             return 1m;

//         // 无敌激活时才免疫
//         return _isInvincible ? 0m : 1m;
//     }

//     // 主动激活无敌（供外部调用）
//     public void Activate()
//     {
//         if (_isInvincible) return;

//         _isInvincible = true;
//         Flash();   // 播放遗物闪烁效果

//         TaskHelper.RunSafely(DeactivateAfterDelay());
//     }

//     private async Task DeactivateAfterDelay()
//     {
//         await Cmd.Wait(_durationSeconds);
//         _isInvincible = false;
//         // 可选：再次闪烁表示无敌结束
//     }
// }