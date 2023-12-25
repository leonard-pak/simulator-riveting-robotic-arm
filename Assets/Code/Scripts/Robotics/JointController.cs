using SimulatorRivetingRoboticArm.Entity;
using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class JointController : MonoBehaviour
    {
        [SerializeField] private Axis axis;

        private ArticulationBody joint;
        private float speed = 0f;

        private Vector3 defaultPosition;
        private Quaternion defaultRotation;
        private ArticulationDrive defaultDrive;

        public float Speed
        {
            set { this.speed = value; }
        }

        public bool IsInLimit
        {
            get { return joint.xDrive.target == joint.xDrive.upperLimit || joint.xDrive.target == joint.xDrive.lowerLimit; }
        }


        private void Start()
        {
            joint = GetComponent<ArticulationBody>();
            defaultPosition = transform.position;
            defaultRotation = transform.rotation;
            defaultDrive = joint.xDrive;
        }
        private void FixedUpdate()
        {
            if (joint.jointType != ArticulationJointType.FixedJoint)
            {
                ArticulationDrive currentDrive = joint.xDrive;
                float newTargetDelta = Time.fixedDeltaTime * speed;


                if (joint.jointType == ArticulationJointType.RevoluteJoint)
                {
                    if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta + currentDrive.target > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta + currentDrive.target < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target += newTargetDelta;
                        }
                    }
                    else
                    {
                        currentDrive.target += newTargetDelta;
                    }
                }

                else if (joint.jointType == ArticulationJointType.PrismaticJoint)
                {
                    if (joint.linearLockX == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta + currentDrive.target > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta + currentDrive.target < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target += newTargetDelta;
                        }
                    }
                    else
                    {
                        currentDrive.target += newTargetDelta;

                    }
                }
                joint.xDrive = currentDrive;
            }
        }
        public void ResetJoint()
        {
            speed = 0;

            joint.enabled = false;
            joint.transform.SetPositionAndRotation(defaultPosition, defaultRotation);
            joint.velocity = Vector3.zero;
            joint.angularVelocity = Vector3.zero;
            joint.xDrive = defaultDrive;
            joint.enabled = true;
        }

    }

}
