using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerEngine.logic.loot;
using TerrainEngine;

namespace ServerEngine.realm.setpieces
{
    class RockDragon : ISetPiece
    {
        public int Size { get { return 5; } }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            Entity dragon = Entity.Resolve(0x00480);
            dragon.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(dragon);
        }
    }
}
