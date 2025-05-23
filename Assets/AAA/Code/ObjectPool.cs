using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
    private readonly Queue<T> _pool = new();
    private readonly Func<T> _factoryMethod;

    public ObjectPool(Func<T> factoryMethod)
    {
        _factoryMethod = factoryMethod;
    }

    public T Get()
    {
        return _pool.Count > 0 ? _pool.Dequeue() : _factoryMethod();
    }

    public void Set(T obj)
    {
        _pool.Enqueue(obj);
    }
}