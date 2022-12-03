using System;
using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Army : PlayerObject,
        IContainsUnits,
        ICanMove,
        IAttacker
    {
        public int Speed
        {
            get => Units.Any() ? Units[0].Speed : 0;
            set { }
        }

        public int AttackDamage
        {
            get => Units.Sum(e => e.AttackDamage);
            set { }
        }

        public int ArmorPierce
        {
            get => Units.Sum(e => e.ArmorPierce);
            set { }
        }

        public Type UnitType
        { 
            get => Units.Any() ? Units[0].GetType() : null;
            set { }
        }

        public override int LineOfSight 
        {
            get => Units.Any() ? ((PlayerObject)Units[0]).LineOfSight : 0;
            set { }
        }

        public override int HitPoints 
        { 
            get => Units.Sum(e => ((PlayerObject)e).HitPoints);
            set { }
        }

        public override int MaxHitPoints
        {
            get => Units.Sum(e => ((PlayerObject)e).MaxHitPoints);
            set { }
        }

        public Tile DestinationTile { get; set; }
        public int StepsTakenThisTurn { get; set; }

        public List<ICanFormGroup> Units { get; private set; }
        public int MaxUnits { get; set; }
        public bool HasAttackedThisTurn { get; set; }

        public Army(TextureLibrary textureLibrary, Player owner) : base(textureLibrary, owner)
        {
            Units = new List<ICanFormGroup>();
        }

        public void SetTexture()
        {
            Texture = TextureLibrary.GetObjectTextureByType(Units[0].GetType());
        }
    }
}
