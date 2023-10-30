using System;
using Unity.Mathematics;
using UnityEngine;
using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm
{
    public class FuselageBuilder : MonoBehaviour
    {
        /**
        * Base block must be square
        */
        [SerializeField] private GameObject blockTrue = null;
        [SerializeField] private GameObject blockFalse = null;
        [SerializeField] private double R = 0f;
        [SerializeField] private double sideSize = 0f;

        [SerializeField] private float xRangeForeground = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleRangeForeground = 0f;

        [Header("For build with background")]
        [SerializeField] private float xBeginForeground = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleBeginForeground = 0f;

        [SerializeField] private float xRangeBackground = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleRangeBackground = 0f;


        private CylindricalPlaneCalculator plane = null;

        public int CountForegroundBlocksX => (int)(xRangeForeground / sideSize);
        public int CountForegroundBlocksY => (int)((math.PI * R * angleRangeForeground / 180) / sideSize);

        private int CountBackgroundBlocksX => (int)(xRangeBackground / sideSize);
        private int CountBackgroundBlocksY => (int)((math.PI * R * angleRangeBackground / 180) / sideSize);

        private int NumBeginForegroundBlockX => (int)(xBeginForeground / sideSize);
        private int NumBeginForegroundBlockY => (int)((math.PI * R * angleBeginForeground / 180) / sideSize);
        /**
         * Functions for iterating over plane projections
         * iter - current iteration, start at 0
         * returns the increment to be made at the current step 
         */

        private void Awake()
        {
            plane = new CylindricalPlaneCalculator(R, sideSize, blockTrue.transform.eulerAngles);
        }
        public void Build(Matrix2D binaryMtx)
        {
            var position = new Vector3();
            var rotation = new Quaternion();
            var y = 0d;
            var z = 0d;
            for (int i = 0; i < binaryMtx.Count; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = 0d;

                for (int j = 0; j < binaryMtx[i].Count; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    position.Set((float)x, (float)y, (float)z);
                    var block = Instantiate(
                        (binaryMtx[i][j]) ? blockTrue : blockFalse,
                        transform, false
                    );
                    block.transform.SetLocalPositionAndRotation(position, rotation);
                }
            }
            return;
        }

        public GameObject Build(int holeX, int holeY)
        {
            var position = new Vector3();
            var rotation = new Quaternion();
            var y = 0d;
            var z = 0d;
            GameObject hole = null;
            for (int i = 0; i < CountForegroundBlocksY; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = 0d;

                for (int j = 0; j < CountForegroundBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    position.Set((float)x, (float)y, (float)z);
                    var block = Instantiate(
                        (j == holeX && i == holeY) ? blockTrue : blockFalse,
                        transform, false
                    );
                    block.transform.SetLocalPositionAndRotation(position, rotation);
                    if (j == holeX && i == holeY)
                    {
                        hole = block;
                    }
                }
            }
            return hole;
        }

        public void BuildWithBackground(Matrix2D binaryMtx)
        {
            var position = new Vector3();
            var rotation = new Quaternion();
            var y = 0d;
            var z = 0d;

            for (int i = 0; i < CountBackgroundBlocksY; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = 0d;
                GameObject block;
                for (int j = 0; j < CountBackgroundBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    position.Set((float)x, (float)y, (float)z);
                    if (
                        NumBeginForegroundBlockY <= i && i <= (NumBeginForegroundBlockY + CountForegroundBlocksY)
                        && NumBeginForegroundBlockX <= j && j <= (NumBeginForegroundBlockX + CountForegroundBlocksX)
                        )
                    {
                        block = Instantiate(
                            (binaryMtx[i][j]) ? blockTrue : blockFalse,
                            transform, false
                        );
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }
                    block.transform.SetLocalPositionAndRotation(position, rotation);
                }
            }
            return;
        }


        public GameObject BuildWithBackground(int NumHoleBlockinForefroundX, int NumHoleBlockinForefroundY)
        {
            NumHoleBlockinForefroundX += NumBeginForegroundBlockX;
            NumHoleBlockinForefroundY += NumBeginForegroundBlockY;
            var position = new Vector3();
            var rotation = new Quaternion();
            var y = 0d;
            var z = 0d;
            GameObject hole = null;

            for (int i = 0; i < CountBackgroundBlocksY; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = -0.5 * sideSize * CountBackgroundBlocksX;
                GameObject block;
                for (int j = 0; j < CountBackgroundBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    position.Set((float)x, (float)y, (float)z);
                    if (
                        NumBeginForegroundBlockY <= i && i <= (NumBeginForegroundBlockY + CountForegroundBlocksY)
                        && NumBeginForegroundBlockX <= j && j <= (NumBeginForegroundBlockX + CountForegroundBlocksX)
                        )
                    {
                        block = Instantiate(
                            (j == NumHoleBlockinForefroundX && i == NumHoleBlockinForefroundY) ? blockTrue : blockFalse
                            , transform
                            , false
                        );
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }
                    block.transform.SetLocalPositionAndRotation(position, rotation);
                    if (j == NumHoleBlockinForefroundX && i == NumHoleBlockinForefroundY)
                    {
                        hole = block;
                    }
                }
            }
            return hole;
        }

        public void Crush()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}