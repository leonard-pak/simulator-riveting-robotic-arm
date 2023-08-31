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

        private double alpha = 0f;
        private double beta = 0f;

        private Vector3 angleStep = Vector3.zero;
        private Vector3 angle0 = Vector3.zero;

        public int MtxDimX => (int)(xMax / sideSize);
        public int MtxDimY => (int)((math.PI * R * angleMax / 180) / sideSize);
        /**
         * Functions for iterating over plane projections
         * iter - current iteration, start at 0
         * returns the increment to be made at the current step 
         */
        private double XPlaneIterationInc(int iter)
        {
            if (iter == 0) return sideSize / 2;
            return sideSize;
        }
        private double YPlaneIterationInc(int iter)
        {
            if (iter == 0) return sideSize * math.cos(alpha / 2) / 2;
            if (iter == 1) return sideSize * (math.cos(alpha / 2) + math.cos(alpha / 2 + iter * alpha)) / 2;
            return sideSize * (math.cos(alpha / 2 + iter * alpha) + math.cos(alpha / 2 + (iter - 1) * alpha)) / 2;
        }
        private double ZPlaneIterationInc(int iter)
        {
            if (iter == 0) return -R + sideSize * math.sin(alpha / 2) / 2;
            if (iter == 1) return sideSize * (math.sin(alpha / 2) + math.cos(beta - alpha)) / 2;
            return sideSize * (math.cos(beta - iter * alpha) + math.cos(beta - (iter - 1) * alpha)) / 2;
        }
        private Vector3 AngleIterationInc(int iter)
        {
            if (iter == 0) return blockType1.transform.rotation.eulerAngles + angle0;
            return angleStep;
        }
        private void Awake()
        {
            alpha = math.acos(1 - (math.pow(sideSize, 2) / (2 * math.pow(R, 2))));
            beta = (math.PI_DBL - alpha) / 2;
            angleStep.Set(0f, 0f, (float)math.degrees(alpha));
            angle0.Set(0f, 0f, (float)math.degrees(alpha / 2));
        }
        public void Build(Matrix2D binaryMtx)
        {
            var position = new Vector3();
            var rotation = new Quaternion();
            var y = 0d;
            var z = 0d;
            for (int i = 0; i < binaryMtx.Count; ++i)
            {
                y += YPlaneIterationInc(i);
                z += ZPlaneIterationInc(i);
                rotation.eulerAngles += AngleIterationInc(i);
                var x = 0d;

                for (int j = 0; j < binaryMtx[i].Count; ++j)
                {
                    x += XPlaneIterationInc(j);
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