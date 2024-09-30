namespace HyperCasual.Core
{
    /// <summary>
    ///     Generic game event
    /// </summary>
    public class GameEvent<T> : AbstractGameEvent
    {
        public T m_Data { get; private set; }

        public override void Reset()
        {
        }

        public void Raise(T data)
        {
            m_Data = data;
            base.Raise();
        }
    }
}