using System;
using UnityEngine;

namespace Frontier
{
    public enum DefaultAttributeType
    {
        Health
    }

    [Serializable]
    public struct BaseDefaultAttribute
    {
        public DefaultAttributeType DefaultAttributeType;
        public float Value;
    }

    [Serializable]
    public class DefaultAttribute : AttributeBase<DefaultAttributeType>
    {
        public DefaultAttribute(DefaultAttributeType attributeType) : base(attributeType)
        {
        }
    }

    [Serializable]
    public class DefaultMaxAttribute : MaxAttributeBase<DefaultAttributeType>
    {
        public DefaultMaxAttribute(DefaultAttributeType attributeType) : base(attributeType)
        {
        }
    }

    [Serializable]
    public class DefaultAttributeModifier : AttributeModifierBase<DefaultAttributeType>{}

    [Serializable]
    public class DefaultAttributeSet : AttributeSet
    {
        [SerializeField] DefaultMaxAttribute m_HealthAttribute = new DefaultMaxAttribute(DefaultAttributeType.Health);

        /* AttributeSet */
        protected override float GetHealth() => m_HealthAttribute.CurrentValue;

        protected override void SetHealth(float value) => m_HealthAttribute.SetBaseValue(m_HealthAttribute.AttributeType, value);

        protected override float GetMaxHealth() => m_HealthAttribute.MaxValue;

        protected override void SetMaxHealth(float value) => m_HealthAttribute.MaxValue = value;
    }
}
