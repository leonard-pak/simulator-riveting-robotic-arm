using SimulatorRivetingRoboticArm.Entity;
using SimulatorRivetingRoboticArm.Plane;

using System;
using Unity.Mathematics;
using UnityEngine;

using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.Fuselage
{
    public class ZoneFuselageBuilder : MonoBehaviour, Interfaces.IFuselageBuilder
    {
        [SerializeField] private PlaneType type;

        [SerializeField] private GameObject blockTrue = null;   //  must be square
        [SerializeField] private GameObject blockFalse = null;  //  must has same dimension
        [SerializeField] private float R = 0f;
        [SerializeField] private float sideSize = 0f;

        [SerializeField] private float xRange = 0f;
        [SerializeField, Range(0f, 360f)] private float angleRange = 0f;

        private Interfaces.IPlainCalcutator plane = null;

        public int CountHoleBlocksX => (int)(xRange / sideSize);
        public int CountHoleBlocksY => (int)((math.PI * R * angleRange / 180) / sideSize);

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
            var upperLimit = binaryMtx.Count / 2;
            var lowerLimit = binaryMtx.Count / 2;
            var upperCorrection = 0;
            var lowerCorrection = -1;
            if (binaryMtx.Count % 2 != 0)
            {
                ++upperLimit;
                upperCorrection = -1;
            }

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

                for (int j = 0; j < binaryMtx[i].Count; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    var block = Instantiate(
                        (binaryMtx[i + upperLimit + upperCorrection][j]) ? blockTrue : blockFalse,
                        transform, false
                    );
                    block.transform.SetLocalPositionAndRotation(
                        new Vector3(x, y, z),
                        rotation
                    );
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

                for (int j = 0; j < binaryMtx[i].Count; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    var block = Instantiate(
                        (binaryMtx[lowerLimit - i + lowerCorrection][j]) ? blockTrue : blockFalse,
                        transform, false
                    );
                    block.transform.SetLocalPositionAndRotation(
                        new Vector3(x, y, z),
                        rotation
                    );
                }
            }
            return;
        }
        public GameObject Build(int holeX, int holeY)
        {
            var upperLimit = CountHoleBlocksY / 2;
            var lowerLimit = CountHoleBlocksY / 2;
            var upperCorrection = 0;
            var lowerCorrection = -1;
            if (CountHoleBlocksY % 2 != 0)
            {
                ++upperLimit;
                ++upperCorrection;
            }
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

                for (int j = 0; j < CountHoleBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;
                    if (j == holeX && i == holeY - upperLimit + upperCorrection)
                    {
                        block = Instantiate(blockTrue, transform, false);
                        hole = block;
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }

                    block.transform.SetLocalPositionAndRotation(
                        new Vector3(x, y, z),
                        rotation
                    );
                }
            }
            rotation = blockTrue.transform.rotation;
            y = 0;
            z = R;
            // Up
            for (int i = 0; i < lowerLimit; ++i)
            {
                y -= plane.YPlaneIterationInc(i);
                z += plane.ZPlaneIterationInc(i);
                rotation.eulerAngles -= plane.AngleIterationInc(i);
                var x = -xRange / 2;

                for (int j = 0; j < CountHoleBlocksX; ++j)
                {
                    x += plane.XPlaneIterationInc(j);
                    GameObject block;

                    if (j == holeX && i == lowerLimit - holeY + lowerCorrection)
                    {
                        block = Instantiate(blockTrue, transform, false);
                        hole = block;
                    }
                    else
                    {
                        block = Instantiate(blockFalse, transform, false);
                    }

                    block.transform.SetLocalPositionAndRotation(
                        new Vector3(x, y, z),
                        rotation
                    );

                }
            }

            if (hole == null)
            {
                Debug.LogWarning("Can not find hole at x: " + holeX + " y: " + holeY + " count: " + CountHoleBlocksY);
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