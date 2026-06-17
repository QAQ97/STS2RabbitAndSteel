using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Players;

namespace RabbitAndSteel.Scripts.Powers;
[RegisterPower]

public sealed class StalematePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => new(
    IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
    BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
);
    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }

        decimal restoreEnergy = base.Amount - 1m;
        if (restoreEnergy < 0m)
        {
            restoreEnergy = 0m;
        }

        await PlayerCmd.SetEnergy(restoreEnergy, player);
        await PowerCmd.Remove(this);
    }
    public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != base.Owner.Player)
		{
			return amount;
		}
		return -1m;
	}
    public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != base.Owner.Player)
		{
			return count;
		}
		return -1m;
	}
}