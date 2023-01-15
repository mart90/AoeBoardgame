namespace AoeBoardgame
{
    interface IMilitaryUnit
    {
        int AttackDamage { get; set; }
        int RangedArmor { get; set; }
        int MeleeArmor { get; set; }
        int ArmorPierce { get; set; }
    }
}
