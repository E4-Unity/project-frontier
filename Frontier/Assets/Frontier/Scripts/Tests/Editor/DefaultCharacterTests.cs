using NUnit.Framework;
using UnityEngine;

namespace Frontier.Editor.Tests
{
    // TODO 스탯 추가 시 데미지 계산식 수정 필요
    [TestFixture]
    public class DefaultCharacterTests
    {
        GameObject m_GameObject;
        Character m_Player;
        Character m_Enemy;

        /* Test Setup */
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_GameObject = new GameObject("Player");
            m_Player = m_GameObject.AddComponent<DefaultCharacter>();
            m_Enemy = m_GameObject.AddComponent<DefaultCharacter>();
        }

        [SetUp]
        public void SetUp()
        {
            m_Player.Init();
            m_Enemy.Init();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(m_GameObject);
            m_Player = null;
            m_Enemy = null;
        }

        /* Test */
        /// <summary>
        /// 캐릭터 초기화 시 부활 상태 및 초기 체력 상태를 검사합니다.
        /// </summary>
        [Test]
        public void InitTest()
        {
            // 초기화
            // SetUp 에서 Init 호출됨

            // 캐릭터 선택
            Character character = m_Player;

            // IsDead 검사
            Assert.That(character.IsDead, Is.EqualTo(false));

            // Health 검사
            Assert.That(character.Health, Is.EqualTo(character.MaxHealth));
        }

        /// <summary>
        /// 데미지에 따른 체력 감소 여부 및 사망 상태를 검사합니다.
        /// </summary>
        /// <param name="damage"></param>
        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void DamageTest(float damage)
        {
            // 예상 체력 계산
            Character character = m_Player;
            float expectedHealth = CalculateExpectedHealth(character, damage);

            // 데미지 입력
            character.ApplyDamage(damage);

            // Health 및 IsDead 검사
            CheckIsDeadWithHealth(character, expectedHealth);
        }

        /// <summary>
        /// Player 가 Enemy 를 공격하였을 때 Player 데미지에 따른 Enemy 체력 감소 여부 및 사망 상태를 검사합니다.
        /// </summary>
        [Test]
        public void AttackTest()
        {
            // Enemy 예상 체력 계산
            float expectedHealth = CalculateExpectedHealth(m_Enemy, m_Player.Damage);

            // Player 가 Enemy 를 공격
            m_Player.Attack(m_Enemy);

            // Enemy 의 Health 및 IsDead 값 검사
            CheckIsDeadWithHealth(m_Enemy, expectedHealth);
        }

        /// <summary>
        /// 즉시 사망 기능을 테스트합니다.
        /// </summary>
        [Test]
        public void KillTest()
        {
            // 캐릭터를 즉시 사망 처리합니다.
            Character character = m_Player;
            character.Kill();

            // Health 및 IsDead 값 검사
            CheckIsDeadWithHealth(character, 0f);
        }

        /// <summary>
        /// 사망 상태에서 공격이 불가능한지 검사합니다.
        /// </summary>
        [Test]
        public void AttackOnDeadTest()
        {
            // Player 를 즉시 사망 처리합니다.
            m_Player.Kill();

            // Enemy 체력을 기억합니다.
            float expectedHealth = m_Enemy.Health;

            // Player 가 Enemy 를 공격합니다.
            m_Player.Attack(m_Enemy);

            // Enemy 체력이 변하지 않았는지 검사합니다.
            Assert.That(m_Enemy.Health, Is.EqualTo(expectedHealth));
        }

        /* 메서드 */
        /// <summary>
        /// 데미지를 가한 후 예상 체력을 계산합니다.
        /// </summary>
        /// <param name="target">데미지를 가할 대상</param>
        /// <param name="damage">데미지</param>
        /// <returns>예상 체력</returns>
        float CalculateExpectedHealth(Character target, float damage)
        {
            // 데미지가 음수면 Health 값은 변함이 없고 데미지가 양수면 데미지만큼 Health 가 줄어듭니다.
            // 단, Health 는 음수가 될 수 없습니다.
            float expectedHealth = damage < 0 || Mathf.Approximately(damage, 0)
                ? target.Health
                : Mathf.Max(target.Health - damage, 0);

            return expectedHealth;
        }

        /// <summary>
        /// Health 및 IsDead 값을 검사합니다.
        /// </summary>
        /// <param name="target">검사 대상</param>
        /// <param name="expectedHealth">예상 체력</param>
        void CheckIsDeadWithHealth(Character target, float expectedHealth)
        {
            // Health 검사
            Assert.That(target.Health, Is.EqualTo(expectedHealth));

            // IsDead 검사
            Assert.That(target.IsDead, Is.EqualTo(Mathf.Approximately(target.Health, 0)));
        }
    }
}
