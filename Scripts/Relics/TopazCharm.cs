
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(SharedRelicPool))]
public class TopazCharm : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    private const int BreakThreshold = 3;
    private int _damageTakenThisCombat;
    private bool _isPermanentlyBroken;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new GoldVar(25)
    };
    
    [SavedProperty]
    public bool IsPermanentlyBroken
    {
        get => _isPermanentlyBroken;
        set
        {
            AssertMutable();
            _isPermanentlyBroken = value;
            if (_isPermanentlyBroken)
            {
                base.Status = RelicStatus.Disabled;
                InvokeDisplayAmountChanged();
            }
        }
    }

    public override bool ShowCounter
    {
        get
        {
            return CombatManager.Instance.IsInProgress 
                   && !IsPermanentlyBroken 
                   && !base.IsCanonical;
        }
    }

    public override int DisplayAmount
    {
        get
        {
            if (!ShowCounter)
                return -1;
            return _damageTakenThisCombat;
        }
    }
	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsPermanentlyBroken) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (target != base.Owner.Creature) return;
        if (result.WasFullyBlocked) return;
        if (result.UnblockedDamage <= 0) return;
        
        _damageTakenThisCombat++;
        InvokeDisplayAmountChanged();
        
        if (_damageTakenThisCombat >= BreakThreshold)
        {
            IsPermanentlyBroken = true;
        }
    }

    public override Task BeforeCombatStart()
    {
        if (!IsPermanentlyBroken)
        {
            _damageTakenThisCombat = 0;
            base.Status = RelicStatus.Active;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        if (!IsPermanentlyBroken)
        {
            base.Status = RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != base.Owner)
		{
			return false;
		}
		if (room == null)
		{
			return false;
		}
		if (!room.RoomType.IsCombatRoom())
		{
			return false;
		}
		if (room.RoomType == RoomType.Boss && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1)
		{
			return false;
		}
        if (IsPermanentlyBroken) return false;
        rewards.Add(new GoldReward(base.DynamicVars.Gold.IntValue, player));
        return true;
    }


    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );
}