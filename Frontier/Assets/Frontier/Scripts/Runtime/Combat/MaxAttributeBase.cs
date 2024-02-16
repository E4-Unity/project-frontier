using System;
using UnityEngine;

namespace Frontier
{
    // TODO 테스트 스크립트 작성
    /// <summary>
    /// 이 Attribute 는 MaxValue 가 추가로 존재하며 BaseValue 와 CurrentValue 는 MaxValue 보다 작거나 같다는 제약이 걸려있으며,
    /// 주로 체력, 마나와 같은 특성 시스템에서 사용합니다.
    /// MaxValue 는 1 보다 작을 수 없으며 값이 수정되면 BaseValue / MaxValue 비율을 유지하기 위해 BaseValue 값도 수정합니다.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Serializable]
    public class MaxAttributeBase<TEnum> : AttributeBase<TEnum> where TEnum : Enum
    {
        /* 필드 */
        [Min(1f)]
        [SerializeField] float m_MaxValue = 1f;

        /* 프로퍼티 */
        public float MaxValue
        {
            get => m_MaxValue;
            set
            {
                // 동일한 값으로 변경하는 경우는 무시합니다.
                if (Mathf.Approximately(m_MaxValue, value)) return;

                // 기존 비율을 기록합니다.
                float ratio = m_MaxValue == 0 ? 0 : BaseValue / m_MaxValue;

                // MaxValue 값을 업데이트합니다.
                m_MaxValue = Mathf.Max(value, 1);

                // 비율 유지를 위해 BaseValue 값도 업데이트합니다.
                BaseValue = m_MaxValue * ratio;
            }
        }

        /* 생성자 */
        public MaxAttributeBase(TEnum attributeType) : base(attributeType)
        {
        }

        /* AttributeBase 인터페이스 */
        // 0 <= BaseValue <= MaxValue
        protected override float ClampBaseValue(float value) => Mathf.Clamp(value, 0, m_MaxValue);

        // 0 <= CurrentValue <= MaxValue
        protected override float ClampCurrentValue(float value) => Mathf.Clamp(value, 0, m_MaxValue);
    }
}
