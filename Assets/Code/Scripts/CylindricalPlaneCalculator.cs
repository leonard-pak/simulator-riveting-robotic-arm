using Unity.Mathematics;
using UnityEngine;

namespace SimulatorRivetingRoboticArm
{
    public class CylindricalPlaneCalculator
    {
        private readonly double R = 0f;
        private readonly double sideSize = 0f;

        private readonly double alpha = 0f;
        private readonly double beta = 0f;

        private Vector3 angleStep = Vector3.zero;
        private Vector3 angle0 = Vector3.zero;

        public CylindricalPlaneCalculator(double R, double sideSize, Vector3 startAngle)
        {
            alpha = math.acos(1 - (math.pow(sideSize, 2) / (2 * math.pow(R, 2))));
            beta = (math.PI_DBL - alpha) / 2;
            angleStep.Set(0f, 0f, (float)math.degrees(alpha));
            angle0.Set(0f, 0f, (float)math.degrees(alpha / 2));

            this.R = R;
            this.sideSize = sideSize;
            angle0 += startAngle;
        }
        public double XPlaneIterationInc(int iter)
        {
            if (iter == 0) return sideSize / 2;
            return sideSize;
        }
        public double YPlaneIterationInc(int iter)
        {
            if (iter == 0) return sideSize * math.cos(alpha / 2) / 2;
            if (iter == 1) return sideSize * (math.cos(alpha / 2) + math.cos(alpha / 2 + iter * alpha)) / 2;
            return sideSize * (math.cos(alpha / 2 + iter * alpha) + math.cos(alpha / 2 + (iter - 1) * alpha)) / 2;
        }
        public double ZPlaneIterationInc(int iter)
        {
            if (iter == 0) return -R + sideSize * math.sin(alpha / 2) / 2;
            if (iter == 1) return sideSize * (math.sin(alpha / 2) + math.cos(beta - alpha)) / 2;
            return sideSize * (math.cos(beta - iter * alpha) + math.cos(beta - (iter - 1) * alpha)) / 2;
        }
        public Vector3 AngleIterationInc(int iter)
        {
            return (iter == 0) ? angle0 : angleStep;
        }
    }
}
