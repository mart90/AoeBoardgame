using System;
using System.Collections.Generic;

namespace AoeBoardgame
{
    class Villager : PlayerObject,
        ICanMove,
        ICanFormGroup,
        IAttacker,
        ICanGatherResources,
        ICanMakeBuildings,
        IHasQueue,
        IConsumesFood
    {
        public int Speed { get; set; }
        public int AttackDamage { get; set; }
        public int ArmorPierce { get; set; }
        public bool HasAttackedThisTurn { get; set; }
        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }
        public Resource? ResourceGathering { get; set; }
        public int FoodConsumption { get; set; }

        public List<Type> BuildingTypesAllowedToMake { get; set; }
        public Type BuildingTypeQueued { get; set; }
        public Tile BuildingDestinationTile { get; set; }

        public int QueueTurnsLeft { get; set; }

        public bool IsSubSelected { get; set; }

        public Villager(TextureLibrary textureLibrary, Player owner) :
            base(textureLibrary, owner)
        {
        }
    }
}
