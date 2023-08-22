using System.Collections.Generic;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Universe;
using Spells;
using Levels;

public class Frog : MonoBehaviour
{
    [Header("Frog Behaviour Characteristics:")]
    public float JumpForce;
    public float MaxSecondsFloating = 3f;
    public List<AudioClip> FrogSounds;
    public AudioClip DeathSound;
    public AudioClip SpellDirection;
    public AudioClip SpellSlow;
    public AudioClip SpellInvisible;
    public AudioClip ManaCollected;
    public AudioClip OctobearCollected;

    [Space(20)]
    [Header("Actions:")]
    public InputAction JumpAction;
    public InputAction CastInvisibleSpellAction;
    public InputAction CastChangePlanetDirectionAction;
    public InputAction CastSlowDownPlanetSpellAction;
    public InputAction QuickRestart;

    [Space(20)]
    [Header("Components")]
    Rigidbody2D rigidBody;
    public AudioSource collectableAudioSource;
    public AudioSource spellAudioSource;
    public AudioSource jumpAndDeathAudioSource;
    CircleCollider2D circleCollider;

    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer LeftFootSpriteRenderer;
    public SpriteRenderer RightFootSpriteRenderer;

    [Space(20)]
    [Header("Effects and More:")]
    [SerializeField] private ParticleSystem jumpParticle;


    [Space(20)]
    [Header("Stats and More:")]
    public float Mana = 0;
    public float MaxMana = 50f;
    public float OctobearTrophies = 0;
    public bool IsInvisible = false;
    public List<Spell> Spells;
    public Planet LandedPlanet = null;
    public bool Won = false;
    public bool Died = false;
    public bool started = false;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
        JumpAction.performed += OnJump;

        CastInvisibleSpellAction.performed += OnCastInvisibleSpell;
        CastChangePlanetDirectionAction.performed += OnCastMovePlanetSpell;
        CastSlowDownPlanetSpellAction.performed += OnCastSlowDownPlanetSpell;
        QuickRestart.performed += delegate { CancelInvoke("DieLostInSpace"); Die("Decided to die?"); };
    }

    private void OnEnable()
    {
        JumpAction.Enable();
        CastInvisibleSpellAction.Enable();
        CastChangePlanetDirectionAction.Enable();
        CastSlowDownPlanetSpellAction.Enable();
        QuickRestart.Enable();
    }

    private void OnDisable()
    {
        JumpAction.Disable();
        CastInvisibleSpellAction.Disable();
        CastChangePlanetDirectionAction.Disable();
        CastSlowDownPlanetSpellAction.Disable();
        QuickRestart.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent<Planet>(out LandedPlanet);

        if (LandedPlanet != null)
        {
            CancelInvoke("DieLostInSpace");
            rigidBody.velocity = Vector2.zero;

            if (LandedPlanet.IsWinPlanet)
            {
                LandedPlanet.StopAllCoroutines();
                Win();
            }

            jumpParticle.Clear();
            jumpParticle.Stop();
            this.transform.SetParent(LandedPlanet.transform, true);

            Vector3 direction = LandedPlanet.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0f, 0f, 180f);
            transform.rotation = targetRotation;

        }

        if ((other.gameObject.tag == "Obstacle" || other.gameObject.tag == "Enemy") && IsInvisible == false && Won == false)
        {
            switch (other.gameObject.tag)
            {
                case "Obstacle":
                    Die("These space obstacles are, indeed, a nightmare!");
                    break;
                case "Enemy":
                    Die("Evil space creatures... beware of them!");
                    break;
                default:
                    Die("Let's try again!");
                    break;
            }
        }

        if (other.gameObject.tag == "Collectable")
        {
            other.TryGetComponent<ManaCollectable>(out var mana);

            if (mana == null)
            {
                collectableAudioSource.clip = OctobearCollected;
                collectableAudioSource.Play();
            }
            else
            {
                collectableAudioSource.clip = ManaCollected;
                collectableAudioSource.Play();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle" && IsInvisible == false)
        {
            Die("Let's try again!");
        }

        if (other.gameObject.TryGetComponent<Planet>(out Planet planet))
        {
            if (planet != LandedPlanet)
            {
                LandedPlanet = planet;
            }
        }
    }

    public void Win()
    {
        Won = true;
        UIEventManager.ShowWinUI();
    }

    public void Die(string deathMessage)
    {
        UIEventManager.ShowDefeatUI(deathMessage);
        DieProcs();
    }

    public void DieLostInSpace()
    {
        UIEventManager.ShowDefeatUI("Don't lose yourself to the endless space...");
        DieProcs();
    }

    public void DieProcs()
    {
        Died = true;
        LevelsInfo.Levels[LevelsInfo.CurrentLevel].PlayersDeathCount++;
        jumpAndDeathAudioSource.clip = DeathSound;
        jumpAndDeathAudioSource.Play();
        circleCollider.enabled = false;
        rigidBody.Sleep();
        jumpParticle.Stop();
        LeftFootSpriteRenderer.material.color = Color.red;
        RightFootSpriteRenderer.material.color = Color.red;
        SpriteRenderer.material.color = Color.red;
        StartCoroutine(ShrinkAndDie(Vector3.zero, 0.75f));
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (this.Won || this.Died) return;

        if (started == true && LandedPlanet == null) return;

        if (started == false) started = true;

        StartCoroutine(DontHitPlanetAfterExitingIt());
        jumpAndDeathAudioSource.clip = FrogSounds[Random.Range(0, FrogSounds.Count)];
        jumpAndDeathAudioSource.Play();

        Invoke("DieLostInSpace", MaxSecondsFloating);
        rigidBody.AddForce(transform.up * JumpForce);
        jumpParticle.Play();
        LandedPlanet = null;
        this.transform.parent = null;
        ResetScale();
    }

    private void OnCastInvisibleSpell(InputAction.CallbackContext ctx)
    {
        if (this.Won || this.Died) return;

        if (Mana != 0 && IsInvisible == false)
        {
            var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.Invisible).FirstOrDefault();

            if (spell.ManaCost > Mana) return;

            spellAudioSource.clip = SpellInvisible;
            spellAudioSource.Play();

            Mana -= spell.ManaCost;

            spell.Act(this.gameObject);
        }
    }

    private void OnCastMovePlanetSpell(InputAction.CallbackContext ctx)
    {
        if (this.Won || this.Died) return;

        if (Mana != 0 && LandedPlanet != null)
        {
            var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.ChangePlanetDirection).FirstOrDefault();

            if (spell.ManaCost > Mana) return;

            spellAudioSource.clip = SpellDirection;
            spellAudioSource.Play();

            Mana -= spell.ManaCost;

            spell.Act(this.LandedPlanet.gameObject);
        }
    }

    private void OnCastSlowDownPlanetSpell(InputAction.CallbackContext ctx)
    {
        if (this.Won || this.Died) return;

        if (Mana != 0 && LandedPlanet != null)
        {
            if (LandedPlanet.IsSlowed == false)
            {
                var spell = Spells.Where(obj => obj.Type == Spell.SpellTypes.SlowDownPlanetSpell).FirstOrDefault();

                if (spell.ManaCost > Mana) return;

                spellAudioSource.clip = SpellSlow;
                spellAudioSource.Play();

                Mana -= spell.ManaCost;

                spell.Act(this.LandedPlanet.gameObject);
            }
        }
    }

    private void ResetScale()
    {
        this.transform.localScale = Vector3.one;
    }

    private IEnumerator ShrinkAndDie(Vector3 scale, float time)
    {
        float progress = 0;
        float rate = 1 / time;

        Vector3 fromScale = transform.localScale;
        Vector3 toScale = scale;

        while (progress < 1)
        {
            progress += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(fromScale, toScale, progress);
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator DontHitPlanetAfterExitingIt()
    {
        this.circleCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        this.circleCollider.enabled = true;
    }
}