using SimulatorRivetingRoboticArm.Entity;
using SimulatorRivetingRoboticArm.Plane;

using System;
using Unity.Mathematics;
using UnityEngine;

using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.Fuselage
{
    public class FullFuselageBuilder : MonoBehaviour, Interfaces.IFuselageBuilder
    {
        [SerializeField] private PlaneType type;

        [SerializeField] private GameObject blockTrue = null;   //  must be square
        [SerializeField] private GameObject blockFalse = null;  //  must has same dimension
        [SerializeField] private float R = 0f;
        [SerializeField] private float sideSize = 0f;

        [SerializeField] private float xRange = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleRange = 0f;

        [SerializeField] private float xHoleZoneRange = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleHoleZoneRange = 0f;

        [SerializeField] private float xHoleZoneBegin = 0f;
        [SerializeField, Range(0f, 360f)] private float angleHoleZoneBegin = 0f;

        private Interfaces.IPlainCalcutator plane = null;

        private int CountBlocksX => (int)(xRange / sideSize);
        private int CountBlocksY => (int)((math.PI * R * angleRange / 180) / sideSize);
        public int CountHoleBlocksX => (int)(xHoleZoneRange / sideSize);
        public int CountHoleBlocksY => (int)((math.PI * R * angleHoleZoneRange / 180) / sideSize);
        private int BeginHoleBlockX => (int)(xHoleZoneBegin / sideSize);
        private int BeginHoleBlockY => (int)((math.PI * R * angleHoleZoneBegin / 180) / sideSize);

        private void Awake()
        {
            switch (type)
            {
                case PlaneType.FLAT:
                    plane = new FlatPlaneCalculator(sideSize);
                    break;
                case PlaneType.CYLINDRICAL:
                    plane = new CylindricalPlaneCalculator(R, sideSize);
                    break;
            }
        }
        public void Build(Matrix2D binaryMtx)
        {
            var upperLimit = Convert.ToInt32(CountBlocksY % 2 != 0) + CountBlocksY / 2;
            var lowerLimit = CountBlocksY / 2;

            var rotation = blockTrue.transform.rotation;
            var y = 0f;
            var z = R;
            // Up
            for (int i = 0; i < upperLimit; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = -xRange / 2;

                for (int j = 0; j < CountBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;
                    var iGlobal = i + lowerLimit;
                    if (
                            BeginHoleBlockY <= iGlobal && iGlobal < (BeginHoleBlockY + CountHoleBlocksY) &&
                            BeginHoleBlockX <= j && j < (BeginHoleBlockX + CountHoleBlocksX)
                        )
                    {

                        block = Instantiate(
                            binaryMtx[i + (lowerLimit - BeginHoleBlockY)][j - BeginHoleBlockX]
                                ? blockTrue
                                : blockFalse,
                            transform, false
                        );
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }
                    block.transform.SetLocalPositionAndRotation(new Vector3(x, y, z), rotation);
                }
            }
            rotation = blockTrue.transform.rotation;
            y = 0f;
            z = R;
            // Down
            for (int i = 0; i < lowerLimit; ++i)
            {
                y -= plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles -= plane.AngleIterationInc(i);
                var x = -xRange / 2;

                for (int j = 0; j < CountBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;
                    var iGlobal = lowerLimit - i - 1;
                    if (
                            BeginHoleBlockY <= iGlobal && iGlobal < (BeginHoleBlockY + CountHoleBlocksY) &&
                            BeginHoleBlockX <= j && j < (BeginHoleBlockX + CountHoleBlocksX)
                       )
                    {
                        block = Instantiate(
                            binaryMtx[(lowerLimit - BeginHoleBlockY) - i - 1][j - BeginHoleBlockX]
                                ? blockTrue
                                : blockFalse,
                            transform, false
                        );
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }
                    block.transform.SetLocalPositionAndRotation(new Vector3(x, y, z), rotation);
                }
            }
            return;
        }
        public GameObject Build(int holeLocalX, int holeLocalY)
        {
            holeLocalX += BeginHoleBlockX;
            holeLocalY += BeginHoleBlockY;

            var upperLimit = Convert.ToInt32(CountBlocksY % 2 != 0) + CountBlocksY / 2;
            var lowerLimit = CountBlocksY / 2;

            GameObject hole = null;

            var rotation = blockTrue.transform.rotation;
            var y = 0f;
            var z = R;
            // Up
            for (int i = 0; i < upperLimit; ++i)
            {
                y += plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles += plane.AngleIterationInc(i);
                var x = -xRange / 2;

                for (int j = 0; j < CountBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;

                    if (j == holeLocalX && i + lowerLimit == holeLocalY)
                    {
                        block = Instantiate(blockTrue, transform, false);
                        hole = block;
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }

                    block.transform.SetLocalPositionAndRotation(new Vector3(x, y, z), rotation);
                }
            }
            rotation = blockTrue.transform.rotation;
            y = 0f;
            z = R;
            // Down
            for (int i = 0; i < lowerLimit; ++i)
            {
                y -= plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles -= plane.AngleIterationInc(i);
                var x = -xRange / 2;

                for (int j = 0; j < CountBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;

                    if (j == holeLocalX && i == lowerLimit - holeLocalY - 1)
                    {
                        block = Instantiate(blockTrue, transform, false);
                        hole = block;
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }

                    block.transform.SetLocalPositionAndRotation(new Vector3(x, y, z), rotation);
                }
            }

            if (hole == null)
            {
                Debug.LogWarning("Can not find hole at x: " + holeLocalX + " y: " + holeLocalY + " count: " + CountBlocksY);
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