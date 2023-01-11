using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        public int HitPoints { get; set; }
        public int AttackDamage { get; set; }

        private int _range;
        private int _lineOfSight;
        
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public TownCenterFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(TownCenter);
            TurnsToComplete = 3;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new TownCenter(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = AttackDamage,
                ArmorPierce = 3,
                Range = _range,
                LineOfSight = _lineOfSight,
                UnitTypesAllowedToMake = new List<Type>(UnitTypesAllowedToMake),
                ResearchAllowedToMake = new List<ResearchEnum>(ResearchAllowedToMake),
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Town center";
            UiDescription = "Villager production and age-up building";

            HitPoints = 120;
            AttackDamage = 6;
            _range = 3;
            _lineOfSight = 4;

            UnitTypesAllowedToMake = new List<Type>
            {
                typeof(Villager)
            };

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 150),
                new ResourceCollection(Resource.Stone, 100)
            };

            ResearchAllowedToMake = new List<ResearchEnum>();
        }
    }
}
