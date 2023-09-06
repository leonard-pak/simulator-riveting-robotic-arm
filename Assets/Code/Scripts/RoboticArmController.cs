using System.Collections.Generic;
using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class RoboticArmController : MonoBehaviour
    {
        [SerializeField] private float velocityLimit = 5; // Units: degree/s
        [SerializeField] private float forceLimit = 10000;
        [SerializeField] private float stiffness = 100000;
        [SerializeField] private float damping = 10000;

        private List<JointController> jointControllers = new();
        private void Awake()
        {
            var tmpBodies = GetComponentsInChildren<ArticulationBody>();
            int defDyanmicVal = 10;
            foreach (ArticulationBody joint in tmpBodies)
            {
                if (joint.isRoot)
                {
                    continue;
                }
                joint.jointFriction = defDyanmicVal;
                joint.angularDamping = defDyanmicVal;
                ArticulationDrive currentDrive = joint.xDrive;
                currentDrive.forceLimit = forceLimit;
                currentDrive.stiffness = stiffness;
                currentDrive.damping = damping;
                joint.xDrive = currentDrive;
                jointControllers.Add(joint.gameObject.AddComponent<JointController>());
            }
        }
        public void RotateJoint(int jointIndex, float target, bool normalized = false)
        {
            var scale = (normalized) ?
                        velocityLimit :
                        1;
            jointControllers[jointIndex].SetSpeed(scale * target);
        }
        public void ResetJoints()
        {
            foreach (var joint in jointControllers)
            {
                joint.ResetJoint();
            }
        }
    }
}

