using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Deft.Networking;
using System.Linq;
using System.Text;
using ENet;

namespace Deft.Networking
{
    public class NetworkServer
    {
        private Host server;
        private Dictionary<uint, Peer> clients;
        private Peer localClient;

        public bool IsActive { get; private set; } = false;

        public void LaunchServer(ushort port)
        {
            var address = new Address();
            address.Port = port;

            server = new Host();
            server.Create(address, 8);

            clients = new Dictionary<uint, Peer>();

            IsActive = true;
        }

        public bool PumpPacket(MemoryStream packetStream)
        {
            ENet.Event netEvent;

            if (server.Service(0, out netEvent) <= 0)
                return false;

            switch (netEvent.Type)
            {
                case ENet.EventType.Connect:
                    // This is the ENet Connection Packet for the initial handshake
                    // If we receive this, we've successfully traversed the NAT.
                    // The Game WelcomePacket will be sent as a Receive Packet.
                    netEvent.Packet.Dispose();
                    return false;

                case ENet.EventType.Receive:
                    uint peerID = netEvent.Peer.ID;

                    // Add new Client
                    if (!clients.ContainsKey(peerID))
                    {
                        // First peer is Local Client
                        if (clients.Count == 0)
                            localClient = netEvent.Peer;

                        clients.Add(peerID, netEvent.Peer);
                    }

                    using (BinaryWriter writer = new BinaryWriter(packetStream, Encoding.Default, true))
                    {
                        Packet p = netEvent.Packet;
                        var packetData = new byte[p.Length];
                        p.CopyTo(packetData);

                        writer.Write(netEvent.Peer.ID);
                        writer.Write(packetData);
                    }

                    netEvent.Packet.Dispose();
                    return true;
            }

            return false;
        }

        public bool SendPacket(uint peerID, MemoryStream stream)
        {
            Packet packet = new Packet();
            packet.Create(stream.GetBuffer(), (int)stream.Length, PacketFlags.Reliable);

            Peer destination = clients[peerID];

            return destination.Send(0, ref packet);
        }

        public void BroadcastPacket(MemoryStream stream)
        {
            Packet packet = new Packet();
            packet.Create(stream.GetBuffer(), (int)stream.Length, PacketFlags.Reliable);

            server.Broadcast(0, ref packet, localClient);
        }

        public void BroadcastPacket(MemoryStream stream, uint excludedPeerID)
        {
            Packet packet = new Packet();
            packet.Create(stream.GetBuffer(), (int)stream.Length, PacketFlags.Reliable);

            server.Broadcast(0, ref packet, clients[excludedPeerID]);
        }

        public void TrueBroadcastPacket(MemoryStream stream)
        {
            Packet packet = new Packet();
            packet.Create(stream.GetBuffer(), (int)stream.Length, PacketFlags.Reliable);

            server.Broadcast(0, ref packet);
        }
    }
}