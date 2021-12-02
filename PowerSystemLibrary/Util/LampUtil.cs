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
        public string OpenOrCloseLamp(string lampIP, AHState aHState, bool isDebug = true)
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

                    FileStream fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":关了红灯");
                    sw.Close();
                    fs.Close();

                    Thread.Sleep(2000);

                    atCmd = "AT+STACH1=1\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);

                    fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":开了绿灯");
                    sw.Close();
                    fs.Close();
                }
                else
                {

                    atCmd = "AT+STACH1=0\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);

                    FileStream fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":关了绿灯");
                    sw.Close();
                    fs.Close();

                    Thread.Sleep(2000);

                    atCmd = "AT+STACH2=1\r\n";
                    buf = StringToAsciiByte(atCmd);//===================
                    serverSocket.Send(buf);

                    fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":开了红灯");
                    sw.Close();
                    fs.Close();
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
