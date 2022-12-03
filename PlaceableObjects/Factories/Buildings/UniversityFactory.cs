using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class UniversityFactory : PlaceableObjectFactory, ICanMakeResearchFactory
    {
        private int _hitPoints;

        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public UniversityFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(University);
            TurnsToComplete = 3;
        }

        public override PlaceableObject Get(Player player)
        {
            return new University(TextureLibrary, player)
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
            UiName = "University";
            UiDescription = "Advanced research building";

            _hitPoints = 100;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 200)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
