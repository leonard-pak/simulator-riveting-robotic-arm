using UnityEngine;

namespace SimulatorRivetingRoboticArm.ML
{
    public class CollisionWithRoboticArmAgent : MonoBehaviour
    {
        private RoboticArmAgent listener;
        public void Initialize(RoboticArmAgent newListener)
        {
            listener = newListener;
        }
        private void OnCollisionEnter(Collision collision)
        {
            listener.CollisionNotify(collision);
        }
    }
}

