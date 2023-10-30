using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.Perception
{
    [RequireComponent(typeof(FuselageBuilder))]
    public class FuselageRandomizerTag : RandomizerTag
    {
        private FuselageBuilder builder = null;
        private void Awake()
        {
            builder = gameObject.GetComponent<FuselageBuilder>();
        }
        public void BuildFuselage(Matrix2D m) => builder.Build(m);
        public void DestroyFuselage() => builder.Crush();
        public int MtxDimX => builder.CountForegroundBlocksX;
        public int MtxDimY => builder.CountForegroundBlocksY;

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
