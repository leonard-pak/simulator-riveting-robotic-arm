using SimulatorRivetingRoboticArm.Entity;
using SimulatorRivetingRoboticArm.Fuselage;
using SimulatorRivetingRoboticArm.Interfaces;

using System;
using System.Collections.Generic;
using UnityEngine;

using Matrix2D = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;

namespace SimulatorRivetingRoboticArm.Testing
{
    [RequireComponent(typeof(ZoneFuselageBuilder))]
    [RequireComponent(typeof(FullFuselageBuilder))]
    public class TestFuselageBuilder : MonoBehaviour
    {
        [Header("Disable all another components,that are using builders")]
        [SerializeField] private FuselageType fuselageType;
        private IFuselageBuilder fuselageBuilder = null;

        private float lastTestTime = 0f;
        [SerializeField, Range(0.1f, 10f)] private float testingPeriod = 1f;

        [SerializeField] private bool testWthMatrix;
        [SerializeField] private bool testWithHole;

        private IEnumerator<bool> testEnumerator;
        private List<Matrix2D> testMatrix;

        private void Awake()
        {
            switch (fuselageType)
            {
                case FuselageType.ZONE:
                    fuselageBuilder = GetComponent<ZoneFuselageBuilder>();
                    break;
                case FuselageType.FULL:
                    fuselageBuilder = GetComponent<FullFuselageBuilder>();
                    break;
            }

            testEnumerator = Testing().GetEnumerator();

            testMatrix = new List<Matrix2D>()
        {
            GetChessMtx(true),
            GetChessMtx(false),
            GetVertLinesMtx(true),
            GetVertLinesMtx(false),
            GetHorLinesMtx(true),
            GetHorLinesMtx(false),
        };
            for (
                    int shift = -fuselageBuilder.CountHoleBlocksX + 1;
                    shift < fuselageBuilder.CountHoleBlocksY;
                    ++shift
                )
            {
                testMatrix.Add(GetDiagMtx(shift));
            }
        }
        void Update()
        {
            if (Time.time - lastTestTime < testingPeriod) return;
            lastTestTime = Time.time;
            if (!testEnumerator.MoveNext())
            {
#if UNITY_EDITOR
                Debug.Log("Stop by Test Component");
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }
        private IEnumerable<bool> Testing()
        {
            if (testWthMatrix)
            {
                foreach (var mtx in testMatrix)
                {
                    fuselageBuilder.Build(mtx);
                    yield return true;
                    fuselageBuilder.Crush();
                }
            }
            if (testWithHole)
            {
                for (int y = 0; y < fuselageBuilder.CountHoleBlocksY; ++y)
                {
                    for (int x = 0; x < fuselageBuilder.CountHoleBlocksX; ++x)
                    {
                        fuselageBuilder.Build(x, y);
                        yield return true;
                        fuselageBuilder.Crush();
                    }
                }
            }
            Debug.Log("Finish Fuselage Testing");
            yield break;
        }
        private Matrix2D GetDiagMtx(int shift)
        {
            Matrix2D mtx = new();
            for (int y = 0; y < fuselageBuilder.CountHoleBlocksY; ++y)
            {
                List<bool> row = new();
                for (int x = 0; x < fuselageBuilder.CountHoleBlocksX; ++x)
                {
                    row.Add(x + shift == y);
                }
                mtx.Add(row);
            }
            return mtx;
        }
        private Matrix2D GetChessMtx(bool isFirstFalse)
        {
            Matrix2D mtx = new();
            for (int y = 0; y < fuselageBuilder.CountHoleBlocksY; ++y)
            {
                List<bool> row = new();
                for (int x = 0; x < fuselageBuilder.CountHoleBlocksX; ++x)
                {
                    var isXEven = x % 2 == 0;
                    var isYEven = y % 2 == 0;
                    row.Add((isXEven == isYEven) ^ isFirstFalse);
                }
                mtx.Add(row);
            }
            return mtx;
        }
        private Matrix2D GetVertLinesMtx(bool isFirstFalse)
        {
            Matrix2D mtx = new();
            for (int y = 0; y < fuselageBuilder.CountHoleBlocksY; ++y)
            {
                List<bool> row = new();
                for (int x = 0; x < fuselageBuilder.CountHoleBlocksX; ++x)
                {
                    var isXEven = x % 2 == 0;
                    row.Add(isXEven ^ isFirstFalse);
                }
                mtx.Add(row);
            }
            return mtx;
        }
        private Matrix2D GetHorLinesMtx(bool isFirstFalse)
        {
            Matrix2D mtx = new();
            for (int y = 0; y < fuselageBuilder.CountHoleBlocksY; ++y)
            {
                List<bool> row = new();
                for (int x = 0; x < fuselageBuilder.CountHoleBlocksX; ++x)
                {
                    var isYEven = y % 2 == 0;
                    row.Add(isYEven ^ isFirstFalse);
                }
                mtx.Add(row);
            }
            return mtx;
        }
    }


}

