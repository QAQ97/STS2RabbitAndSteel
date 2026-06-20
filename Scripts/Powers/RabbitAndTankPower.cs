using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class RabbitAndTankPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (side == base.Owner.Side)
		{
			await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount*7, base.Owner, null);
            await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount*7, base.Owner, null);
		}
	}
}