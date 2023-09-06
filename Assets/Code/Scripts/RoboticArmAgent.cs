using SimulatorRivetingRoboticArm.Robotics;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.ML
{
    public class RoboticArmAgent : Agent
    {
        [SerializeField] private FuselageBuilder fuselageBuilder;
        private Matrix2D fuselageMtx;
        private int[] targerIdx;

        [SerializeField] private RoboticArmController controller;
        protected override void Awake()
        {
            base.Awake();
            fuselageMtx = new();
            int xDim = fuselageBuilder.MtxDimX;
            int yDim = fuselageBuilder.MtxDimY;
            for (int y = 0; y < yDim; ++y)
            {
                fuselageMtx.Add(new List<bool>());
                for (int x = 0; x < xDim; ++x)
                {
                    fuselageMtx[y].Add(false);
                }
            }
            targerIdx = new int[2];
        }
        public override void OnEpisodeBegin()
        {
            fuselageBuilder.Crush();
            targerIdx[0] = Random.Range(0, fuselageMtx.Count);
            targerIdx[1] = Random.Range(0, fuselageMtx[0].Count);
            fuselageMtx[targerIdx[0]][targerIdx[1]] = true;

            controller.ResetJoints();
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            for (int i = 0; i < actions.ContinuousActions.Length; ++i)
            {
                controller.RotateJoint(i, actions.ContinuousActions[i], true);
            }

        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var actions = actionsOut.ContinuousActions;
            actions[0] = Input.GetAxis("Horizontal");
            actions[1] = Input.GetAxis("Vertical");
        }

        public void CollisionNotify(Collision collision)
        {

        }
    }
}
