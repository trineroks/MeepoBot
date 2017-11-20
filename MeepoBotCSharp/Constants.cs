//trineroks 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp
{
    class Constants
    {
        public const string COMMAND_PREFIX = "!m";
        public const string URL_GITHUB = "https://github.com/trineroks/MeepoBot";
        public const string CREATOR = "trineroks";

        public const bool TESTBUILD = false;

        public const string COMMAND_GITHUB = COMMAND_PREFIX + "git";
        public const string COMMAND_HELP = COMMAND_PREFIX + "help";
        public const string COMMAND_SETGAME = COMMAND_PREFIX + "setgame";
        public const string COMMAND_SETNICK = COMMAND_PREFIX + "setnick";
        public const string COMMAND_FLOODMESSAGE = COMMAND_PREFIX + "flood";
        public const string COMMAND_LISTUSERS = COMMAND_PREFIX + "listuser";
        public const string COMMAND_TTS = COMMAND_PREFIX + "tts";
        public const string COMMAND_PERMISSION = COMMAND_PREFIX + "perm";
        public const string COMMAND_CREATECHANNEL = COMMAND_PREFIX + "channel";
        public const string COMMAND_SETROLE = COMMAND_PREFIX + "setrole";

        public const string COMMAND_TOGGLEMCCREE = "!mccree";
        public const string COMMAND_SUMMON = COMMAND_PREFIX + "summon";

        public const string COMMAND_STARTRESISTANCE = COMMAND_PREFIX + "resistance";
        public const string COMMAND_STARTSECRETSANTA = COMMAND_PREFIX + "santa";
        public const string COMMAND_STARTDND = COMMAND_PREFIX + "dnd";
        public const string COMMAND_JOINPARTYGAME = COMMAND_PREFIX + "join";
        public const string COMMAND_LEAVEPARTYGAME = COMMAND_PREFIX + "leave";
        public const string COMMAND_CANCELPARTYGAME = COMMAND_PREFIX + "cancel";
        public const string COMMAND_LISTSERVERSWITHGAMES = COMMAND_PREFIX + "listgames";
        public const string COMMAND_DEVKILLGAME = COMMAND_PREFIX + "kill";
        public const string COMMAND_STARTGAME = COMMAND_PREFIX + "start";
        public const string COMMAND_DEVTESTNEWLINE = COMMAND_PREFIX + "newline";
        public const string COMMAND_DEVCONVERTINT = COMMAND_PREFIX + "convertint";
        public const string COMMAND_CUSTOMPARSER = COMMAND_PREFIX + "parse";
        public const string COMMAND_CUSTOMREMOVER = COMMAND_PREFIX + "remove";

        //GAME CONSTANTS//

        public const long GAME_DELETIONDELAY = 120000; //How long a game can stay unstarted until it's auto deleted

        public const string GAME_RESISTANCE = "RESISTANCE";
        public const string GAME_DNDGAME = "DUNGEONS AND DRAGONS";

        public const string ERROR = "ERROR";

        public class AudioPlayer //TO BE MOVED
        {
            public const float VOLUME = (float)0.4;

            public const string COMMAND_PLAYURL = COMMAND_PREFIX + "play";
            public const string COMMAND_PLAYURLMP3 = COMMAND_PREFIX + "play3";
            public const string COMMAND_PAUSEPLAYER = COMMAND_PREFIX + "pause";
            public const string COMMAND_SKIPPLAYER = COMMAND_PREFIX + "skip";
        }

        public class DnD
        {
            public const string PRIVATECOMMAND_HELP = COMMAND_PREFIX + "help";
            public const string PRIVATECOMMAND_HELPONE = COMMAND_PREFIX + "help1";
            public const string PRIVATECOMMAND_HELPTWO = COMMAND_PREFIX + "help2";

            public const string PRIVATECOMMAND_HOSTLISTCARDS = COMMAND_PREFIX + "list";
            public const string PRIVATECOMMAND_HOSTSELECTCARD = COMMAND_PREFIX + "select";
            
            public const string PRIVATECOMMAND_HOSTEDITELEMENT = COMMAND_PREFIX + "edit";
            public const string PRIVATECOMMAND_HOSTALLEDITELEMENT = COMMAND_PREFIX + "alledit";

            public const string PRIVATECOMMAND_HOSTADDTOLIST = COMMAND_PREFIX + "addto";
            public const string PRIVATECOMMAND_HOSTALLADDTOLIST = COMMAND_PREFIX + "alladdto";

            public const string PRIVATECOMMAND_HOSTREMOVEFROMLIST = COMMAND_PREFIX + "remove";
            public const string PRIVATECOMMAND_HOSTALLREMOVEFROMLIST = COMMAND_PREFIX + "allremove";

            public const string PRIVATECOMMAND_HOSTSORTELEMENT = COMMAND_PREFIX + "sort";
            public const string PRIVATECOMMAND_HOSTALLSORTELEMENT = COMMAND_PREFIX + "allsort";

            public const string PRIVATECOMMAND_HOSTADDFIELDTEXT = COMMAND_PREFIX + "addtext";
            public const string PRIVATECOMMAND_HOSTALLADDFIELDTEXT = COMMAND_PREFIX + "addalltext";

            public const string PRIVATECOMMAND_HOSTADDFIELDLIST = COMMAND_PREFIX + "addlist";
            public const string PRIVATECOMMAND_HOSTALLADDFIELDLIST = COMMAND_PREFIX + "addalllist";

            public const string PRIVATECOMMAND_HOSTDELETEFIELD = COMMAND_PREFIX + "deletefield";
            public const string PRIVATECOMMAND_HOSTALLDELETEFIELD = COMMAND_PREFIX + "alldeletefield";

            public const string PRIVATECOMMAND_HOSTCOMMONFIELDS = COMMAND_PREFIX + "common";

            public const string PRIVATECOMMAND_HOSTDESELECT = COMMAND_PREFIX + "deselect";

            public const string PRIVATECOMMAND_HOSTCREATENPCCARD = COMMAND_PREFIX + "create";
            public const string PRIVATECOMMAND_HOSTDELETENPCCARD = COMMAND_PREFIX + "delete";

            public const string PRIVATECOMMAND_HOSTEDITNPCCARDS = COMMAND_PREFIX + "npc";
            public const string PRIVATECOMMAND_HOSTEDITPLAYERCARDS = COMMAND_PREFIX + "player";

            public const string COMMAND_HOSTPRESENTPLAYER = COMMAND_PREFIX + "showplayer";
            public const string COMMAND_HOSTPRESENTNPC = COMMAND_PREFIX + "shownpc";

            public const string PRIVATECOMMAND_HOSTCLONECARD = COMMAND_PREFIX + "clone"; //only for NPCs
            public const string PRIVATECOMMAND_HOSTEDITTITLE = COMMAND_PREFIX + "title"; //only for NPCs

            public const string PRIVATECOMMAND_GETPLAYERCARD = COMMAND_PREFIX + "card";
            public const string COMMAND_PRINTALLVALUES = COMMAND_PREFIX + "print";
        }

        public class Resistance
        {
            public const string COMMAND_DRAFT = COMMAND_PREFIX + "draft";
            public const string COMMAND_LISTPLAYERS = COMMAND_PREFIX + "players";
            public const string COMMAND_LISTCURRENTTEAM = COMMAND_PREFIX + "team";
            public const string COMMAND_LISTSITUATION = COMMAND_PREFIX + "situation";
            public const string COMMAND_CONFIRMDRAFT = COMMAND_PREFIX + "confirm";
            public const string COMMAND_UNDRAFT = COMMAND_PREFIX + "remove";
            public const string COMMAND_CLEARDRAFT = COMMAND_PREFIX + "clear";
            public const string COMMAND_HELP = COMMAND_PREFIX + "help";

            public const string PRIVATECOMMAND_PASS = COMMAND_PREFIX + "pass";
            public const string PRIVATECOMMAND_FAIL = COMMAND_PREFIX + "fail";
            public const string PRIVATECOMMAND_ACCEPTTEAM = COMMAND_PREFIX + "accept";
            public const string PRIVATECOMMAND_REJECTTEAM = COMMAND_PREFIX + "reject";
            public const string PRIVATECOMMAND_ROLECARD = COMMAND_PREFIX + "card";

            public const long GAME_SYSTEMMESSAGEDELAY = 30000;
        }

        public class SecretSanta
        {
            public const string COMMAND_ADDPARTICIPANT = COMMAND_PREFIX + "sadd";
            public const string COMMAND_STARTSANTA = COMMAND_PREFIX + "sshuffle";
            public const string COMMAND_ENDDATE = COMMAND_PREFIX + "senddate";
            public const string COMMAND_WIPEPARTICIPANTS = COMMAND_PREFIX + "swipe";
            public const string COMMAND_PRINTPARTICIPANTS = COMMAND_PREFIX + "sprint";
            public const string COMMAND_SAVETEST = COMMAND_PREFIX + "ssave";
            public const string COMMAND_LOADTEST = COMMAND_PREFIX + "sload";
            public const string COMMAND_SENDSANTACARDS = COMMAND_PREFIX + "ssend";
            public const string COMMAND_QUIT = COMMAND_PREFIX + "squit";
        }
    }

    class SystemMessages
    {
        public const string MESSAGE_CHANNELNAMEEXCEPTION = "Channel name must be alphanumeric with dashes or underscores!";
        public const string MESSAGE_ROLESETEXCEPTION = "MeepoBot either has insufficient privileges OR is attempting to assign a role to a user higher in the role hierarchy! This can be fixed by dragging MeepoBot's role to the top of the server role hierarchy. But fuck it I'll make an exception for Dylan.";
        public const string MESSAGE_HIGHNOON = "It's high noon at ";

        public const string MESSAGE_GAMESTARTEDORNONE = "A party game has either started or one isn't being hosted yet!";
        public const string MESSAGE_GAMESTARTEDNOLEAVE = "You are either trying to leave a party game that has started, or one isn't being hosted yet!";
        public const string MESSAGE_GAMECANCELED = " lobby has been canceled by the host.";
        public const string MESSAGE_GAMEDELETED = " lobby has been deleted!";
        public const string MESSAGE_CANCELNOTHOST = "You cannot cancel this game; you are not the host!";
        public const string MESSAGE_CANCELGAMESTARTED = "You cannot cancel this game; the game has started!";
        public const string MESSAGE_GAMECREATED = " lobby has been created! To join type " + Constants.COMMAND_JOINPARTYGAME + ". The game will be hosted on the text channel: ";
        public const string MESSAGE_GAMEHASSTARTED = "A party game is already running on this server!";
        public const string MESSAGE_GAMEALREADYJOINED = "You have already joined the party game!";
        public const string MESSAGE_GAMEROLESDISTRIBUTED = "The game roles have been distributed by PM. Check for a PM from me on the top left of your Discord client.";
    }

}
