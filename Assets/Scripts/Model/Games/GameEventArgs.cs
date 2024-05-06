using System;
using System.Collections.Generic;
using Model.Games.Players;

namespace Model.Games
{
    public class GameEventArgs : EventArgs
    {
        public bool IsPlayerWin { get; set; } = false;
        public string PlayerName { get; set; } = string.Empty;
        public List<IPlayer> Players = new();
    }
}