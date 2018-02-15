using System;
using Newtonsoft.Json;
namespace Server
{
    [Serializable]
    public abstract class BaseGameObject
    {
        [JsonIgnore]
        public abstract object BaseInstance { get; }

        public virtual int Id { get; protected set; }
        public virtual int Owner { get; protected set; }
    }

    [Serializable]
    public class GameObject<T> : BaseGameObject
    {
        public T Instance { get; private set; }

        [JsonIgnore]
        public override object BaseInstance => Instance;

        public GameObject(T instance, int id, int owner)
        {
            Instance = instance;
            Id = id;
            Owner = owner;
        }
    }
}
