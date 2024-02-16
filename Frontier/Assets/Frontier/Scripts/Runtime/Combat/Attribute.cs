using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frontier
{
    public enum ModifierMagnitudeType
    {
        Flat,
        Percentage
    }

    /// <summary>
    /// BaseValue 값을 변경하지 않는 대신 BaseValue 값을 사용하여 CurrentValue 값을 결정합니다.
    /// ScriptableObject 에서 정의하여 사용합니다. 참조를 통해 Attribute 에 추가 혹은 제거되므로 유의해야 합니다.
    /// </summary>
    [Serializable]
    public class AttributeModifierBase<TEnum> where TEnum : Enum
    {
        /* 필드 */
        [SerializeField] TEnum m_AttributeType;
        [SerializeField] ModifierMagnitudeType m_MagnitudeType;
        [SerializeField] float m_Value;

        /* 프로퍼티 */
        public TEnum AttributeType => m_AttributeType;
        public ModifierMagnitudeType MagnitudeType => m_MagnitudeType;
        public float Value => m_Value;

        /* 생성자 */
        public AttributeModifierBase(){}
        public AttributeModifierBase(TEnum attributeType, ModifierMagnitudeType magnitudeType, float value)
        {
            m_AttributeType = attributeType;
            m_MagnitudeType = magnitudeType;
            m_Value = value;
        }
    }

    // TODO OnPropertyChanged 이벤트 추가?
    /// <summary>
    /// 체력, 마나, 공격력, 방어력 등과 같이 특성 혹은 스탯 시스템에 사용하기 위한 기본 데이터 클래스입니다.
    /// BaseValue 및 CurrentValue 로 구성되어 있으며 둘 다 0 보다 작은 값을 가질 수 없습니다.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Serializable]
    public class AttributeBase<TEnum> where TEnum : Enum
    {
        /* 필드 */
        [SerializeField] TEnum m_AttributeType;
        float m_BaseValue;
        float m_CurrentValue;

#if UNITY_EDITOR
        // Inspector 창에서 직접 확인하기 위한 용도입니다.
        List<AttributeModifierBase<TEnum>> m_ModifiersForDebug = new List<AttributeModifierBase<TEnum>>();
#endif
        readonly HashSet<AttributeModifierBase<TEnum>> m_Modifiers = new HashSet<AttributeModifierBase<TEnum>>();

        // Modifier 가 추가 혹은 제거되었는지 확인하는 플래그
        bool m_Dirty;

        /* 프로퍼티 */
        public TEnum AttributeType => m_AttributeType;
        public float BaseValue
        {
            get => m_BaseValue;
            protected set
            {
                // 동일한 값으로 변경하는 경우는 무시합니다.
                if (Mathf.Approximately(m_BaseValue, value)) return;

                // BaseValue 가 수정되어 재계산이 필요합니다.
                m_Dirty = true;

                // 0 <= BaseValue
                m_BaseValue = ClampBaseValue(value);
            }
        }

        public float CurrentValue
        {
            get
            {
                // 더티 플래그가 있으면 재계산을 진행합니다.
                if (m_Dirty) CalculateCurrentValue();

                return m_CurrentValue;
            }
            private set
            {
                // 동일한 값으로 변경하는 경우는 무시합니다.
                if (Mathf.Approximately(m_CurrentValue, value)) return;

                // 더티 플래그를 지웁니다.
                m_Dirty = false;

                // 0 <= CurrentValue
                m_CurrentValue = ClampCurrentValue(value);
            }
        }

        /* 생성자 */
        public AttributeBase(){}
        public AttributeBase(TEnum attributeType){ m_AttributeType = attributeType; }

        /* API */
        /// <summary>
        /// BaseValue 에 값을 추가합니다.
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="value"></param>
        public void SetBaseValue(TEnum attributeType, float value)
        {
            // 동일한 AttributeType 만 연산이 가능합니다.
            if (!EqualityComparer<TEnum>.Default.Equals(attributeType, m_AttributeType)) return;

            BaseValue = value;
        }

        /// <summary>
        /// Attribute 에 새로운 Modifier 를 추가합니다.
        /// 중복 추가는 불가능합니다.
        /// </summary>
        /// <param name="modifier"></param>
        public void AddModifier(AttributeModifierBase<TEnum> modifier)
        {
            // 동일한 AttributeType 만 연산이 가능합니다.
            if (!EqualityComparer<TEnum>.Default.Equals(modifier.AttributeType, m_AttributeType)) return;

            // 동일한 Modifier 가 이미 존재하는지 확인합니다.
            if (m_Modifiers.Contains(modifier)) return;

            // Modifier 가 수정되어 재계산이 필요합니다.
            m_Dirty = true;

            // Modifier 를 추가합니다.
            m_Modifiers.Add(modifier);

#if UNITY_EDITOR
            m_ModifiersForDebug.Add(modifier);
#endif
        }

        /// <summary>
        /// Attribute 에 추가된 Modifier 를 제거합니다.
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifier(AttributeModifierBase<TEnum> modifier)
        {
            // 동일한 AttributeType 만 연산이 가능합니다.
            if (!EqualityComparer<TEnum>.Default.Equals(modifier.AttributeType, m_AttributeType)) return;

            // 제거할 Modifier 가 존재하는지 확인합니다.
            if (!m_Modifiers.Contains(modifier)) return;

            // Modifier 가 수정되어 재계산이 필요합니다.
            m_Dirty = true;

            // Modifier 를 제거합니다.
            m_Modifiers.Remove(modifier);

#if UNITY_EDITOR
            m_ModifiersForDebug.Remove(modifier);
#endif
        }

        /// <summary>
        /// 모든 필드를 기본값으로 초기화합니다.
        /// </summary>
        public void Clear()
        {
            m_AttributeType = default;
            m_BaseValue = 0;
            m_CurrentValue = 0;
            m_Dirty = false;
            m_Modifiers.Clear();
        }

        /* 메서드 */
        // 0 <= BaseValue
        protected virtual float ClampBaseValue(float value) => Mathf.Max(value, 0);

        // 0 <= CurrentValue
        protected virtual float ClampCurrentValue(float value) => Mathf.Max(value, 0);

        /// <summary>
        /// 추가된 모든 Modifier 가 적용된 값을 계산하여 CurrentValue 를 업데이트 합니다.
        /// </summary>
        void CalculateCurrentValue()
        {
            // Modifier 분류
            float flat = 0;
            float percentage = 0;

            foreach (var modifier in m_Modifiers)
            {
                switch (modifier.MagnitudeType)
                {
                    case ModifierMagnitudeType.Flat:
                        flat += modifier.Value;
                        break;
                    case ModifierMagnitudeType.Percentage:
                        percentage += modifier.Value;
                        break;
                    default:
                        continue;
                }
            }

            // CurrentValue 계산
            float currentValue = m_BaseValue * (1 + percentage) + flat;
            CurrentValue = currentValue;
        }
    }
}
