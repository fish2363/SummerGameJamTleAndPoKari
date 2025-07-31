using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Member.CUH.Code.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public bool IsDead { get; set; }
        public UnityEvent OnHitEvent;
        public UnityEvent OnDeadEvent;
        
        protected Dictionary<Type, IEntityComponent> _components;

        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
            InitializeComponents();
            AfterInitialize();
        }

        protected virtual void Start()
        {
            
        }


        protected virtual void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }

        protected virtual void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }
        
        protected virtual void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(compo => compo.AfterInitialize());
        }

        public T GetCompo<T>(bool isDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
                return (T)component;
            
            if(isDerived == false) return default(T);
            
            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if(findType != null) 
                return (T)_components[findType];
            
            return default(T);
        }
        public IEntityComponent GetCompo(Type type)
            => _components.GetValueOrDefault(type);

        public void DestroyEntity()
        {
            Destroy(this);
        }
    }
}
