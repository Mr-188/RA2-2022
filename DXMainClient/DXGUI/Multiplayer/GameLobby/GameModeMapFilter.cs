using DTAClient.Domain.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DTAClient.DXGUI.Multiplayer.GameLobby
{
    public class GameModeMapFilter
    {
        public Func<List<GameModeMap>> GetGameModeMaps;

        public GameModeMapFilter(Func<List<GameModeMap>> filterAction)
        {
            GetGameModeMaps = filterAction;
        }

        public bool Any() => GetGameModeMaps().Any();
    }
}
