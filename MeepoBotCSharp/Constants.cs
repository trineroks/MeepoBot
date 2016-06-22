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
        public static string COMMAND_PREFIX = "!m";
        public static string URL_GITHUB = "https://github.com/trineroks/MeepoBot";
        public static string CREATOR = "trineroks";

        public static bool TESTBUILD = false;

        public static string COMMAND_GITHUB = COMMAND_PREFIX + "git";
        public static string COMMAND_HELP = COMMAND_PREFIX + "help";
        public static string COMMAND_SETGAME = COMMAND_PREFIX + "setgame";
        public static string COMMAND_SETNICK = COMMAND_PREFIX + "setnick";
        public static string COMMAND_FLOODMESSAGE = COMMAND_PREFIX + "flood";
        public static string COMMAND_LISTUSERS = COMMAND_PREFIX + "listuser";
        public static string COMMAND_TTS = COMMAND_PREFIX + "tts";
        public static string COMMAND_PERMISSION = COMMAND_PREFIX + "perm";
        public static string COMMAND_CREATECHANNEL = COMMAND_PREFIX + "channel";
        public static string COMMAND_SETROLE = COMMAND_PREFIX + "setrole";

        public static string COMMAND_TOGGLEMCCREE = "!mccree";
        public static string COMMAND_SUMMON = COMMAND_PREFIX + "summon";

        public static string COMMAND_STARTRESISTANCE = COMMAND_PREFIX + "resistance";
        public static string COMMAND_STARTDND = COMMAND_PREFIX + "dnd";
        public static string COMMAND_JOINPARTYGAME = COMMAND_PREFIX + "join";
        public static string COMMAND_LEAVEPARTYGAME = COMMAND_PREFIX + "leave";
        public static string COMMAND_CANCELPARTYGAME = COMMAND_PREFIX + "cancel";
        public static string COMMAND_LISTSERVERSWITHGAMES = COMMAND_PREFIX + "listgames";
        public static string COMMAND_DEVKILLGAME = COMMAND_PREFIX + "kill";
        public static string COMMAND_STARTGAME = COMMAND_PREFIX + "start";
        public static string COMMAND_DEVTESTNEWLINE = COMMAND_PREFIX + "newline";
        public static string COMMAND_DEVCONVERTINT = COMMAND_PREFIX + "convertint";
        public static string COMMAND_CUSTOMPARSER = COMMAND_PREFIX + "parse";
        public static string COMMAND_CUSTOMREMOVER = COMMAND_PREFIX + "remove";

        //GAME CONSTANTS//

        public static long GAME_DELETIONDELAY = 120000; //How long a game can stay unstarted until it's auto deleted

        public static string GAME_RESISTANCE = "RESISTANCE";
        public static string GAME_DNDGAME = "DUNGEONS AND DRAGONS";

        public static string ERROR = "ERROR";

        public class AudioPlayer //TO BE MOVED
        {
            public static float VOLUME = (float)0.4;

            public static string COMMAND_PLAYURL = COMMAND_PREFIX + "play";
            public static string COMMAND_PLAYURLMP3 = COMMAND_PREFIX + "play3";
            public static string COMMAND_PAUSEPLAYER = COMMAND_PREFIX + "pause";
            public static string COMMAND_SKIPPLAYER = COMMAND_PREFIX + "skip";
        }

        public class DnD
        {
            public static string PRIVATECOMMAND_HELP = COMMAND_PREFIX + "help";
            public static string PRIVATECOMMAND_HELPONE = COMMAND_PREFIX + "help1";
            public static string PRIVATECOMMAND_HELPTWO = COMMAND_PREFIX + "help2";

            public static string PRIVATECOMMAND_HOSTLISTCARDS = COMMAND_PREFIX + "list";
            public static string PRIVATECOMMAND_HOSTSELECTCARD = COMMAND_PREFIX + "select";
            
            public static string PRIVATECOMMAND_HOSTEDITELEMENT = COMMAND_PREFIX + "edit";
            public static string PRIVATECOMMAND_HOSTALLEDITELEMENT = COMMAND_PREFIX + "alledit";

            public static string PRIVATECOMMAND_HOSTADDTOLIST = COMMAND_PREFIX + "addto";
            public static string PRIVATECOMMAND_HOSTALLADDTOLIST = COMMAND_PREFIX + "alladdto";

            public static string PRIVATECOMMAND_HOSTREMOVEFROMLIST = COMMAND_PREFIX + "remove";
            public static string PRIVATECOMMAND_HOSTALLREMOVEFROMLIST = COMMAND_PREFIX + "allremove";

            public static string PRIVATECOMMAND_HOSTSORTELEMENT = COMMAND_PREFIX + "sort";
            public static string PRIVATECOMMAND_HOSTALLSORTELEMENT = COMMAND_PREFIX + "allsort";

            public static string PRIVATECOMMAND_HOSTADDFIELDTEXT = COMMAND_PREFIX + "addtext";
            public static string PRIVATECOMMAND_HOSTALLADDFIELDTEXT = COMMAND_PREFIX + "addalltext";

            public static string PRIVATECOMMAND_HOSTADDFIELDLIST = COMMAND_PREFIX + "addlist";
            public static string PRIVATECOMMAND_HOSTALLADDFIELDLIST = COMMAND_PREFIX + "addalllist";

            public static string PRIVATECOMMAND_HOSTDELETEFIELD = COMMAND_PREFIX + "deletefield";
            public static string PRIVATECOMMAND_HOSTALLDELETEFIELD = COMMAND_PREFIX + "alldeletefield";

            public static string PRIVATECOMMAND_HOSTCOMMONFIELDS = COMMAND_PREFIX + "common";

            public static string PRIVATECOMMAND_HOSTDESELECT = COMMAND_PREFIX + "deselect";

            public static string PRIVATECOMMAND_HOSTCREATENPCCARD = COMMAND_PREFIX + "create";
            public static string PRIVATECOMMAND_HOSTDELETENPCCARD = COMMAND_PREFIX + "delete";

            public static string PRIVATECOMMAND_HOSTEDITNPCCARDS = COMMAND_PREFIX + "npc";
            public static string PRIVATECOMMAND_HOSTEDITPLAYERCARDS = COMMAND_PREFIX + "player";

            public static string COMMAND_HOSTPRESENTPLAYER = COMMAND_PREFIX + "showplayer";
            public static string COMMAND_HOSTPRESENTNPC = COMMAND_PREFIX + "shownpc";

            public static string PRIVATECOMMAND_HOSTCLONECARD = COMMAND_PREFIX + "clone"; //only for NPCs
            public static string PRIVATECOMMAND_HOSTEDITTITLE = COMMAND_PREFIX + "title"; //only for NPCs

            public static string PRIVATECOMMAND_GETPLAYERCARD = COMMAND_PREFIX + "card";
            public static string COMMAND_PRINTALLVALUES = COMMAND_PREFIX + "print";
        }

        public class Resistance
        {
            public static string COMMAND_DRAFT = COMMAND_PREFIX + "draft";
            public static string COMMAND_LISTPLAYERS = COMMAND_PREFIX + "players";
            public static string COMMAND_LISTCURRENTTEAM = COMMAND_PREFIX + "team";
            public static string COMMAND_LISTSITUATION = COMMAND_PREFIX + "situation";
            public static string COMMAND_CONFIRMDRAFT = COMMAND_PREFIX + "confirm";
            public static string COMMAND_UNDRAFT = COMMAND_PREFIX + "remove";
            public static string COMMAND_CLEARDRAFT = COMMAND_PREFIX + "clear";
            public static string COMMAND_HELP = COMMAND_PREFIX + "help";

            public static string PRIVATECOMMAND_PASS = COMMAND_PREFIX + "pass";
            public static string PRIVATECOMMAND_FAIL = COMMAND_PREFIX + "fail";
            public static string PRIVATECOMMAND_ACCEPTTEAM = COMMAND_PREFIX + "accept";
            public static string PRIVATECOMMAND_REJECTTEAM = COMMAND_PREFIX + "reject";
            public static string PRIVATECOMMAND_ROLECARD = COMMAND_PREFIX + "card";

            public static long GAME_SYSTEMMESSAGEDELAY = 30000;
        }
    }

    class SystemMessages
    {
        public static string MESSAGE_CHANNELNAMEEXCEPTION = "Channel name must be alphanumeric with dashes or underscores!";
        public static string MESSAGE_ROLESETEXCEPTION = "MeepoBot either has insufficient privileges OR is attempting to assign a role to a user higher in the role hierarchy! This can be fixed by dragging MeepoBot's role to the top of the server role hierarchy. But fuck it I'll make an exception for Dylan.";
        public static string MESSAGE_HIGHNOON = "It's high noon at ";

        public static string MESSAGE_GAMESTARTEDORNONE = "A party game has either started or one isn't being hosted yet!";
        public static string MESSAGE_GAMESTARTEDNOLEAVE = "You are either trying to leave a party game that has started, or one isn't being hosted yet!";
        public static string MESSAGE_GAMECANCELED = " lobby has been canceled by the host.";
        public static string MESSAGE_GAMEDELETED = " lobby has been deleted!";
        public static string MESSAGE_CANCELNOTHOST = "You cannot cancel this game; you are not the host!";
        public static string MESSAGE_CANCELGAMESTARTED = "You cannot cancel this game; the game has started!";
        public static string MESSAGE_GAMECREATED = " lobby has been created! To join type " + Constants.COMMAND_JOINPARTYGAME + ". The game will be hosted on the text channel: ";
        public static string MESSAGE_GAMEHASSTARTED = "A party game is already running on this server!";
        public static string MESSAGE_GAMEALREADYJOINED = "You have already joined the party game!";
        public static string MESSAGE_GAMEROLESDISTRIBUTED = "The game roles have been distributed by PM. Check for a PM from me on the top left of your Discord client.";
    }

}
