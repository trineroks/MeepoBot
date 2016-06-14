//trineroks 2016

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MeepoBotCSharp
{
    public class ResistanceGame : Game
    {
        private enum GAMEROLE
        {
            RESISTANCE,
            SPY,
            ASSASSIN
        }
        private enum ALLEGIANCE
        {
            RESISTANCE,
            SPY,
            NONALIGNED,
            NULL
        }
        private enum GAMESTATE
        {
            DEFAULT,
            ASSIGNROLE,
            TEAMDRAFT,
            VOTEONTEAM,
            VOTEMISSION,
            SELECTLEADER,
            SELECTNEXTMISSION,
            MISSIONBRIEFING,
            ENDGAME
        }
        private enum TEAMVOTE
        {
            NOTSUBMITTED,
            APPROVED,
            REJECTED
        }
        private enum MISSIONVOTE
        {
            NOTSUBMITTED,
            PASSED,
            FAILED
        }

        private class Mission
        {
            int mission;
            int requiredPlayers;
            int requiredFails;
            bool pass;
            bool completed;
            int numFails;
            List<string> participants = new List<string>();
            public Mission()
            {
                mission = 0;
                requiredPlayers = 0;
                requiredFails = 0;
                numFails = 0;
                completed = false;
                pass = false;
            }
            public Mission(int missionNum, int reqPlayers, int reqFails)
            {
                mission = missionNum;
                requiredPlayers = reqPlayers;
                requiredFails = reqFails;
                numFails = 0;
                completed = false;
                pass = false;
            }
            public void setMissionNumber(int missionNum) { mission = missionNum; }
            public void setRequiredPlayers(int reqPlayers) { requiredPlayers = reqPlayers; }
            public void setRequiredFails(int reqFails) { requiredFails = reqFails; }
            public void setMissionPassed(bool passed) { pass = passed; }
            public void setMissionCompleted(bool complete) { completed = complete; }
            public void setNumberOfFails(int fails) { numFails = fails; }
            public void addParticipant(string name) { participants.Add(name); }
            public int getMissionNumber() { return mission; }
            public int getRequiredPlayers() { return requiredPlayers; }
            public int getRequiredFails() { return requiredFails; }
            public bool MissionPassed() { return pass; }
            public bool MissionCompleted() { return completed; }
            public int NumberOfFails() { return numFails; }
            public List<string> getParticipants() { return participants; }
        }

        private class ResistancePlayer
        {
            ulong playerID;
            GAMEROLE playerRole;
            string playerNickname;
            string playerNonNick;
            TEAMVOTE teamVote;
            MISSIONVOTE missionVote;
            ALLEGIANCE allegiance;
            public ResistancePlayer()
            {
                playerID = 0;
                playerRole = 0;
                playerNickname = "JACK";
                playerNonNick = "JACK";
                allegiance = ALLEGIANCE.NONALIGNED;
                teamVote = TEAMVOTE.NOTSUBMITTED;
                missionVote = MISSIONVOTE.NOTSUBMITTED;
            }
            public ResistancePlayer(ulong ID, string playerName, string playerNonMen)
            {
                playerID = ID;
                playerRole = 0;
                playerNickname = playerName;
                playerNonNick = playerNonMen;
                allegiance = ALLEGIANCE.NONALIGNED;
                teamVote = TEAMVOTE.NOTSUBMITTED;
                missionVote = MISSIONVOTE.NOTSUBMITTED;
            }
            public ResistancePlayer(ulong ID, string playerName, string playerNonMen, GAMEROLE role, ALLEGIANCE playerAllegiance)
            {
                playerID = ID;
                playerRole = role;
                playerNickname = playerName;
                playerNonNick = playerNonMen;
                allegiance = playerAllegiance;
                teamVote = TEAMVOTE.NOTSUBMITTED;
                missionVote = MISSIONVOTE.NOTSUBMITTED;
            }
            public GAMEROLE getPlayerRole() { return playerRole; }
            public ulong getPlayerID() { return playerID; }
            public string getPlayerNickname() { return playerNickname; }
            public string getPlayerName() { return playerNonNick; }
            public string getPlayerRoleString()
            {
                switch(playerRole)
                {
                    case GAMEROLE.RESISTANCE:
                        return "RESISTANCE (RESISTANCE)";
                    case GAMEROLE.SPY:
                        return "SPY (SPY)";
                    case GAMEROLE.ASSASSIN:
                        return "ASSASSIN (SPY)";
                    default:
                        return "NO ROLE";
                }
            }
            public TEAMVOTE getTeamVote() { return teamVote; }
            public MISSIONVOTE getMissionVote() { return missionVote; }
            public ALLEGIANCE getAllegiance() { return allegiance; }
            public void setTeamVote(TEAMVOTE vote) { teamVote = vote; }
            public void setMissionVote(MISSIONVOTE vote) { missionVote = vote; }
            public void setAllegiance(ALLEGIANCE playerAllegiance) { allegiance = playerAllegiance; }
            public void resetTeamVote() { teamVote = TEAMVOTE.NOTSUBMITTED; }
            public void resetMissionVote() { missionVote = MISSIONVOTE.NOTSUBMITTED; }
        }

        private int neededPlayers = 3;
        private List<Mission> missions = new List<Mission>();
        private List<ResistancePlayer> players = new List<ResistancePlayer>();
        private static int rejectsNeededToFail = 5;

        private int currentMission = 1;
        private List<ResistancePlayer> playersOnMission = new List<ResistancePlayer>();
        private ulong missionLeader;
        private int missionLeaderIndex = 0;
        private int teamRejectCount = 0;

        private Random rand = new Random();
        private GAMESTATE STATE = GAMESTATE.DEFAULT;

        public ResistanceGame(Server server, Channel textChannel, Channel voiceChannel, Channel parentChannel, Role playersDiscordRole, DiscordClient botClient, ulong host) 
                                : base(server, textChannel, voiceChannel, parentChannel, playersDiscordRole, botClient, host)
        {
            setGameType(Constants.GAME_RESISTANCE);
        }

        public override void GameLoop()
        {
            if (!getGameStarted())
                checkToStart();
            else
            {
                if (GameState() == GAMESTATE.ASSIGNROLE)
                    setUpGame();
                else if (GameState() == GAMESTATE.SELECTNEXTMISSION)
                    selectNextMissionLogic();
                else if (GameState() == GAMESTATE.MISSIONBRIEFING)
                    missionBriefingLogic();
                else if (GameState() == GAMESTATE.SELECTLEADER)
                    selectLeaderLogic();
                else if (GameState() == GAMESTATE.TEAMDRAFT)
                    teamDraftLogic();
                else if (GameState() == GAMESTATE.VOTEONTEAM)
                    teamVoteLogic();
                else if (GameState() == GAMESTATE.VOTEMISSION)
                    voteMissionLogic();
                else if (GameState() == GAMESTATE.ENDGAME)
                    endGameLogic();
            }
            base.GameLoop();
        }

        public override void checkToStart()
        {
            List<ulong> playerList = new List<ulong>();
            foreach(ulong ID in getAllPlayerIDs())
                playerList.Add(ID);
            if (playerList.Count() == neededPlayers)
            {
                setGameStart();
                string players = "";
                for (int i = 0; i < playerList.Count(); i++)
                {
                    players += getClient().GetServer(getGameServerID()).GetUser(playerList.ElementAt(i)).NicknameMention + " ";
                }
                getClient().GetChannel(getTextChannelID()).SendMessage("The game has started with players " + players);
            }
            setGameState(GAMESTATE.ASSIGNROLE);
        }

        private string getSpies()
        {
            string toSend = "";
            foreach (ResistancePlayer player in players)
            {
                if (player.getAllegiance() == ALLEGIANCE.SPY)
                {
                    toSend += player.getPlayerName() + ", ";
                }
            }
            return toSend;
        }

        private async void endGameLogic()
        {
            setGameForDeletion();
            string spies = "";
            string resistance = "";
            string endMessage = "";
            foreach(ResistancePlayer player in players)
            {
                if (player.getAllegiance() == ALLEGIANCE.SPY)
                {
                    spies += player.getPlayerName() + ", ";
                }
                else if (player.getAllegiance() == ALLEGIANCE.RESISTANCE)
                {
                    resistance += player.getPlayerName() + ", ";
                }
            }
            if (teamRejectCount >= rejectsNeededToFail)
            {
                endMessage += "The Spies win because 5 proposed teams for a mission have been rejected!\n";
                endMessage += "The spies were " + spies + "\n";
                endMessage += "The resistance were " + resistance;
                await getClient().GetChannel(getParentChannel()).SendMessage(endMessage);
            }
            else if (evaluateForGameEnd() == ALLEGIANCE.RESISTANCE)
            {
                endMessage += "The Resistance win by succeeding 3 of the 5 missions!\n";
                endMessage += "The spies were " + spies + "\n";
                endMessage += "The resistance were " + resistance;
                await getClient().GetChannel(getParentChannel()).SendMessage(endMessage);
            }
            else
            {
                endMessage += "The Spies win by sabotaging 3 of the 5 missions!\n";
                endMessage += "The spies were " + spies + "\n";
                endMessage += "The resistance were " + resistance;
                await getClient().GetChannel(getParentChannel()).SendMessage(endMessage);
            }
        }

        private async void voteMissionLogic()
        {
            teamRejectCount = 0;
            Mission theMission = getCurrentMission();
            string waitingOn = "Waiting on the following players to submit their votes for mission " + theMission.getMissionNumber() + ":";
            bool canProceed = true;
            foreach (ResistancePlayer player in playersOnMission)
            {
                if (player.getMissionVote() == MISSIONVOTE.NOTSUBMITTED)
                {
                    waitingOn += player.getPlayerNickname() + ", ";
                    canProceed = false;
                }
            }
            if (gameClock.ElapsedMilliseconds > 20000 && !canProceed)
            {
                await getClient().GetChannel(getTextChannelID()).SendMessage(waitingOn);
                gameClock.Restart();
                return;
            }
            else if (canProceed)
            {
                theMission.setMissionCompleted(true);
                await getClient().GetChannel(getTextChannelID()).SendMessage("The members have returned from their mission.");
                string results = "```Mission "+ theMission.getMissionNumber() + " result (Requires " + theMission.getRequiredFails() + " fails to fail): ";
                results += "``````";
                string pass = "Pass: ";
                string fail = "Fail: ";
                int Passes = 0;
                int Fails = 0;
                foreach (ResistancePlayer player in playersOnMission)
                {
                    if (player.getMissionVote() == MISSIONVOTE.PASSED)
                        Passes++;
                    else
                        Fails++;
                    player.resetMissionVote();
                }
                results += pass + Passes + "\n";
                results += fail + Fails + "```";
                theMission.setNumberOfFails(Fails);
                if (didMissionPass(Fails))
                {
                    results += "The mission was a success.";
                    theMission.setMissionPassed(true);
                }
                else
                {
                    results += "The mission was sabotaged!";
                    theMission.setMissionPassed(false);
                }
                await getClient().GetChannel(getTextChannelID()).SendMessage(results+getCurrentSituationString());
                setGameState(GAMESTATE.SELECTNEXTMISSION);
            }
        }

        private ALLEGIANCE evaluateForGameEnd()
        {
            int fails = 0;
            int passes = 0;
            foreach (Mission mission in missions)
            {
                if(mission.MissionCompleted())
                {
                    if (mission.MissionPassed())
                        passes++;
                    else
                        fails++;
                }
            }
            if (fails >= 3)
                return ALLEGIANCE.SPY;
            else if (passes >= 3)
                return ALLEGIANCE.RESISTANCE;
            else
                return ALLEGIANCE.NULL;
        }

        private async void missionBriefingLogic()
        {
            await getClient().GetChannel(getTextChannelID()).SendMessage(presentMissionInfo());
            setGameState(GAMESTATE.SELECTLEADER);
        }

        private void selectNextMissionLogic()
        {
            setNextMission();
            if (currentMission >= 5)
            {
                setGameState(GAMESTATE.ENDGAME);
            }
            else if (evaluateForGameEnd() != ALLEGIANCE.NULL)
            {
                setGameState(GAMESTATE.ENDGAME);
            }
            else
                setGameState(GAMESTATE.MISSIONBRIEFING);
        }

        private async void selectLeaderLogic()
        {
            setNextLeader();
            await getClient().GetChannel(getTextChannelID()).SendMessage(presentMissionLeader());
            playersOnMission.Clear();
            setGameState(GAMESTATE.TEAMDRAFT);
        }

        private void teamDraftLogic()
        {
            return;
        }

        private async void teamVoteLogic()
        {
            string waitingOn = "Waiting on the following players to submit their votes: ";
            bool canProceed = true;
            foreach (ResistancePlayer player in players)
            {
                if (player.getTeamVote() == TEAMVOTE.NOTSUBMITTED)
                {
                    waitingOn += player.getPlayerNickname() + ", ";
                    canProceed = false;
                }
            }
            if (gameClock.ElapsedMilliseconds > 20000 && !canProceed)
            {
                await getClient().GetChannel(getTextChannelID()).SendMessage(waitingOn);
                gameClock.Restart();
                return;
            }
            else if (canProceed)
            {
                await getClient().GetChannel(getTextChannelID()).SendMessage("The votes have been tallied!");
                string results = "```Proposed Mission Team: ";
                string accept = "Accepted: ";
                string reject = "Rejected: ";
                int teamAccepts = 0;
                int teamUnAccepts = 0;
                foreach (ResistancePlayer player in playersOnMission)
                {
                    results += player.getPlayerName() + ", ";
                }
                results += "``````";
                foreach (ResistancePlayer player in players)
                {
                    if (player.getTeamVote() == TEAMVOTE.APPROVED)
                    {
                        accept += player.getPlayerName() + ", ";
                        teamAccepts++;
                    }
                    else
                    {
                        reject += player.getPlayerName() + ", ";
                        teamUnAccepts++;
                    }
                    player.resetTeamVote();
                }
                results += accept + "\n";
                results += reject + "```";
                await getClient().GetChannel(getTextChannelID()).SendMessage(results);
                if (isTeamAccepted(teamAccepts, teamUnAccepts))
                {
                    await getClient().GetChannel(getTextChannelID()).SendMessage("The team has been accepted by the majority. The mission is a go. " 
                                                                            + missionPlayerListString() + "please PM me or click " + getClient().CurrentUser.Mention 
                                                                            + " to submit your votes for the mission.");
                    foreach (ResistancePlayer player in playersOnMission)
                    {
                        getCurrentMission().addParticipant(player.getPlayerName());
                        if (player.getAllegiance() == ALLEGIANCE.RESISTANCE)
                            await getClient().GetServer(getGameServerID()).GetUser(player.getPlayerID()).SendMessage(getCurrentSituationString()+"Enter !mpass.");
                        else if (player.getAllegiance() == ALLEGIANCE.SPY)
                            await getClient().GetServer(getGameServerID()).GetUser(player.getPlayerID()).SendMessage(getCurrentSituationString() + "!mpass to pass this mission.\n!mfail to sabotage this mission.");
                    }  
                    setGameState(GAMESTATE.VOTEMISSION);
                }
                else
                {
                    await getClient().GetChannel(getTextChannelID()).SendMessage("The team has been rejected by the majority. New team leader will be selected.");
                    teamRejectCount++;
                    if (teamRejectCount >= rejectsNeededToFail)
                        setGameState(GAMESTATE.ENDGAME);
                    else
                        setGameState(GAMESTATE.SELECTLEADER);
                    
                }
            }
        }

        private string missionPlayerListString()
        {
            string toSend = "";
            foreach (ResistancePlayer player in playersOnMission)
                toSend += player.getPlayerName() + ", ";
            return toSend;
        }

        private async void setUpGame()
        {
            missions.Add(new Mission(1, 2, 1));
            missions.Add(new Mission(2, 3, 1));
            missions.Add(new Mission(3, 2, 1));
            missions.Add(new Mission(4, 3, 1));
            missions.Add(new Mission(5, 3, 1));

            List<ulong> holder = new List<ulong>();
            foreach (ulong ID in getAllPlayerIDs())
                holder.Add(ID);
            Server server = getClient().GetServer(getGameServerID());
            int i = 0;
            while (i < 2)
            {
                int pickSpy = rand.Next(0, holder.Count());
                ulong playerID = holder.ElementAt(pickSpy);
                string playerName = server.GetUser(playerID).NicknameMention;
                string playerNonNick = server.GetUser(playerID).Name;
                players.Add(new ResistancePlayer(playerID, playerName, playerNonNick, GAMEROLE.SPY, ALLEGIANCE.SPY));
                holder.RemoveAt(pickSpy);
                i++;
            }
            foreach (ulong playerID in holder)
            {
                string playerName = server.GetUser(playerID).NicknameMention;
                string playerNonNick = server.GetUser(playerID).Name;
                players.Add(new ResistancePlayer(playerID, playerName, playerNonNick, GAMEROLE.RESISTANCE, ALLEGIANCE.RESISTANCE));
            }
            await getClient().GetChannel(getTextChannelID()).SendMessage(SystemMessages.MESSAGE_GAMEROLESDISTRIBUTED);
            foreach (ResistancePlayer player in players)
            {
                await server.GetUser(player.getPlayerID()).SendMessage(provideRoleCardString(player) + provideChannelLink());
            }
            missionLeader = players.ElementAt(missionLeaderIndex).getPlayerID();
            setGameState(GAMESTATE.MISSIONBRIEFING);
        }
        private string presentMissionInfo()
        {
            Mission theMission = getCurrentMission();
            return ("Mission " + theMission.getMissionNumber() + " requires a team of " + theMission.getRequiredPlayers() + ". This mission can be failed with " + theMission.getRequiredFails() + " fails.");
        }

        private string presentMissionLeader()
        {
            ResistancePlayer player = players.ElementAt(missionLeaderIndex);
            return ("The current mission leader is " + player.getPlayerNickname());
        }
        private void setGameState(GAMESTATE state)
        {
            STATE = state;
            gameClock.Restart();
        }
        private GAMESTATE GameState()
        {
            return STATE;
        }

        private bool isTeamAccepted(int acceptCount, int rejectCount)
        {
            return ((acceptCount - rejectCount) < 0) ? false : true;
        }

        private bool didMissionPass(int fail)
        {
            return (fail < getCurrentMission().getRequiredFails()) ? true : false;
        }

        private void setNextLeader()
        {
            missionLeaderIndex++;
            if (missionLeaderIndex >= players.Count())
                missionLeaderIndex = 0;
            missionLeader = players.ElementAt(missionLeaderIndex).getPlayerID();
        }

        private void setNextMission()
        {
            currentMission++;
        }

        private Mission getCurrentMission()
        {
            return missions.ElementAt(currentMission - 1);
        }

        private string getCurrentTeamString()
        {
            Mission thisMission = getCurrentMission();
            string listPlayers = "```";
            listPlayers += "Current team composition for mission " + currentMission + "\n\n";
            if (!playersOnMission.Any())
            {
                listPlayers += "There is no team currently proposed.\n\n";
            }
            else
            {
                for (int i = 0; i < playersOnMission.Count(); i++)
                {
                    int list = i + 1;
                    ResistancePlayer currentPlayer = playersOnMission.ElementAt(i);
                    listPlayers += list + ") " + currentPlayer.getPlayerName() + "\n";
                }   
            }
            listPlayers += "\nThis team requires " + thisMission.getRequiredPlayers() + " players. Spies need " + thisMission.getRequiredFails() + " fails to sabotage this mission.```";
            return listPlayers;
        }

        private string getCurrentSituationString()
        {
            string situationString = "```";
            situationString += "Current state of resistance operations:\n";
            int i = 0;
            foreach (Mission mission in missions)
            {
                situationString += "Mission " + mission.getMissionNumber() + " (" + mission.getRequiredPlayers() + " players needed, " + mission.getRequiredFails() + " fails to fail):";
                if (mission.MissionCompleted())
                {
                    if (mission.MissionPassed())
                        situationString += " SUCCESSFUL.";
                    else
                    {
                        situationString += " FAILED with " + mission.NumberOfFails() + " sabotages.";
                    }
                    situationString += "\nMission participants: ";
                    foreach (string participant in mission.getParticipants())
                    {
                        situationString += participant + ", ";
                    }
                    situationString += "\n";
                }
                else if (i == currentMission - 1)
                    situationString += " CURRENT MISSION.";
                else
                    situationString += " PLANNED.";
                situationString += "\n";
                i++;
            }
            situationString += "```";
            return situationString;
        }

        private ResistancePlayer findPlayer(ulong id)
        {
            foreach (ResistancePlayer player in players)
            {
                if (id == player.getPlayerID())
                    return player;
            }
            return null;
        }

        private string provideChannelLink()
        {
            return "\nClick " + getClient().GetChannel(getTextChannelID()).Mention + " to return to the game channel.";
        }

        private string provideRoleCardString(ResistancePlayer player)
        {
            string toSend = "";
            toSend += "```" + player.getPlayerName() + ", you are a " + player.getPlayerRoleString() + " in a RESISTANCE lobby on server " + getClient().GetServer(getGameServerID()).Name;
            if (player.getAllegiance() == ALLEGIANCE.SPY)
            {
                toSend += "\n\nThe list of spies are: " + getSpies();
            }
            toSend += "```";
            return toSend;
        }

        private string provideHelpString()
        {
            string toSend = "";
            toSend += "**" + Resistance.COMMAND_DRAFT + "**" + " USAGE: !mdraft # # ... - where # corresponds to the # of the player on !mlistplayers. ONLY for mission leaders to draft a team for a mission.\n";
            toSend += "**" + Resistance.COMMAND_CONFIRMDRAFT + "**" + " USAGE: !mconfirm - will lock in the currently drafted team. ONLY for mission leaders to draft a team for a mission.\n";
            toSend += "**" + Resistance.COMMAND_UNDRAFT + "**" + " USAGE: !mclear - will clear the currently drafted team to allow drafting for a new team. ONLY for mission leaders to draft a team for a mission.\n";
            toSend += "**" + Resistance.COMMAND_LISTPLAYERS + "**" + " USAGE: !mlistplayers - returns list of all players in the game.\n";
            toSend += "**" + Resistance.COMMAND_LISTCURRENTTEAM + "**" + " USAGE: !mlistteam - returns list of currently drafted team.\n";
            toSend += "**" + Resistance.COMMAND_LISTSITUATION + "**" + " USAGE: !msituation - returns the list of all missions and their statuses.\n";
            return toSend;
        }

        /*
        private string providePrivateHelpString()
        {

        }
        */

        public override async void evaluateInput(string input, MessageEventArgs e)
        {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            Channel gameChannel = getClient().GetChannel(getTextChannelID());
            Server gameServer = getClient().GetServer(getGameServerID());
            if (e.Channel.IsPrivate)
            {
                string toSend = "";
                ResistancePlayer thisPlayer = findPlayer(e.User.Id);
                if (thisPlayer == null)
                    await getClient().GetChannel(getTextChannelID()).SendMessage("Exception: Resistance Player is null.");
                else if (GameState() == GAMESTATE.VOTEONTEAM)
                {
                    if (command == Resistance.PRIVATECOMMAND_ACCEPTTEAM)
                    {
                        if (thisPlayer.getTeamVote() != TEAMVOTE.NOTSUBMITTED)
                            toSend += "You have already submitted a vote!";
                        else
                        {
                            thisPlayer.setTeamVote(TEAMVOTE.APPROVED);
                            toSend += "You have approved this team draft.";
                        }
                        toSend += provideChannelLink();
                        await e.Channel.SendMessage(toSend);
                    }
                    else if (command == Resistance.PRIVATECOMMAND_REJECTTEAM)
                    {
                        if (thisPlayer.getTeamVote() != TEAMVOTE.NOTSUBMITTED)
                            toSend += "You have already submitted a vote!";
                        else
                        {
                            thisPlayer.setTeamVote(TEAMVOTE.REJECTED);
                            toSend += "You have rejected this team draft.";
                        }
                        toSend += provideChannelLink();
                        await e.Channel.SendMessage(toSend);
                    }
                }
                else if (GameState() == GAMESTATE.VOTEMISSION)
                {
                    if (command == Resistance.PRIVATECOMMAND_PASS)
                    {
                        if (thisPlayer.getMissionVote() != MISSIONVOTE.NOTSUBMITTED)
                            toSend += "You have already submitted a vote!";
                        else
                        {
                            thisPlayer.setMissionVote(MISSIONVOTE.PASSED);
                            toSend += "You have succeeded your objective.";
                        }
                        toSend += provideChannelLink();
                        await e.Channel.SendMessage(toSend);
                    }
                    else if (command == Resistance.PRIVATECOMMAND_FAIL)
                    {
                        if (thisPlayer.getMissionVote() != MISSIONVOTE.NOTSUBMITTED)
                            toSend += "You have already submitted a vote!";
                        else if (thisPlayer.getAllegiance() == ALLEGIANCE.RESISTANCE)
                        {
                            thisPlayer.setMissionVote(MISSIONVOTE.PASSED);
                            toSend += "Resistance members can only choose to pass! Your vote has been submitted as !mpass.";
                        }
                        else
                        {
                            thisPlayer.setMissionVote(MISSIONVOTE.FAILED);
                            toSend += "You have sabotaged this mission.";
                        }
                        toSend += provideChannelLink();
                        await e.Channel.SendMessage(toSend);
                    }
                }
            }
            else if (command == Resistance.COMMAND_HELP)
            {
                await e.Channel.SendMessage(provideHelpString());
            }
            else if (command == Constants.COMMAND_DEVKILLGAME && e.User.Id == getHostID())
            {
                setGameForDeletion();
                await getClient().GetChannel(getParentChannel()).SendMessage("DEV KILLED RESISTANCE LOBBY");
            }
            else if (command == Resistance.COMMAND_LISTPLAYERS)
            {
                string listPlayers = "```";
                for (int i = 0; i < players.Count(); i++)
                {
                    int list = i + 1;
                    ResistancePlayer currentPlayer = players.ElementAt(i);
                    listPlayers += list + ") " + currentPlayer.getPlayerName() + "\n";

                }
                listPlayers += "```";
                await getClient().GetChannel(getTextChannelID()).SendMessage(listPlayers);
            }
            else if (command == Resistance.COMMAND_LISTCURRENTTEAM)
            {
                Mission thisMission = getCurrentMission();
                if (GameState() == GAMESTATE.TEAMDRAFT || GameState() == GAMESTATE.VOTEONTEAM)
                {
                    await getClient().GetChannel(getTextChannelID()).SendMessage(getCurrentTeamString());
                }
            }
            else if (command == Resistance.COMMAND_CONFIRMDRAFT && GameState() == GAMESTATE.TEAMDRAFT)
            {
                string confirm = "";
                if (e.User.Id != missionLeader)
                {
                    confirm += "You cannot draft a team because you are not the mission leader! The current mission leader is: " + players.ElementAt(missionLeaderIndex).getPlayerNickname();
                }
                else if (playersOnMission.Count() == getCurrentMission().getRequiredPlayers())
                {
                    confirm += "After deliberation, " + players.ElementAt(missionLeaderIndex).getPlayerNickname() + " has decided to finalize on the team draft. Go to your PMs (top left corner of Discord client) OR click on " + getClient().CurrentUser.Mention + " and privately message me !maccept if you agree with this team draft, or !mreject if you disagree.";
                    foreach (ResistancePlayer player in players)
                    {
                        await gameServer.GetUser(player.getPlayerID()).SendMessage(getCurrentTeamString() + "!maccept if you agree with this draft. \n!mreject if you disagree with this draft.");
                    }
                    setGameState(GAMESTATE.VOTEONTEAM);
                }
                else
                {
                    confirm += "You must draft a full team before you can confirm it!";
                }
                await gameChannel.SendMessage(confirm);
            }
            else if (command == Resistance.COMMAND_CLEARDRAFT && GameState() == GAMESTATE.TEAMDRAFT)
            {
                string confirm = "";
                if (e.User.Id != missionLeader)
                {
                    confirm += "You cannot draft a team because you are not the mission leader! The current mission leader is: " + players.ElementAt(missionLeaderIndex).getPlayerNickname();
                }
                else
                {
                    confirm += "Team draft is cleared: please start a new draft.";
                    playersOnMission.Clear();
                }
                await gameChannel.SendMessage(confirm);
            }
            else if (command == Resistance.COMMAND_LISTSITUATION)
            {
                await getClient().GetChannel(getTextChannelID()).SendMessage(getCurrentSituationString());
            }
            else if (command == Resistance.COMMAND_DRAFT && GameState() == GAMESTATE.TEAMDRAFT)
            {
                string toSend = "";
                if (e.User.Id != missionLeader)
                {
                    toSend += "You cannot draft a team because you are not the mission leader! The current mission leader is: " + players.ElementAt(missionLeaderIndex).getPlayerNickname();
                    await gameChannel.SendMessage(toSend);
                    return;
                }
                else if (inputLen < 2)
                {
                    toSend += "USAGE: !mdraft # # ..., where # corresponds to the # the player is ordered in !mlistplayers.";
                    await gameChannel.SendMessage(toSend);
                    return;
                }
                else
                {
                    for (int i = 1; i < toParse.Length; i++)
                    {
                        int index;
                        if (playersOnMission.Count() == getCurrentMission().getRequiredPlayers())
                        {
                            toSend += "The team is drafted. Use !mconfirm to confirm this draft or !mclear to clear this draft.";
                            await gameChannel.SendMessage(toSend + getCurrentTeamString());
                            return;
                        }
                        else if (Int32.TryParse(toParse[i], out index))
                        {
                            if (index <= players.Count() && index > 0)
                            {
                                bool dontAdd = false;
                                ResistancePlayer toAddPlayer = players.ElementAt(index - 1);
                                if (!playersOnMission.Any())
                                {
                                    playersOnMission.Add(toAddPlayer);
                                }
                                else
                                {
                                    foreach (ResistancePlayer player in playersOnMission)
                                    {
                                        if (toAddPlayer.getPlayerID() == player.getPlayerID())
                                        {
                                            dontAdd = true;
                                            break;
                                        }
                                    }
                                    if(!dontAdd)
                                        playersOnMission.Add(toAddPlayer);
                                }
                            }
                        }
                    }
                    toSend += getCurrentTeamString();
                }
                await gameChannel.SendMessage(toSend);
            }
        }
    }
}
