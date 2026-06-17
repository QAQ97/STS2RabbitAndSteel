using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class CursePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    private const decimal DAMAGE_PERCENT = 0.3m;

    public int NextTurnDamage => CalculateTotalDamageNextTurn();

    private int TotalStacks => base.Owner?.GetTotalDelayedDamageStacks() ?? 0;

    private int CalculateTotalDamageNextTurn()
    {
        int totalStacks = TotalStacks;
        if (totalStacks == 0) return 0;

        decimal curse = System.Math.Ceiling(totalStacks * DAMAGE_PERCENT);
        if (curse < 1 && totalStacks > 0) curse = 1;

        if (base.Owner?.CombatState == null)
            return (int)curse;

        curse = Hook.ModifyDamage(
            base.Owner.CombatState.RunState,
            base.Owner.CombatState,
            base.Owner,
            null,
            curse,
            ValueProp.Unblockable,
            null,
            ModifyDamageHookType.All,
            CardPreviewMode.None,
            out _
        );

        return (int)curse;
    }

    public override async Task AfterSideTurnStart(CombatSide side,IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (base.Owner == null || side != base.Owner.Side)
            return;

        int totalStacks = TotalStacks;
        if (totalStacks == 0) return;

        int damage = CalculateTotalDamageNextTurn();
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            damage,
            ValueProp.Unblockable,
            null,
            null
        );
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,  IEnumerable<Creature> participants)
    {
        if (base.Owner == null || side != base.Owner.Side)
            return;
        if (base.Amount <= 0)
        {
            await PowerCmd.Remove(this);
            return;
        }

        await PowerCmd.Decrement(this);

        if (base.Amount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}