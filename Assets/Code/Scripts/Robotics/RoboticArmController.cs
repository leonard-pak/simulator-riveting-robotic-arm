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
        private readonly List<JointController> jointControllers = new();
        private readonly List<AngleEncoderSensor> jointSensors = new();

        public bool IsJointsLimit
        {
            get
            {
                bool res = false;
                foreach (var joint in jointControllers)
                {
                    res |= joint.IsInLimit;
                }
                return res;
            }
        }

        public List<float> JointPositions(bool normalized = false)
        {

            List<float> res = new();
            foreach (var sensor in jointSensors)
            {
                if (normalized)
                {
                    res.Add(sensor.NormValue);
                }
                else
                {
                    res.Add(sensor.Value);
                }

            }
            return res;

        }

        public void RotateJoint(int jointIndex, float target, bool normalized = false)
        {
            var scale = (normalized) ?
                        velocityLimit :
                        1;
            jointControllers[jointIndex].Speed = scale * target;
        }

        public void ResetJoints()
        {
            foreach (var joint in jointControllers)
            {
                joint.ResetJoint();
            }
            foreach (var sensor in jointSensors)
            {
                sensor.ResetSensor();
            }
        }

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
                jointControllers.Add(joint.gameObject.GetComponent<JointController>());
                var sensor = joint.gameObject.GetComponent<AngleEncoderSensor>();
                sensor.UpperLimit = currentDrive.upperLimit;
                sensor.LowerLimit = currentDrive.lowerLimit;
                jointSensors.Add(sensor);
            }
        }
    }
}

