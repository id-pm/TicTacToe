using Open.Nat;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TTTServer
{
    class Program
    {
        public static IPAddress IPb;
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания
        static void Main(string[] args)
        {

            //HUI();
            //HUYAKA();
            Thread.Sleep(6000);
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
        private static async void HUI()
        {
            var discoverer = new NatDiscoverer();

            // using SSDP protocol, it discovers NAT device.
            var device = await discoverer.DiscoverDeviceAsync();
            IPb = await device.GetExternalIPAsync();
            // display the NAT's IP address
            Console.WriteLine("The external IP Address is: {0} ", IPb);

            // create a new mapping in the router [external_ip:1702 -> host_machine:1602]
            await device.CreatePortMapAsync(new Mapping(Protocol.Tcp,  1700, 3210, "For testing"));

            //// configure a TCP socket listening on port 1602
            //var endPoint = new IPEndPoint(IPAddress.Any, 3210);
            //var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //socket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            //socket.Bind(endPoint);
            //socket.Listen(4);
        }
        private static async void HUYAKA()
        {
            Console.WriteLine("POSHLA huyaka");
            try
            {
                var nat = new NatDiscoverer();

                var cts = new CancellationTokenSource();
                var device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, 3210, 1700, "The mapping name"));
            }
            catch (NatDeviceNotFoundException e)
            {
                Console.WriteLine("Open.NAT wasn't able to find an Upnp device");
            }
            catch (MappingException me)
            {
                switch (me.ErrorCode)
                {
                    case 718:
                        Console.WriteLine("The external port already in use.");
                        break;
                    case 728:
                        Console.WriteLine("The router's mapping table is full.");
                        break;
                    default:
                        {
                            Console.WriteLine("NE POHUI");
                            break;
                        }
                }
            }
        }
    }
}