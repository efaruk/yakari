using System.Collections.Generic;
using Bogus;

namespace Yakari.Demo
{
    public interface IDemoHelper
    {
        List<DemoObject> GenerateDemoObjects(int count);
        List<Person> GeneratePersons(int count);
        byte[] GetBytes();
    }
}