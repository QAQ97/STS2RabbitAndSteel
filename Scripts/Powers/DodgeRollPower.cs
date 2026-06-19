using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public sealed class DodgeRollPower : ModPowerTemplate, IPetKeywordHandler
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move)
    ];

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public async Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay, Creature pet)
    {

        var playerCreature = pet.PetOwner?.Creature;
        if (playerCreature == null) return false;
        var player = cardPlay.Card.Owner;
        if (player != pet.PetOwner)
            return false;

        decimal actualBlock = await CreatureCmd.GainBlock(playerCreature, base.DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, playerCreature, actualBlock, playerCreature, cardPlay.Card);
        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);

        return true;
    }
}