using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class DeepFreezePower : ModPowerTemplate, IHealthBarForecastSource
{
    // ----- 基础属性 -----
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        int stacks = base.Amount;
        if (stacks <= 0)
            return;

        int damage = stacks * 10;
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );


        await PowerCmd.Apply<VulnerablePower>(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            1,
            base.Owner,
            null
        );

        await PowerCmd.Apply<WeakPower>(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            1,
            base.Owner,
            null
        );
    }


    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        int stacks = base.Amount;
        if (stacks <= 0)
            return Enumerable.Empty<HealthBarForecastSegment>();

        int predictedDamage = stacks * 10;
        Color deepFrostColor = new Color(0.2f, 0.2f, 0.8f, 1f);
        return HealthBarForecasts.Single(
            predictedDamage,
            deepFrostColor,
            HealthBarForecastGrowthDirection.FromRight
        );
    }
}