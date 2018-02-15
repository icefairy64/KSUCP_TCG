using System;
using System.Collections.Generic;

namespace Server.Messages
{
    [Serializable]
    public class SubmitDeck
    {
        public List<int> CardIds;
    }

    [Serializable]
    public class Cast
    {
        public int CardObjectId;
    }

    [Serializable]
    public class AttackCreature
    {
        public int CreatureObjectId;
        public int TargetCreatureObjectId;
    }

    [Serializable]
    public class AttackPlayer
    {
        public int CreatureObjectId;
        public int TargetPlayerId;
    }
}
