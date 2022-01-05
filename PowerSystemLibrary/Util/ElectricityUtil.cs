using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class ElectricityUtil
    {
        private static int Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["electricityPort"]);     
        public bool IsElectricityOn(ref string message, string electricityGatewayIP, int electricityAddress)
        {
            IPAddress ip = IPAddress.Parse(electricityGatewayIP);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(ip, Port));

            byte[] fullSendArray = new byte[12];
            fullSendArray[0] = 10;
            fullSendArray[1] = 24;
            fullSendArray[2] = 0;
            fullSendArray[3] = 0;
            fullSendArray[4] = 0;
            fullSendArray[5] = 6;
            fullSendArray[6] = 1;
            fullSendArray[7] = 1;
            fullSendArray[8] = 0;
            fullSendArray[9] = (byte)electricityAddress;
            fullSendArray[10] = 0;
            fullSendArray[11] = 1;


            clientSocket.Send(fullSendArray, fullSendArray.Length, SocketFlags.None);

            byte[] resultArray = new byte[100];
            int length = clientSocket.Receive(resultArray);
            byte[] dataArray = new byte[length];
            Array.Copy(resultArray, 0, dataArray, 0, length);

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            if (dataArray[0] == 10 && dataArray[1] == 24)
            {
                if (dataArray[dataArray.Length - 1] == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                message = "无法获取现场电柜带电状态";
            }

            return true;
        }
    }
}
