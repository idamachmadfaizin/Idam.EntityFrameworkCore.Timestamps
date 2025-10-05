using Bogus;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Faker;

public sealed class BaseEntityFaker<T> : Faker<T>
    where T : BaseEntity
{
    public BaseEntityFaker()
    {
        RuleFor(o => o.Name, f => f.Name.FullName());
        RuleFor(o => o.Description, f => f.Lorem.Sentence());
    }
}