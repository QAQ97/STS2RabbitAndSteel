using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class PerformerPower : ModPowerTemplate
{
    private class Data
    {
        public int cardsLeft = 15;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public override int DisplayAmount => GetInternalData<Data>().cardsLeft;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;

        var data = GetInternalData<Data>();
        data.cardsLeft--;
        InvokeDisplayAmountChanged();

        if (data.cardsLeft <= 0)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, 2, Owner.Player);
            data.cardsLeft = 10;
            InvokeDisplayAmountChanged();
        }
    }
}