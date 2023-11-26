using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class CollisionSubject : MonoBehaviour
    {
        private Interfaces.ICollisionObserver observer;
        public void Initialize(Interfaces.ICollisionObserver newObserver)
        {
            observer = newObserver;
        }
        private void OnCollisionStay(Collision collision)
        {
            observer.Notify(collision);
        }
    }
}

