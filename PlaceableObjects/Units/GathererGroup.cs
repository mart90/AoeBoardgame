using System.Linq;

namespace AoeBoardgame
{
    class GathererGroup : Army, ICanGatherResources
    {
        public Resource? ResourceGathering
        {
            get => null;
            set
            {
                foreach (ICanGatherResources unit in Units.Cast<ICanGatherResources>())
                {
                    unit.ResourceGathering = value;
                }
            }
        }

        public GathererGroup(TextureLibrary textureLibrary, Player owner) : base(textureLibrary, owner)
        {
        }
    }
}
