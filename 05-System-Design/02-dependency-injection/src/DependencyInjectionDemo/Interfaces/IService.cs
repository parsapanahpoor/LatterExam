namespace DependencyInjectionDemo.Interfaces;

public interface IService
{
    Guid ServiceInstanceId { get; }
    Guid RepositoryInstanceId { get; }
    string Process();
}
