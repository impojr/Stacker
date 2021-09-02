using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public interface IStacker
    {
        void ResetGrid();
        void Tick();
        List<MissResult> Place();
        void ResetHeight();
    }
}
