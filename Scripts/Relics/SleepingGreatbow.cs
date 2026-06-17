using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(SharedRelicPool))]
public sealed class SleepingGreatbow : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DamageVar(6m, ValueProp.Unpowered)
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner)
        {
            var attackingEnemies = player.Creature.CombatState.HittableEnemies
                .Where(e => e.Monster?.IntendsToAttack != true)
                .ToList();

            if (attackingEnemies.Any())
            {
                Flash();
                await CreatureCmd.Damage(choiceContext, attackingEnemies, base.DynamicVars.Damage, base.Owner.Creature);
            }
        }
    }

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );
}