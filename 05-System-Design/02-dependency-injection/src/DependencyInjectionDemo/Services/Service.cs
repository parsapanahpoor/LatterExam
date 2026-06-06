using DependencyInjectionDemo.Interfaces;

namespace DependencyInjectionDemo.Services;

public class Service(IRepository repository) : IService
{
    public Guid ServiceInstanceId { get; } = Guid.NewGuid();
    public Guid RepositoryInstanceId => repository.InstanceId;

    public string Process() =>
        $"Service[{ServiceInstanceId:N}] -> {repository.GetData()}";
}
