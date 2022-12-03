using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class TownCenterFactory : PlaceableObjectFactory, ICanMakeUnitsFactory, ICanMakeResearchFactory
    {
        public int HitPoints { get; set; }

        private int _attackDamage;
        private int _range;
        private int _lineOfSight;
        
        public List<Type> UnitTypesAllowedToMake { get; set; }
        public List<ResearchEnum> ResearchAllowedToMake { get; set; }

        public TownCenterFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(TownCenter);
            TurnsToComplete = 4;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new TownCenter(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = _attackDamage,
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
            _attackDamage = 5;
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
