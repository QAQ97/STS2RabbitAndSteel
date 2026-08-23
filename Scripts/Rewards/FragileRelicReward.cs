using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib.Combat.Rewards;

namespace RabbitAndSteel.Scripts.Rewards;

public class FragileRelicReward : ModCustomReward
{
    private RelicModel? _relic;

    public FragileRelicReward(Player player) : base(player)
    {
    }

    // 使用注册的 RewardType
    public override RewardType ModRewardType => Entry.RelicRewardType;
    protected override string? RewardIconPath => $"res://RabbitAndSteel/images/ancient/arts/{GetType().Name}.png";
    protected override string DescriptionLocKey => "FRAGILE_RELIC_REWARD";

    // 标记内容已查看（用于图鉴解锁等）
    public override void MarkContentAsSeen()
    {
        if (_relic != null)
        {
            SaveManager.Instance.MarkRelicAsSeen(_relic);
        }
    }

    // 点击奖励时的实际效果
    protected override async Task<bool> OnSelect()
    {
        // 从 FragilePool 中获取一个遗物
        _relic = RelicFactory.PullNextRelicFromFront(Player)
            .ToMutable();

        // 获得遗物
        await RelicCmd.Obtain(_relic, Player);

        return true;
    }
}