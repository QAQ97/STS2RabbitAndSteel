using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public sealed class DodgeRollPower : ModPowerTemplate, IPetKeywordHandler, IPetDebuffHandler
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
        var player = cardPlay.Card.Owner;
        if (player != pet.PetOwner) return false;
        return await ExecuteAction(choiceContext, pet, cardPlay);
    }
    public async Task<bool> OnPetDebuffTriggered(PlayerChoiceContext choiceContext, PowerModel debuffPower, Creature debuffTarget, Creature pet)
    {
        var player = pet.PetOwner;
        if (player == null || player.Creature == null) return false;
        return await ExecuteAction(choiceContext, pet, null);
    }

    

    private async Task<bool> ExecuteAction(PlayerChoiceContext choiceContext, Creature pet, CardPlay? cardPlay)
    {
        var player = pet.PetOwner;
        if (player == null) return false;
        var playerCreature = player.Creature;
        if (playerCreature == null) return false;


        decimal baseActual = await CreatureCmd.GainBlock(playerCreature, base.DynamicVars.Block, cardPlay);
        decimal petDexAmount = pet.PetOwner?.Creature?.Powers.OfType<DexteritySpellPower>().Sum(p => p.Amount) ?? 0m;
        decimal extraActual = 0m;
        if (petDexAmount > 0m)
        {
            extraActual = await CreatureCmd.GainBlock(playerCreature, new BlockVar(petDexAmount, ValueProp.Move), cardPlay);
        }
        decimal actualBlock = baseActual + extraActual;

   
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, playerCreature, actualBlock, playerCreature, cardPlay?.Card);
        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);

        return true;
    }
}