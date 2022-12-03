namespace AoeBoardgame
{
    interface IAttacker
    {
        int AttackDamage { get; set; }
        int ArmorPierce { get; set; }
        bool HasAttackedThisTurn { get; set; }
    }
}
