using System.Collections.Generic;
using System.Linq;

namespace AoeBoardgame
{
    class Britons : Civilization
    {
        public Britons(TextureLibrary textureLibrary) : base(textureLibrary) { }

        public override IEnumerable<PlaceableObjectFactory> GetFactories(Player player)
        {
            var factories = base.GetFactories(player).ToList();

            // TODO add unique units/buildings

            return factories;
        }
    }
}
