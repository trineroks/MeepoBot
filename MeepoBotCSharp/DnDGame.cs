//trineroks 2016

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* TODO LIST
 * 
 * FEEDBACK FOR HOST WHEN ENTERING COMMANDS
 * PROVIDE REDIRECT LINK BACK TO GAME CHANNEL FROM PM
 * ALLOW HOST TO VIEW ALL COMMON FIELDS ACROSS EVERY PLAYER
 * ALLOW HOST TO VIEW CONTENTS OF EVERY PLAYER THAT HAS THAT RESPECTIVE FIELD
 * 
 */
namespace MeepoBotCSharp
{
    class DnDGame : Game
    {
        private enum GAMESTATE
        {
            DEFAULT,
            PLAYING,
            ENDGAME
        }

        private class Element
        {
            private string _name;
            public Element(string name)
            {
                _name = name;
            }
            public string getName()
            {
                return _name;
            }
            public void setName(string name)
            {
                _name = name;
            }
        }

        private class FieldElement : Element
        {
            private string _content;

            public FieldElement() : base("N/A")
            {
                _content = "N/A";
            }
            public FieldElement(string name, string content = "N/A")
                : base(name)
            {
                _content = content;
            }
            public string getContent()
            {
                return _content;
            }
            public void setContent(string content)
            {
                _content = content;
            }
        }

        private class ListElement : Element
        {
            private List<string> _content = new List<string>();

            public ListElement() : base("N/A") { }

            public ListElement(string name, string parseContent = null)
                : base(name)
            {
                if (parseContent != null)
                {
                    string[] toParse = customParser(parseContent);
                    for (int i = 0; i < toParse.Count(); i++)
                    {
                        _content.Add(toParse[i]);
                    }
                }
            }

            public void addToList(string parseContent)
            {
                string[] toParse = customParser(parseContent);
                for (int i = 0; i < toParse.Count(); i++)
                {
                    _content.Add(toParse[i]);
                }
            }

            public int listSize()
            {
                return _content.Count();
            }

            public void clearList()
            {
                _content.Clear();
            }

            public bool removeFromList(string remove)
            {
                string toPass = remove.Trim(' ');
                foreach (string contents in _content)
                {
                    if(contents.Contains(toPass))
                    {
                        _content.Remove(contents);
                        return true;
                    }
                }
                return false;
            }

            public bool removeFromList(int userIndex)
            {
                int index = userIndex - 1;
                if (index >= 0 && index < _content.Count())
                {
                    _content.RemoveAt(index);
                    return true;
                }
                return false;
            }

            public string getListAsString()
            {
                string toSend = "";
                foreach (string element in _content)
                {
                    toSend += element + ", ";
                }
                toSend = toSend.TrimEnd(' ');
                toSend = toSend.TrimEnd(',');
                return toSend;
            }

            private string[] customParser(string input)
            {
                string[] toParse = input.Split(',');
                for (int i=0; i < toParse.Count(); i++)
                {
                    toParse[i] = toParse[i].Trim();
                }
                return toParse;
            }
        }

        private class Card
        {
            private List<Element> _fields = new List<Element>();
            private string _name;
            
            public Card(string name)
            {
                _name = name;
            }

            public string getName()
            {
                return _name;
            }

            public void setName(string name)
            {
                _name = name;
            }

            public Card addElement(Element element)
            {
                foreach (Element cardelements in _fields)
                {
                    if (cardelements.getName() == element.getName()) //no duplicates allowed to avoid confusion
                        return this;
                }
                _fields.Add(element);
                return this;
            }

            public Card addElementAt(Element element, int pos)
            {
                if (pos >= 0 && pos < _fields.Count())
                    _fields.Insert(pos, element);
                return this;
            }

            public bool removeElement(int userIndex)
            {
                int index = userIndex - 1;
                if (index >= 0 && index < _fields.Count())
                {
                    _fields.RemoveAt(index);
                    return true;
                }
                return false;
            }

            public bool removeElement(string name)
            {
                string toPass = name.Trim(' ');
                foreach (Element element in _fields)
                {
                    if (element.getName().Contains(toPass))
                    {
                        _fields.Remove(element);
                        return true;
                    }
                }
                return false;
            }

            public Element getElement(int userIndex)
            {
                int index = userIndex - 1;
                if (index >= 0 && index < _fields.Count())
                {
                    return _fields.ElementAt(index);
                }
                return null;
            }

            public Element getElement(string name)
            {
                string toPass = name.Trim(' ');
                foreach (Element element in _fields)
                {
                    if (element.getName().Contains(toPass))
                        return element;
                }
                return null;
            }

            public List<Element> getElements()
            {
                return _fields;
            }

            public string cardToString()
            {
                string toSend = "";
                toSend += "```" + _name + "\'s Card:\n\n";
                int i = 1;
                foreach (Element element in _fields)
                {
                    toSend += i + ") " + element.getName() + checkIfList(element) + ": " + retrieveStringsFromElement(element) + "\n";
                    i++;
                }
                toSend += "```";
                return toSend;
            }

            private string checkIfList(Element element)
            {
                ListElement listElement = element as ListElement;
                if (listElement != null)
                {
                    return "(list)";
                }
                return "";
            }

            private string retrieveStringsFromElement(Element element)
            {
                FieldElement strElement = element as FieldElement;
                ListElement listElement = element as ListElement;
                if (strElement != null)
                {
                    return getFieldElement(strElement);
                }
                else if (listElement != null)
                {
                    return getListElement(listElement);
                }
                return "";
            }

            private string getFieldElement(FieldElement element)
            {
                return element.getContent();
            }

            private string getListElement(ListElement element)
            {
                return element.getListAsString();
            }
        }

        private class PlayerCard : Card
        {
            private ulong _ID;
            public PlayerCard(string name, ulong ID)
                : base(name)
            {
                _ID = ID;
            }
            public ulong getPlayerID()
            {
                return _ID;
            }
        }

        private Random rand = new Random();
        private GAMESTATE STATE = GAMESTATE.DEFAULT;
        private int minimumPlayers = 2;
        private List<PlayerCard> playerCards = new List<PlayerCard>();
        private bool start = false;
        private PlayerCard HostCurrentCard;

        public DnDGame(Server server, Channel textChannel, Channel voiceChannel, Channel parentChannel, Role playersDiscordRole, DiscordClient botClient, ulong host) 
                                : base(server, textChannel, voiceChannel, parentChannel, playersDiscordRole, botClient, host)
        {
            setGameType(Constants.GAME_DNDGAME);
        }

        public override void GameLoop()
        {
            if (!getGameStarted())
                checkToStart();
            else
            {
                if (GameState() == GAMESTATE.ENDGAME)
                {

                }
            }
            base.GameLoop();
        }

        public override async void checkToStart()
        {
            int playerCount = getAllPlayerIDs().Count();
            if (playerCount >= minimumPlayers && start)
            {
                initializeGame();
                return;
            }
            else if (start)
            {
                int atLeast = minimumPlayers - playerCount;
                await getClient().GetChannel(getTextChannelID()).SendMessage("Not enough players to start! You need at least " + atLeast + " more players to start!");
                start = false;
            }
        }

        private async void initializeGame()
        {
            HostCurrentCard = null;
            setGameStart();
            string playersInGame = "";
            foreach (ulong ID in getAllPlayerIDs())
            {
                if (ID != getHostID())
                {
                    playersInGame += getClient().GetServer(getGameServerID()).GetUser(ID).NicknameMention + " ";
                    PlayerCard card = new PlayerCard(getClient().GetServer(getGameServerID()).GetUser(ID).Name, ID);
                    card.addElement(new FieldElement("Name"))
                        .addElement(new FieldElement("Level"))
                        .addElement(new FieldElement("Race"))
                        .addElement(new FieldElement("Class"))
                        .addElement(new ListElement("Inventory"));
                    playerCards.Add(card);
                }
            }
            await getClient().GetChannel(getTextChannelID()).SendMessage("The game has started with players " + playersInGame);
            setGameState(GAMESTATE.PLAYING);
        }

        public override async void evaluateInput(string input, MessageEventArgs e)
        {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            Channel gameChannel = getClient().GetChannel(getTextChannelID());
            Server gameServer = getClient().GetServer(getGameServerID());
            if (command == Constants.COMMAND_CANCELPARTYGAME && e.User.Id == getHostID())
            {
                if (!getGameStarted())
                {
                    setGameForDeletion();
                    await getClient().GetChannel(getParentChannel()).SendMessage(getGameType() + SystemMessages.MESSAGE_GAMECANCELED);
                }
            }

            else if (command == Constants.COMMAND_DEVKILLGAME)
            {
                setGameForDeletion();
            }

            else if (command == Constants.COMMAND_STARTGAME && e.User.Id == getHostID())
            {
                start = true;
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
            else if (!getGameStarted())
                return;
            else
            {
                if (e.Channel.IsPrivate)
                {
                    string toSend = "";
                    PlayerCard playerCard = findPlayer(e.User.Id);
                    if (playerCard == null && e.User.Id != getHostID())
                        await getClient().GetChannel(getTextChannelID()).SendMessage("Exception: DnD Player is null.");
                    if (e.User.Id != getHostID())
                    {
                        if (command == Constants.DnD.PRIVATECOMMAND_GETPLAYERCARD)
                        {
                            toSend += playerCard.cardToString();
                            await e.Channel.SendMessage(toSend);
                            return;
                        }
                    }
                    else if (e.User.Id == getHostID())
                    {
                        if (command == Constants.DnD.PRIVATECOMMAND_HOSTLISTPLAYERS)
                        {
                            deselectCard();
                            await e.Channel.SendMessage(getPlayerList());
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTDESELECT)
                        {
                            deselectCard();
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD)
                        {
                            if (toParse.Count() < 2 || toParse.Count() > 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD
                                                            + " #/name, where # corresponds to the player's position on "
                                                            + Constants.DnD.PRIVATECOMMAND_HOSTLISTPLAYERS + " OR name.");
                                return;
                            }
                            else
                            {
                                PlayerCard card = retrievePlayerCard(toParse[1]);
                                deselectCard();
                                if (card != null)
                                {
                                    toSend += card.cardToString();
                                    selectCard(card);
                                    await e.Channel.SendMessage(toSend);
                                }
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT)
                        {
                            if (isCardSelected()) //if card is selected, no need to check for player argument
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT
                                                                + " #/name content, where #/name corresponds to the element's position on the player card OR element name, and content is what you want on that field.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    modifyContent(toModify, content);
                                }
                                deselectCard();
                                return;
                            }
                            else //if card isn't selected, first argument is player
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT
                                                                + " #/name #/name content, where #/name corresponds to the player's position on the player list OR player name, #/name corresponds to the element's position OR element's name, and content is what you want on that field.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        modifyContent(toModify, content);
                                    }
                                }
                                return;
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLEDITELEMENT)
                        {
                            if (toParse.Count() < 3)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLEDITELEMENT
                                                            + " name content, where name corresponds to element name, and content is what you want on that field. Will modify all players who have this field.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTALLEDITELEMENT.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            foreach (PlayerCard card in playerCards)
                            {
                                Element toEdit = card.getElement(toParse[1]);
                                if (toEdit != null)
                                {
                                    modifyContent(toEdit, content);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT)
                        {
                            if (isCardSelected())
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " name #, where name corresponds to element name, and # is where on the playercard you want that field to be in.");
                                    return;
                                }
                                int playerIndex;
                                if (Int32.TryParse(toParse[2], out playerIndex))
                                {
                                    sortElement(HostCurrentCard, toParse[1], playerIndex);
                                    deselectCard();
                                    return;
                                }
                                else
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " name #, where #/name corresponds to element name, and # is where on the playercard you want that field to be in.");
                                    return;
                                }
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " #/name name #, where #/name corresponds to player's name/position, name is the name of the element, and # is where on the playercard you want that field to be in.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    int playerIndex;
                                    if (Int32.TryParse(toParse[3], out playerIndex))
                                    {
                                        sortElement(cardToModify, toParse[2], playerIndex);
                                        return;
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " #/name name #, where #/name corresponds to player's name/position, name is the name of the element, and # is where on the playercard you want that field to be in.");
                                        return;
                                    }
                                }
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLSORTELEMENT)
                        {
                            if (toParse.Count() < 3)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLSORTELEMENT
                                                                + " name #, where #/name corresponds to element name, and # is where on the playercard you want that field to be in. Will modify all players who have this field.");
                                return;
                            }
                            deselectCard();
                            int playerIndex;
                            if (Int32.TryParse(toParse[2], out playerIndex))
                            {
                                foreach (PlayerCard card in playerCards)
                                {
                                    sortElement(card, toParse[1], playerIndex);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTADDFIELD)
                        {
                            if (isCardSelected())
                            {
                                if (toParse.Count() < 2)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELD
                                                                + " name, where name is the name of the field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                HostCurrentCard.addElement(new FieldElement(toParse[1]));
                                deselectCard();
                            }
                            else
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELD
                                                                + " #/name name, where #/name corresponds to player's name/position and name is the name of the field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.addElement(new FieldElement(toParse[2]));
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELD)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELD
                                                            + " name, where name is the name of the field you wish to add. Field name can only be 1 word. Will modify all players.");
                                return;
                            }
                            deselectCard();
                            foreach (PlayerCard card in playerCards)
                            {
                                card.addElement(new FieldElement(toParse[1]));
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST)
                        {
                            if (isCardSelected())
                            {
                                if (toParse.Count() < 2)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST
                                                            + " name, where name is the name of the list field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                HostCurrentCard.addElement(new ListElement(toParse[1]));
                                deselectCard();
                            }
                            else
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST
                                                            + " #/name name, where #/name corresponds to player's name/position and name is the name of the list field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.addElement(new ListElement(toParse[2]));
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST
                                                            + " name, where name is the name of the list field you wish to add. Field name can only be 1 word. Will modify all players.");
                                return;
                            }
                            deselectCard();
                            foreach (PlayerCard card in playerCards)
                            {
                                card.addElement(new ListElement(toParse[1]));
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTDELETEFIELD)
                        {
                            if (isCardSelected())
                            {
                                if (toParse.Count() < 2)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDELETEFIELD
                                                            + " name, where name is the name of the field you wish to delete. Field name can only be 1 word.");
                                    return;
                                }
                                HostCurrentCard.removeElement(toParse[1]);
                                deselectCard();
                            }
                            else
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDELETEFIELD
                                                            + " #/name name, where #/name corresponds to player's name/position and name is the name of the field you wish to delete. Field name can only be 1 word.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.removeElement(toParse[2]);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD
                                                            + " name, where name is the name of the field you wish to delete. Field name can only be 1 word. Will modify all players.");
                                return;
                            }
                            deselectCard();
                            foreach (PlayerCard card in playerCards)
                            {
                                card.removeElement(toParse[1]);
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST)
                        {
                            if (isCardSelected()) //if card is selected, no need to check for player argument
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST
                                                                + " #/name content, where #/name corresponds to the list field's position on the player card OR element name, and content is what you want to add to the list field. Separated by commas.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    addContentToList(toModify, content);
                                }
                                deselectCard();
                                return;
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST
                                                            + " #/name #/name content, where #/name corresponds to the player's position OR player name, #/name corresponds to the list field's position OR element's name, and content is what you want to add to the list field. Separated by commas.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        addContentToList(toModify, content);
                                    }
                                }
                                return;
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLADDTOLIST)
                        {
                            if (toParse.Count() < 3)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDTOLIST
                                                            + " name content, where name corresponds to element name, and content is what you want to add to the list field. Separated by commas. Will modify all players who have this field.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTALLADDTOLIST.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            foreach (PlayerCard card in playerCards)
                            {
                                Element element = retrieveElement(card, toParse[1]);
                                if (element != null)
                                {
                                    addContentToList(element, content);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST)
                        {
                            if (isCardSelected()) //if card is selected, no need to check for player argument
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST
                                                                + " #/name content, where #/name corresponds to element position OR name, and content is what you want to remove from list field.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    removeFromList(toModify, content);
                                    deselectCard();
                                }
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST
                                                                + " #/name #/name content, where #/name corresponds to the player's position OR player name, name corresponds to element position OR name, and content is what you want to remove from list field.");
                                    return;
                                }
                                deselectCard();
                                PlayerCard cardToModify = retrievePlayerCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        removeFromList(toModify, content);
                                    }
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLREMOVEFROMLIST)
                        {
                            if (toParse.Count() < 3)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLREMOVEFROMLIST
                                                            + " #/name content, where #/name corresponds to element position OR name, and content is what you want to remove from list field. Will modify all players who have this field.");
                                return;
                            }
                            string content = input.Remove(0, Constants.DnD.PRIVATECOMMAND_HOSTALLREMOVEFROMLIST.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            foreach(PlayerCard card in playerCards)
                            {
                                Element toModify = retrieveElement(card, toParse[1]);
                                if (toModify != null)
                                {
                                    removeFromList(toModify, content);
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }

        private void sortElement(PlayerCard card, string elementName, int userIndex)
        {
            int index = userIndex - 1;
            Element toModify = card.getElement(elementName);
            if (toModify != null)
            {
                if (index < 0 || index >= card.getElements().Count()-1)
                    return;
                card.removeElement(elementName);
                card.addElementAt(toModify, index);
            }
            return;
        }

        private void removeFromList(Element element, string command)
        {
            int playerIndex;
            ListElement listElement = element as ListElement;
            if (listElement != null)
            {
                if (!listElement.removeFromList(command))
                {
                    if (Int32.TryParse(command, out playerIndex))
                    {
                        if (playerIndex < 1 || playerIndex > listElement.listSize())
                            return;
                        else
                            listElement.removeFromList(playerIndex);
                    }
                }
            }
            return;
        }

        private Element retrieveElement(PlayerCard card, string command)
        {
            int playerIndex;
            foreach (Element element in card.getElements())
            {
                if (element.getName().Contains(command))
                    return element;
            }
            if (Int32.TryParse(command, out playerIndex))
            {
                if (playerIndex < 1 || playerIndex > HostCurrentCard.getElements().Count())
                    return null;
                else
                    return HostCurrentCard.getElement(playerIndex);
            }
            return null;
        }

        private PlayerCard retrievePlayerCard(string command)
        {
            int playerIndex;
            foreach (PlayerCard player in playerCards)
            {
                if (player.getName().Contains(command))
                    return player;
            }
            if (Int32.TryParse(command, out playerIndex))
            {
                int index = playerIndex - 1;
                if (playerIndex < 1 || playerIndex > playerCards.Count())
                    return null;
                else
                    return playerCards.ElementAt(index);
            }
            return null;
        }

        private string getPlayerList()
        {
            string toSend = "```Player List:\n\n";
            int i = 1;
            foreach (PlayerCard card in playerCards)
            {
                toSend += i + ") " + card.getName() + "\n";
                i++;
            }
            toSend += "```";
            return toSend;
        }

        private void modifyContent(Element element, string content)
        {
            FieldElement fieldElement = element as FieldElement;
            ListElement listElement = element as ListElement;
            if (fieldElement != null)
            {
                editFieldElement(fieldElement, content);
                return;
            }
            else if (listElement != null)
            {
                editListElement(listElement, content);
                return;
            }
            return;
        }

        private void addContentToList(Element element, string content)
        {
            ListElement listElement = element as ListElement;
            if (listElement != null)
            {
                listElement.addToList(content);
                return;
            }
            return;
        }

        private void editFieldElement(FieldElement element, string content)
        {
            element.setContent(content);
        }

        private void editListElement(ListElement element, string content)
        {
            element.clearList();
            string[] parseContent = content.Split(',');
            for (int i=0; i < parseContent.Count(); i++)
            {
                parseContent[i] = parseContent[i].Trim(' ');
                element.addToList(parseContent[i]);
            }
        }

        private void selectCard(PlayerCard card)
        {
            HostCurrentCard = card;
            return;
        }

        private bool isCardSelected()
        {
            return (HostCurrentCard != null) ? true : false;
        }

        private void deselectCard()
        {
            HostCurrentCard = null;
            return;
        }

        private PlayerCard findPlayer(ulong id)
        {
            foreach (PlayerCard playerCard in playerCards)
            {
                if (id == playerCard.getPlayerID())
                    return playerCard;
            }
            return null;
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
            return rand.Next(1, maxValue + 1);
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
    }
}
