using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    [RequireComponent(typeof(PersistentObject))]
    public abstract class Enemy : MonoBehaviour
    {
        public UnityEvent<PersistentObject,EnemyLevelState> EnemyStateChanged;

        protected bool killed = false;
        protected PersistentObject persistentObject;

        protected virtual void Awake()
        {
            persistentObject = GetComponent<PersistentObject>();
        }

        protected virtual void OnDisable()
        {
            var data = new EnemyLevelState(persistentObject.Id);
            if (killed)
            {
                data.active = false;
                data.behaviourEnabled = false;
            }
            else
            {
                data.active = true;
                data.behaviourEnabled = true;
            }
            EnemyStateChanged?.Invoke(persistentObject,data);
        }

        public abstract void SetEnemyState(EnemyLevelState data);
    }
}