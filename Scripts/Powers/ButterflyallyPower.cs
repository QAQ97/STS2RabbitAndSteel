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
public sealed class ButterflyallyPower : ModPowerTemplate, IPetKeywordHandler
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new SummonVar(2m)    
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

        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);
		IEnumerable<Creature> enumerable = base.CombatState.PlayerCreatures.Where((Creature c) => c?.IsAlive ?? false).ToList();
		foreach (Creature item in enumerable)
		{
			await OstyCmd.Summon(choiceContext, item.Player, base.DynamicVars.Summon.BaseValue, this);
		}
        return true;
    }
}