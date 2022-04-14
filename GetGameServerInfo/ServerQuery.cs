﻿using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ServerQuery
{
    public class A2S_INFO
    {
        // \xFF\xFF\xFF\xFFTSource Engine Query\x00 because UTF-8 doesn't like to encode 0xFF
        public static readonly byte[] REQUEST = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

        #region Strong Typing Enumerators

        [Flags]
        public enum ExtraDataFlags : byte
        {
            GameID = 0x01,
            SteamID = 0x10,
            Keywords = 0x20,
            Spectator = 0x40,
            Port = 0x80
        }

        public enum VACFlags : byte
        {
            Unsecured = 0,
            Secured = 1
        }

        public enum VisibilityFlags : byte
        {
            Public = 0,
            Private = 1
        }

        public enum EnvironmentFlags : byte
        {
            Linux = 0x6C,   //l
            Windows = 0x77, //w
            Mac = 0x6D,     //m
            MacOsX = 0x6F   //o
        }

        public enum ServerTypeFlags : byte
        {
            Dedicated = 0x64,     //d
            Nondedicated = 0x6C,   //l
            SourceTV = 0x70   //p
        }

        #endregion Strong Typing Enumerators

        public byte Header { get; set; }        // I
        public byte Protocol { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public string Folder { get; set; }
        public string Game { get; set; }
        public short ID { get; set; }
        public byte Players { get; set; }
        public byte MaxPlayers { get; set; }
        public byte Bots { get; set; }
        public ServerTypeFlags ServerType { get; set; }
        public EnvironmentFlags Environment { get; set; }
        public VisibilityFlags Visibility { get; set; }
        public VACFlags VAC { get; set; }
        public string Version { get; set; }
        public ExtraDataFlags ExtraDataFlag { get; set; }

        #region Extra Data Flag Members

        public ulong GameID { get; set; }           //0x01
        public ulong SteamID { get; set; }          //0x10
        public string Keywords { get; set; }        //0x20
        public string Spectator { get; set; }       //0x40
        public short SpectatorPort { get; set; }   //0x40
        public short Port { get; set; }             //0x80

        #endregion Extra Data Flag Members

        public A2S_INFO(IPEndPoint ep)
        {
            UdpClient udp = new UdpClient();
            udp.Send(REQUEST, REQUEST.Length, ep);
            MemoryStream ms = new MemoryStream(udp.Receive(ref ep));    // Saves the received data in a memory buffer
            BinaryReader br = new BinaryReader(ms, Encoding.UTF8);      // A binary reader that treats charaters as Unicode 8-bit
            ms.Seek(4, SeekOrigin.Begin);   // skip the 4 0xFFs
            Header = br.ReadByte();
            Protocol = br.ReadByte();
            Name = ReadNullTerminatedString(ref br);
            Map = ReadNullTerminatedString(ref br);
            Folder = ReadNullTerminatedString(ref br);
            Game = ReadNullTerminatedString(ref br);
            ID = br.ReadInt16();
            Players = br.ReadByte();
            MaxPlayers = br.ReadByte();
            Bots = br.ReadByte();
            ServerType = (ServerTypeFlags)br.ReadByte();
            Environment = (EnvironmentFlags)br.ReadByte();
            Visibility = (VisibilityFlags)br.ReadByte();
            VAC = (VACFlags)br.ReadByte();
            Version = ReadNullTerminatedString(ref br);
            ExtraDataFlag = (ExtraDataFlags)br.ReadByte();

            #region These EDF readers have to be in this order because that's the way they are reported

            if (ExtraDataFlag.HasFlag(ExtraDataFlags.Port))
                Port = br.ReadInt16();
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.SteamID))
                SteamID = br.ReadUInt64();
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.Spectator))
            {
                SpectatorPort = br.ReadInt16();
                Spectator = ReadNullTerminatedString(ref br);
            }
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.Keywords))
                Keywords = ReadNullTerminatedString(ref br);
            if (ExtraDataFlag.HasFlag(ExtraDataFlags.GameID))
                GameID = br.ReadUInt64();

            #endregion These EDF readers have to be in this order because that's the way they are reported

            br.Close();
            ms.Close();
            udp.Close();
        }

        /// <summary>Reads a null-terminated string into a .NET Framework compatible string.</summary>
        /// <param name="input">Binary reader to pull the null-terminated string from.  Make sure it is correctly positioned in the stream before calling.</param>
        /// <returns>String of the same encoding as the input BinaryReader.</returns>
        public static string ReadNullTerminatedString(ref BinaryReader input)
        {
            StringBuilder sb = new StringBuilder();
            char read = input.ReadChar();
            while (read != '\x00')
            {
                sb.Append(read);
                read = input.ReadChar();
            }
            return sb.ToString();
        }
    }

    /// <summary>Adopted from Valve description: https://developer.valvesoftware.com/wiki/Server_queries#A2S_PLAYER </summary>
    public class A2S_PLAYER
    {
        public struct Player
        {
            public Player(ref BinaryReader br)
            {
                Index = br.ReadByte();
                Name = ReadNullTerminatedString(ref br);
                Score = br.ReadInt32();
                Duration = br.ReadSingle();
            }

            public byte Index { get; set; }
            public string Name { get; set; }
            public int Score { get; set; }
            public float Duration { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        // \xFF\xFF\xFF\xFFU\xFF\xFF\xFF\xFF
        public static readonly byte[] REQUEST = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x55, 0xFF, 0xFF, 0xFF, 0xFF };

        public byte Header { get; set; }        // D
        public Player[] Players { get; set; }

        public A2S_PLAYER(IPEndPoint ep)
        {
            UdpClient udp = new UdpClient();
            udp.Send(REQUEST, REQUEST.Length, ep); // Request Challenge.
            byte[] challenge_response = udp.Receive(ref ep);
            if (challenge_response.Length == 9 && challenge_response[4] == 0x41) //B
            {
                challenge_response[4] = 0x55; //U
                // \xFF\xFF\xFF\xFFU[CHALLENGE]
                udp.Send(challenge_response, challenge_response.Length, ep); // Request data.

                MemoryStream ms = new MemoryStream(udp.Receive(ref ep));    // Saves the received data in a memory buffer
                BinaryReader br = new BinaryReader(ms, Encoding.UTF8);      // A binary reader that treats charaters as Unicode 8-bit
                ms.Seek(4, SeekOrigin.Begin);   // skip the 4 0xFFs
                Header = br.ReadByte(); // D
                Players = new Player[br.ReadByte()];
                for (int i = 0; i < Players.Length; i++)
                    Players[i] = new Player(ref br);
                br.Close();
                ms.Close();
                udp.Close();
            }
            else
                throw new Exception("Response invalid.");
        }

        /// <summary>Reads a null-terminated string into a .NET Framework compatible string.</summary>
        /// <param name="input">Binary reader to pull the null-terminated string from.  Make sure it is correctly positioned in the stream before calling.</param>
        /// <returns>String of the same encoding as the input BinaryReader.</returns>
        public static string ReadNullTerminatedString(ref BinaryReader input)
        {
            StringBuilder sb = new StringBuilder();
            char read = input.ReadChar();
            while (read != '\x00')
            {
                sb.Append(read);
                read = input.ReadChar();
            }
            return sb.ToString();
        }
    }
}