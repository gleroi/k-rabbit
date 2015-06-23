using Rabbit.World;

namespace Rabbit.AI
{
    /// <summary>
    /// Basic 
    /// </summary>
    class Ai
    {
        protected WorldState[] GenerateForPlayer(WorldState world, int playerId)
        {
            var states = new WorldState[4];

            states[0] = world.ApplyAction(playerId, Direction.N);
            states[1] = world.ApplyAction(playerId, Direction.S);
            states[2] = world.ApplyAction(playerId, Direction.E);
            states[3] = world.ApplyAction(playerId, Direction.O);
            return states;
        }

    }
}
