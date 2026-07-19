using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class ParasitePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    // 动态变量：记录宿主最后一次受到的伤害是否为攻击（1=是，0=否）
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("LastDamageWasAttack", 0m)
    ];

    // 每次受到伤害时记录伤害是否为攻击
    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            bool isAttack = props.IsPoweredAttack();
            base.DynamicVars["LastDamageWasAttack"].BaseValue = isAttack ? 1m : 0m;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 仅当宿主正常死亡（非因移除阻止）且为本力量拥有者时触发
        if (wasRemovalPrevented || creature != base.Owner)
            return;

        // 如果最后一次伤害是攻击，则不触发回血
        if (base.DynamicVars["LastDamageWasAttack"].BaseValue > 0)
            return;

        // 获取施加者
        Creature? applier = base.Applier;
        if (applier == null || !applier.IsAlive)
            return;

        int healAmount = base.Amount;
        if (healAmount <= 0)
            return;

        Flash();
        SfxCmd.Play("heal.mp3");
        VfxCmd.PlayOnCreature(applier, "vfx/heal");
        await CreatureCmd.Heal(applier, healAmount);
    }
}