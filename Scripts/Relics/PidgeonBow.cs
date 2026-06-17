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
public sealed class PidgeonBow : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DamageVar(3m, ValueProp.Unpowered)
    };

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner) return;
        var enemies = player.Creature.CombatState.HittableEnemies;
        if (enemies.Count == 0) return;
        var randomEnemy = player.RunState.Rng.CombatTargets.NextItem(enemies);
        await CreatureCmd.Damage(choiceContext, randomEnemy, base.DynamicVars.Damage, base.Owner.Creature);
    }

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );
}