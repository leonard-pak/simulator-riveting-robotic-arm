using UnityEngine;

namespace SimulatorRivetingRoboticArm
{
    public class CollisionSubject : MonoBehaviour
    {
        private ICollisionObserver observer;
        public void Initialize(ICollisionObserver newObserver)
        {
            observer = newObserver;
        }
        private void OnCollisionEnter(Collision collision)
        {
            observer.Notify(collision);
        }
    }
}

