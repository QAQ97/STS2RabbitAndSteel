using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Models;

namespace RabbitAndSteel.Scripts
{
    [RegisterSingleton]
    public class SustainKeywordHandler : HookedSingletonModel
    {
        private static readonly ConditionalWeakTable<CardModel, Counter> _turnCounts = new();

        private class Counter
        {
            public int Value;
        }

        public SustainKeywordHandler() : base(HookType.Combat) { }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player) return;

            var playerCreature = participants.FirstOrDefault(c => c.Side == CombatSide.Player && c.IsPlayer);
            if (playerCreature == null) return;

            var player = playerCreature.Player;
            if (player == null) return;

            var handPile = PileType.Hand.GetPile(player);
            if (handPile == null) return;

            // 遍历手牌，增加所有带有 sustain 关键词的卡牌计数
            foreach (var card in handPile.Cards.ToList())
            {
                if (card.Keywords.Contains(MyKeywords.sustain))
                {
                    var counter = _turnCounts.GetOrCreateValue(card);
                    counter.Value++;
                    Log.Info($"Sustain card {card.Id} now has {counter.Value} turns retained.");
                }
            }

            await Task.CompletedTask;
        }

        // 获取某张卡的保留回合数
        public static int GetSustainTurnCount(CardModel card)
        {
            if (card == null || !card.Keywords.Contains(MyKeywords.sustain))
                return 0;

            return _turnCounts.TryGetValue(card, out var counter) ? counter.Value : 0;
        }

        // 重置某张卡的计数（通常在打出后调用）
        public static void ResetSustainTurnCount(CardModel card)
        {
            if (card == null) return;
            if (_turnCounts.TryGetValue(card, out var counter))
                counter.Value = 0;
        }
    }
}