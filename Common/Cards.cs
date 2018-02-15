using System;
using System.Collections.Generic;
namespace Common
{
    public interface ICard
    {
        int CardId { get; }
        int CastingCost { get; }
    }

    public interface ICreatureCard : ICard
    {
        int Power { get; }
        int Toughness { get; }
    }

    public interface ISpellCard : ICard
    {
        IList<TargetKind> TargetKinds { get; }
    }

    public enum TargetKind
    {
        Creature,
        Player
    }
}
