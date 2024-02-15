using UnityEngine;

namespace Frontier
{
    public interface IDamageable
    {
        void ApplyDamage(float damage);
        void Kill();
    }

    // TODO 스탯, 애니메이션
    public abstract class Character : MonoBehaviour, IDamageable
    {
        /* 필드 */
        [SerializeField] float m_Damage = 10f; // TODO 임시 사용중, 추후 스탯 컴포넌트에서 구현

        /* 프로퍼티 */
        // Attributes
        protected abstract AttributeSet Attributes { get; }

        public float MaxHealth
        {
            get => Attributes.MaxHealth;
            protected set => Attributes.MaxHealth = value;
        }

        public float Health
        {
            get => Attributes.Health;
            protected set => Attributes.Health = value;
        }

        public bool IsDead
        {
            get => Attributes.IsDead;
            protected set => Attributes.IsDead = value;
        }

        // 데미지
        public float Damage => m_Damage;

        /* API */
        /// <summary>
        /// 캐릭터 초기화 실행
        /// </summary>
        public void Init()
        {
            // 부활 처리
            IsDead = false;

            // 캐릭터 초기화 이벤트 처리
            OnInit();
        }

        /// <summary>
        /// 캐릭터 초기화 이벤트 처리
        /// </summary>
        protected virtual void OnInit()
        {
            // AttributeSet 초기화
            Health = MaxHealth;
        }

        /// <summary>
        /// 공격 실행
        /// </summary>
        /// <param name="target">공격 대상 캐릭터</param>
        public void Attack(Character target)
        {
            // 죽은 상태에서는 공격할 수 없습니다.
            if (IsDead) return;

            // 이미 죽은 캐릭터를 공격하지 않습니다.
            if (target.IsDead) return;

            // 공격 이벤트 처리
            OnAttack(target);
        }

        /// <summary>
        /// 공격 이벤트 처리
        /// </summary>
        /// <param name="target">공격 대상 캐릭터</param>
        protected virtual void OnAttack(Character target)
        {
            target.ApplyDamage(Damage);
        }

        /* IDamageable 인터페이스 */
        public void ApplyDamage(float damage) => TakeDamage(damage);

        public void Kill() => Die();

        /* 메서드 */
        /// <summary>
        /// IDamageable.ApplyDamage 에 연결하기 위한 메서드로 죽은 상태이거나 데미지가 양수가 아닌 경우에는 동작하지 않습니다.
        /// 데미지 계산 및 기타 이벤트는 OnTakeDamage 가상 메서드를 오버라이드하여 구현하세요.
        /// </summary>
        /// <param name="damage"></param>
        void TakeDamage(float damage)
        {
            // 죽은 상태에서는 데미지를 입을 수 없습니다.
            if(IsDead) return;

            // 데미지는 반드시 양수여야 합니다.
            if (damage < 0 || Mathf.Approximately(damage, 0)) return;

            // 데미지 계산 및 기타 이벤트 처리
            OnTakeDamage(damage);
        }

        /// <summary>
        /// 데미지 계산 및 기타 이벤트 처리
        /// </summary>
        /// <param name="damage"></param>
        protected virtual void OnTakeDamage(float damage)
        {
            Health -= damage;
        }

        /// <summary>
        /// IDamageable.Kill 에 연결하기 위한 메서드로 이미 죽은 상태에서는 동작하지 않습니다.
        /// 죽음 이벤트는 OnDied 가상 메서드를 오버라이드하여 구현하세요.
        /// </summary>
        void Die()
        {
            // 이미 죽은 상태입니다.
            if (IsDead) return;

            // 죽음 이벤트 처리
            OnDied();
        }

        /// <summary>
        /// 죽음 이벤트 처리
        /// </summary>
        protected virtual void OnDied()
        {
            IsDead = true;
        }
    }
}
