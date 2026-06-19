using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using RabbitAndSteel.Scripts.Powers;
using RabbitAndSteel.Scripts.Models;

namespace RabbitAndSteel.Scripts
{
    [RegisterSingleton]
    public class PetKeywordHandler : HookedSingletonModel
    {
        public PetKeywordHandler() : base(HookType.Combat) { }

        public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var card = cardPlay.Card;
            if (card == null) return;

            if (!card.Keywords.Contains(MyKeywords.pet))
                return;

            var player = card.Owner;
            var allies = player?.Creature?.CombatState?.Allies;
            if (allies == null) return;

            foreach (var petEntry in allies)
            {
                if (petEntry?.Monster is not ZangHuaMonster) continue;

                bool handled = false;
                foreach (var power in petEntry.Powers)
                {
                    if (power is IPetKeywordHandler handler)
                    {
                        handled = await handler.OnPetKeywordTriggered(choiceContext, cardPlay, petEntry);
                        if (handled)
                        {
                            Log.Info($"Pet {petEntry.Name} handled keyword via power {power.GetType().Name} for card {card.Id}");
                            break;
                        }
                    }
                }
                if (handled) break;
            }
        }
    }
}