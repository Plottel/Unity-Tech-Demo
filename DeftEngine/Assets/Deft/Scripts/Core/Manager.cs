using UnityEngine;

namespace Deft
{
    public abstract class Manager : MonoBehaviour
    {
        public GameAdmin admin;

        public virtual void OnAwake() { }
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnLateUpdate() { }
    }
}
