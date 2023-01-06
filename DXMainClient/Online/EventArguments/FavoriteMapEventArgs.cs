using DTAClient.Domain.Multiplayer;
using System;

namespace DTAClient.Online.EventArguments
{
    public class FavoriteMapEventArgs : EventArgs
    {
        public readonly Map Map;

        public FavoriteMapEventArgs(Map map)
        {
            Map = map;
        }
    }
}
