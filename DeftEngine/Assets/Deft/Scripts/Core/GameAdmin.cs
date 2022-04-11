using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deft
{
    public class GameAdmin : MonoBehaviour
    {
        private List<Manager> managerList;
        private Dictionary<Type, List<Manager>> managerLookup;

        void Awake()
        {
            managerList = new List<Manager>();
            managerLookup = new Dictionary<Type, List<Manager>>();
        }

        void Start() => NotifyManagersStart();
        void Update() => NotifyManagersUpdate();
        void FixedUpdate() => NotifyManagersFixedUpdate(); // TODO: Take control of FixedUpdate and Delta Time.
        void LateUpdate() => NotifyManagersLateUpdate();

        public void NotifyManagersAwake()
        {
            foreach (var manager in managerList)
                manager.OnAwake();
        }

        public void NotifyManagersStart()
        {
            foreach (var manager in managerList)
                manager.OnStart();
        }

        public void NotifyManagersUpdate()
        {
            foreach (var manager in managerList)
                manager.OnUpdate();
        }

        public void NotifyManagersFixedUpdate()
        {
            foreach (var manager in managerList)
                manager.OnFixedUpdate();
        }

        public void NotifyManagersLateUpdate()
        {
            foreach (var manager in managerList)
                manager.OnLateUpdate();
        }

        public T AddManager<T>(string name) where T : Manager
        {
            Type t = typeof(T);
            var newManager = new GameObject().AddComponent<T>();

            newManager.name = name;
            newManager.admin = this;
            newManager.transform.parent = transform;

            List<Manager> managerLookupEntry;
            if (!managerLookup.TryGetValue(t, out managerLookupEntry))
            {
                managerLookupEntry = new List<Manager>();
                managerLookup.Add(t, managerLookupEntry);
            }

            managerList.Add(newManager);
            managerLookupEntry.Add(newManager);

            return newManager;
        }

        public T GetManager<T>() where T : Manager
        {
            if (managerLookup.TryGetValue(typeof(T), out List<Manager> managerLookupEntry))
                return managerLookupEntry[0] as T;
            return null;
        }

        public T GetManager<T>(string name) where T : Manager
        {
            if (managerLookup.TryGetValue(typeof(T), out List<Manager> managerLookupEntry))
            {
                foreach (var manager in managerLookupEntry)
                {
                    if (manager.name == name)
                        return manager as T;
                }

                return null;
            }

            return null;
        }
    }
}