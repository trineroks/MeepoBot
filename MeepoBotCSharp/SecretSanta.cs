//trineroks 2017

using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MeepoBotCSharp 
{
    public class SecretSanta 
    {
        private enum STATE 
        {
            SIGNUP,
            DISTRIBUTION
        }

        private List<User> participants = new List<User>();
        private Server server;
        private DiscordClient client;
        private Channel textChannel;
        private static Random rand = new Random();
        private int month, day, year;

        public SecretSanta(Server server, DiscordClient client) 
        {
            this.server = server;
            this.client = client;
            this.month = 6;
            this.day = 22;
            this.year = 1941;
        }

        private void shuffle() 
        {
            int i = participants.Count;
            while (i > 1) {
                i--;
                int j = rand.Next(i + 1);
                User value = participants[j];
                participants[j] = participants[i];
                participants[i] = value;
            }
        }

        //Add a participant to the Secret Santa roster
        public void addParticpant(ulong ID) 
        { 
            foreach (User participant in participants) 
            {
                if (participant.Id == ID) 
                {
                    return;
                }
            }
            User user = server.GetUser(ID);
            if (user != null)
                participants.Add(user);
        }

        public async void startSanta() 
        {
            shuffle();
            await textChannel.SendMessage("Relationships set:");
            for (int i = 0; i < participants.Count; i++) 
            {
                string msg = "";
                int j = i + 1;
                if (j >= participants.Count)
                    j = 0;
                msg += participants[i] + " --> " + participants[j];
                await textChannel.SendMessage(msg);
                await participants[i].SendMessage(provideSantaCard(participants[i], participants[j]));
            }
            saveParticipantsAndRelationship();
        }

        public void saveParticipantsAndRelationship() 
        {
            int len = participants.Count;
            int arrlen = 1 + (len * 8); //each ulong takes up 8 bytes. Confidence that this will not exceed a 2GB size array (might change later)
                                        //this is added to 1 because we must include the first byte to hold the container size.
            arrlen += 12; //4 bytes for each 32bit int in the date section
            BinSerializer serializer = new BinSerializer(arrlen);
            serializer.writeByte((byte)len);
            foreach (User participant in participants) 
            {
                serializer.writeUInt64(participant.Id);
            }
            serializer.writeInt(month);
            serializer.writeInt(day);
            serializer.writeInt(year);
            File.WriteAllBytes("SecretSanta.roks", serializer.data);

            string msg = "File saved, inputting the following: ";
            foreach (User participant in participants) {
                msg += participant.Name + ", ";
            }
            msg += "End Date: " + month + "/" + day + "/" + year;
            Console.WriteLine(msg);
        }

        public void loadParticipantsAndRelationship() 
        {
            participants.Clear();
            byte[] file = File.ReadAllBytes("SecretSanta.roks");
            BinReader reader = new BinReader(file);
            if (file.Length < 1) {
                Console.WriteLine("The file is empty.");
                return;
            }
            int participantCount = reader.readByte();
            for (int i = 0; i < participantCount; i++) 
            {
                ulong userID = reader.readUInt64();
                addParticpant(userID);
            }
            month = reader.readInt();
            day = reader.readInt();
            year = reader.readInt();

            string msg = "File loaded, outputting the following: " + participantCount + " participants named: ";
            foreach (User participant in participants) {
                msg += participant.Name + ", ";
            }
            msg += "End Date: " + month + "/" + day + "/" + year;
            Console.WriteLine(msg);
        }

        public async void evaluateInput(string input, MessageEventArgs e) 
        {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;
            textChannel = e.Channel;
            if (command == "")
                return;
            else if (command == Constants.SecretSanta.COMMAND_STARTSANTA)
            {
                startSanta();
            }
            else if (command == Constants.SecretSanta.COMMAND_WIPEPARTICIPANTS) 
            {
                participants.Clear();
            }
            else if (command == Constants.SecretSanta.COMMAND_ADDPARTICIPANT) 
            {
                if (inputLen < 2 || inputLen > 2)
                    await e.Channel.SendMessage("Incorrect usage. Use " + Constants.SecretSanta.COMMAND_ADDPARTICIPANT + " @<username>.");
                else 
                {
                    string pattern = @"([0-9]+)"; //just extract the digits
                    Console.WriteLine(toParse[1]);
                    Match m = Regex.Match(toParse[1], pattern);
                    if (m.Success) 
                    {
                        string ID = m.Groups[1].Value;
                        ulong longID = Convert.ToUInt64(ID, 10);
                        Console.WriteLine(ID);
                        addParticpant(longID);
                    }
                }
            }
            else if (command == Constants.SecretSanta.COMMAND_PRINTPARTICIPANTS) 
            {
                if (participants.Count == 0) {
                    Console.WriteLine("Empty participants.");
                }
                foreach(User participant in participants) 
                {
                    Console.WriteLine(participant.Name);
                }
            }
            else if (command == Constants.SecretSanta.COMMAND_SAVETEST) 
            {
                saveParticipantsAndRelationship();
            }
            else if (command == Constants.SecretSanta.COMMAND_LOADTEST) 
            {
                loadParticipantsAndRelationship();
            }
            else if (command == Constants.SecretSanta.COMMAND_ENDDATE) 
            {
                if (inputLen < 2 || inputLen > 4) {
                    await e.Channel.SendMessage("Incorrect usage. Use " + Constants.SecretSanta.COMMAND_ENDDATE + " MM DD YYYY.");
                    return;
                }
                int tempMonth = Convert.ToInt32(toParse[1]);
                int tempDay = Convert.ToInt32(toParse[2]);
                int tempYear = Convert.ToInt32(toParse[3]);
                if (tempMonth > 12 || tempDay > 31 || tempMonth < 1 || tempDay < 1 || tempYear < 1) 
                {
                    await e.Channel.SendMessage("Incorrect usage. Please enter a valid date.");
                    return;
                }
                this.month = tempMonth;
                this.day = tempDay;
                this.year = tempYear;
            }
            else if (command == Constants.SecretSanta.COMMAND_SENDSANTACARDS) 
            {
                for (int i = 0; i < participants.Count; i++) {
                    string msg = "";
                    int j = i + 1;
                    if (j >= participants.Count)
                        j = 0;
                    msg += participants[i] + " --> " + participants[j];
                    await textChannel.SendMessage(msg);
                    await participants[i].SendMessage(provideSantaCard(participants[i], participants[j]));
                }
            }
        }

        private string provideSantaCard(User recipient, User giftRecipient) 
        {
            string msg = "THIS IS A TEST MESSAGE. YOUR NAME IS " + recipient.Name + " AND YOU ARE LINKED WITH " + giftRecipient.Name + ". THE DEADLINE IS " + month + "/" + day + "/" + year +
                        ". THIS IS NOT THE ACTUAL SECRET SANTA MESSAGE, ONLY FOR TESTING!!!!";
            return msg;
        }

    }
}
