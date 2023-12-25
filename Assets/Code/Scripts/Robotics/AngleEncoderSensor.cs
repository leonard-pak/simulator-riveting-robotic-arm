using SimulatorRivetingRoboticArm.Entity;
using UnityEngine;

namespace SimulatorRivetingRoboticArm.Robotics
{
    public class AngleEncoderSensor : MonoBehaviour
    {
        [SerializeField] private bool isInverse = false;
        [SerializeField] private bool isMultiTurn = false;
        [SerializeField] private Axis axis = Axis.X;
        private Quaternion zeroRotation;

        // For multiturn
        private int turnCnt = 0;
        private float prevAxisRotation = 0;
        public float Value
        {
            get
            {
                var curAxisRotation = GetAxisValue((Quaternion.Inverse(zeroRotation) * transform.localRotation).eulerAngles);

                if (isMultiTurn)
                {
                    curAxisRotation = MultiTurnValue(curAxisRotation);
                }
                else
                {
                    curAxisRotation = HalfTurnValue(curAxisRotation);
                }

                curAxisRotation *= (isInverse ? -1 : 1);
                return curAxisRotation;
            }
        }

        public float NormValue
        {
            get
            {
                if (Value > 0)
                {
                    return Value / UpperLimit;
                }
                else
                {
                    return -1 * Value / LowerLimit;
                }
            }
        }

        public float UpperLimit { set; private get; }
        public float LowerLimit { set; private get; }


        private float HalfTurnValue(float value)
        {
            if (value > 180 && value < 360)
            {
                value -= 360;
            }
            return value;
        }

        private float MultiTurnValue(float value)
        {
            if (value < 45 && prevAxisRotation > 315)
            {
                ++turnCnt;
            }
            if (value > 315 && prevAxisRotation < 45)
            {
                --turnCnt;
            }
            prevAxisRotation = value;
            return value + turnCnt * 360;
        }


        public void ResetSensor()
        {
            zeroRotation = transform.localRotation;

            if (isMultiTurn)
            {
                turnCnt = 0;
                prevAxisRotation = 0;
            }
        }

        private float GetAxisValue(Vector3 vect)
        {
            return axis switch
            {
                Axis.X => vect.x,
                Axis.Y => vect.y,
                Axis.Z => vect.z,
                _ => 0f
            };
        }

        private void Start()
        {
            ResetSensor();
        }
    }
}