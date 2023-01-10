namespace AoeBoardgame
{
    class GathererGroupFactory : PlaceableObjectFactory
    {
        public int MaxUnits { get; set; }

        public GathererGroupFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(GathererGroup);
        }

        public override PlaceableObject Get(Player player)
        {
            return new GathererGroup(TextureLibrary, player)
            {
                UiName = UiName,
                MaxUnits = MaxUnits
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Gatherer group";
            UiDescription = "A group of economic units";

            MaxUnits = 2;
        }
    }
}
