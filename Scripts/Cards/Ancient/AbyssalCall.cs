using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Random;
using RabbitAndSteel.Scripts.Commands;
using RabbitAndSteel.Scripts.Monsters.Pets;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(EventCardPool))]
public class AbyssalCall : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.Self;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );


    public AbyssalCall() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        Rng rng = Owner.PlayerRng.Rewards;
        string petName = RollPet(rng);
        switch (petName)
        {
            case "Hero":
                await PetManager.SummonPet<Hero>(choiceContext, Owner, this);
                break;
            case "Dreadwyrm":
                await PetManager.SummonPet<Dreadwyrm>(choiceContext, Owner, this);
                break;
            case "Stormbeast":
                await PetManager.SummonPet<Stormbeast>(choiceContext, Owner, this);
                break;
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    
    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType resultPileTypeForCardPlay = base.GetResultPileTypeForCardPlay();
        if (resultPileTypeForCardPlay != PileType.Discard)
        {
            return resultPileTypeForCardPlay;
        }
        return PileType.Hand;
    }

    public static string RollPet(Rng rng)
    {
        var choices = new WeightedList<string>();
        choices.Add("Hero", 1);
        choices.Add("Stormbeast", 1);
        
        return choices.GetRandom(rng, remove: true);
    }
    
}