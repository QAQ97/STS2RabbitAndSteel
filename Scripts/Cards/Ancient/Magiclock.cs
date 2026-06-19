using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class Magiclock : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public Magiclock() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int handCount = base.Owner.PlayerCombatState.Hand.Cards.Count;
        if (handCount == 0) return;

        // 根据是否升级决定最少选择数量
        int minCount = IsUpgraded ? 0 : 3;

        // 未升级且手牌不足 3 张时，无法满足最小选择数，直接返回（不产生任何效果）
        if (!IsUpgraded && handCount < 3) return;

        var prefs = new CardSelectorPrefs(
            prompt: new LocString("card_selection", "MAGICLOCK_SELECT_PROMPT"),
            minCount: minCount,
            maxCount: handCount
        );

        var selectedCards = await CardSelectCmd.FromHand(
            prefs: prefs,
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this
        );

        int exhaustedCount = selectedCards.Count();
        if (exhaustedCount == 0) return;

        int strengthGain = 0;
        int dexterityGain = 0;

        foreach (var card in selectedCards)
        {
            if (card is AbyssalCall)
            {
                strengthGain += 3;
                dexterityGain += 3;
                continue;
            }

            if (card.Rarity == CardRarity.Basic ||
                card.Rarity == CardRarity.Common ||
                card.Rarity == CardRarity.Token)
            {
                continue;
            }

            strengthGain += 2;
        }

        foreach (var card in selectedCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        if (strengthGain > 0)
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext,
                base.Owner.Creature,
                strengthGain,
                base.Owner.Creature,
                this
            );
        }
        if (dexterityGain > 0)
        {
            await PowerCmd.Apply<DexterityPower>(
                choiceContext,
                base.Owner.Creature,
                dexterityGain,
                base.Owner.Creature,
                this
            );
        }

        if (exhaustedCount > 0)
        {
            await CardPileCmd.Draw(choiceContext, exhaustedCount, base.Owner);
        }
    }

    // 战斗开始时自动打出（之前已实现的逻辑）
    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner && CombatState.RoundNumber == 1)
        {
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }
}