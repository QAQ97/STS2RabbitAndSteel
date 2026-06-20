using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Combat;
using STS2RitsuLib.Combat.HandSize;
using MegaCrit.Sts2.Core.Entities.Players;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class CanaryCharm : ModCardTemplate, IMaxHandSizeModifier
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    private int retainTurns = 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(8m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => ((CanaryCharm)card).retainTurns)
    ];

    public CanaryCharm() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        bool inHand = PileType.Hand.GetPile(Owner).Cards.Contains(this);
        if (inHand)
        {
            retainTurns++;
        }

        await Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int damage = (int)base.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(5);
    }
    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        if (player == Owner && PileType.Hand.GetPile(Owner).Cards.Contains(this))
            return currentMaxHandSize + 1;
        return currentMaxHandSize;
    }
}