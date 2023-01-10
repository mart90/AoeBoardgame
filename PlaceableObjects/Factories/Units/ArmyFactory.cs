namespace AoeBoardgame
{
    class ArmyFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        public ArmyFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Army);
        }

        public override PlaceableObject Get(Player player)
        {
            return new Army(TextureLibrary, player)
            {
                UiName = UiName,
                MaxUnits = MaxUnits
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Army";
            UiDescription = "A group of military units";

            MaxUnits = 2;
        }
    }
}
