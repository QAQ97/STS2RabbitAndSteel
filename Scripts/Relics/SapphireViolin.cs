
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(SharedRelicPool))]
public class SapphireViolin : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    private int _damageTakenThisCombat;
    private bool _isPermanentlyBroken;
    
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
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (IsPermanentlyBroken) return 1m;
        if (!props.IsCardOrMonsterMove()) return 1m;
        if (cardSource?.Owner != base.Owner) return 1m;
        
        return 1.3m;
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
        
        if (_damageTakenThisCombat >= 2)
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

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[0];
}