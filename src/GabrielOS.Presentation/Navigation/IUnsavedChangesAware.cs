namespace GabrielOS.Presentation.Navigation;

public interface IUnsavedChangesAware
{
    bool HasUnsavedChanges { get; }
}
