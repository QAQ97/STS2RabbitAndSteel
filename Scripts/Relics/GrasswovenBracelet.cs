using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
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

    // 本次战斗中遇到过的临时 Power（如 DarkShacklesPower / PiercingWailPower）
    private readonly List<ITemporaryPower> _tempPowers = new();

    public override Task BeforeCombatStart()
    {
        base.Status = RelicStatus.Active;
        _tempPowers.Clear();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        base.Status = RelicStatus.Normal;
        _tempPowers.Clear();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 每当一个 Power 的数量即将变化时，如果是临时 Power，就记下来。
    /// 用于后续判断“内部 Power”是否由它施加。
    /// </summary>
    public override Task BeforePowerAmountChanged(
        PowerModel power, decimal amount, Creature target,
        Creature? applier, CardModel? cardSource)
    {
        if (cardSource?.Owner != base.Owner) return Task.CompletedTask;
        if (applier != base.Owner.Creature) return Task.CompletedTask;
        if (target.Side == base.Owner.Creature.Side) return Task.CompletedTask;

        if (power is ITemporaryPower temp && !_tempPowers.Contains(temp))
        {
            _tempPowers.Add(temp);
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenMultiplicative(
        PowerModel power, Creature giver, decimal amount,
        Creature? target, CardModel? cardSource)
    {
        // 只关心“我方卡牌施加到敌方”的 Power
        if (cardSource?.Owner != base.Owner) return 1m;
        if (giver != base.Owner.Creature) return 1m;
        if (target == null) return 1m;
        if (target.Side == base.Owner.Creature.Side) return 1m;

        // 关键：如果当前要改的 Power 是某个临时 Power 内部的 Power（如 DarkShacklesPower 内部的 StrengthPower），
        // 就不再乘，避免 1.3 × 1.3 的双重计数。
        if (_tempPowers.Any(t => t.InternallyAppliedPower?.GetType() == power.GetType()))
            return 1m;

        return 1.3m;
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