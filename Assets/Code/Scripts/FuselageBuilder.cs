using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace SimulatorRivetingRoboticArm
{
    using Matrix2D = List<List<bool>>;
    public class FuselageBuilder : MonoBehaviour
    {
        /**
        * Base block must be square
        */
        [SerializeField] private GameObject blockType1 = null;
        [SerializeField] private GameObject blockType2 = null;
        [SerializeField] private double R = 0f;
        [SerializeField] private double sideSize = 0f;

        [SerializeField] private float xMax = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleMax = 0f;

        private CylindricalPlaneCalculator plane = null;

        public int MtxDimX => (int)(xMax / sideSize);
        public int MtxDimY => (int)((math.PI * R * angleMax / 180) / sideSize);
        /**
         * Functions for iterating over plane projections
         * iter - current iteration, start at 0
         * returns the increment to be made at the current step 
         */

        private void Awake()
        {
            plane = new CylindricalPlaneCalculator(R, sideSize, blockType1.transform.eulerAngles);
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
                        (binaryMtx[i][j]) ? blockType1 : blockType2,
                        transform, false
                    );
                    block.transform.localPosition = position;
                    block.transform.localRotation = rotation;
                }
            }
            return;
        }

        public void Destroy()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}