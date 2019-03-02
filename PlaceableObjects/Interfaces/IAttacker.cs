namespace AoeBoardgame
{
    interface IAttacker
    {
        int AttackDamage { get; set; }
        void Attack(IAttackable defender);
    }
}
