using System.Linq;

namespace AoeBoardgame
{
    interface IAttackable
    {
        int HitPoints { get; set; }
        int MaxHitPoints { get; set; }
        int MeleeArmor { get; set; }
        int RangedArmor { get; set; }
    }

    static class IAttackableMethods
    {
        /// <summary>
        /// Returns true if we died
        /// </summary>
        public static bool TakeDamage<T>(this T defender, int damage, Tile defenderTile) where T : IAttackable
        {
            if (defender is Army army)
            {
                int damagePerUnit = damage / army.Units.Count;

                foreach (PlayerObject unit in army.Units)
                {
                    unit.HitPoints -= damagePerUnit;
                }

                int restDamage = damage - (damagePerUnit * army.Units.Count);

                if (restDamage > 0)
                {
                    ((PlayerObject)army.Units.First()).HitPoints -= restDamage;
                }

                army.Units.RemoveAll(e => ((PlayerObject)e).HitPoints <= 0);
                army.Owner.OwnedObjects.RemoveAll(e => e.HitPoints <= 0);

                if (!army.Units.Any())
                {
                    return true;
                }
                else if (army.Units.Count == 1)
                {
                    // Disband army
                    defenderTile.SetObject((PlayerObject)army.Units[0]);
                    army.Owner.OwnedObjects.Remove(army);
                }
            }
            else
            {
                defender.HitPoints -= damage;

                if (defender.HitPoints <= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
