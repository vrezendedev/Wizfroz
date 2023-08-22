using UnityEngine;

public class OctobearTrophyCollectable : Collectable
{
    [Header("Characteristics:")]
    public float OctobearTrophiesGivenOnCollision;

    public override void ActOnEnter(Frog f)
    {
        particle.Play();
        f.OctobearTrophies += OctobearTrophiesGivenOnCollision;
        UIEventManager.GotCollectableToUI("octobear");
    }

    public override void ActOnExit(Frog f)
    {
        Destroy(this.gameObject);
    }
}
