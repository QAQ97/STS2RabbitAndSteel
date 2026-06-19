using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class HydoursBlob : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { MyKeywords.pet, CardKeyword.Exhaust };
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
        new DynamicVar("BaseValue", 1)
    };

    public HydoursBlob() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int maxSelectCount = base.DynamicVars["BaseValue"].IntValue;
        
        var prompt = new LocString("card_selection", "ADD_PET_KEYWORD_ENCHANCEMENT");
        var prefs = new CardSelectorPrefs(prompt, minCount: 0, maxCount: maxSelectCount);
        
        var selectedCards = await CardSelectCmd.FromHand(
            prefs: prefs,
            context: choiceContext,
            player: base.Owner,
            filter: (CardModel c) => !c.Keywords.Contains(MyKeywords.pet),
            source: this
        );

        foreach (var card in selectedCards)
        {
            CardCmd.ApplyKeyword(card, MyKeywords.pet);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}