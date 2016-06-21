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
            public virtual string getContent() { return ""; }
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
            public override string getContent()
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
                if (parseContent == "")
                    return;
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
                toPass = toPass.ToLower();
                foreach (string contents in _content)
                {
                    string temp = contents.ToLower();
                    if (temp == toPass)
                    {
                        _content.Remove(contents);
                        return true;
                    }
                }
                foreach (string contents in _content)
                {
                    string temp = contents.ToLower();
                    if(temp.Contains(toPass))
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

            public override string getContent()
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
                    if (toParse[i] == "")
                        break;
                    else
                    {
                        toParse[i] = toParse[i].Trim();
                    }
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

            public List<string> getElementNames()
            {
                List<string> toSend = new List<string>();
                string toPass = "";
                foreach(Element cardelements in _fields)
                {
                    toPass = "";
                    toPass += cardelements.getName() + checkIfList(cardelements);
                    toSend.Add(toPass);
                }
                return toSend;
            }

            public Card addElement(Element element)
            {
                foreach (Element cardelements in _fields)
                {
                    if (cardelements.getName().ToLower() == element.getName().ToLower()) //no duplicates allowed to avoid confusion
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
                toPass = toPass.ToLower();
                foreach (Element element in _fields)
                {
                    string temp = element.getName().ToLower();
                    if (toPass == temp)
                    {
                        _fields.Remove(element);
                        return true;
                    }
                }
                foreach (Element element in _fields)
                {
                    string temp = element.getName().ToLower();
                    if (temp.Contains(toPass))
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
                toPass = toPass.ToLower();
                foreach (Element element in _fields)
                {
                    string temp = element.getName().ToLower();
                    if (toPass == temp)
                        return element;
                }
                foreach (Element element in _fields)
                {
                    string temp = element.getName().ToLower();
                    if (temp.Contains(toPass))
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
                    toSend += i + ") " + element.getName() + checkIfList(element) + ": " + element.getContent() + "\n";
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
        private List<Card> NPCCards = new List<Card>();

        private bool start = false;
        private Card HostCurrentCard;
        private bool editingPlayer = true;

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
            if (command == "")
                return;
            else if (command == Constants.COMMAND_CANCELPARTYGAME && e.User.Id == getHostID())
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
            else if (command == Constants.DnD.COMMAND_PRINTALLVALUES) //move this to private player command and public player command.
            {
                string toSend = "";
                if (toParse.Count() < 2)
                {
                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.COMMAND_PRINTALLVALUES
                                                + " name, where name corresponds to field name.");
                    return;
                }
                if (e.User.Id != getHostID())
                {
                    toSend += getPrintAllValuesString(toParse[1]);
                }
                else
                {
                    if (editingPlayer)
                        toSend += getPrintAllValuesString(toParse[1]);
                    else
                        toSend += getPrintAllNPCValuesString(toParse[1]);
                }
                await e.Channel.SendMessage(toSend);
                return;
            }
            else if (command == Constants.DnD.PRIVATECOMMAND_GETPLAYERCARD && e.User.Id != getHostID())
            {
                PlayerCard playerCard = findPlayer(e.User.Id);
                string toSend = "";
                if (playerCard != null)
                {
                    toSend += playerCard.cardToString();
                    toSend += provideChannelLink();
                    await e.User.SendMessage(toSend);
                }
                else
                {
                    await e.User.SendMessage("Exception: DnD Player is null.");
                }
                return;
            }
            else
            {
                if (e.Channel.IsPrivate)
                {
                    string toSend = "";
                    PlayerCard playerCard = findPlayer(e.User.Id);
                    if (playerCard == null && e.User.Id != getHostID())
                        await getClient().GetChannel(getTextChannelID()).SendMessage("Exception: DnD Player is null.");
                    else if (e.User.Id == getHostID())
                    {
                        if (command == Constants.DnD.PRIVATECOMMAND_HELP)
                        {
                            await e.Channel.SendMessage("Type " + Constants.DnD.PRIVATECOMMAND_HELPONE + " for the first help page or " + Constants.DnD.PRIVATECOMMAND_HELPTWO + " for the second help page.");
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HELPONE)
                        {
                            await e.Channel.SendMessage(provideHelpStringOne());
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HELPTWO)
                        {
                            await e.Channel.SendMessage(provideHelpStringTwo());
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTLISTCARDS)
                        {
                            deselectCard();
                            if (editingPlayer)
                            {
                                await e.Channel.SendMessage(getPlayerList());
                            }
                            else
                            {
                                await e.Channel.SendMessage(getNPCList());
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTDESELECT)
                        {
                            deselectCard();
                            await e.Channel.SendMessage("Current card deselected.");
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD)
                        {
                            if (toParse.Count() < 2 || toParse.Count() > 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD
                                                            + " #/name, where # corresponds to the card's position on "
                                                            + Constants.DnD.PRIVATECOMMAND_HOSTLISTCARDS + " OR name.");
                                return;
                            }
                            else
                            {
                                Card card = retrieveCard(toParse[1]);
                                deselectCard();
                                if (card != null)
                                {
                                    toSend += card.cardToString();
                                    selectCard(card);
                                    toSend += provideChannelLink();
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
                                                                + " #/name content, where #/name corresponds to the element's position on the card OR element name, and content is what you want on that field.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    modifyContent(toModify, content);
                                    toSend += HostCurrentCard.cardToString();
                                    toSend += provideChannelLink();
                                    await e.Channel.SendMessage(toSend);
                                }
                                //deselectCard();
                                return;
                            }
                            else //if card isn't selected, first argument is player
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT
                                                                + " #/name #/name content, where #/name corresponds to the card's position/name, #/name corresponds to the element's position OR element's name, and content is what you want on that field.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        modifyContent(toModify, content);
                                        toSend += cardToModify.cardToString();
                                        toSend += provideChannelLink();
                                        selectCard(cardToModify);
                                        await e.Channel.SendMessage(toSend);
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
                                                            + " name content, where name corresponds to element name, and content is what you want on that field. Will modify all cards who have this field.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    Element toEdit = card.getElement(toParse[1]);
                                    if (toEdit != null)
                                    {
                                        modifyContent(toEdit, content);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    Element toEdit = card.getElement(toParse[1]);
                                    if (toEdit != null)
                                    {
                                        modifyContent(toEdit, content);
                                    }
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
                                                                + " name #, where name corresponds to element name, and # is where on the card you want that field to be in.");
                                    return;
                                }
                                int playerIndex;
                                if (Int32.TryParse(toParse[2], out playerIndex))
                                {
                                    sortElement(HostCurrentCard, toParse[1], playerIndex);
                                    toSend += HostCurrentCard.cardToString();
                                    toSend += provideChannelLink();
                                    await e.Channel.SendMessage(toSend);
                                    //deselectCard();
                                    return;
                                }
                                else
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " name #, where #/name corresponds to element name/position, and # is where on the card you want that field to be in.");
                                    return;
                                }
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " #/name name #, where #/name corresponds to card's position/name, name is the name of the element, and # is where on the playercard you want that field to be in.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    int playerIndex;
                                    if (Int32.TryParse(toParse[3], out playerIndex))
                                    {
                                        sortElement(cardToModify, toParse[2], playerIndex);
                                        toSend += cardToModify.cardToString();
                                        toSend += provideChannelLink();
                                        selectCard(cardToModify);
                                        await e.Channel.SendMessage(toSend);
                                        return;
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT
                                                                + " #/name name #, where #/name corresponds to card's position/name, name is the name of the element, and # is where on the playercard you want that field to be in.");
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
                                                                + " name #, where #/name corresponds to element name, and # is where on the playercard you want that field to be in. Will modify all cards who have this field.");
                                return;
                            }
                            deselectCard();
                            int playerIndex;
                            if (Int32.TryParse(toParse[2], out playerIndex))
                            {
                                if (editingPlayer)
                                {
                                    foreach (Card card in playerCards)
                                    {
                                        sortElement(card, toParse[1], playerIndex);
                                    }
                                }
                                else
                                {
                                    foreach (Card card in NPCCards)
                                    {
                                        sortElement(card, toParse[1], playerIndex);
                                    }
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDTEXT)
                        {
                            if (isCardSelected())
                            {
                                if (toParse.Count() < 2)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDTEXT
                                                                + " name, where name is the name of the text field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                HostCurrentCard.addElement(new FieldElement(toParse[1]));
                                toSend += HostCurrentCard.cardToString();
                                toSend += provideChannelLink();
                                await e.Channel.SendMessage(toSend);
                                //deselectCard();
                            }
                            else
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDTEXT
                                                                + " #/name name, where #/name corresponds to card's position/name and name is the name of the text field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.addElement(new FieldElement(toParse[2]));
                                    toSend += cardToModify.cardToString();
                                    toSend += provideChannelLink();
                                    selectCard(cardToModify);
                                    await e.Channel.SendMessage(toSend);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDTEXT)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDTEXT
                                                            + " name, where name is the name of the text field you wish to add. Field name can only be 1 word. Will modify all cards.");
                                return;
                            }
                            deselectCard();
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    card.addElement(new FieldElement(toParse[1]));
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    card.addElement(new FieldElement(toParse[1]));
                                }
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
                                toSend += HostCurrentCard.cardToString();
                                toSend += provideChannelLink();
                                await e.Channel.SendMessage(toSend);
                                //deselectCard();
                            }
                            else
                            {
                                if (toParse.Count() < 3)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST
                                                            + " #/name name, where #/name corresponds to card's position/name and name is the name of the list field you wish to add. Field name can only be 1 word.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.addElement(new ListElement(toParse[2]));
                                    toSend += cardToModify.cardToString();
                                    toSend += provideChannelLink();
                                    selectCard(cardToModify);
                                    await e.Channel.SendMessage(toSend);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST
                                                            + " name, where name is the name of the list field you wish to add. Field name can only be 1 word. Will modify all cards.");
                                return;
                            }
                            deselectCard();
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    card.addElement(new ListElement(toParse[1]));
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    card.addElement(new ListElement(toParse[1]));
                                }
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
                                toSend += HostCurrentCard.cardToString();
                                toSend += provideChannelLink();
                                await e.Channel.SendMessage(toSend);
                                //deselectCard();
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
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    cardToModify.removeElement(toParse[2]);
                                    toSend += cardToModify.cardToString();
                                    toSend += provideChannelLink();
                                    selectCard(cardToModify);
                                    await e.Channel.SendMessage(toSend);
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD
                                                            + " name, where name is the name of the field you wish to delete. Field name can only be 1 word. Will modify all cards.");
                                return;
                            }
                            deselectCard();
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    card.removeElement(toParse[1]);
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    card.removeElement(toParse[1]);
                                }
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
                                                                + " #/name content, where #/name corresponds to the list field's position/name, and content is what you want to add to the list field. Separated by commas.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    addContentToList(toModify, content);
                                    toSend += HostCurrentCard.cardToString();
                                    toSend += provideChannelLink();
                                    await e.Channel.SendMessage(toSend);
                                }
                                //deselectCard();
                                return;
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST
                                                            + " #/name #/name content, where #/name corresponds to the player's position/name, #/name corresponds to the list field's position/name, and content is what you want to add to the list field. Separated by commas.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        addContentToList(toModify, content);
                                        toSend += cardToModify.cardToString();
                                        toSend += provideChannelLink();
                                        selectCard(cardToModify);
                                        await e.Channel.SendMessage(toSend);
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
                                                            + " name content, where name corresponds to list field name, and content is what you want to add to the list field. Separated by commas. Will modify all cards that have this field.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    Element element = retrieveElement(card, toParse[1]);
                                    if (element != null)
                                    {
                                        addContentToList(element, content);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    Element element = retrieveElement(card, toParse[1]);
                                    if (element != null)
                                    {
                                        addContentToList(element, content);
                                    }
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
                                                                + " #/name content, where #/name corresponds to list field's position/name, and content is what you want to remove from list field.");
                                    return;
                                }
                                Element toModify = retrieveElement(HostCurrentCard, toParse[1]);
                                if (toModify != null)
                                {
                                    string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                    content = content.TrimStart(' ');//!medit "2       content"
                                    content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                    content = content.TrimStart(' '); //!medit 2 "content"
                                    removeFromList(toModify, content);
                                    toSend += HostCurrentCard.cardToString();
                                    toSend += provideChannelLink();
                                    await e.Channel.SendMessage(toSend);
                                    //deselectCard();
                                }
                            }
                            else
                            {
                                if (toParse.Count() < 4)
                                {
                                    await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST
                                                                + " #/name #/name content, where #/name corresponds to the card's position/name, #/name corresponds to list field's position/name, and content is what you want to remove from list field.");
                                    return;
                                }
                                deselectCard();
                                Card cardToModify = retrieveCard(toParse[1]);
                                if (cardToModify != null)
                                {
                                    Element toModify = retrieveElement(cardToModify, toParse[2]);
                                    if (toModify != null)
                                    {
                                        string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                                        content = content.TrimStart(' ');//!medit "2       content"
                                        content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                                        content = content.TrimStart(' '); //!medit 2 "content"
                                        content = content.Remove(0, toParse[2].Length + 1);
                                        content = content.TrimStart(' ');
                                        removeFromList(toModify, content);
                                        toSend += cardToModify.cardToString();
                                        toSend += provideChannelLink();
                                        selectCard(cardToModify);
                                        await e.Channel.SendMessage(toSend);
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
                                                            + " #/name content, where #/name corresponds to list field's position/name, and content is what you want to remove from list field. Will modify all cards that have this field.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');//!medit "2       content"
                            content = content.Remove(0, toParse[1].Length + 1); //!medit 2 "     content"
                            content = content.TrimStart(' '); //!medit 2 "content"
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    Element toModify = retrieveElement(card, toParse[1]);
                                    if (toModify != null)
                                    {
                                        removeFromList(toModify, content);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    Element toModify = retrieveElement(card, toParse[1]);
                                    if (toModify != null)
                                    {
                                        removeFromList(toModify, content);
                                    }
                                }
                            }
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTCOMMONFIELDS)
                        {
                            toSend += "```Common Fields Across All " + getIfEditingPlayer() + " Cards \n\n";
                            List<string> temporary = new List<string>();
                            bool toggle = false;
                            deselectCard();
                            if (editingPlayer)
                            {
                                foreach (Card card in playerCards)
                                {
                                    if (!toggle)
                                    {
                                        temporary = card.getElementNames();
                                        toggle = true;
                                    }
                                    temporary = temporary.Intersect(card.getElementNames()).ToList();
                                }
                            }
                            else
                            {
                                foreach (Card card in NPCCards)
                                {
                                    if (!toggle)
                                    {
                                        temporary = card.getElementNames();
                                        toggle = true;
                                    }
                                    temporary = temporary.Intersect(card.getElementNames()).ToList();
                                }
                            }
                            foreach (string element in temporary)
                            {
                                toSend += element + ", ";
                            }
                            toSend = toSend.TrimEnd(' ');
                            toSend = toSend.TrimEnd(',');
                            toSend += "```";
                            toSend += provideChannelLink();
                            await e.Channel.SendMessage(toSend);
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTCREATENPCCARD && !editingPlayer)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITNPCCARDS
                                                            + " title, where title is the title of the card you wish to create.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');
                            NPCCards.Add(new Card(content));
                            await e.Channel.SendMessage("New NPC card " + content + " created.");
                            return;
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTDELETENPCCARD && !editingPlayer)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDELETENPCCARD
                                                            + " #/title, where #/title is the position/title of the card you wish to delete.");
                                return;
                            }
                            deselectCard();
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');
                            if (deleteNPC(content))
                            {
                                await e.Channel.SendMessage("NPC Card successfully deleted.");
                                return;
                            }
                            else
                            {
                                await e.Channel.SendMessage("NPC Card not found.");
                                return;
                            }
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTEDITNPCCARDS)
                        {
                            deselectCard();
                            editingPlayer = false;
                            await e.Channel.SendMessage("Now editing NPC Cards.");
                        }
                        else if (command == Constants.DnD.PRIVATECOMMAND_HOSTEDITPLAYERCARDS)
                        {
                            deselectCard();
                            editingPlayer = true;
                            await e.Channel.SendMessage("Now editing Player Cards.");
                        }
                    }
                }
                else //commands on the main game channel
                {
                    if (e.User.Id == getHostID())
                    {
                        string toSend = "";
                        if (command == Constants.DnD.COMMAND_HOSTPRESENTPLAYER)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.COMMAND_HOSTPRESENTPLAYER
                                                            + " #/title, where #/title is the position/title of the player card you wish to show.");
                                return;
                            }
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');
                            Card card = retrievePlayerCard(content);
                            if (card != null)
                            {
                                toSend += card.cardToString();
                                await e.Channel.SendMessage(toSend);
                            }
                        }
                        else if (command == Constants.DnD.COMMAND_HOSTPRESENTNPC)
                        {
                            if (toParse.Count() < 2)
                            {
                                await e.Channel.SendMessage("USAGE: " + Constants.DnD.COMMAND_HOSTPRESENTNPC
                                                            + " #/title, where #/title is the position/title of the NPC card you wish to show.");
                                return;
                            }
                            string content = input.Remove(0, command.Length + 1); //!medit "   2       content"
                            content = content.TrimStart(' ');
                            Card card = retrieveNPCCard(content);
                            if (card != null)
                            {
                                toSend += card.cardToString();
                                await e.Channel.SendMessage(toSend);
                            }
                        }
                    }
                }
            }
        }

        private string provideHelpStringOne()
        {
            string toSend = "";
            toSend += "**" + Constants.DnD.PRIVATECOMMAND_HOSTEDITNPCCARDS + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITNPCCARDS + " - will toggle editing to NPC cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTEDITPLAYERCARDS + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITPLAYERCARDS + " - will toggle editing to Player cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTCREATENPCCARD + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTCREATENPCCARD + " name - name is the name of the card. Will create a new NPC card. Works only on NPC mode.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTDELETENPCCARD + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDELETENPCCARD + " name - name corresponds to the name of the card you wish to delete. Will delete an NPC card. Works only on NPC mode.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTLISTCARDS + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTLISTCARDS + " - will list all cards. Depends on whether you are editing NPC or Player cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSELECTCARD + " #/name - #/name corresponds to position/name of the card you which to select.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTDESELECT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDESELECT + " - will deselect currently selected card.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTEDITELEMENT + " - will edit a field on any given card. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLEDITELEMENT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLEDITELEMENT + " name content - name corresponds to the name of the field you wish to edit and content is the content you want in that field. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDTOLIST + " - will add content to a list field. Multiple items can be added by separating with commas. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLADDTOLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDTOLIST + " name content - name corresponds to the name of the list field you wish to add to and content is the content you wish to add. Multiple items can be added by separating with commas. Will effect all cards.";
            
            return toSend;
        }

        private string provideHelpStringTwo()
        {
            string toSend = "";
            toSend += "**" + Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTREMOVEFROMLIST + " - will remove an item from a list field. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLREMOVEFROMLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLREMOVEFROMLIST + " name content - name corresponds to the name of the list field you wish to remove from and content is the content you wish to remove. Multiple items can be added by separating with commas. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDTEXT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDTEXT + " - will add a new text field to the card. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDTEXT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDTEXT + " title - title is what you want the title of this new text field to be. Will add a new text field to the card. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTADDFIELDLIST + " - will add a new list field to the card. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLADDFIELDLIST + " title - title is what you want the title of this new list field to be. Will add a new list field to the card. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTSORTELEMENT + " - will move a field to the specified position on the card. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLSORTELEMENT + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLSORTELEMENT + " name # - name corresponds to the name of the field you wish to move and # is the position on the card you wish to move it to. Will move a field to the specified position on the card. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTDELETEFIELD + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTDELETEFIELD + " - will delete a field from the card. TODO: ADD EXTRA COMMAND FOR DUAL USAGE.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTALLDELETEFIELD + " name - name corresponds to the name of the field you wish to delete. Will delete a field from the card. Will effect all cards.";
            toSend += "\n**" + Constants.DnD.PRIVATECOMMAND_HOSTCOMMONFIELDS + "** USAGE: " + Constants.DnD.PRIVATECOMMAND_HOSTCOMMONFIELDS + " - will list fields common to all cards.";
            toSend += "\n**" + Constants.DnD.COMMAND_PRINTALLVALUES + "** USAGE: " + Constants.DnD.COMMAND_PRINTALLVALUES + " name - name corresponds to the name of the field you wish to print. Will print values of the field from all cards.";
            return toSend;
        }

        private bool deleteNPC(string command)
        {
            int playerIndex;
            string temp = command.ToLower();
            foreach (Card card in NPCCards)
            {
                string name = card.getName().ToLower();
                if (name == temp)
                    NPCCards.Remove(card);
            }
            foreach (Card card in NPCCards)
            {
                string name = card.getName().ToLower();
                if (name.Contains(temp))
                    NPCCards.Remove(card);
            }
            if (Int32.TryParse(command, out playerIndex))
            {
                if (playerIndex < 1 || playerIndex > NPCCards.Count())
                    return false;
                else
                {
                    int index = playerIndex - 1;
                    NPCCards.RemoveAt(index);
                    return true;
                }   
            }
            return false;
        }

        private string getIfEditingPlayer()
        {
            if (editingPlayer)
                return "Player";
            else
                return "NPC";
        }

        private string getPrintAllValuesString(string command)
        {
            string toSend = "```" + command + " \n\n";
            int i = 1;
            foreach(Card card in playerCards)
            {
                Element toAdd = card.getElement(command);
                toSend += i + ") " + card.getName() + ": ";
                if (toAdd == null)
                    toSend += "(Does not have field)";
                else
                    toSend += toAdd.getContent();
                toSend += "\n";
                i++;
            }
            toSend += "```";
            return toSend;
        }

        private string getPrintAllNPCValuesString(string command)
        {
            string toSend = "```" + command + " \n\n";
            int i = 1;
            foreach (Card card in NPCCards)
            {
                Element toAdd = card.getElement(command);
                toSend += i + ") " + card.getName() + ": ";
                if (toAdd == null)
                    toSend += "(Does not have field)";
                else
                    toSend += toAdd.getContent();
                toSend += "\n";
                i++;
            }
            toSend += "```";
            return toSend;
        }

        private void sortElement(Card card, string elementName, int userIndex)
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

        private Element retrieveElement(Card card, string command)
        {
            int playerIndex;
            string temp = command.ToLower();
            foreach (Element element in card.getElements())
            {
                string name = element.getName().ToLower();
                if (name == temp)
                    return element;
            }
            foreach (Element element in card.getElements())
            {
                string name = element.getName().ToLower();
                if (name.Contains(temp))
                    return element;
            }
            if (Int32.TryParse(command, out playerIndex))
            {
                if (HostCurrentCard == null)
                    return null;
                else if (playerIndex < 1 || playerIndex > HostCurrentCard.getElements().Count())
                    return null;
                else
                    return HostCurrentCard.getElement(playerIndex);
            }
            return null;
        }

        private Card retrievePlayerCard(string command)
        {
            int playerIndex;
            string toSend = command.ToLower();
            foreach (PlayerCard player in playerCards)
            {
                string temp = player.getName().ToLower();
                if (toSend == temp)
                    return player;
            }
            foreach (PlayerCard player in playerCards)
            {
                string temp = player.getName().ToLower();
                if (temp.Contains(toSend))
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

        private Card retrieveNPCCard(string command)
        {
            int playerIndex;
            string toSend = command.ToLower();
            foreach (Card player in NPCCards)
            {
                string temp = player.getName().ToLower();
                if (toSend == temp)
                    return player;
            }
            foreach (Card player in NPCCards)
            {
                string temp = player.getName().ToLower();
                if (temp.Contains(toSend))
                    return player;
            }
            if (Int32.TryParse(command, out playerIndex))
            {
                int index = playerIndex - 1;
                if (playerIndex < 1 || playerIndex > NPCCards.Count())
                    return null;
                else
                    return NPCCards.ElementAt(index);
            }
            return null;
        }

        private Card retrieveCard(string command)
        {
            if (editingPlayer)
            {
                return retrievePlayerCard(command);
            }
            else
            {
                return retrieveNPCCard(command);
            }
        }

        private string getNPCList()
        {
            string toSend = "```NPC List:\n\n";
            int i = 1;
            foreach (Card card in NPCCards)
            {
                toSend += i + ") " + card.getName() + "\n";
                i++;
            }
            toSend += "```";
            return toSend;
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

        private string checkIfList(Element element)
        {
            ListElement listElement = element as ListElement;
            if (listElement != null)
            {
                return "(list)";
            }
            return "";
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
            element.addToList(content);
        }

        private void selectCard(Card card)
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

        private string provideChannelLink()
        {
            return "\nClick " + getClient().GetChannel(getTextChannelID()).Mention + " to return to the game channel.";
        }
    }
}
