using Bogus;
using Idam.EFTimestamps.Tests.Entities;

namespace Idam.EFTimestamps.Tests.Faker;
public class BaseFaker<T> : Faker<T>
    where T : BaseEntity
{
    public BaseFaker()
    {
        var random = new Randomizer();

        RuleFor(o => o.Name, f => f.Name.FullName());
        if (random.Number() == 1)
        {
            RuleFor(o => o.Description, f => f.Lorem.Sentence());
        }
    }
}
