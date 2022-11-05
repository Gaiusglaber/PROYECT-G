using System;
using System.Collections.Generic;

namespace ProyectG.Toolbox.Pooling
{
    public class ObjectPool<T> where T : class
    {
        #region EXPOSED_FIELDS
        public int maxPoolSize;
        #endregion

        #region PRIVATE_FIELDS
        private Queue<T> pool = new Queue<T>();

        private Func<T> OnGenerateObject = null;
        private Action<T> OnGetObject = null;
        private Action<T> OnReleaseObject = null;
        #endregion

        #region CONSTRUCTOR
        public ObjectPool(Func<T> onGenerateAction, Action<T> onGetObject, Action<T> onReleaseObject, int maxPoolSize)
        {
            this.maxPoolSize = maxPoolSize;

            OnGetObject = onGetObject;
            OnReleaseObject = onReleaseObject;
            OnGenerateObject = onGenerateAction;
        }
        #endregion

        #region PUBLIC_METHODS
        public T Get()
        {
            T obj = null; 

            if(pool.Count < maxPoolSize)
            {
                if(pool.Count == 0)
                {
                    obj = OnGenerateObject?.Invoke();
                    pool.Enqueue(obj);
                    OnGetObject?.Invoke(obj);
                    return obj;
                }
            }

            obj = pool.Dequeue();
            OnGetObject?.Invoke(obj);
            return obj;
        }

        public void Release(T obj)
        {
            if (pool.Count < maxPoolSize)
            {
                pool.Enqueue(obj);
            }
            
            OnReleaseObject?.Invoke(obj);
        }
        #endregion
    }
}
