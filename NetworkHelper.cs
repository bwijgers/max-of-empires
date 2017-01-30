﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MaxOfEmpires
{
    
    public class NetworkHelper
    {
        public bool connected = false;
        Socket startSocket;
        Socket connection;
        public bool server = true;
        byte[] typeBuffer = new byte[1];
        byte[] buffer = new byte[1];
        bool receivedGrid;
        int otherPlayerID;
        string otherPlayerName;
        EconomyGrid ecoGrid;
        IPAddress ConnectionAdress;
        EndPoint hostEndPoint;
        int port = 25565;
        EndPoint t;
        SocketPermission permission;
        Socket listener;
        Socket handler;

        public void StartHost()
        {
            permission = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", port);
            byte[] ip = new byte[4]
            {
                127,0,0,1
            };
            IPHostEntry ipHost = Dns.GetHostEntry("");
            //IPAddress address = new IPAddress(ip);
            IPAddress address = ipHost.AddressList[0];
            startSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            t = new IPEndPoint(address, port);
            startSocket.Bind(t);
            startSocket.Listen(10);
            AsyncCallback a = new AsyncCallback(AcceptConnection);
        }

        public void AcceptConnection(IAsyncResult ar)
        {
            listener = (Socket)ar.AsyncState;
            handler = listener.EndAccept(ar);
            handler.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), "");
        }

        public void StartClient(byte[] ip)
        {
            permission = new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", port);
            hostEndPoint = new IPEndPoint(new IPAddress(ip), port);
            IPHostEntry ipHost = Dns.GetHostEntry("");
            IPAddress address = ipHost.AddressList[0];
            connection = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            t = new IPEndPoint(address, port);
            connection.Bind(t);
            connection.Connect(hostEndPoint);
            connection.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), "");
        }

        public void CheckConnection()
        {
            if (server)
            {
                try
                {
                    connection.Listen(1);
                    connection = startSocket.Accept();
                    connected = true;
                }
                catch (SocketException)
                {

                }

            }
            else
            {
                try
                {
                    connection.Connect(hostEndPoint);
                    connected = true;
                }
                catch (SocketException)
                {

                }
            }
        }

        public void Resync()
        {

        }
        public void ReceiveGrid()
        {
            //byte[] sizeBuffer = new byte[2];
            //connection.Receive(sizeBuffer);
            //int size = sizeBuffer[0] * sizeBuffer[1];
            //byte[] terrainBuffer = new byte[size];
            //connection.Receive(sizeBuffer);
            //receivedGrid = true;
            //setGrid(terrainBuffer);
        }
        public void ReceiveTurn()
        {
            //byte[] IDBuffer = new byte[1];
            //connection.Receive(sizeBuffer);
            //if (server)
            //{
            //    if(IDBuffer[0]!= otherPlayerID || currentGrid.currentPlayer.Name != otherPlayerName)
            //    {
            //        Resync();
            //    }
            //    else
            //    {
            //        currentGrid.nextTurn();
            //        SendTurn(currentgrid.currentplayer.intId);
            //    }
            //}
            //else
            //{

            //}
        }
        public void ReceiveMove()
        {

        }
        public void ReceiveHit()
        {

        }
        public void ReceiveBuild()
        {

        }
        public void ReceiveRecruit()
        {

        }
        public void ReceiveUpgrade()
        {

        }

        public void ReceiveMessage(IAsyncResult ar)
        {
            int bytes = connection.Receive(typeBuffer, 1, SocketFlags.None);
            while (bytes > 0)
            {
                switch (typeBuffer[0])
                {
                    case 255:
                        Resync();
                        break;
                    case 0:
                        ReceiveGrid();
                        break;
                    case 1:
                        ReceiveTurn();
                        break;
                    case 2:
                        ReceiveMove();
                        break;
                    case 3:
                        ReceiveHit();
                        break;
                    case 4:
                        ReceiveBuild();
                        break;
                    case 5:
                        ReceiveRecruit();
                        break;
                    case 6:
                        ReceiveUpgrade();
                        break;
                    default:
                        break;
                }

                bytes = connection.Receive(typeBuffer, 1, SocketFlags.None);
            }
        }
    }


}
