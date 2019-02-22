using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoeBoardgame
{
    class Player
    {
        public TileColor Color { get; set; }

        public int Food { get; set; }
        public int Wood { get; set; }
        public int Gold { get; set; }
        public int Iron { get; set; }
        public int Stone { get; set; }

        public List<Worker> Workers { get; set; }
        public List<Unit> Units { get; set; }
        public List<Building> Buildings { get; private set; }

        public Player(TileColor color)
        {
            Color = color;

            Workers = new List<Worker>();
            Units = new List<Unit>();
            Buildings = new List<Building>();
        }
    }
}
