using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using RabbitAndSteel.Scripts.Commands;
using RabbitAndSteel.Scripts.Monsters.Pets;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(EventCardPool))]
public class AbyssalCall : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public AbyssalCall() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        // 1. 获取已解锁宠物列表（通过你的持久化访问方式）
        var state = Entry.Player.Get(Owner); // 假设这是获取 PlayerRunState 的方法
        if (state?.UnlockedPets == null || state.UnlockedPets.Count == 0)
            return;

        // 2. 过滤掉当前已存活的宠物（避免重复召唤同一类型）
        var currentPet = Owner.Creature.CombatState.Allies
            .FirstOrDefault(c => c.PetOwner == Owner && c.IsAlive);
        string currentType = currentPet?.Monster?.GetType().Name;

        var available = state.UnlockedPets.Where(p => p != currentType).ToList();
        if (available.Count == 0)
            return;

        string chosenPetType;

        // 3. 如果只有一个可选，直接选择
        if (available.Count == 1)
        {
            chosenPetType = available[0];
        }
        else
        {
            // 多个可选：创建临时卡牌列表用于选择界面（使用 ModelDb，不直接 new）
            var petCards = new List<CardModel>();
            foreach (var petName in available)
            {
                CardModel? card = null;
                switch (petName)
                {
                    case "Hero":
                        card = ModelDb.Card<HeroCard>().ToMutable();
                        break;
                    case "Stormbeast":
                        card = ModelDb.Card<StormbeastCard>().ToMutable();
                        break;
                }
                if (card != null)
                {
                    card.Owner = Owner; // 设置拥有者，确保显示正确
                    petCards.Add(card);
                }
            }

            if (petCards.Count == 0)
                return;

            var prefs = new CardSelectorPrefs(
                prompt: new LocString("card_selection", "ABYSSAL_CALL_CHOOSE_PET"),
                minCount: 1,
                maxCount: 1
            );
            var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, petCards, Owner, prefs);
            var chosenCard = selected.FirstOrDefault();
            if (chosenCard == null)
                return;

            // 根据选中的卡牌类型确定宠物名称
            chosenPetType = chosenCard switch
            {
                HeroCard => "Hero",
                StormbeastCard => "Stormbeast",
                _ => null
            };
            if (chosenPetType == null)
                return;
        }

        // 4. 移除当前宠物（如果存在）
        if (currentPet != null)
            PetManager.RemovePetSafely(currentPet);

        // 5. 召唤新宠物
        switch (chosenPetType)
        {
            case "Hero":
                await PetManager.SummonPet<Hero>(choiceContext, Owner, this);
                break;
            case "Stormbeast":
                await PetManager.SummonPet<Stormbeast>(choiceContext, Owner, this);
                break;
            case "Dreadwyrm":
                await PetManager.SummonPet<Dreadwyrm>(choiceContext, Owner, this);
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
            return resultPileTypeForCardPlay;
        return PileType.Hand;
    }
}