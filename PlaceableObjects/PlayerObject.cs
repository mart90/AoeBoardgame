using System.Collections.Generic;

namespace AoeBoardgame
{
    abstract class PlayerObject : PlaceableObject, IAttackable
    {
        public virtual int HitPoints { get; set; }
        public virtual int MaxHitPoints { get; set; }
        public virtual int MeleeArmor { get; set; }
        public virtual int RangedArmor { get; set; }
        public virtual int LineOfSight { get; set; }
        public Player Owner { get; set; } // TODO circular dependency
        public List<Tile> VisibleTiles { get; set; }

        protected PlayerObject(TextureLibrary textureLibrary, Player owner)
        {
            TextureLibrary = textureLibrary;
            Texture = TextureLibrary.GetObjectTextureByType(GetType());
            Owner = owner;
            VisibleTiles = new List<Tile>();

            ColorTexture = new TileColorTexture
            {
                Texture = textureLibrary.GetTileColorByType(Owner.Color),
                TileColor = Owner.Color
            };
        }

        public bool CanMergeWith(PlaceableObject obj)
        {
            if (!(obj is PlayerObject playerObject && playerObject.Owner == Owner))
            {
                return false;
            }

            if (obj is ICanMakeBuildings builder && builder.HasBuildingQueued())
            {
                return false;
            }

            if (obj.GetType() == GetType() && this is ICanFormGroup)
            {
                return true;
            }

            if (obj is IEconomicBuilding building && this is ICanGatherResources)
            {
                if (this is GathererGroup group)
                {
                    return building.Units.Count + group.Units.Count <= building.MaxUnits;
                }
                else
                {
                    return building.Units.Count < building.MaxUnits;
                }
            }

            if (obj is Army army)
            {
                if (this is Army mergerArmy)
                {
                    if (army.UnitType != mergerArmy.UnitType)
                    {
                        return false;
                    }

                    return army.Units.Count + mergerArmy.Units.Count <= army.MaxUnits;
                }
                else if (army.UnitType == GetType())
                {
                    return army.Units.Count < army.MaxUnits;
                }
            }

            return false;
        }

        /// <summary>
        /// Does not take range into account
        /// </summary>
        public bool CanAttack(PlaceableObject obj)
        {
            if (!(this is IAttacker) || !(obj is IAttackable))
            {
                return false;
            }

            if (obj is PlayerObject playerObject && playerObject.Owner == Owner)
            {
                return false;
            }

            return true;
        }

        public bool IsFrenchCavalry()
        {
            return Owner.Civilization is France && (this is ICavalry || this is Army army && army.Units[0] is ICavalry);
        }
    }
}
