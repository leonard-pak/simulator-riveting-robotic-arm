using UnityEngine;

namespace SimulatorRivetingRoboticArm.Plane
{
    public class FlatPlaneCalculator : Interfaces.IPlainCalcutator
    {
        private readonly float sideSize = 0f;

        public FlatPlaneCalculator(float sideSize)
        {
            this.sideSize = sideSize;
        }
        public float XPlaneIterationInc(int iter)
        {
            return iter == 0 ? sideSize / 2 : sideSize;
        }
        public float YPlaneIterationInc(int iter)
        {
            return iter == 0 ? sideSize / 2 : sideSize;

        }
        public float ZPlaneIterationInc(int iter) => 0;
        public Vector3 AngleIterationInc(int iter) => Vector3.zero;
    }
}
