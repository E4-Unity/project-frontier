using System;
using UnityEngine;

namespace Frontier
{
    // TODO Attribute 클래스 작성 (Base, Current, Max 3개의 Value 와 OnPropertyChanged 이벤트로 구성)
    /// <summary>
    /// 체력과 죽음 상태는 기본으로 구현되어 있으며, 마나, 기력 등 필요에 따라 자손 클래스에서 추가 Attribute 정의
    /// </summary>
    [Serializable]
    public class AttributeSet
    {
        /* 필드 */
        [Min(1f)]
        [SerializeField] float m_MaxHealth = 100f;
        float m_Health;
        bool m_Dead;

        /* 프로퍼티 */
        public float MaxHealth
        {
            get => m_MaxHealth;
            set
            {
                // 이미 죽은 상태여도 최대 체력 값은 수정할 수 있습니다.

                // 동일한 값이라면 무시합니다.
                if (Mathf.Approximately(m_MaxHealth, value)) return;

                // 체력 비율 계산
                float healthRatio = m_Health / m_MaxHealth;

                // 1 <= MaxHealth
                m_MaxHealth = Mathf.Max(value, 1);

                // 체력 비율 유지
                Health = m_MaxHealth * healthRatio;
            }
        }

        public float Health
        {
            get => m_Health;
            set
            {
                // 이미 죽은 상태에서는 체력 값을 수정할 수 없습니다.
                if (m_Dead) return;

                // 동일한 값이라면 무시합니다.
                if (Mathf.Approximately(m_Health, value)) return;

                // 0 <= Health <= MaxHealth
                m_Health = Mathf.Clamp(value, 0, m_MaxHealth);

                // Health 가 0이 되면 죽은 상태 처리
                if (Mathf.Approximately(m_Health, 0)) m_Dead = true;
            }
        }

        public bool IsDead
        {
            get => m_Dead;
            set
            {
                // 동일한 값이라면 무시합니다.
                if (m_Dead == value) return;

                if (value)
                {
                    // 즉시 사망 시 체력은 0이 됩니다.
                    Health = 0;
                    // m_Dead = true; // Health 프로퍼티에 포함되어 있습니다.
                }
                else
                {
                    // 부활 시 체력은 1이 됩니다.
                    // 부활 시 체력 설정은 외부 클래스에서 진행합니다.
                    m_Dead = false;
                    Health = 1f;
                }
            }
        }
    }
}
