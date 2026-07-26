using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Combat.HandSize;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class ForkAndSpoon : ModCardTemplate, IMaxHandSizeModifier
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 添加 sustain 关键词
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, MyKeywords.sustain];

    protected override bool ShouldGlowRedInternal => SustainKeywordHandler.GetSustainTurnCount(this) == 0;
    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable)
                return false;
            return SustainKeywordHandler.GetSustainTurnCount(this) > 0;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("RefreshTrigger", 0),
    ];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public ForkAndSpoon() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ForkAndSpoonPower>(choiceContext, Owner.Creature, 1m , Owner.Creature, this);
        SustainKeywordHandler.ResetSustainTurnCount(this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}