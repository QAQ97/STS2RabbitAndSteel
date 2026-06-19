using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;
[RegisterRelic(typeof(SharedRelicPool))]
public sealed class NightstarGrimoire : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    private const string _turnsKey = "Turns";
    private const string _damageKey = "Damage";
    private bool _isActivating;
    private int _turnCounter;
    public override bool ShowCounter => true;
    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );
    public override int DisplayAmount
    {
        get
        {
            if (IsActivating)
                return base.DynamicVars[_turnsKey].IntValue;
            return _turnCounter;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DynamicVar(_turnsKey, 3m),
        new DamageVar(15m, ValueProp.Unpowered)
    };

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }
    public int TurnCounter
    {
        get => _turnCounter;
        private set
        {
            AssertMutable();
            _turnCounter = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override Task BeforeCombatStart()
    {
        TurnCounter = 0;
        base.Status = RelicStatus.Normal;
        IsActivating = false;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner) return;
        TurnCounter++;
        int requiredTurns = base.DynamicVars[_turnsKey].IntValue;
        base.Status = (TurnCounter == requiredTurns - 1) ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        if (TurnCounter == requiredTurns)
        {
            TaskHelper.RunSafely(DoActivateVisuals());
            int damage = base.DynamicVars[_damageKey].IntValue;
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature.CombatState.HittableEnemies, base.DynamicVars.Damage, base.Owner.Creature);
            TurnCounter = 0;
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        TurnCounter = 0;
        base.Status = RelicStatus.Normal;
        IsActivating = false;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}