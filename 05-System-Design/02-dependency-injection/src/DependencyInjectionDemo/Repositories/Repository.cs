using DependencyInjectionDemo.Interfaces;

namespace DependencyInjectionDemo.Repositories;

public class Repository : IRepository
{
    public Guid InstanceId { get; } = Guid.NewGuid();

    public string GetData() => $"Repository[{InstanceId:N}]";
}
