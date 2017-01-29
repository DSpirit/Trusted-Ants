using JetBrains.Annotations;

namespace Assets.Scripts.Interfaces
{
    public interface IStateMachine
    {
        void RegisterObserver(IStateObserver observer);
        void RemoveObserver(IStateObserver observer);
        void NotifyObservers();

    }
}