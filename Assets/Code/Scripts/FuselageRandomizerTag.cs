using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace SimulatorRivetingRoboticArm.Perception
{
    using Matrix2D = List<List<bool>>;

    public class FuselageRandomizerTag : RandomizerTag
    {
        /**
         * Base block must be square
         */
        [SerializeField] protected GameObject blockType1 = null;
        [SerializeField] protected GameObject blockType2 = null;
        [SerializeField] protected double R = 0f;
        [SerializeField] protected double sideSize = 0f;

        [SerializeField] protected float xMax = 0f;
        [SerializeField, Range(0f, 360f)] protected float angleMax = 0f;

        protected double alpha = 0f;
        protected double beta = 0f;

        protected Vector3 angleStep = Vector3.zero;
        protected Vector3 angle0 = Vector3.zero;

        public int MtxDimX => (int)(xMax / sideSize);
        public int MtxDimY => (int)((math.PI * R * angleMax / 180) / sideSize);
        /**
         * Functions for iterating over plane projections
         * iter - current iteration, start at 0
         * returns the increment to be made at the current step 
         */
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

        public void BuildFuselage(Matrix2D binaryMtx)
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

        public void DestroyFuselage()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    [Serializable]
    [AddRandomizerMenu("Fuselage Randomizer")]
    public class FuselageRandomizer : Randomizer
    {
        public BooleanParameter withHole = new();
        protected override void OnIterationStart()
        {
            var tags = tagManager.Query<FuselageRandomizerTag>();
            foreach (var tag in tags)
            {
                Matrix2D binaryMtx = new();
                int xDim = tag.MtxDimX;
                int yDim = tag.MtxDimY;
                for (int y = 0; y < yDim; ++y)
                {
                    binaryMtx.Add(new List<bool>());
                    for (int x = 0; x < xDim; ++x)
                    {
                        binaryMtx[y].Add(withHole.Sample());
                    }
                }
                tag.BuildFuselage(binaryMtx);
            }
        }
        protected override void OnIterationEnd()
        {
            var tags = tagManager.Query<FuselageRandomizerTag>();
            foreach (var tag in tags)
            {
               tag.DestroyFuselage();
            }
        }
    }
}
