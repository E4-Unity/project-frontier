# Character

## 요구 사항

- AttributeSet 을 가지고 있다.
  - 클래스에 따라 고유의 AttributeSet 사용 (마나, 기력 등)
  - 스탯을 가지고 있다. (공격력, 방어력 등)
- 데미지를 주고 받을 수 있다.
  - 죽은 상태에서는 불가능합니다.
  - Attack(Character target)
  - TakeDamage(float damage)
- 애니메이션을 사용한다.

## 클래스 다이어그램

```mermaid
classDiagram
    %% 클래스 선언
    class IDamageable
    class AttributeSet
    class Character
    class DefaultCharacter
    
    %% 클래스 관계 정의
    Character ..|> IDamageable
    AttributeSet --* Character
    DefaultCharacter --|> Character
    
    %% 클래스 정의
    class IDamageable{
        <<interface>>
        +ApplyDamage(float damage) void
        +Kill() void
    }
    
    class Character{
        <<abstract>>
        %% 프로퍼티
        Attributes : AttributeSet ~~#get~~*
        Health : float ~~+get-set~~
        MaxHealth : float ~~+get-set~~
        IsDead : bool ~~+get-set~~
        
        %% API
        +Init() void
        +Attack(Character target) void
        
        %% 재정의 메서드
        #OnInit() void*
        #OnAttack(Character target) void*
        #OnTakeDamage(float damage) void*
        #OnDied() void*
        
        %% 메서드
        -TakeDamage(float damage) void
        -Die() void
    }
    
    class DefaultCharacter{
        -m_Attributes : AttributeSet = new AttributeSet()
    }
```
