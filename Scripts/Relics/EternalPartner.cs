using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using RabbitAndSteel.Scripts.Cards.Ancient;
using RabbitAndSteel.Scripts.Characters;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(AncientRelicPool))]
[RegisterCharacterStarterRelic(typeof(AncientCharacter))]
public class EternalPartner : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );

    public override bool ShowCounter => !base.IsCanonical;
    public override int DisplayAmount
    {
        get
        {
            if (base.IsCanonical || Owner == null)
                return -1;

            var data = Entry.Player.Get(Owner);
            return data?.CurrentStacks ?? -1;
        }
    }


    public void AddStacks(int amount)
    {
        if (Owner == null)
            return;

        Entry.Player.Modify(Owner, data =>
        {
            data.CurrentStacks += amount;
        });
        InvokeDisplayAmountChanged();
    }

    public void RemoveStacks(int amount)
    {
        if (Owner == null)
            return;

        Entry.Player.Modify(Owner, data =>
        {
            data.CurrentStacks -= amount;
        });
        InvokeDisplayAmountChanged();
    }

    public int GetCurrentStacks()
    {
        if (Owner == null)
            return 0;

        var data = Entry.Player.Get(Owner);
        return data?.CurrentStacks ?? 0;
    }

    public void SetStacks(int value)
    {
        if (Owner == null)
            return;

        Entry.Player.Modify(Owner, data =>
        {
            data.CurrentStacks = value;
        });
        InvokeDisplayAmountChanged();
    }


     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Owner.Creature.CombatState.RoundNumber != 1)
            return;
        Flash();

        var template = ModelDb.Card<AbyssalCall>();
        var card = Owner.Creature.CombatState.CreateCard(template, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}