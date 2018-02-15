using System;
using Common;

namespace Server
{
    public class Creature : ICreatureCard
    {
        public ICreatureCard Prototype { get; protected set; }

        public virtual int Power => Prototype.Power;
        public virtual int Toughness => Prototype.Toughness;
        public virtual int CastingCost => Prototype.CastingCost;

        public int Damage { get; set; }
        public bool AbleToAttack { get; set; }
        public int Owner { get; set; }

        public int RealHealth => Toughness - Damage;

        public Creature(ICreatureCard prototype, int owner)
        {
            Prototype = prototype;
            Owner = owner;
        }
    }
}
