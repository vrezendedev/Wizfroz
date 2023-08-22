using System.Collections;

using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private CapsuleCollider2D capsuleCollider;
    public ParticleSystem particle;

    private void Awake()
    {
        this.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        this.TryGetComponent<CircleCollider2D>(out circleCollider);
        this.TryGetComponent<CapsuleCollider2D>(out capsuleCollider);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<Frog>(out var player))
        {

            if (this.circleCollider != null)
                circleCollider.enabled = false;
            else
                capsuleCollider.enabled = false;

            this.spriteRenderer.hideFlags = HideFlags.None;
            this.spriteRenderer.color = new Color(0, 0, 0, 0);
            StopAllCoroutines();
            ActOnEnter(player);
        }

    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Frog>(out var player))
        {
            StartCoroutine(WaitTillParticleEndsPlaying(player));
        }
    }

    public abstract void ActOnEnter(Frog f);

    public abstract void ActOnExit(Frog f);

    private IEnumerator WaitTillParticleEndsPlaying(Frog f)
    {
        while (particle.isPlaying)
        {
            yield return new WaitForSeconds(1f);
        }

        ActOnExit(f);
    }
}
