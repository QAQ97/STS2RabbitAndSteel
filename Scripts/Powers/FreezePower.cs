using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class FreezePower : ModPowerTemplate, IHealthBarForecastSource
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
    {
        if (orb.Owner == base.Owner.Player && orb is FrostOrb)
        {
           await PowerCmd.ModifyAmount(choiceContext, this, 1, base.Owner, null);
        }
        await Task.CompletedTask;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }

        int stacks = (int)base.Amount;
        if (stacks <= 0)
            return;

        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            stacks * 3m,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );
        if (base.Owner.IsAlive)
        {
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        int stacks = (int)Amount;
        if (stacks <= 3) return;

        int convertCount = stacks / 3;
        if (convertCount <= 0) return;

        await PowerCmd.ModifyAmount(choiceContext, this, -convertCount * 3, base.Owner, null);

        await PowerCmd.Apply<DeepFreezePower>(
            choiceContext,
            base.Owner,
            convertCount,
            base.Owner,
            null
        );
    }

    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        int stacks = (int)base.Amount;
        if (stacks <= 0)
            return Enumerable.Empty<HealthBarForecastSegment>();

        int predictedDamage = stacks * 3;
        Color frostColor = new Color(0.3f, 0.6f, 1.0f, 1f);
        return HealthBarForecasts.Single(
            predictedDamage,
            frostColor,
            HealthBarForecastGrowthDirection.FromRight
        );
    }
}