using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;
[RegisterRelic(typeof(SharedRelicPool))]
public sealed class GrasswovenBracelet : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new MaxHpVar(8)
    };

    public override Task BeforeCombatStart()
    {
        base.Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (cardSource == null) return 1m;
        if (cardSource.Owner != base.Owner) return 1m;
        if (giver != base.Owner.Creature) return 1m;
        if (target == null) return 1m;
        
        return 1.3m;
    }
    public override Task AfterCombatEnd(CombatRoom room)
    {
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
    public override async Task AfterObtained()
	{
		await CreatureCmd.GainMaxHp(base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue);
	}

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );
}