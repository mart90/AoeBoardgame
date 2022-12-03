﻿using System.Collections.Generic;

namespace AoeBoardgame
{
    class GuardTowerFactory : PlaceableObjectFactory
    {
        public bool HasMinimumRange { get; set; }
        public int HitPoints { get; set; }

        private int _attackDamage;
        private int _range;
        private int _lineOfSight;

        public GuardTowerFactory(TextureLibrary textureLibrary) :
            base(textureLibrary)
        {
            Type = typeof(GuardTower);
            TurnsToComplete = 3;
            HasMinimumRange = true;
        }

        public override PlaceableObject Get(Player player) 
        {
            return new GuardTower(TextureLibrary, player)
            {
                HitPoints = HitPoints,
                MaxHitPoints = HitPoints,
                AttackDamage = _attackDamage,
                ArmorPierce = 3,
                Range = _range,
                HasMinimumRange = HasMinimumRange,
                LineOfSight = _lineOfSight,
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Guard\ntower";
            UiDescription = "Building with long line of sight and a ranged attack";

            HitPoints = 100;
            _attackDamage = 8;
            _range = 4;
            _lineOfSight = 5;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 100),
                new ResourceCollection(Resource.Stone, 150)
            };
        }
    }
}