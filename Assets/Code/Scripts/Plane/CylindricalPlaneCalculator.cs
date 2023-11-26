using Unity.Mathematics;
using UnityEngine;

namespace SimulatorRivetingRoboticArm.Plane
{
    public class CylindricalPlaneCalculator : Interfaces.IPlainCalcutator
    {
        private readonly float sideSize = 0f;

        private readonly float alpha = 0f;
        private readonly float beta = 0f;

        private Vector3 angleStep = Vector3.zero;
        private Vector3 angleFirst = Vector3.zero;

        public CylindricalPlaneCalculator(float R, float sideSize)
        {
            alpha = math.acos(1 - (math.pow(sideSize, 2) / (2 * math.pow(R, 2))));
            beta = (math.PI - alpha) / 2;
            angleStep.Set(0f, 0f, math.degrees(alpha));
            angleFirst.Set(0f, 0f, math.degrees(alpha / 2));

            this.sideSize = sideSize;
        }
        public float XPlaneIterationInc(int iter)
        {
            return iter == 0 ? sideSize / 2 : sideSize;
        }
        public float YPlaneIterationInc(int iter)
        {
            if (iter == 0) return sideSize * math.cos(alpha / 2) / 2;
            if (iter == 1) return sideSize * (math.cos(alpha / 2) + math.cos(alpha / 2 + iter * alpha)) / 2;
            return sideSize * (math.cos(alpha / 2 + iter * alpha) + math.cos(alpha / 2 + (iter - 1) * alpha)) / 2;
        }
        public float ZPlaneIterationInc(int iter)
        {
            if (iter == 0) return -sideSize * math.sin(alpha / 2) / 2;
            if (iter == 1) return -sideSize * (math.sin(alpha / 2) + math.cos(beta - alpha)) / 2;
            return -sideSize * (math.cos(beta - iter * alpha) + math.cos(beta - (iter - 1) * alpha)) / 2;
        }
        public Vector3 AngleIterationInc(int iter)
        {
            return (iter == 0) ? angleFirst : angleStep;
        }
    }
}
