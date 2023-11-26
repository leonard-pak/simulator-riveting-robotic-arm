using UnityEngine;

namespace SimulatorRivetingRoboticArm.Testing
{
    public class OnlyInEditor : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }

}
