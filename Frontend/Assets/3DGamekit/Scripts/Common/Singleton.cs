using System;

public class Singleton<T> where T : class
{
    static public T Instance
    {
        get
        {
            return instance;
        }
    }

    private static readonly T instance = (T)Activator.CreateInstance(typeof(T), true);
}
