using UnityEngine;

namespace Frontier
{
    public class DefaultCharacter : Character
    {
        /* 필드 */
        [SerializeField] AttributeSet m_Attributes = new DefaultAttributeSet();

        /* Character 인터페이스 */
        protected override AttributeSet Attributes => m_Attributes;
    }
}
