using System;
using System.Collections.Generic;
using Bogus;
using StackExchange.Redis;

namespace Yakari.Demo.Konsole
{
    public class DemoHelper : IDemoHelper
    {
        private const int Million = 1000000;

        private static byte[] _bytes;

        private static readonly object InitializationLock = new object();
        public byte[] GetBytes()
        {
            if (_bytes != null) return _bytes;

            lock (InitializationLock)
            {
                var b = new byte[1];
                var rnd = new Random();
                _bytes = new byte[Million];
                for (int i = 0; i < Million; i++)
                {
                    rnd.NextBytes(b);
                    _bytes[i] = b[0];
                }
            }
            return _bytes;
        }

        private Faker<DemoObject> _demoFaker;
        public List<DemoObject> GenerateDemoObjects(int count)
        {
            SetupDemoFaker();
            var list = new List<DemoObject>(_demoFaker.Generate(count));
            return list;
        }

        public List<Person> GeneratePersons(int count)
        {
            var list = new List<Person>(count);
            for (int i = 0; i < count; i++)
            {
                var p = new Person();
                list.Add(p);
            }
            return list;
        }

        private void SetupDemoFaker()
        {
            if (_demoFaker != null) return;
            _demoFaker = new Faker<DemoObject>()
                .RuleFor(f => f.Id, () => Guid.NewGuid())
                .RuleFor(f => f.Name, f => f.Name.FindName())
                .RuleFor(f => f.Dead, f => f.PickRandom<bool>())
                .RuleFor(f => f.CreatedAt, f=> f.Date.Past(50))
                .RuleFor(f => f.Count, f => f.PickRandom<long>());
        }
    }
}