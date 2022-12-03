namespace AoeBoardgame
{
    class RangedArmyFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        public RangedArmyFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(RangedArmy);
        }

        public override PlaceableObject Get(Player player)
        {
            return new RangedArmy(TextureLibrary, player)
            {
                MaxUnits = MaxUnits
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Ranged army";
            UiDescription = "A group of ranged military units";

            MaxUnits = 2;
        }
    }
}
