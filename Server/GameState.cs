using System;
using System.Collections.Generic;
using Common;
namespace Server
{
    public class PlayerState
    {
        public List<GameObject<ICard>> Deck;
        public List<GameObject<ICard>> Discard;
        public List<GameObject<ICard>> Hand;

        public List<GameObject<Creature>> Creatures;

        public int Health;
        public int Energy;
        public int MaxEnergy;

        public PlayerState(int startingHealth)
        {
            Deck = new List<GameObject<ICard>>();
            Discard = new List<GameObject<ICard>>();
            Hand = new List<GameObject<ICard>>();
            Creatures = new List<GameObject<Creature>>();
            Health = startingHealth;
            Energy = 0;
            MaxEnergy = 0;
        }
    }

    public class GameState
    {
        public List<PlayerState> Players;
        public int CurrentPlayer;

        public GameState(int playerCount, int startingHealth)
        {
            CurrentPlayer = 0;
            Players = new List<PlayerState>();
            for (int i = 0; i < playerCount; i++)
                Players.Add(new PlayerState(startingHealth));
        }
    }
}
