using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace RabbitAndSteel.Scripts.Powers
{
    public interface IPetKeywordHandler
    {
        // --- 实现（卡牌打出） ---
        Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay, Creature pet);
    }

    public interface IPetDebuffHandler
    {
        // --- 实现（Debuff 施加） ---
        Task<bool> OnPetDebuffTriggered(PlayerChoiceContext choiceContext, PowerModel debuffPower, Creature debuffTarget, Creature pet);
    }
}