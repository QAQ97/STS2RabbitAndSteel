using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class DeadlinePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );
    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new CalculatedIntVar(() =>
        {
            if (Owner == null) return 0;
            int percent = Amount;
            var doom = Owner.GetPower<DoomPower>();
            int doomAmount = doom?.Amount ?? 0;
            return (int)((percent / 100.0) * Owner.MaxHp) + doomAmount;
        }, "Threshold")
    };
        

    /// <summary>
    /// 在生命值变化后（仅当减少时）检测是否满足死亡条件。
    /// </summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        // 只作用于自身
        if (creature != Owner) return;

        // 仅当受到伤害（血量减少）时触发
        if (delta >= 0) return;

        // 已死亡则跳过
        if (creature.IsDead) return;

        // 获取本 Power 的层数（视为百分比数值）
        int percent = Amount;
        if (percent <= 0) return;

        // 获取目标身上的 DoomPower 层数（如果存在）
        var doomPower = creature.GetPower<DoomPower>();
        int doomAmount = doomPower?.Amount ?? 0;

        // 计算死亡阈值：层数% × 最大生命值 + DoomPower层数
        int threshold = (int)((percent / 100.0) * creature.MaxHp) + doomAmount;

        // 如果当前生命值 ≤ 阈值，则触发死亡
        if (creature.CurrentHp <= threshold)
        {
            // 复用原版 DoomPower 的处决逻辑（特效 + 钩子）
            await DoomPower.DoomKill(new List<Creature> { creature });
        }
    }
    public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        return HealthBarForecasts.Single(
            context.Creature.GetPowerAmount<DeadlinePower>(),
            new Color(1f, 1f, 1f, 1f),
            HealthBarForecastGrowthDirection.FromRight
        );
    }
}