using System;

public class Singleton<T> where T : class
{
    private static readonly T instance = (T)Activator.CreateInstance(typeof(T), true);
    public static T Instance()
    {
        return instance;
    }
}
