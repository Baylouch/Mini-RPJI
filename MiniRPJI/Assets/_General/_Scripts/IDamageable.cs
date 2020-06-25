/* IDamageable.cs : Interface
 * 
 * Doit être parent de chaque health composants du jeu, afin de pouvoir utiliser MalusApplier.cs sur eux.
 * 
 * */

public interface IDamageable
{
    void TakeDamage(int amount, bool playSound);
    bool IsDead();
}
