using UnityEngine;

namespace SimulatorRivetingRoboticArm
{
    public interface ICollisionObserver
    {
        public void Notify(Collision collision);
    }
}

