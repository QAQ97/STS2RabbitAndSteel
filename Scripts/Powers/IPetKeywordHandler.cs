using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace RabbitAndSteel.Scripts.Powers
{
    public interface IPetKeywordHandler
    {
        Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay, Creature pet);
    }
}