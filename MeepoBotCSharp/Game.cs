//trineroks 2016

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp
{
    
    public class Game
    {
        private ulong ServerID;
        private ulong TextChannelID;
        private ulong VoiceChannelID;
        private ulong ParentChannelID;
        private string GameType;
        private bool gameStarted = false;
        private DiscordClient client;
        private Role restrictedToDiscordRole;
        private ulong Host;
        private bool flagForDeletion = false;
        private List<ulong> Players = new List<ulong>();
        public Stopwatch gameClock = new Stopwatch();
        public Game(Server server, Channel textChannel, Channel voiceChannel, Channel parentChannel, Role playerDiscordRole, DiscordClient botClient, ulong host)
        {
            ServerID = server.Id;
            VoiceChannelID = voiceChannel.Id;
            TextChannelID = textChannel.Id;
            ParentChannelID = parentChannel.Id;
            restrictedToDiscordRole = playerDiscordRole;
            Host = host;
            client = botClient;
            gameClock.Start();
        }


        //IMPLEMENT THESE IN EVERY NEW GAME MODULE

        public virtual void GameLoop()
        {
            if (!gameStarted && gameClock.ElapsedMilliseconds >= Constants.GAME_DELETIONDELAY)
                setGameForDeletion();
        }
        public virtual void checkToStart() { }
        public virtual async void evaluateInput(string input, MessageEventArgs e) { }

        //IMPLEMENT THESE IN EVERY NEW GAME MODULE
        public DiscordClient getClient()
        {
            return client;
        }
        public void setGameForDeletion()
        {
            flagForDeletion = true;
        }
        public bool isMarkedForDeletion()
        {
            return flagForDeletion;
        }
        public ulong getParentChannel()
        {
            return ParentChannelID;
        }
        public ulong getHostID()
        {
            return Host;
        }
        public Role getPlayerRole()
        {
            return restrictedToDiscordRole;
        }
        public bool playerAlreadyJoined(User user)
        {
            return Players.Contains(user.Id);
        }
        public void addPlayer(User user)
        {
            Players.Add(user.Id);
        }
        public void removePlayer(User user)
        {
            Players.Remove(user.Id);
        }
        public ulong getPlayerID(int index)
        {
            return Players.ElementAt(index);
        }
        public List<ulong> getAllPlayerIDs()
        {
            return Players;
        }
        public void clearPlayers()
        {
            Players.Clear();
        }
        public bool getGameStarted()
        {
            return gameStarted;
        }
        public void setGameStart()
        {
            gameStarted = true;
        }
        public ulong getGameServerID()
        {
            return ServerID;
        }
        public string getGameType()
        {
            return GameType;
        }
        public void setGameType(string type)
        {
            GameType = type;
        }
        public ulong getVoiceChannelID()
        {
            return VoiceChannelID;
        }
        public ulong getTextChannelID()
        {
            return TextChannelID;
        }
    }
}
