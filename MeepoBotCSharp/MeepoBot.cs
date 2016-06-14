//trineroks 2016

using Discord;
using Discord.Audio;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp 
{
    public class MeepoBot 
    {
        private DiscordClient client;
        private IAudioClient audioClient;
        private bool McCree;
        Discord.Channel McCreeChannel = null;
        private List<Game> PartyGames = new List<Game>();

        public MeepoBot() 
        {
            audioClient = null;
            client = new DiscordClient();
            client.UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
            });
            client.MessageReceived += bot_MessageReceived;
            McCree = false;
            client.ExecuteAndWait(async () =>
            {
                await client.Connect("BOTACCOUNTMAIL@EMAIL.COM", "PASSWORD");
                while (true)
                {
                    if (McCree == true)
                    {
                        try
                        {
                            var date = DateTime.Now;
                            Console.WriteLine(date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString() + " at " + McCreeChannel.Id + " on ID " + audioClient.Id);
                            if (date.Minute == 00 && date.Second == 0)
                            {
                                    SendAudio("audio\\highnoon.mp3");
                                    switch (date.Hour)
                                    {
                                        case 0:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Karachi, Pakistan!");
                                            break;
                                        case 1:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Dubai, UAE!");
                                            break;
                                        case 2:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Helsinki, Finland!");
                                            break;
                                        case 3:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Frankfurt, Germany!");
                                            break;
                                        case 4:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Lisbon, Portugal!");
                                            break;
                                        case 5:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Reykjavik, Iceland!");
                                            break;
                                        case 6:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "the Saint Peter and Saint Paul Archipelago!");
                                            break;
                                        case 7:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Nuuk, Greenland!");
                                            break;
                                        case 8:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Brasilia, Brazil!");
                                            break;
                                        case 9:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Caracas, Venezuela!");
                                            break;
                                        case 10:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Winnipeg, Canada!");
                                            break;
                                        case 11:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Salt Lake City, Utah!");
                                            break;
                                        case 12:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Los Angeles, California!");
                                            break;
                                        case 13:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Anchorage, Alaska!");
                                            break;
                                        case 14:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Archipel des Tuamoto!");
                                            break;
                                        case 15:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Honolulu, Hawaii!");
                                            break;
                                        case 16:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "the Midway Islands!");
                                            break;
                                        case 17:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Wake Island!");
                                            break;
                                        case 18:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Palikir, Federated States of Micronesia!");
                                            break;
                                        case 19:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Port Moresby, Papua New Guinea!");
                                            break;
                                        case 20:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Seoul, Republic of Korea!");
                                            break;
                                        case 21:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Shanghai, China!");
                                            break;
                                        case 22:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Hanoi, Vietnam!");
                                            break;
                                        case 23:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage(SystemMessages.MESSAGE_HIGHNOON + "Almaty, Kazakhstan!");
                                            break;
                                        default:
                                            await client.GetChannel(McCreeChannel.Id).SendMessage("It's high noon somewhere!");
                                            break;
                                }
                            }

                        } catch (Exception)
                        {
                            Console.WriteLine("Weird Error");
                        }
                    }
                    try
                    {
                        if (PartyGames.Any())
                        {
                            for (int i = 0; i < PartyGames.Count(); i++)
                            {
                                Game game = PartyGames.ElementAt(i);
                                game.GameLoop();

                                string debug = "";
                                foreach (ulong ID in game.getAllPlayerIDs())
                                {
                                    debug += client.GetServer(game.getGameServerID()).GetUser(ID).Name + " ";
                                }
                                debug += PartyGames.Count();
                                Console.WriteLine(debug);
                                if (game.isMarkedForDeletion())
                                {
                                    await client.GetChannel(game.getParentChannel()).SendMessage(game.getGameType() + SystemMessages.MESSAGE_GAMEDELETED);
                                    cleanupGame(game);
                                    i--;
                                }
                            }
                        }
                    }catch(Exception)
                    {
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
            if (inputLen == 1)
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
                else if (command == Constants.COMMAND_SUMMON)
                {
                    var voiceChannel = e.User.VoiceChannel;
                    audioClient = await client.GetService<AudioService>().Join(voiceChannel);
                }
                else if (command == Constants.COMMAND_TOGGLEMCCREE)
                {
                    McCree = !McCree;
                    McCreeChannel = e.Channel;
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
                                await e.Channel.SendMessage(e.User.NicknameMention + " has joined the Resistance game. Click " + client.GetChannel(game.getTextChannelID()).Mention + " to enter the text channel.");
                            }
                        }
                    }
                    else
                        await e.Channel.SendMessage(SystemMessages.MESSAGE_GAMESTARTEDORNONE);
                }
                else if (command == Constants.COMMAND_CANCELPARTYGAME)
                {
                    Game game = ServerStartingGame(server);
                    if (game != null)
                    {
                        if (!game.getGameStarted())
                        {
                            if (e.User.Id == game.getHostID())
                            {
                                cleanupGame(game);
                                await client.GetChannel(game.getParentChannel()).SendMessage(game.getGameType() + SystemMessages.MESSAGE_GAMECANCELED);
                            }
                            else
                                await e.Channel.SendMessage(SystemMessages.MESSAGE_CANCELNOTHOST);
                        }
                        else
                            await e.Channel.SendMessage(SystemMessages.MESSAGE_CANCELGAMESTARTED);
                    }
                }
            }
            else if (inputLen > 1)
            {
                if (command == Constants.COMMAND_SETGAME)
                {
                    string game = input.Remove(0, Constants.COMMAND_SETGAME.Length + 1);
                    client.SetGame(game);
                }
                else if (command == Constants.COMMAND_CREATECHANNEL)
                {
                    string channelname = input.Remove(0, Constants.COMMAND_CREATECHANNEL.Length + 1);
                    await createNewChannel(channelname, ChannelType.Text, e);
                }
                else if (command == Constants.COMMAND_TTS)
                {
                    string message = input.Remove(0, Constants.COMMAND_TTS.Length + 1);
                    await e.Channel.SendTTSMessage(message);
                }
                else if (command == Constants.COMMAND_SETROLE)
                {
                    string rolename = input.Remove(0, Constants.COMMAND_SETROLE.Length + 1);
                    ServerPermissions permissions = new ServerPermissions(true);
                    try
                    {
                        await addToRole(rolename, permissions, e.User, e);
                    } catch (Exception)
                    {
                        return;
                    }
                }
                else if (command == Constants.COMMAND_STARTRESISTANCE)
                {
                    bool createGame = false;
                    if (!PartyGames.Any())
                        createGame = true;
                    Game game = null;
                    if (!createGame && PartyGames.Any())
                    {
                        int i = 0; bool found = false;
                        while (i < PartyGames.Count())
                        {
                            game = PartyGames.ElementAt(i);
                            if (game.getGameServerID() == server.Id)
                            {
                                found = true;
                            }
                            i++;
                        }
                        createGame = found ? false : true;
                    }
                    if (createGame)
                    {
                        Discord.Channel text, voice = null;
                        Discord.Role gamerole = null;
                        string channelname = input.Remove(0, Constants.COMMAND_STARTRESISTANCE.Length + 1);
                        ServerPermissions permissions = new ServerPermissions();
                        gamerole = await addToRole(Constants.GAME_RESISTANCE, permissions, e.User, e);
                        if (gamerole != null)
                        {
                            text = await createNewChannel(channelname, ChannelType.Text, e);
                            voice = await createNewChannel(channelname + " Voice", ChannelType.Voice, e);
                            if (text != null)
                            {
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
                        }
                    }
                    else
                    {
                        Console.WriteLine(SystemMessages.MESSAGE_GAMEHASSTARTED + game.getGameType());
                    }
                }
                else if (command == Constants.COMMAND_FLOODMESSAGE)
                {
                    int repeat;
                    if (Int32.TryParse(toParse[1], out repeat)) 
                    { 
                    string message = input.Remove(0, Constants.COMMAND_FLOODMESSAGE.Length + toParse[1].Length + 2);
                    for (int i = 0; i < repeat; i++ )
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
                return false;
            }
            return true;
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

        private void cleanupGame(Game game)
        {
            client.GetChannel(game.getTextChannelID()).Delete();
            client.GetChannel(game.getVoiceChannelID()).Delete();
            game.getPlayerRole().Delete();
            Console.WriteLine("Game was ended at server: " + game.getGameServerID());
            PartyGames.Remove(game);
        }

        private async Task setChannelToTop(Discord.Channel channel)
        {
            await channel.Edit(null, null, 0);
        }

        private void SendAudio(string filePath)
        {
            var channelCount = client.GetService<AudioService>().Config.Channels;
            var OutFormat = new WaveFormat(48000, 16, channelCount);
            using (var MP3Reader = new Mp3FileReader(filePath)) 
            using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) 
            {
                resampler.ResamplerQuality = 60; 
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) 
                {
                    if (byteCount < blockSize)
                    {
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    audioClient.Send(buffer, 0, blockSize); 
                }
            }
        }

    }
}
