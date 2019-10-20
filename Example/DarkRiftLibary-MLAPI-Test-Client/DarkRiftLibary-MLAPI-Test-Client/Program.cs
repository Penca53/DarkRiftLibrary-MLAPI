using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DarkRift;
using DarkRift.Client;
using Penca53.DarkRift.MLAPI;

namespace DarkRiftLibary_MLAPI_Test_Client
{
    class Program
    {
        public static DarkRiftClient DarkRiftClient = new DarkRiftClient();
        public static House House = new House() { Width = 50, Height = 30, Inhabitants = 25 };

        static void Main(string[] args)
        {
            MLAPI.SyncTypes.Add(1, typeof(House));

            DarkRiftClient.MessageReceived += MessageReceived;
            DarkRiftClient.Connect(System.Net.IPAddress.Loopback, 4296, IPVersion.IPv4);

            Console.ReadLine();
        }

        static void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Received a message with tag: " + e.Tag);

            using (DarkRiftReader reader = e.GetMessage().GetReader())
            {
                if (e.Tag == Tags.HouseNormal)
                {
                    House _houseNormal = reader.ReadSerializable<House>();

                    Console.WriteLine("HouseNormal Width is: " + _houseNormal.Width);
                    Console.WriteLine("HouseNormal Height is: " + _houseNormal.Height);
                    Console.WriteLine("HouseNormal Inhabitants is: " + _houseNormal.Inhabitants);
                }
                else if (e.Tag == Tags.HouseNothing)
                {
                    ISync _houseNothing = reader.ReadSerializableSmart();

                    if (_houseNothing.TypeID == 1)
                    {
                        House _house = (House)_houseNothing;

                        Console.WriteLine("HouseNothing Width is: " + _house.Width);
                        Console.WriteLine("HouseNothing Height is: " + _house.Height);
                        Console.WriteLine("HouseNothing Inhabitants is: " + _house.Inhabitants);
                    }
                }
                else if (e.Tag == Tags.HouseType)
                {
                    House _houseType = reader.ReadSerializableSmart<House>();

                    Console.WriteLine("HouseType Width is: " + _houseType.Width);
                    Console.WriteLine("HouseType Height is: " + _houseType.Height);
                    Console.WriteLine("HouseType Inhabitants is: " + _houseType.Inhabitants);
                }
                else if (e.Tag == Tags.HouseTag)
                {
                    ISync _houseTag = reader.ReadSerializableSmart(1);

                    if (_houseTag.TypeID == 1)
                    {
                        House _house = (House)_houseTag;

                        Console.WriteLine("HouseTag Width is: " + _house.Width);
                        Console.WriteLine("HouseTag Height is: " + _house.Height);
                        Console.WriteLine("HouseTag Inhabitants is: " + _house.Inhabitants);
                    }
                }
                else if (e.Tag == Tags.HouseTypeANDTag)
                {
                    House _houseTypeANDTag = reader.ReadSerializableSmart<House>(1);

                    Console.WriteLine("HouseTypeANDTag Width is: " + _houseTypeANDTag.Width);
                    Console.WriteLine("HouseTypeANDTag Height is: " + _houseTypeANDTag.Height);
                    Console.WriteLine("HouseTypeANDTag Inhabitants is: " + _houseTypeANDTag.Inhabitants);
                }
                else if (e.Tag == Tags.HouseUpdateNothing)
                {
                    Console.WriteLine("HouseUpdateTag Width was: " + House.Width);
                    Console.WriteLine("HouseUpdateTag Height was: " + House.Height);
                    Console.WriteLine("HouseUpdateTag Inhabitants was: " + House.Inhabitants);

                    House.ReadSerializableSmart(reader);

                    Console.WriteLine("HouseUpdateNothing Width is: " + House.Width);
                    Console.WriteLine("HouseUpdateNothing Height is: " + House.Height);
                    Console.WriteLine("HouseUpdateNothing Inhabitants is: " + House.Inhabitants);
                }
                else if (e.Tag == Tags.HouseUpdateTag)
                {
                    Console.WriteLine("HouseUpdateTag Width was: " + House.Width);
                    Console.WriteLine("HouseUpdateTag Height was: " + House.Height);
                    Console.WriteLine("HouseUpdateTag Inhabitants was: " + House.Inhabitants);

                    House.ReadSerializableSmart(reader, 1);

                    Console.WriteLine("HouseUpdateTag Width is: " + House.Width);
                    Console.WriteLine("HouseUpdateTag Height is: " + House.Height);
                    Console.WriteLine("HouseUpdateTag Inhabitants is: " + House.Inhabitants);
                }
            }
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
