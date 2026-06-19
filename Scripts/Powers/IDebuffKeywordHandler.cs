using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace RabbitAndSteel.Scripts.Powers
{
    public interface IDebuffKeywordHandler
    {
        /// <summary>
        /// 当持有者成功施加一个 Debuff 时调用
        /// </summary>
        /// <param name="choiceContext">当前玩家选择上下文</param>
        /// <param name="debuffPower">被施加的 Debuff Power</param>
        /// <param name="debuffTarget">Debuff 的目标生物</param>
        /// <param name="pet">触发效果的宠物（Creature 类型）</param>
        /// <returns>是否已处理</returns>
        Task<bool> OnOwnerDebuffApplied(PlayerChoiceContext choiceContext,
                                        PowerModel debuffPower,
                                        Creature debuffTarget,
                                        Creature pet);
    }
}