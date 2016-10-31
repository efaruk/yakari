using System.Collections.Generic;
using Bogus;

namespace Yakari.Demo
{
    public interface IDemoHelper
    {
        string TribeName { get; set; }
        string MemberName { get; set; }
        List<DemoObject> GenerateDemoObjects(int count);
        List<Person> GeneratePersons(int count);
        byte[] GetBytes();
    }
}