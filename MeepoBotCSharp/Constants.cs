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
        public static string URL_GITHUB = "https://github.com/trineroks";
        public static string CREATOR = "trineroks";

        public static bool TESTBUILD = false;

        public static string COMMAND_GITHUB = "!mgit";
        public static string COMMAND_HELP = "!mhelp";
        public static string COMMAND_SETGAME = "!msetgame";
        public static string COMMAND_SETNICK = "!msetnick";
        public static string COMMAND_FLOODMESSAGE = "!mflood";
        public static string COMMAND_LISTUSERS = "!mlistuser";
        public static string COMMAND_TTS = "!mtts";
        public static string COMMAND_PERMISSION = "!mperm";
        public static string COMMAND_CREATECHANNEL = "!mchannel";
        public static string COMMAND_SETROLE = "!msetrole";

        public static string COMMAND_TOGGLEMCCREE = "!mccree";
        public static string COMMAND_SUMMON = "!msummon";

        public static string COMMAND_STARTRESISTANCE = "!mresistance";
        public static string COMMAND_JOINPARTYGAME = "!mjoin";
        public static string COMMAND_LEAVEPARTYGAME = "!mleave";
        public static string COMMAND_CANCELPARTYGAME = "!mcancel";
        public static string COMMAND_LISTSERVERSWITHGAMES = "!mlistgames";
        public static string COMMAND_DEVKILLGAME = "!mkill";
        public static string COMMAND_DEVTESTNEWLINE = "!mnewline";
        public static string COMMAND_DEVCONVERTINT = "!mconvertint";

        //GAME CONSTANTS//

        public static long GAME_DELETIONDELAY = 120000; //How long a game can stay unstarted until it's auto deleted

        public static string GAME_RESISTANCE = "RESISTANCE";

        public static string ERROR = "ERROR";
    }

    class SystemMessages
    {
        public static string MESSAGE_CHANNELNAMEEXCEPTION = "Channel name must be alphanumeric with dashes or underscores!";
        public static string MESSAGE_ROLESETEXCEPTION = "MeepoBot either has insufficient privileges OR is attempting to assign a role to a user higher in the role hierarchy! This can be fixed by dragging MeepoBot's role to the top of the server role hierarchy.";
        public static string MESSAGE_HIGHNOON = "It's high noon at ";

        public static string MESSAGE_GAMESTARTEDORNONE = "A party game has either started or one isn't being hosted yet!";
        public static string MESSAGE_GAMECANCELED = " lobby has been canceled by the host.";
        public static string MESSAGE_GAMEDELETED = " lobby has been deleted!";
        public static string MESSAGE_CANCELNOTHOST = "You cannot cancel this game; you are not the host!";
        public static string MESSAGE_CANCELGAMESTARTED = "You cannot cancel this game; the game has started!";
        public static string MESSAGE_GAMECREATED = " lobby has been created! To join type !mjoin. The game will be hosted on the text channel: #";
        public static string MESSAGE_GAMEHASSTARTED = "A party game is already running on this server: ";
        public static string MESSAGE_GAMEALREADYJOINED = "You have already joined the party game!";
        public static string MESSAGE_GAMEROLESDISTRIBUTED = "The game roles have been distributed by PM. Check for a PM from me on the top left of your Discord client.";
    }

    class Resistance
    {
        public static string COMMAND_DRAFT = "!mdraft";
        public static string COMMAND_LISTPLAYERS = "!mlistplayers";
        public static string COMMAND_LISTCURRENTTEAM = "!mlistteam";
        public static string COMMAND_LISTSITUATION = "!msituation";
        public static string COMMAND_CONFIRMDRAFT = "!mconfirm";
        public static string COMMAND_UNDRAFT = "!mremove";
        public static string COMMAND_CLEARDRAFT = "!mclear";
        public static string COMMAND_HELP = "!mhelp";

        public static string PRIVATECOMMAND_PASS = "!mpass";
        public static string PRIVATECOMMAND_FAIL = "!mfail";
        public static string PRIVATECOMMAND_ACCEPTTEAM = "!maccept";
        public static string PRIVATECOMMAND_REJECTTEAM = "!mreject";
    }
}
