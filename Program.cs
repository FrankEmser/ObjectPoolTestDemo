using System.Collections.Concurrent;
using Microsoft.Extensions.ObjectPool;

namespace ObjectPoolTestDemo
{
    /// In Powershell:
    /// Add-Type -Path ObjectPoolTestDemo.dll
    /// [ObjectPoolTestDemo.TestRun]$global:testRunner = New-Object -TypeName 'ObjectPoolTestDemo.TestRun'
    /// $testRunner.Run()

    public class TestRun
    {
        public void Run()
        {
            var pool = new MyOwnObjectPool<DummyObject>(() => new DummyObject());
        }
    }
    internal class DummyObject
    {
    }    
    internal class MyOwnObjectPool<T> : ObjectPool<T> where T: class, new()
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public MyOwnObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objects = new ConcurrentBag<T>();
        }

        public override T Get() => _objects.TryTake(out T? item) ? item : _objectGenerator();

        public override void Return(T item) => _objects.Add(item);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var testRunner = new TestRun(); 
            testRunner.Run();
            Console.WriteLine("Press the Enter key to exit.");
            Console.ReadLine();
        }


    }

}