using System;
using System.Collections.Generic;
using Bogus;
using Microsoft.Extensions.Configuration;

namespace Yakari.Core.Demo
{
    public class DemoHelper : IDemoHelper
    {
        const int Million = 1000000;

        static byte[] _bytes;

        static readonly object InitializationLock = new object();
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

        Faker<DemoObject> _demoFaker;
        public List<DemoObject> GenerateDemoObjects(int count)
        {
            SetupDemoFaker();
            var list = new List<DemoObject>(count);
            for (int i = 0; i < count; i++)
            {
                var dob = _demoFaker.Generate();
                list.Add(dob);
            }
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

        void SetupDemoFaker()
        {
            if (_demoFaker != null) return;
            var bl = new[] {true, false};
            _demoFaker = new Faker<DemoObject>()
                .RuleFor(f => f.Id, f => Guid.NewGuid())
                .RuleFor(f => f.Name, f => f.Name.FindName())
                .RuleFor(f => f.Dead, f => f.PickRandom(bl))
                .RuleFor(f => f.CreatedAt, f=> f.Date.Past(50))
                .RuleFor(f => f.Count, f => f.Random.Number(int.MinValue, int.MaxValue));
        }

        public static IConfigurationRoot Configuration
        {
            get
            {

                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                builder.AddEnvironmentVariables();
                return builder.Build();
            }
        }
    }
}