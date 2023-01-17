using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class WonderFactory : PlaceableObjectFactory
    {
        private int _hitPoints;

        public WonderFactory(TextureLibrary textureLibrary)
            : base(textureLibrary)
        {
            Type = typeof(Wonder);
            TurnsToComplete = 5;
        }

        public override PlaceableObject Get(Player player)
        {
            return new Wonder(TextureLibrary, player)
            {
                UiName = UiName,
                HitPoints = _hitPoints,
                MaxHitPoints = _hitPoints,
                LineOfSight = 3,
                MeleeArmor = 2,
                RangedArmor = 5
            };
        }

        protected override void SetBaseValues()
        {
            UiName = "Wonder";
            UiDescription = "If you defend this building for 20 turns, you win the game";

            _hitPoints = 150;

            Cost = new List<ResourceCollection>
            {
                new ResourceCollection(Resource.Wood, 500),
                new ResourceCollection(Resource.Gold, 500)
            };
        }
    }
}
