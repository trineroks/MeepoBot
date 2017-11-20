//trineroks 2016

using Discord;
using Discord.Audio;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeepoBotCSharp 
{
    public class MeepoBot 
    {
        private DiscordClient client;
        private YoutubeStreamer youtubeStreamer;
        private List<Game> PartyGames = new List<Game>();
        private Random rand = new Random();
        private bool isHostingGame = false;
        private SecretSanta secretSanta;

        public MeepoBot() 
        {
            client = new DiscordClient();
            client.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
            client.MessageReceived += bot_MessageReceived;
            youtubeStreamer = new YoutubeStreamer(client);
            client.ExecuteAndWait(async () => {
                await client.Connect("","");
                while (true) {
                    youtubeStreamer.PlayerLoop();
                    try {
                        if (PartyGames.Any()) {
                            for (int i = 0; i < PartyGames.Count(); i++) {
                                Game game = PartyGames.ElementAt(i);
                                game.GameLoop();
                                if (game.isMarkedForDeletion()) {
                                    await client.GetChannel(game.getParentChannel()).SendMessage(game.getGameType() + SystemMessages.MESSAGE_GAMEDELETED);
                                    await cleanupGame(game);
                                    i--;
                                }
                            }
                        }
                    }
                    catch (Exception) {
                        Console.WriteLine("No games");
                    }
                }
            });
        }

        private void bot_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.IsAuthor)
               return;
            if (PartyGames.Any())
            {
                foreach (Game game in PartyGames)
                {
                    if (e.Server == client.GetServer(game.getGameServerID()))
                    {
                        if (e.Channel == client.GetChannel(game.getTextChannelID()))
                        {
                            game.evaluateInput(e.Message.Text, e);
                            return;
                        }
                    }
                    foreach (ulong ID in game.getAllPlayerIDs())
                    {
                        if (e.User.Id == ID && e.Channel.IsPrivate)
                        {
                            game.evaluateInput(e.Message.Text, e);
                            return;
                        }
                    }
                }
            }
            youtubeStreamer.evaluateInput(e.Message.Text, e);
            if (secretSanta != null) {
                secretSanta.evaluateInput(e.Message.RawText, e);
            }
            parseInput(e.Message.Text, e);
        }

        private async void parseInput(string input, MessageEventArgs e)
        {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            Server server = e.Server;
            if (e.Channel.IsPrivate)
                return;
            else if (command == "")
                return;
            else if (inputLen == 1)
            {
                if (command == Constants.COMMAND_HELP)
                    await e.Channel.SendMessage("Testing Testing 1 2 3");
                else if (command == Constants.COMMAND_DEVTESTNEWLINE)
                {
                    string sentence = "```";
                    for (int i = 0; i < 5; i++)
                    {
                        sentence += i + ") " + "Satsuo" + "\n";
                    }
                    sentence += "```";
                    await e.Channel.SendMessage(sentence);
                }
                else if (command == Constants.COMMAND_LISTUSERS)
                {
                    foreach (Discord.User user in e.Channel.Users)
                    {
                        //TODO: Check for users in a voice channel and only list those
                        await e.Channel.SendMessage(user.ToString());
                        await user.SendMessage("Will this work?");
                    }
                }
                else if (command == Constants.COMMAND_GITHUB)
                {
                    await e.Channel.SendMessage(Constants.URL_GITHUB);
                }
                else if (command == Constants.COMMAND_LISTSERVERSWITHGAMES)
                {
                    if (!PartyGames.Any())
                    {
                        await e.Channel.SendMessage("No server is running a game right now!");
                    }
                    else
                    {
                        for (int i = 0; i < PartyGames.Count(); i++)
                        {
                            Game game = PartyGames.ElementAt(i);
                            await e.Channel.SendMessage("Server " + game.getGameServerID().ToString() + " running game type " + game.getGameType() + " on text channel " +
                                                        game.getTextChannelID().ToString() + " and on voice channel " + game.getVoiceChannelID().ToString());
                        }
                    }
                }
                else if (command == Constants.COMMAND_JOINPARTYGAME)
                {
                    Game game = ServerStartingGame(server);
                    if (game != null)
                    {
                        if (game.playerAlreadyJoined(e.User))
                        {
                            await e.Channel.SendMessage(SystemMessages.MESSAGE_GAMEALREADYJOINED);
                        }
                        else
                        {
                            bool added = await addToRole(game.getPlayerRole(), e.User, e);
                            if (added)
                            {
                                game.addPlayer(e.User);
                                await e.Channel.SendMessage(e.User.NicknameMention + " has joined the " + game.getGameType() + " game. Click " + client.GetChannel(game.getTextChannelID()).Mention + " to enter the text channel.");
                            }
                        }
                    }
                    else
                        await e.Channel.SendMessage(SystemMessages.MESSAGE_GAMESTARTEDORNONE);
                }
                else if (command == Constants.COMMAND_LEAVEPARTYGAME)
                {
                    Game game = ServerStartingGame(server);
                    if (game != null)
                    {
                        if (game.playerAlreadyJoined(e.User))
                        {
                            game.removePlayer(e.User);
                            await e.Channel.SendMessage(e.User.NicknameMention + " has left the " + game.getGameType() + " game.");
                        }
                        else
                        {
                            await e.Channel.SendMessage("You haven't even joined the game!");
                        }
                    }
                    else
                        await e.Channel.SendMessage(SystemMessages.MESSAGE_GAMESTARTEDNOLEAVE);
                }
                else if (commandRollDice(command))
                {
                    string value = command.Remove(0, 2);
                    if (isValidDiceRoll(value))
                    {
                        int rollValue = getDiceRoll(value);
                        await e.Channel.SendMessage(e.User.Mention + " rolled a d" + value + " and got: " + rollValue);
                        return;
                    }
                    else
                    {
                        await e.Channel.SendMessage("USAGE: !d#, where # is between 1-" + Int32.MaxValue + ".");
                        return;
                    }
                }
                //The Secret Santa module
                else if (command == Constants.COMMAND_STARTSECRETSANTA) {
                    secretSanta = new SecretSanta(server, client);
                    Console.WriteLine("Secret Santa initialized");
                }
            }
            else if (inputLen > 1)
            {
                if (command == Constants.COMMAND_SETGAME) {
                    string game = input.Remove(0, Constants.COMMAND_SETGAME.Length + 1);
                    client.SetGame(game);
                }
                else if (command == Constants.COMMAND_CREATECHANNEL) {
                    string channelname = input.Remove(0, Constants.COMMAND_CREATECHANNEL.Length + 1);
                    await createNewChannel(channelname, ChannelType.Text, e);
                }
                else if (command == Constants.COMMAND_TTS) {
                    string message = input.Remove(0, Constants.COMMAND_TTS.Length + 1);
                    await e.Channel.SendTTSMessage(message);
                }
                else if (command == Constants.COMMAND_SETROLE) {
                    string rolename = input.Remove(0, Constants.COMMAND_SETROLE.Length + 1);
                    ServerPermissions permissions = new ServerPermissions(true);
                    try {
                        await addToRole(rolename, permissions, e.User, e);
                    }
                    catch (Exception) {
                        return;
                    }
                }
                else if (command == Constants.COMMAND_STARTDND) {
                    if (!isHostingGame) {
                        Discord.Channel text, voice = null;
                        Discord.Role gamerole = null;
                        string channelname = input.Remove(0, Constants.COMMAND_STARTDND.Length + 1);
                        ServerPermissions permissions = new ServerPermissions();
                        gamerole = await addToRole(Constants.GAME_DNDGAME, permissions, e.User, e);
                        if (gamerole != null) {
                            text = await createNewChannel(channelname, ChannelType.Text, e);
                            if (text != null) {
                                isHostingGame = true;
                                voice = await createNewChannel(channelname + " Voice", ChannelType.Voice, e);
                                ChannelPermissionOverrides memberPermOverride = new ChannelPermissionOverrides(PermValue.Deny, PermValue.Deny, PermValue.Allow,
                                                                                    PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny,
                                                                                    PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny);
                                ChannelPermissionOverrides everyonePermOverride = new ChannelPermissionOverrides(PermValue.Deny, PermValue.Deny, PermValue.Allow,
                                                                                      PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                                                                                      PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny);
                                await text.AddPermissionsRule(gamerole, memberPermOverride);
                                await text.AddPermissionsRule(e.Server.EveryoneRole, everyonePermOverride);
                                Game DnDGame = new DnDGame(server, text, voice, e.Channel, gamerole, client, e.User.Id);
                                DnDGame.addPlayer(e.User);
                                PartyGames.Add(DnDGame);

                                await setChannelToTop(text);
                                await setChannelToTop(voice);

                                await e.Channel.SendMessage(DnDGame.getGameType() + SystemMessages.MESSAGE_GAMECREATED + text.Mention);

                                Console.WriteLine("Game was created at server: " + server.Id);
                            }
                            else {
                                await gamerole.Delete();
                            }
                        }
                    }
                    else {
                        Console.WriteLine(SystemMessages.MESSAGE_GAMEHASSTARTED);
                    }
                }
                else if (command == Constants.COMMAND_STARTRESISTANCE) {
                    bool createGame = false;
                    if (!PartyGames.Any())
                        createGame = true;
                    Game game = null;
                    if (!createGame && PartyGames.Any()) {
                        int i = 0; bool found = false;
                        while (i < PartyGames.Count()) {
                            game = PartyGames.ElementAt(i);
                            if (game.getGameServerID() == server.Id) {
                                found = true;
                            }
                            i++;
                        }
                        createGame = found ? false : true;
                    }
                    if (createGame && !isHostingGame) {
                        Discord.Channel text, voice = null;
                        Discord.Role gamerole = null;
                        string channelname = input.Remove(0, Constants.COMMAND_STARTRESISTANCE.Length + 1);
                        ServerPermissions permissions = new ServerPermissions();
                        gamerole = await addToRole(Constants.GAME_RESISTANCE, permissions, e.User, e);
                        if (gamerole != null) {
                            text = await createNewChannel(channelname, ChannelType.Text, e);
                            if (text != null) {
                                isHostingGame = true;
                                voice = await createNewChannel(channelname + " Voice", ChannelType.Voice, e);
                                ChannelPermissionOverrides memberPermOverride = new ChannelPermissionOverrides(PermValue.Deny, PermValue.Deny, PermValue.Allow,
                                                                                    PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny,
                                                                                    PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny);
                                ChannelPermissionOverrides everyonePermOverride = new ChannelPermissionOverrides(PermValue.Deny, PermValue.Deny, PermValue.Allow,
                                                                                      PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny,
                                                                                      PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Deny);
                                await text.AddPermissionsRule(gamerole, memberPermOverride);
                                await text.AddPermissionsRule(e.Server.EveryoneRole, everyonePermOverride);
                                Game resistanceGame = new ResistanceGame(server, text, voice, e.Channel, gamerole, client, e.User.Id);
                                resistanceGame.addPlayer(e.User);
                                PartyGames.Add(resistanceGame);

                                await setChannelToTop(text);
                                await setChannelToTop(voice);

                                await e.Channel.SendMessage(resistanceGame.getGameType() + SystemMessages.MESSAGE_GAMECREATED + text.Mention);

                                Console.WriteLine("Game was created at server: " + server.Id);
                            }
                            else {
                                await gamerole.Delete();
                            }
                        }
                    }
                    else {
                        Console.WriteLine(SystemMessages.MESSAGE_GAMEHASSTARTED);
                    }
                }
                else if (command == Constants.COMMAND_FLOODMESSAGE) {
                    int repeat;
                    if (Int32.TryParse(toParse[1], out repeat)) {
                        string message = input.Remove(0, Constants.COMMAND_FLOODMESSAGE.Length + toParse[1].Length + 2);
                        for (int i = 0; i < repeat; i++)
                            await e.Channel.SendMessage(message);
                    }
                    else
                        await e.Channel.SendMessage("Invalid use of command, nerd");
                }
            }
            if (Constants.TESTBUILD)
                await e.Channel.SendMessage("Length of input: " + inputLen);
        }

        private async Task<Discord.Role> addToRole(string roleName, ServerPermissions permissions, Discord.User target, MessageEventArgs e)
        {
            Discord.Role newrole = null;
            try
            {
                newrole = await e.Server.CreateRole(roleName, permissions);
                await target.AddRoles(newrole);
            } catch (Exception)
            {
                await newrole.Delete();
                await e.Channel.SendMessage(SystemMessages.MESSAGE_ROLESETEXCEPTION);
                return null;
            }
            return newrole;
        }

        private async Task<bool> addToRole(Discord.Role role, Discord.User target, MessageEventArgs e)
        {
            try
            {
                await target.AddRoles(role);
            } catch (Exception)
            {
                await e.Channel.SendMessage(SystemMessages.MESSAGE_ROLESETEXCEPTION);
                return true;
            }
            return true;
        }

        private bool commandRollDice(string command)
        {
                if (command[0] == '!')
                {
                    if (Char.ToLower(command[1]) == 'd')
                        return true;
                }
            return false;
        }

        private bool isValidDiceRoll(string trimmedCommand)
        {
            int maxValue;
            if (Int32.TryParse(trimmedCommand, out maxValue))
            {
                if (maxValue <= 0 || maxValue >= Int32.MaxValue)
                    return false;
                else
                    return true;
            }
            return false;
        }

        private int getDiceRoll(string trimmedCommand)
        {
            int maxValue;
            Int32.TryParse(trimmedCommand, out maxValue);
            return rand.Next(1, maxValue+1);
        }

        private async Task<Discord.Channel> createNewChannel(string channelName, ChannelType type, MessageEventArgs e)
        {
            Discord.Channel channel = null;
            try
            {
                channel = await e.Server.CreateChannel(channelName, type);
            } catch (Exception)
            {
                await e.Channel.SendMessage(SystemMessages.MESSAGE_CHANNELNAMEEXCEPTION);
            }
            return channel;
        }

        private Game ServerStartingGame(Server server)
        {
            ulong serverID = server.Id;
            if (!PartyGames.Any())
            {
                return null;
            }
            else
            {
                for (int i = 0; i < PartyGames.Count(); i++)
                {
                    Game game = PartyGames.ElementAt(i);
                    if (game.getGameServerID() == serverID)
                    {
                        if (!game.getGameStarted())
                            return game;
                    }
                }
            }
            return null;
        }

        private async Task<bool> cleanupGame(Game game)
        {
            await client.GetChannel(game.getTextChannelID()).Delete();
            await client.GetChannel(game.getVoiceChannelID()).Delete();
            await game.getPlayerRole().Delete();
            Console.WriteLine("Game was ended at server: " + game.getGameServerID());
            PartyGames.Remove(game);
            isHostingGame = false;
            return true;
        }

        private async Task setChannelToTop(Discord.Channel channel)
        {
            await channel.Edit(null, null, 0);
        }

        private string[] customParser(string input)
        {
            string[] toParse = input.Split(',');
            for (int i = 0; i < toParse.Count(); i++)
            {
                toParse[i] = toParse[i].Trim();
            }
            return toParse;
        }

    }
}
