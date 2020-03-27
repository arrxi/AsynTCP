using System;

using System.Threading;

public abstract class Singleton<T> where T : new() {
    private static T _instance;
    private static object _lock = new object();

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                object lockObject = Singleton<T>._lock;
                Monitor.Enter(lockObject);
                try
                {
                    if (_instance == null)
                        _instance = Activator.CreateInstance<T>();
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
            return _instance;
        }
    }

    //public virtual void Init()
    //{
    //}
}