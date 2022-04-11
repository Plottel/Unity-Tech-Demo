using System;
using UnityEngine;
using System.IO;
using System.Text;
using ENet;

namespace Deft.Networking
{
    public class NetworkClient
    {
        private Address serverAddress;
        private Host client;
        private Peer server;

        private Action onConnect;

        public void JoinServer(string hostName, ushort port, Action onConnectCallback = null)
        {
            // TODO: NAT Traversal.
            var address = new Address();
            address.SetHost(hostName);
            address.Port = port;

            client = new Host();
            client.Create();

            server = client.Connect(address);

            onConnect = onConnectCallback;
        }

        public bool PumpPacket(MemoryStream packetStream)
        {
            ENet.Event netEvent;

            if (client.Service(0, out netEvent) <= 0)
                return false;

            switch (netEvent.Type)
            {
                case ENet.EventType.Connect:
                    onConnect?.Invoke();
                    onConnect = null;
                    netEvent.Packet.Dispose();
                    break;

                case ENet.EventType.Receive:
                    // User-Defined packets will be Receive Packets
                    using (BinaryWriter writer = new BinaryWriter(packetStream, Encoding.Default, true))
                    {
                        Packet p = netEvent.Packet;
                        var packetData = new byte[p.Length];
                        p.CopyTo(packetData);

                        writer.Write(netEvent.Peer.IP);
                        writer.Write(packetData);
                    }

                    netEvent.Packet.Dispose();
                    return true;
            }

            return false;
        }

        public bool SendPacket(MemoryStream stream)
        {
            Packet packet = new Packet();
            int length = (int)stream.Length;
            packet.Create(stream.GetBuffer(), length, PacketFlags.Reliable);

            return server.Send(0, ref packet);
        }
    }
}

