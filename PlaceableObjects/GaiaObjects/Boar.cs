namespace AoeBoardgame
{
    class Boar : GaiaObject, IAttackable, IAttacker
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }

        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }

        public bool HasAttackedThisTurn { get; set; } // Unused

        public Boar(TextureLibrary textureLibrary) : base(textureLibrary)
        {
            HitPoints = 25;
            MaxHitPoints = 25;
            MeleeArmor = 0;
            RangedArmor = 0;
            AttackDamage = 6;
        }
    }
}
