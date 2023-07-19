using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace SimulatorRivetingRoboticArm.Perception
{
    using Matrix2D = List<List<bool>>;

    public class FuselageRandomizerTag : RandomizerTag
    {
        [SerializeField] protected GameObject blockType1 = null;
        [SerializeField] protected GameObject blockType2 = null;
        [SerializeField] protected float xSize = 0f;
        [SerializeField] protected float ySize = 0f;
        [SerializeField, Range(0f, 3f)] protected float xMax = 0f;
        [SerializeField, Range(0f, 3f)] protected float yMax = 0f;

        protected float x0 = 0f;
        protected float y0 = 0f;

        public int MtxDimX => (int)(xMax / xSize);
        public int MtxDimY => (int)(yMax / ySize);

        private void Awake()
        {
            x0 = xSize / 2;
            y0 = ySize / 2;
        }

        public void BuildFuselage(Matrix2D binaryMtx)
        {
            for (int i = 0; i < binaryMtx.Count; ++i)
            {
                float y = y0 + i * ySize;
                if (y > yMax) goto FINISH_BUILD;
                for (int j = 0; j < binaryMtx[i].Count; ++j)
                {
                    float x = x0 + j * xSize;
                    if (x > xMax) goto FINISH_BUILD;
                    Instantiate(
                        (binaryMtx[i][j]) ? blockType1 : blockType2,
                        new Vector3(x, y),
                        blockType1.transform.rotation,
                        transform
                    );
                }
            }
        FINISH_BUILD:
            return;
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
                for (int x = 0; x < xDim; ++x)
                {
                    binaryMtx.Add(new List<bool>());
                    for (int y = 0; y < yDim; ++y)
                    {
                        binaryMtx[x].Add(withHole.Sample());
                    }
                }
                tag.BuildFuselage(binaryMtx);
            }
        }
    }
}
