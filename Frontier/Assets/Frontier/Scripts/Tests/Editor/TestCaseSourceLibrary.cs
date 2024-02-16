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
        /// 고정 테스트 케이스를 생성합니다.
        /// 랜덤 테스트는 매우 큰 값을 다루기 때문에 상대적으로 작은 값은 오차 범위에 포함되기 때문에 고정 테스트 케이스 역시 필요합니다.
        /// </summary>
        /// <returns></returns>
        public static float[] FixedFloats() => new float[]
        {
            float.MinValue, float.MaxValue, 0,
            1, -1,
            5, -5,
            10, -10,
            50, -50,
            100, -100,
            500, -500,
            1000, -1000,
            0.1f, -0.1f,
            0.5f, -0.5f,
            0.001f, -0.001f,
            0.0005f, -0.0005f
        };
    }
}
