using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkRift;
using DarkRift.Server;
using Penca53.DarkRift.MLAPI;

namespace DarkRiftLibrary_MLAPI_Test_Server
{
    public class Program : Plugin
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version();

        Dictionary<ushort, IClient> Clients = new Dictionary<ushort, IClient>();


        public static House House = new House() { Width = 30.25f, Height = 12.5f, Inhabitants = 50 };

        public Program(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

        void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Clients.Add(e.Client.ID, e.Client);

            Console.Write("Insert a message number from 0 to 6 ");
            SendMessage(int.Parse(Console.ReadLine()));
        }

        void SendMessage(int id)
        {
            if (id == 0)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.Write(House);

                    using (Message message = Message.Create(Tags.HouseNormal, writer))
                    {
                        foreach(IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 1)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, ExtraSyncData.TypeIDANDTag, 1);

                    using (Message message = Message.Create(Tags.HouseNothing, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 2)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, 1, true);

                    using (Message message = Message.Create(Tags.HouseType, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 3)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, 1, false, true);

                    using (Message message = Message.Create(Tags.HouseTag, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 4)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, 1);

                    using (Message message = Message.Create(Tags.HouseTypeANDTag, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 5)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, ExtraSyncData.TypeIDANDTag, 1);

                    using (Message message = Message.Create(Tags.HouseUpdateNothing, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }
            else if (id == 6)
            {
                using (DarkRiftWriter writer = DarkRiftWriter.Create())
                {
                    writer.WriteSmart(House, ExtraSyncData.TypeID, 1);

                    using (Message message = Message.Create(Tags.HouseUpdateTag, writer))
                    {
                        foreach (IClient client in Clients.Values)
                        {
                            client.SendMessage(message, SendMode.Reliable);
                        }
                    }
                }
            }

            Console.WriteLine("Insert a message number from 0 to 6 ");
            SendMessage(int.Parse(Console.ReadLine()));
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Clients.Remove(e.Client.ID);
        }
    }

    static class Tags
    {
        public static readonly ushort HouseNormal = 0;
        public static readonly ushort HouseNothing = 1;
        public static readonly ushort HouseType = 2;
        public static readonly ushort HouseTag = 3;
        public static readonly ushort HouseTypeANDTag = 4;
        public static readonly ushort HouseUpdateNothing = 5;
        public static readonly ushort HouseUpdateTag = 6;
    }
}
