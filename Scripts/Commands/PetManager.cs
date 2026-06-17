using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace RabbitAndSteel.Scripts.Commands
{
    public static class PetManager
    {
        private static async Task ApplyPowerToPet(PlayerChoiceContext choiceContext, Creature pet, ZangHuaMonster petModel, Creature owner, CardModel? cardSource)
        {
            var powerType = petModel.DefaultPowerType;
            var applyMethod = typeof(PowerCmd).GetMethod("Apply", new[] { typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool) });
            if (applyMethod == null)
                throw new InvalidOperationException("PowerCmd.Apply method not found with expected signature.");

            var genericApply = applyMethod.MakeGenericMethod(powerType);
            await (Task)genericApply.Invoke(null, new object[] { choiceContext, pet, 1m, owner, cardSource, false });
        }

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

        public static Creature? GetPet<T>(Player player) where T : ZangHuaMonster
        {
            if (player?.Creature?.CombatState == null) return null;
            return player.Creature.CombatState.Allies
                .FirstOrDefault(c => c.Monster is T && c.PetOwner == player && c.IsAlive);
        }
    }
}