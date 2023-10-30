using SimulatorRivetingRoboticArm.Robotics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SimulatorRivetingRoboticArm.ML
{
    public class RoboticArmAgent : Agent, ICollisionObserver
    {
        // For fuselage
        [SerializeField] private FuselageBuilder fuselageBuilder;
        private int[] targerIdx;
        // For robotic arm
        [SerializeField] private RoboticArmController controller;
        [SerializeField] private InputActionAsset inputActions;
        // For rewards
        private Transform targetHole = null;
        [SerializeField] private Transform eef = null;
        [SerializeField] private float positionErr = 0.1f; // meters
        [SerializeField] private float angleErr = 0.1f; // degrees
        // For visualization
        [SerializeField] private Material successEpisodeMaterial;
        [SerializeField] private Material failureEpisodeMaterial;
        [SerializeField] private MeshRenderer indicator;
        // For throttle
        private float lastNotify = 0f;
        [SerializeField, Range(0f, 1f)] private float collisionNotifyPeriod = 0.1f;
        public override void Initialize()
        {
            targerIdx = new int[] { 0, 0 };
            var subjects = GetComponentsInChildren<CollisionSubject>();
            foreach (var subject in subjects)
            {
                subject.Initialize(this);
            }
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
            fuselageBuilder.Crush();
            //controller.ResetJoints();

            targerIdx[0] = Random.Range(0, fuselageBuilder.CountForegroundBlocksX);
            targerIdx[1] = Random.Range(0, fuselageBuilder.CountForegroundBlocksY);

            targetHole = fuselageBuilder.BuildWithBackground(targerIdx[0], targerIdx[1]).transform;
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            for (int i = 0; i < actions.ContinuousActions.Length; ++i)
            {
                controller.RotateJoint(i, actions.ContinuousActions[i], true);
            }

            var distance = Vector3.Distance(targetHole.position, eef.position);
            var angle = Vector3.Angle(targetHole.up, eef.up);

            if (distance < positionErr && angle < angleErr)
            {
                SuccessEndEpisode();
            }
            else if (distance < 1f)
            {
                AddReward(1 - distance);
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
        public void Notify(Collision collision)
        {
            if (Time.time - lastNotify < collisionNotifyPeriod) return;
            lastNotify = Time.time;

            if (collision.gameObject.CompareTag("fuselage"))
            {
                SetReward(-1f);
                FailEndEpisode();
            }
        }
        private void SuccessEndEpisode()
        {
            indicator.material = successEpisodeMaterial;
            EndEpisode();
        }
        private void FailEndEpisode()
        {
            indicator.material = failureEpisodeMaterial;
            EndEpisode();
        }
    }
}
