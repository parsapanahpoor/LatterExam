namespace DependencyInjectionDemo.Interfaces;

public interface IRepository
{
    Guid InstanceId { get; }
    string GetData();
}
