using UnityEngine;

namespace SimulatorRivetingRoboticArm.Interfaces
{
    public interface IPlainCalcutator
    {
        // NOTE: The functions below don`t return IEnumerable or IEnumerator
        // because we need to implement more logic one each iteration.

        /**
         * Return increment value for X axis at each iteration
         */
        public float XPlaneIterationInc(int iter);
        /**
         * Return increment value for Y axis at each iteration
         */
        public float YPlaneIterationInc(int iter);
        /**
         * Return increment value for Z axis at each iteration
         */
        public float ZPlaneIterationInc(int iter);
        /**
         * Return increment value for angle axis at each iteration
         */
        public Vector3 AngleIterationInc(int iter);
    }
}