using System;
using System.Collections.Generic;
using Common;
using System.Linq;

namespace Server
{
    public class Engine
    {
        readonly static Random Random = new Random();

        readonly GameState State;

        readonly List<BaseGameObject> GameObjects;

        PlayerState CurrentPlayerState => State.Players[State.CurrentPlayer];

        public Engine(List<List<ICard>> decks)
        {
            State = new GameState(decks.Count, 20);

            GameObjects = new List<BaseGameObject>();

            int playerIndex = 0;
            foreach (var deck in decks)
            {
                Shuffle(deck);
                var registeredDeck = deck.Select(RegisterGameObject);
                State.Players[playerIndex].Hand = registeredDeck.Take(7).ToList();
                State.Players[playerIndex].Deck = registeredDeck.Skip(7).ToList();
                playerIndex++;
            }
            StartTurn();
        }

        GameObject<T> RegisterGameObject<T>(T instance, int owner)
        {
            var obj = new GameObject<T>(instance, GameObjects.Count, owner);
            GameObjects.Add(obj);
            return obj;
        }

        static void Shuffle(List<ICard> pile)
        {
            var newWeights = pile.ToDictionary(card => card, card => Random.Next(pile.Count));
            pile.Sort((a, b) => newWeights[a] - newWeights[b]);
        }

        void Draw(int playerIndex)
        {
            var card = CurrentPlayerState.Deck[0];
            CurrentPlayerState.Deck.RemoveAt(0);
            CurrentPlayerState.Hand.Add(card);
        }

        void StartTurn()
        {
            CurrentPlayerState.MaxEnergy++;
            CurrentPlayerState.Energy = CurrentPlayerState.MaxEnergy;
            Draw(State.CurrentPlayer);
            foreach (var creature in CurrentPlayerState.Creatures.Unwrap())
                creature.AbleToAttack = true;
        }

        public void EndTurn(int playerIndex)
        {
            if (playerIndex != State.CurrentPlayer)
                throw new InvalidOperationException("You are not an active player");

            State.CurrentPlayer = (State.CurrentPlayer + 1) % State.Players.Count;
            StartTurn();
        }

        GameObject<T> FindGameObjectFor<T>(T instance) where T : class
        {
            return GameObjects.SingleOrDefault(x => x.BaseInstance == instance) as GameObject<T>;
        }

        GameObject<R> MorphGameObject<T, R>(GameObject<T> obj, Func<T, R> morpher)
        {
            var newObject = new GameObject<R>(morpher(obj.Instance), obj.Id, obj.Owner);
            GameObjects.Replace(obj, newObject);
            return newObject;
        }

        public void Cast(int playerIndex, GameObject<ICard> card)
        {
            if (playerIndex != State.CurrentPlayer)
                throw new InvalidOperationException("You are not an active player");

            if (!CurrentPlayerState.Hand.Contains(card))
                throw new InvalidOperationException("You cannot cast cards that are not in your hand");

            if (CurrentPlayerState.Energy < card.Instance.CastingCost)
                throw new InvalidOperationException("Not enough energy");

            CurrentPlayerState.Hand.Remove(card);
            CurrentPlayerState.Energy -= card.Instance.CastingCost;
            
            switch (card.Instance)
            {
                case ICreatureCard creatureCard:
                    CurrentPlayerState.Creatures.Add(MorphGameObject(card, x => new Creature(creatureCard, playerIndex)));
                    break;
                case ISpellCard spellCard:
                    CurrentPlayerState.Discard.Add(card);
                    break;
            }
        }

        public void Attack(int playerIndex, GameObject<Creature> creature, int targetPlayer)
        {
            if (playerIndex != State.CurrentPlayer)
                throw new InvalidOperationException("You are not an active player");

            if (!creature.Instance.AbleToAttack)
                throw new InvalidOperationException("Creature is unable to attack");

            State.Players[targetPlayer].Health -= creature.Instance.Power;
            creature.Instance.AbleToAttack = false;
        }

        public void CheckCreatureZeroHealthSBA(GameObject<Creature> creature)
        {
            if (creature.Instance.RealHealth <= 0)
            {
                var owner = State.Players[creature.Owner];
                owner.Creatures.Remove(creature);
                owner.Discard.Add(MorphGameObject(creature, x => x.Prototype as ICard));
            }
        }

        public void Attack(int playerIndex, GameObject<Creature> creature, GameObject<Creature> targetCreature)
        {
            if (playerIndex != State.CurrentPlayer)
                throw new InvalidOperationException("You are not an active player");

            if (!creature.Instance.AbleToAttack)
                throw new InvalidOperationException("Creature is unable to attack");

            targetCreature.Instance.Damage += creature.Instance.Power;
            creature.Instance.Damage += targetCreature.Instance.Power;

            CheckCreatureZeroHealthSBA(creature);
            CheckCreatureZeroHealthSBA(targetCreature);

            creature.Instance.AbleToAttack = false;
        }
    }
}
