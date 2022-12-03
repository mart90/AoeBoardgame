﻿using System.Linq;

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
        public static bool TakeDamage<T>(this T defender, IAttacker attacker, int damage) where T : IAttackable
        {
            if (defender is Army army)
            {
                if (army.Units[0] is Scout || army.Units[0] is Knight && attacker is Pikeman)
                {
                    damage *= 3;
                }

                int damagePerUnit = damage / army.Units.Count;

                foreach (PlayerObject unit in army.Units)
                {
                    unit.HitPoints -= damagePerUnit;
                }

                army.Units.RemoveAll(e => ((PlayerObject)e).HitPoints <= 0);
                army.Owner.OwnedObjects.RemoveAll(e => e.HitPoints <= 0);

                if (!army.Units.Any())
                {
                    return true;
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