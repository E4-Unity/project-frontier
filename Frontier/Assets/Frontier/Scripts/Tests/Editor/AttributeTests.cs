using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Frontier.Editor.Tests
{
    [TestFixture]
    public class AttributeTests
    {
        /* 테스트 전용 클래스 작성 */
        enum AttributeTypeTest
        {
            AttackDamage,
            Armor
        }

        struct BaseAttributeTest
        {
            public AttributeTypeTest AttributeType;
            public float Value;

            public BaseAttributeTest(AttributeTypeTest attributeType, float value)
            {
                AttributeType = attributeType;
                Value = value;
            }
        }

        class AttributeTest : AttributeBase<AttributeTypeTest>
        {
            public AttributeTest(AttributeTypeTest attributeType) : base(attributeType)
            {
            }
        }

        class AttributeModifierTest : AttributeModifierBase<AttributeTypeTest>
        {
            public AttributeModifierTest(AttributeTypeTest attributeType, ModifierMagnitudeType magnitudeType, float value) : base(attributeType, magnitudeType, value)
            {
            }
        }

        /* 필드 */
        readonly AttributeTest m_AttackDamage = new AttributeTest(AttributeTypeTest.AttackDamage);
        readonly AttributeTest m_Armor = new AttributeTest(AttributeTypeTest.Armor);

        /* Test Setup */
        [SetUp]
        public void SetUp()
        {
            m_AttackDamage.Clear();
            m_Armor.Clear();

            // 기본값 설정
            m_AttackDamage.SetBaseValue(AttributeTypeTest.AttackDamage, 10);
            m_Armor.SetBaseValue(AttributeTypeTest.Armor, 5);
        }

        /* Test */
        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.FixedFloats))]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void SetBaseValueTest(float value)
        {
            /* 초기화 */
            AttributeTest attribute = m_AttackDamage;
            AttributeTypeTest otherAttributeType = AttributeTypeTest.Armor;

            // 다른 Attribute 로 설정이 되었는지 확인
            Assert.True(attribute.AttributeType != otherAttributeType);

            /* 시드 값으로부터 테스트 케이스 생성 */
            float firstValue = value / 10;
            float secondValue = value / 5;
            float thirdValue = value / 3;

            List<BaseAttributeTest> baseAttributes = new List<BaseAttributeTest>()
            {
                new BaseAttributeTest(attribute.AttributeType, firstValue),
                new BaseAttributeTest(otherAttributeType, secondValue),
                new BaseAttributeTest(attribute.AttributeType, thirdValue),
                new BaseAttributeTest(attribute.AttributeType, thirdValue),
                new BaseAttributeTest(otherAttributeType, thirdValue)
            };

            /* 예상 값 계산 */
            float expected = attribute.BaseValue;

            foreach (var baseAttribute in baseAttributes)
            {
                // 동일한 Attribute 가 아니라면 무시
                if(attribute.AttributeType != baseAttribute.AttributeType) continue;

                expected += baseAttribute.Value;
            }

            // 0 <= BaseValue
            expected = Mathf.Max(0, expected);

            /* AddBaseValue */
            foreach (var baseAttribute in baseAttributes)
            {
                // AddBaseValue
                float baseValue = attribute.BaseValue + baseAttribute.Value;
                attribute.SetBaseValue(baseAttribute.AttributeType, baseValue);
            }

            /* 결과 비교 */
            Assert.That(attribute.BaseValue, Is.EqualTo(expected));
            Assert.That(attribute.CurrentValue, Is.EqualTo(attribute.BaseValue));
        }

        [Test]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.FixedFloats))]
        [TestCaseSource(typeof(TestCaseSourceLibrary), nameof(TestCaseSourceLibrary.RandomAllFloats))]
        public void AddModifierTest(float value)
        {
            /* 초기화 */
            AttributeTest attribute = m_AttackDamage;
            AttributeTypeTest otherAttributeType = AttributeTypeTest.Armor;

            // 다른 Attribute 로 설정이 되었는지 확인
            Assert.True(attribute.AttributeType != otherAttributeType);

            /* 시드 값으로부터 테스트 케이스 생성 */
            float firstValue = value / 10;
            float secondValue = value / 5;
            float thirdValue = value / 3;

            List<AttributeModifierTest> attributeModifiers = new List<AttributeModifierTest>()
            {
                new AttributeModifierTest(attribute.AttributeType, ModifierMagnitudeType.Flat, thirdValue),
                new AttributeModifierTest(otherAttributeType, ModifierMagnitudeType.Flat, thirdValue), // 무시
                new AttributeModifierTest(attribute.AttributeType, ModifierMagnitudeType.Percentage, secondValue),
                new AttributeModifierTest(attribute.AttributeType, ModifierMagnitudeType.Flat, secondValue),
                new AttributeModifierTest(attribute.AttributeType, ModifierMagnitudeType.Percentage, firstValue),
                new AttributeModifierTest(otherAttributeType, ModifierMagnitudeType.Percentage, firstValue) // 무시
            };

            AttributeModifierTest modifierToRemove =
                new AttributeModifierTest(attribute.AttributeType, ModifierMagnitudeType.Percentage, secondValue);

            /* 계산 시 임시 변수 사용 여부에 따라 결과가 달라질 수 있기 때문에 반드시 동일한 방식으로 계산해야 합니다. */
            /* 예상 값 계산 */
            float flat = 0;
            float percentage = 0;
            foreach (var modifier in attributeModifiers)
            {
                // 다른 Attribute 는 무시합니다.
                if(attribute.AttributeType != modifier.AttributeType) continue;

                // 분류
                switch (modifier.MagnitudeType)
                {
                    case ModifierMagnitudeType.Flat:
                        flat += modifier.Value;
                        break;
                    case ModifierMagnitudeType.Percentage:
                        percentage += modifier.Value;
                        break;
                }
            }

            float expected = attribute.BaseValue * (1 + percentage) + flat;
            expected = Mathf.Max(expected, 0);

            /* AddModifier */
            foreach (var modifier in attributeModifiers)
            {
                attribute.AddModifier(modifier);
            }

            // Remove Modifier
            attribute.AddModifier(modifierToRemove);
            attribute.RemoveModifier(modifierToRemove);

            /* 결과 비교 */
            Assert.That(attribute.CurrentValue, Is.EqualTo(expected));
        }
    }
}
