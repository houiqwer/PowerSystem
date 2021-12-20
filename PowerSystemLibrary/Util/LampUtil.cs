using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class LampUtil
    {
        private static int Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["lampPort"]);
        public string OpenOrCloseLamp(string lampIP, AHState aHState, bool isDebug = false)
        {
            string message = string.Empty;
            if (isDebug)
            {
                return message;
            }
            try
            {
                IPAddress ip = IPAddress.Parse(lampIP);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(new IPEndPoint(ip, Port));

                string atCmd = "AT+STACH1=1\r\n";
                byte[] buf = new byte[0];
                if (aHState == AHState.正常)
                {
                    atCmd = "AT+STACH2=0\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);

                    Thread.Sleep(2000);

                    atCmd = "AT+STACH1=1\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);
                
                }
                else
                {

                    atCmd = "AT+STACH1=0\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);
                  

                    Thread.Sleep(2000);

                    atCmd = "AT+STACH2=1\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);
                }

                serverSocket.Dispose();
            }
            catch (Exception ex)
            {
                message = "无法连接现场报警灯。";

                new DAO.LogDAO().AddLog(LogCode.系统错误, ex.Message, new DBContext.PowerSystemDBContext());
            }

            return message;
        }

        static public byte[] StringToAsciiByte(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }
}
