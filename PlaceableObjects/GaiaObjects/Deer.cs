namespace AoeBoardgame
{
    class Deer : GaiaObject, IAttackable
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int MeleeArmor { get; set; }
        public int RangedArmor { get; set; }
        
        public Deer(TextureLibrary textureLibrary) : base(textureLibrary)
        {
            UiName = "Boar";
            HitPoints = 4;
            MaxHitPoints = 4;
            MeleeArmor = 0;
            RangedArmor = 0;
        }
    }
}
