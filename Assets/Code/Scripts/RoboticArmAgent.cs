using SimulatorRivetingRoboticArm.Robotics;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Extensions.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.ML
{
    public class RoboticArmAgent : Agent
    {
        [SerializeField] private FuselageBuilder fuselageBuilder;
        private Matrix2D fuselageMtx;
        private int[] targerIdx;

        [SerializeField] private GameObject roboticArmPrefab;
        private GameObject roboticArm;
        private RoboticArmController controller;

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private PhysicsSensorSettings jointsSensorSetings;

        private bool ignoreCollision = false;

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
        protected override void OnEnable()
        {
            base.OnEnable();
            inputActions.Enable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            inputActions.Disable();
        }
        public override void OnEpisodeBegin()
        {
            InitializeRoboticArm();
            InitializeFuselage();
        }
        private void InitializeRoboticArm()
        {
            if (!roboticArm)
            {
                Destroy(roboticArm);
            }

            roboticArm = Instantiate(roboticArmPrefab, transform, false);
            controller = roboticArm.GetComponent<RoboticArmController>();

            var bodies = roboticArm.GetComponentsInChildren<ArticulationBody>();
            foreach (var joint in bodies)
            {
                if (joint.isRoot)
                {
                    var sensor = joint.gameObject.AddComponent<ArticulationBodySensorComponent>();
                    sensor.RootBody = joint;
                    sensor.Settings = jointsSensorSetings;
                }
                var obs = joint.gameObject.AddComponent<CollisionWithRoboticArmAgent>();
                obs.Initialize(this);
            }
        }
        private void LateUpdate()
        {
            ignoreCollision = false;
        }
        private void InitializeFuselage()
        {
            if (fuselageBuilder.IsBuilt)
            {
                fuselageBuilder.Crush();
            }
            targerIdx[0] = Random.Range(0, fuselageMtx.Count);
            targerIdx[1] = Random.Range(0, fuselageMtx[0].Count);
            fuselageMtx[targerIdx[0]][targerIdx[1]] = true;
            fuselageBuilder.Build(fuselageMtx);
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
            for (int i = 0; i < actionsOut.ContinuousActions.Length; ++i)
            {
                actions[i] = inputActions.FindActionMap("Robotic Arm").FindAction("Link " + (i + 1).ToString()).ReadValue<float>();
            }
        }

        public void CollisionNotify(Collision collision)
        {
            if (ignoreCollision) return;
            ignoreCollision = true;
            if (collision.gameObject.CompareTag("fuselage"))
            {
                SetReward(-1f);
                ResetEpisode();
            }
        }

        private void ResetEpisode()
        {
            Destroy(roboticArm);
            fuselageBuilder.Crush();
            roboticArm = null;
            EndEpisode();
        }

    }

}
