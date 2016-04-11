using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.realm.setpieces
{
    interface ISetPiece
    {
        int Size { get; }
        void RenderSetPiece(World world, IntPoint pos);
    }
}
