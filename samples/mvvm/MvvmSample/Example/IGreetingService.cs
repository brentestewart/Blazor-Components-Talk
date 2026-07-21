namespace MvvmSample.Example;

/// <summary>A stand-in for whatever data/service your VM depends on. Injected into the VM.</summary>
public interface IGreetingService
{
    Task<IReadOnlyList<string>> GetPeopleAsync();
}
