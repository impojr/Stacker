namespace Assets.Scripts.Core
{
    public class MissResult
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        public MissResult(int x, int y)
        {
            XPos = x;
            YPos = y;
        }
    }
}
