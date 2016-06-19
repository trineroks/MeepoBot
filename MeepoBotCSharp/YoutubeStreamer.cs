//trineroks 2016

using Discord.Audio;
using Discord;
using YoutubeExtractor;
using VideoLibrary;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeepoBotCSharp
{
    class YoutubeStreamer
    {
        private enum PLAYERSTATE
        {
            DEFAULT,
            STOPPED,
            PAUSED,
            PLAYING,
            SKIPPING
        }

        private IAudioClient audioClient;
        private DiscordClient client;
        private Queue<VideoInfo> playbackQueue = new Queue<VideoInfo>();
        private Thread audioThread = null;
        private bool threadCreated = false;
        private PLAYERSTATE STATE = PLAYERSTATE.DEFAULT;
        private Channel mainTextChannel;

        public YoutubeStreamer(DiscordClient disClient)
        {
            client = disClient;
            audioClient = null;
            mainTextChannel = null;
        }

        public void PlayerLoop()
        {
            checkQueue();
        }

        private void togglePause()
        {
            setPlayerState((PlayerState() != PLAYERSTATE.PAUSED) ? PLAYERSTATE.PAUSED : PLAYERSTATE.DEFAULT);
        }

        private void updateAudioClient(IAudioClient update)
        {
            audioClient = update;
            playbackQueue.Clear();
        }

        private async void addToQueue(string url, Channel response)
        {
            try
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
                VideoInfo video = videoInfos
                                  .Where(info => info.CanExtractAudio)
                                  .OrderByDescending(info => info.AudioBitrate)
                                  .First();
                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }

                playbackQueue.Enqueue(video);
                await response.SendMessage("**Enqueued: " + video.Title + "**");
            } catch (Exception)
            {
                await response.SendMessage("**Invalid Youtube URL!**");
            }
        }

        private void checkQueue()
        {
            if (!threadCreated)
            {
                audioThread = new Thread(playFromQueue);
                audioThread.Start();
                threadCreated = true;
            }
        }

        private void setPlayerState(PLAYERSTATE state)
        {
            STATE = state;
        }

        private PLAYERSTATE PlayerState()
        {
            return STATE;
        }

        private void playFromQueue()
        {
            while (true)
            {
                for (int i = 0; i < playbackQueue.Count(); i++)
                {
                    broadcastAudio(playbackQueue.Dequeue());
                }
            }
        }

        private async void broadcastAudio(VideoInfo video)
        {
            Stream youtubeAudio = new MemoryStream();
            var audioDownloader = new AudioDownloader(video, null);
            audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);
            youtubeAudio = audioDownloader.ExecuteStream();
            using (youtubeAudio)
            {
                await mainTextChannel.SendMessage("**Now playing: " + video.Title + "**");
                sendAudioUsingDiscord(youtubeAudio);
            }
            return;
        }

        private void sendAudio(string filePath)
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

        private void sendAudioUsingDiscord(Stream stream)
        {
            var channelCount = client.GetService<AudioService>().Config.Channels;
            //48000
            var OutFormat = new WaveFormat(48000, 16, channelCount);
            stream.Position = 0;
            using (var MP3Reader = new Mp3FileReader(stream))
            using (var WaveChannel = new WaveChannel32(MP3Reader))
            using (var resampler = new MediaFoundationResampler(WaveChannel, OutFormat))
            {
                WaveChannel.PadWithZeroes = false;
                WaveChannel.Volume = Constants.AudioPlayer.VOLUME;
                resampler.ResamplerQuality = 60;
                int blockSize = OutFormat.AverageBytesPerSecond / 50; //50
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
                {
                    while (PlayerState() == PLAYERSTATE.PAUSED)
                    {
                        if (PlayerState() == PLAYERSTATE.SKIPPING)
                        {
                            setPlayerState(PLAYERSTATE.DEFAULT);
                            return;
                        }
                    }
                    if (PlayerState() == PLAYERSTATE.SKIPPING)
                    {
                        setPlayerState(PLAYERSTATE.DEFAULT);
                        return;
                    }
                    else if (byteCount < blockSize)
                    {
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    audioClient.Send(buffer, 0, blockSize);
                }
            }
        }

        private void sendAudioUsingDiscordHQ(Stream stream)
        {
            var channelCount = client.GetService<AudioService>().Config.Channels;
            var OutFormat = new WaveFormat(48000, 16, channelCount);
            stream.Position = 0;
            using (var AACReader = new MediaFoundationReader(stream))
            using (var resampler = new MediaFoundationResampler(AACReader, OutFormat))
            {
                resampler.ResamplerQuality = 60;
                int blockSize = OutFormat.AverageBytesPerSecond / 50; //50
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

        public async void evaluateInput(string input, MessageEventArgs e)
        {
            string[] toParse = input.Split(' ');
            string command = toParse[0];
            int inputLen = toParse.Length;

            if (command == Constants.COMMAND_SUMMON)
            {
                var voiceChannel = e.User.VoiceChannel;
                mainTextChannel = e.Channel;
                if (voiceChannel != null)
                {
                    updateAudioClient(await client.GetService<AudioService>().Join(voiceChannel));
                    await mainTextChannel.SendMessage("Summoned to " + mainTextChannel.Mention);
                }
                return;
            }
            else if (command == Constants.AudioPlayer.COMMAND_PLAYURL)
            {
                if (inputLen > 2)
                {
                    await e.Channel.SendMessage("Incorrect usage.");
                }
                else
                {
                    addToQueue(toParse[1], e.Channel);
                }
                return;
            }
            else if (command == Constants.AudioPlayer.COMMAND_PAUSEPLAYER)
            {
                togglePause();
            }
            else if (command == Constants.AudioPlayer.COMMAND_SKIPPLAYER)
            {
                setPlayerState(PLAYERSTATE.SKIPPING);
            }
            else if (command == Constants.AudioPlayer.COMMAND_PLAYURLMP3)
            {
                Thread play = new Thread(() => sendAudio(toParse[1]));
                play.Start();
            }
        }
    }
}
