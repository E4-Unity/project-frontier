using System;
using UnityEngine;

namespace Frontier
{
    /// <summary>
    /// 체력과 죽음 상태는 기본으로 구현되어 있으며, 마나, 기력 등 필요에 따라 자손 클래스에서 추가 Attribute 정의
    /// </summary>
    [Serializable]
    public abstract class AttributeSet
    {
        /* 필드 */
        bool m_Dead;

        /* 프로퍼티 */
        public float MaxHealth
        {
            get => GetMaxHealth();
            set => SetMaxHealth(value);
        }

        public float Health
        {
            get => GetHealth();
            set
            {
                // 이미 죽은 상태에서는 체력 값을 수정할 수 없습니다.
                if (m_Dead) return;

                SetHealth(value);

                // Health 가 0이 되면 죽은 상태 처리
                if (Mathf.Approximately(GetHealth(), 0)) m_Dead = true;
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

        /* 메서드 */
        protected abstract float GetHealth();
        protected abstract void SetHealth(float value);
        protected abstract float GetMaxHealth();
        protected abstract void SetMaxHealth(float value);
    }
}
