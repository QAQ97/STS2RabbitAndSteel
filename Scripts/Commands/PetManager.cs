using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using RabbitAndSteel.Scripts.Cards.Ancient;
using RabbitAndSteel.Scripts.Models;
using RabbitAndSteel.Scripts.Monsters.Pets;

namespace RabbitAndSteel.Scripts.Commands
{
    public static class PetManager
    {
        //宠物能力
        private static async Task ApplyPowerToPet(PlayerChoiceContext choiceContext, Creature pet, ZangHuaMonster petModel, Creature owner, CardModel? cardSource)
        {
            var powerType = petModel.DefaultPowerType;
            var applyMethod = typeof(PowerCmd).GetMethod("Apply", new[] { typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool) });
            if (applyMethod == null)
                throw new InvalidOperationException("PowerCmd.Apply method not found with expected signature.");

            var genericApply = applyMethod.MakeGenericMethod(powerType);
            await (Task)genericApply.Invoke(null, new object[] { choiceContext, pet, 1m, owner, cardSource, false });
        }

        //宠物生成
        public static async Task<Creature> SummonPet<T>(PlayerChoiceContext choiceContext, Player owner, CardModel? cardSource = null)
            where T : ZangHuaMonster, new()
        {
            if (owner?.Creature == null)
                throw new InvalidOperationException("Owner or owner's creature is null");

            var combatState = owner.Creature.CombatState;
            if (combatState == null)
                throw new InvalidOperationException("CombatState is null");

            var pet = combatState.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == owner);
            bool isReviving = pet != null && pet.IsDead;
            bool alreadyAlive = pet != null && pet.IsAlive;

            if (alreadyAlive)
                return pet;

            if (isReviving)
            {
                owner.PlayerCombatState?.AddPetInternal(pet);
                await CreatureCmd.SetMaxHp(pet, 1);
            }
            else
            {
                pet = await PlayerCmd.AddPet<T>(owner);
                await CreatureCmd.SetCurrentHp(pet, 1);
                var newNode = NCombatRoom.Instance?.GetCreatureNode(pet);
                if (newNode != null && cardSource is CardModel)
                {
                    newNode.Modulate = Colors.Transparent;
                    Tween tween = newNode.CreateTween();
                    tween.TweenProperty(newNode, "modulate", Colors.White, 0.35f).SetDelay(0.1f);
                    newNode.StartReviveAnim();
                }
            }

            if (pet.Monster is ZangHuaMonster petModel)
            {
                await ApplyPowerToPet(choiceContext, pet, petModel, owner.Creature, cardSource);
            }

            return pet;
        }

        //宠物获取
        public static Creature? GetPet<T>(Player player) where T : ZangHuaMonster
        {
            if (player?.Creature?.CombatState == null) return null;
            return player.Creature.CombatState.Allies
                .FirstOrDefault(c => c.Monster is T && c.PetOwner == player && c.IsAlive);
        }




        public static void RemovePetSafely(Creature pet)
        {
            if (pet?.CombatState == null || pet.PetOwner == null)
                return;

            // ---- 1. 从 PlayerCombatState._pets 中移除 ----
            var playerCombatState = pet.PetOwner.PlayerCombatState;
            if (playerCombatState != null)
            {
                var petsField = typeof(PlayerCombatState).GetField("_pets",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (petsField?.GetValue(playerCombatState) is List<Creature> petsList)
                {
                    if (petsList.Remove(pet))
                    {
                        // 解绑死亡事件
                        var onPetDiedMethod = typeof(PlayerCombatState).GetMethod("OnPetDied",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                        if (onPetDiedMethod != null)
                        {
                            var handler = Delegate.CreateDelegate(
                                typeof(Action<Creature>), playerCombatState, onPetDiedMethod) as Action<Creature>;
                            if (handler != null)
                                pet.Died -= handler;
                        }
                    }
                }
            }

            // ---- 2. 从 CombatState.Allies 中移除 ----
            var combatState = pet.CombatState;
            var alliesField = combatState.GetType().GetField("_allies",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?? combatState.GetType().GetField("_allyList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (alliesField?.GetValue(combatState) is IList<Creature> alliesList)
            {
                alliesList.Remove(pet);
            }

            // ---- 3. 销毁场景节点（无死亡动画） ----
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(pet);
            if (creatureNode != null)
            {
                NCombatRoom.Instance!.RemoveCreatureNode(creatureNode);
                creatureNode.QueueFree();
            }
        }


        //召唤逻辑
        public static async Task SummonOrSwitchPet(PlayerChoiceContext choiceContext, Player owner, CardModel? cardSource = null)
        {
            if (owner?.Creature == null)
                return;

            // 1. 获取已解锁宠物列表（根据你的持久化方式，这里假设 Entry.Player.Get(owner) 可用）
            var state = Entry.Player.Get(owner); // 若此方法不存在，请替换为实际获取方式
            if (state?.UnlockedPets == null || state.UnlockedPets.Count == 0)
                return;

            // 2. 获取当前存活的宠物类型（如果有）
            var currentPet = owner.Creature.CombatState.Allies
                .FirstOrDefault(c => c.PetOwner == owner && c.IsAlive);
            string currentType = currentPet?.Monster?.GetType().Name;

            // 3. 过滤掉已存活的类型
            var available = state.UnlockedPets.Where(p => p != currentType).ToList();
            if (available.Count == 0)
                return;

            // 4. 确定要召唤的宠物类型
            string chosenPetType;
            if (available.Count == 1)
            {
                chosenPetType = available[0];
            }
            else
            {
                // 多个可选 → 弹出选择界面
                var petCards = new List<CardModel>();
                foreach (var petName in available)
                {
                    CardModel? card = petName switch
                    {
                        "Hero" => ModelDb.Card<HeroCard>().ToMutable(),
                        "Stormbeast" => ModelDb.Card<StormbeastCard>().ToMutable(),
                        "Dreadwyrm" => ModelDb.Card<DreadwyrmCard>().ToMutable(),
                        "LichOfFlowers" => ModelDb.Card<LichOfFlowersCard>().ToMutable(),
                        _ => null
                    };
                    if (card != null)
                    {
                        card.Owner = owner;
                        petCards.Add(card);
                    }
                }

                if (petCards.Count == 0)
                    return;

                var prefs = new CardSelectorPrefs(
                    prompt: new LocString("card_selection", "ABYSSAL_CALL_CHOOSE_PET"),
                    minCount: 1,
                    maxCount: 1
                );
                var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, petCards, owner, prefs);
                var chosenCard = selected.FirstOrDefault();
                if (chosenCard == null)
                    return;

                chosenPetType = chosenCard switch
                {
                    HeroCard => "Hero",
                    StormbeastCard => "Stormbeast",
                    DreadwyrmCard => "Dreadwyrm",
                    LichOfFlowersCard => "LichOfFlowers",
                    _ => null
                };
                if (chosenPetType == null)
                    return;
            }

            // 5. 移除当前宠物（如果存在）
            if (currentPet != null)
                RemovePetSafely(currentPet);

            // 6. 召唤新宠物
            switch (chosenPetType)
            {
                case "Hero":
                    await SummonPet<Hero>(choiceContext, owner, cardSource);
                    break;
                case "Stormbeast":
                    await SummonPet<Stormbeast>(choiceContext, owner, cardSource);
                    break;
                case "Dreadwyrm":
                    await SummonPet<Dreadwyrm>(choiceContext, owner, cardSource);
                    break;
                case "LichOfFlowers":
                    await SummonPet<LichOfFlowers>(choiceContext, owner, cardSource);
                    break;
            }
        }
    }
}
