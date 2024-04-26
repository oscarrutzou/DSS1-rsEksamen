

namespace BaseProject.ObserverPattern
{
    public interface ISubject
    {
        public void Attach(IObserver observer);
        public void Detach(IObserver observer);
        /// <summary>
        /// Notify the observer/observers when a action happens
        /// </summary>
        public void Notify();
    }
}
