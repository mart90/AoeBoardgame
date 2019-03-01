namespace AoeBoardgame
{
    interface ICanAttack
    {
        int AttackDamage { get; set; }
        void Attack(ICanBeAttacked defender);
    }
}
