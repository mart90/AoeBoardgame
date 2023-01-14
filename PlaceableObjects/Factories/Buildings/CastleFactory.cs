using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class CastleFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        public bool HasMinimumRange { get; set; }
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }

        private int _range;
        private int _lineOfSight;
        
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public CastleFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Castle);
            TurnsToComplete = 5;
            HasMinimumRange = true;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new Castle(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = AttackDamage,
                ArmorPierce = 5,
                Range = _range,
                HasMinimumRange = HasMinimumRange,
                LineOfSight = _lineOfSight,
                UnitTypesAllowedToMake = new List<Type>(UnitTypesAllowedToMake),
                ResearchAllowedToMake = new List<ResearchEnum>(ResearchAllowedToMake),
                MeleeArmor = 10,
                RangedArmor = 10
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Castle";
            UiDescription = "Strong defensive building that produces your civ's unique unit";

            HitPoints = 280;
            AttackDamage = 20;
            _range = 4;
            _lineOfSight = 5;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Stone, 500)
            };

            UnitTypesAllowedToMake = new List<Type>();
            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
