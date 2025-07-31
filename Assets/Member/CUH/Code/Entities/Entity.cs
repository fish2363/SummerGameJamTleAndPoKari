using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Member.CUH.Code.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        protected Dictionary<Type, IEntityComponent> _components;
    
        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            GetComponentsInChildren<IEntityComponent>(true).ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        
            InitComponents();
            AfterInitialize();
        }

        protected virtual void InitComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        protected virtual void AfterInitialize()
        {
            _components.Values.ToList().ForEach(component =>
            {
                if (component is IAfterInitable afterInitable)
                {
                    afterInitable.AfterInitialize();
                }
            });
        }
    
        public T GetCompo<T>(bool isdDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
            {
                return (T)component;
            }

            if (isdDerived == false) return default(T);

            Type findType = _components.Keys.FirstOrDefault(t => t.IsSubclassOf(typeof(T)));
            if(findType != null)
                return (T)_components[findType];

            return default(T);
        }

    }
}
