using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rabbit.World;
using Xunit;

namespace Rabbit.Tests.World
{
    static class ACompteur
    {
        internal static void Is(Compteur compteur, int x, int y)
        {
            Assert.Equal(compteur.Pos.X, x);
            Assert.Equal(compteur.Pos.Y, y);
        }
    }
}
