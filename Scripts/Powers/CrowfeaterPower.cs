using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers
{
    [RegisterPower]
    public class CrowfeaterPower : ModPowerTemplate
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile => new(
            IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
            BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
        );

        public override async Task AfterPowerAmountChanged(
            PlayerChoiceContext choiceContext,
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (amount > 0m && applier == Owner && power.Type == PowerType.Debuff)
            {
                Flash();

                var allies = Owner?.CombatState?.Allies;
                if (allies == null) return;

                foreach (Creature pet in allies)
                {
                    if (pet.Monster is not ZangHuaMonster)
                        continue;

                    bool handled = false;
                    foreach (var petPower in pet.Powers)
                    {
                        if (petPower is IDebuffKeywordHandler handler)
                        {
                            handled = await handler.OnOwnerDebuffApplied(
                                choiceContext,
                                debuffPower: power,
                                debuffTarget: power.Owner,
                                pet: pet);
                            if (handled)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}