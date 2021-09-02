namespace Assets.Scripts.Core
{
    public enum State
    {
        Occupied,
        Vacant
    }

    public class Square
    {
        public State State { get; set; }

        public Square()
        {
            State = State.Vacant;
        }
    }
}