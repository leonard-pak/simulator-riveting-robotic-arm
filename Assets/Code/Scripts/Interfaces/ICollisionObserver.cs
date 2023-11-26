using UnityEngine;

namespace SimulatorRivetingRoboticArm.Interfaces
{
    public interface ICollisionObserver
    {
        public void Notify(Collision collision);
    }
}

