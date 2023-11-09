using AutoMapper;

namespace Entities.Contracts
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
