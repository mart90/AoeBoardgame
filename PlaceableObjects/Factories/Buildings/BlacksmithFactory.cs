using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class BlacksmithFactory : PlaceableObjectFactory, ICanMakeResearchFactory
    {
        private int _hitPoints;

        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public BlacksmithFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Blacksmith);
            TurnsToComplete = 2;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Blacksmith(TextureLibrary, player)
            {
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = 1,
                ResearchAllowedToMake = new List<ResearchEnum>(ResearchAllowedToMake),
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Blacksmith";
            UiDescription = "Military research building";

            _hitPoints = 70;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
