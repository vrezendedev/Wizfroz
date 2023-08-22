using UnityEngine;

public class ManaCollectable : Collectable
{
    [Header("Characteristics:")]
    public float ManaGivenOnCollision;

    public override void ActOnEnter(Frog f)
    {
        particle.Play();
        if (f.Mana == f.MaxMana) return;
        var currentMana = f.Mana + ManaGivenOnCollision;
        if (currentMana > f.MaxMana) f.Mana = f.MaxMana;
        else f.Mana = currentMana;
        UIEventManager.UpdateManaBarUI(f.Mana);
        UIEventManager.GotCollectableToUI("mana");
    }

    public override void ActOnExit(Frog f)
    {
        Destroy(this.gameObject);
    }
}
