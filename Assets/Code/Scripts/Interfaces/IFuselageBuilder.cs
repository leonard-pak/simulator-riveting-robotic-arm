using UnityEngine;

using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.Interfaces
{
    public interface IFuselageBuilder
    {
        /**
         * Number of block in zone of holes on X axis
         */
        int CountHoleBlocksX { get; }
        /**
         * Number of block in zone of holes on Y axis
         */
        int CountHoleBlocksY { get; }

        /**
          * Build fuselage with binaryMtx pattern
          */
        void Build(Matrix2D binaryMtx);
        /**
         * Build fuselage with the hole at [holeX; holeY]
         * Return this hole.
         */
        GameObject Build(int holeX, int holeY);
        /**
         * Destroy all child objects
         */
        void Crush();
    }
}