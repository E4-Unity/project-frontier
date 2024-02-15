using NUnit.Framework;
using UnityEngine;

namespace Frontier.Editor.Tests
{
    [TestFixture]
    public class AttributeSetTests
    {
        /* AttributeSet */
        AttributeSet m_HealthAttribute = new AttributeSet();

        float MaxHealth
        {
            get => m_HealthAttribute.MaxHealth;
            set => m_HealthAttribute.MaxHealth = value;
        }

        float Health
        {
            get => m_HealthAttribute.Health;
            set => m_HealthAttribute.Health = value;
        }

        bool IsDead
        {
            get => m_HealthAttribute.IsDead;
            set => m_HealthAttribute.IsDead = value;
        }

        /* Test */
        /// <summary>
        /// 부활 처리 및 체력을 최대치로 설정
        /// </summary>
        [SetUp]
        public void Init()
        {
            IsDead = false;
            MaxHealth = 100;
            Health = MaxHealth;
        }

        /// <summary>
        /// MaxHealth 범위 검사
        /// </summary>
        /// <param name="value">MaxHealth</param>
        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.FixedFloats))]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void SetMaxHealthTest(float value)
        {
            // 1 <= MaxHealth
            float expected = Mathf.Max(1, value);
            MaxHealth = value;
            Assert.That(MaxHealth, Is.EqualTo(expected));
        }

        /// <summary>
        /// Health 범위 및 Health 값에 따른 IsDead 값 검사
        /// </summary>
        /// <param name="ratio">Health / MaxHealth</param>
        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.FixedFloats))]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void SetHealthTest(float ratio)
        {
            // 0 <= Health <= MaxHealth
            float value = MaxHealth * ratio;
            float expectedHealth = Mathf.Clamp(value, 0, MaxHealth);
            Health = value;
            Assert.That(Health, Is.EqualTo(expectedHealth));

            // Health 값에 따른 IsDead 검사 (Health == 0 이면 IsDead = true)
            Assert.That(IsDead, Is.EqualTo(Mathf.Approximately(Health, 0)));
        }

        /// <summary>
        /// IsDead 설정에 따른 Health 값 검사
        /// </summary>
        /// <param name="value">IsDead</param>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetIsDeadTest(bool value)
        {
            // 반대값으로 초기화
            IsDead = !value;

            // 값 설정
            IsDead = value;

            // Health 검사
            float expectedHealth = value ? 0f : 1f;
            Assert.That(Health, Is.EqualTo(expectedHealth));
        }

        /// <summary>
        /// MaxHealth 값이 수정되도 HealthRatio 가 유지되는지 검사 (소수점 여섯 번째 자리까지만 비교)
        /// </summary>
        /// <param name="value">MaxHealth</param>
        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.FixedFloats))]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void HealthRatioTest(float value)
        {
            // TODO ratio 도 랜덤 값으로 여러 번 테스트
            // 초기화
            float expected = 0.2f;
            Health = MaxHealth * expected;
            expected = Health / MaxHealth;

            // MaxHealth 값 수정
            MaxHealth = value;
            float healthRatio = Health / MaxHealth;

            // HealthRatio 유지 여부 확인
            Assert.True(Mathf.Approximately(healthRatio, expected));

            // MaxHealth 값 변화로 인해 사망 처리가 되면 안 됩니다
            Assert.That(IsDead, Is.EqualTo(false));
        }
    }
}
