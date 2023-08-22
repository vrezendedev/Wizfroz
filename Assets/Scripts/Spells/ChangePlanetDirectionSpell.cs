using UnityEngine;
using Universe;

namespace Spells
{
    public class ChangePlanetDirectionSpell : Spell
    {
        public ChangePlanetDirectionSpell() : base(SpellTypes.ChangePlanetDirection) { }

        public override void Action(GameObject target)
        {
            target.TryGetComponent<Planet>(out Planet planet);

            if (planet == null) return;

            planet.RotateDirection *= -1;
        }
    }
}
