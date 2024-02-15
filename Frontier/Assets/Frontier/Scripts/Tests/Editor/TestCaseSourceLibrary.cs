using System;
using Random = UnityEngine.Random;

namespace Frontier.Editor.Tests
{
    public class TestCaseSourceLibrary
    {
        /// <summary>
        /// 음수 랜덤 테스트 케이스 생성
        /// </summary>
        /// <returns>[float.MinValue ~ 0] 범위 사이의 랜덤 값</returns>
        public static float[] RandomNegativeFloats() => RandomFloats(float.MinValue, 0);

        /// <summary>
        /// 양수 랜덤 테스트 케이스 생성
        /// </summary>
        /// <returns>[0 ~ float.MaxValue] 범위 사이의 랜덤 값</returns>
        public static float[] RandomPositiveFloats() => RandomFloats(0, float.MaxValue);

        /// <summary>
        /// 랜덤 테스트 케이스 생성
        /// </summary>
        /// <returns>[float.MinValue ~ float.MaxValue] 범위 사이의 랜덤 값</returns>
        public static float[] RandomAllFloats() => RandomFloats(float.MinValue, float.MaxValue);

        public static float[] RandomFloats(float min, float max)
        {
            float[] values = new float[100];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = Random.Range(min, max);
            }

            return values;
        }

        /// <summary>
        /// 고정 테스트 케이스 생성
        /// </summary>
        /// <returns></returns>
        public static float[] FixedFloats() => new float[]
        {
            float.MinValue, float.MinValue * 0.5f, 0, float.MaxValue * 0.5f, float.MaxValue
        };
    }
}
