using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class JointController : MonoBehaviour
    {
        private ArticulationBody joint;
        private float speed = 0f;

        public float SetSpeed(float newSpeed) => speed = newSpeed;

        private void Awake()
        {
            joint = GetComponent<ArticulationBody>();
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

            var drive = joint.xDrive;
            drive.target = 0;
            joint.xDrive = drive;
        }
    }

}
