using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float forceLimit = 1000;
        [SerializeField] private float stiffness = 10000;
        [SerializeField] private float damping = 100;

        private ArticulationBody[] articulationBodies;
        private void Awake()
        {
            articulationBodies = GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in articulationBodies)
            {
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                currentDrive.stiffness = stiffness;
                currentDrive.damping = damping;
                joint.xDrive = currentDrive;
            }
        }

        public void RotateJoint(int jointIndex, float target)
        {
            var jointXDriver = articulationBodies[jointIndex].xDrive;
            jointXDriver.target = target;
            articulationBodies[jointIndex].xDrive = jointXDriver;
        }
    }
}

