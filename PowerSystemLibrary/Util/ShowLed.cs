using PowerSystemLibrary.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PowerSystemLibrary.Util.Led5kSDK;

namespace PowerSystemLibrary.Util
{
    public class ShowLed
    {

        public string ShowLedMethod(string ip, bool IsNormal = false, int BrandCount = 0, bool isDebug = true)
        {
            string message = "";
            if (isDebug)
            {
                return message;
            }
            try
            {
                Encoding encoding = Encoding.GetEncoding("GB2312");
                InitSdk(2, 2);
                Connect(ip);
                if (m_dwCurHand == 0)
                {
                    //失败
                    message = "LED灯牌SDK初始化登陆失败。";
                }
                else
                {
                    //先清空所有节目
                    OFS_DeleteFile(m_dwCurHand, 0, null);


                    Led5kProgram m_Program = new Led5kProgram();
                    m_Program.AreaNum = 1;
                    m_Program.overwrite = true;
                    m_Program.name = "Program";
                    m_Program.ProgramWeek = 1;
                    m_Program.IsPlayOnTime = false;
                    m_Program.IsValidAlways = true;
                    m_Program.DisplayType = 0;
                    m_Program.PlayTimes = 10;

                    Led5kProgram m_Program2 = new Led5kProgram();
                    m_Program2.AreaNum = 1;
                    m_Program2.overwrite = true;
                    m_Program2.name = "Program2";
                    m_Program2.ProgramWeek = 1;
                    m_Program2.IsPlayOnTime = false;
                    m_Program2.IsValidAlways = true;
                    m_Program2.DisplayType = 0;
                    m_Program2.PlayTimes = 10;


                    //添加正常工作
                    //string message = @"\C2正\C2常\C2工\C2作";

                    if (IsNormal)
                    {
                        m_Program.m_arealist.Add(AddArea(@"\C2正\C2常\C2工\C2作"));

                        int err = m_Program.SendProgram(m_dwCurHand);
                        if (err != 0)
                        {
                            //失败
                            message = "修改LED灯牌状态失败，失败码:" + err;
                        }
                        FileStream fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.BaseStream.Seek(0, SeekOrigin.End);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":正常工作");
                        sw.Close();
                        fs.Close();
                    }
                    else
                    {
                        m_Program.m_arealist = new System.Collections.Generic.List<Led5kstaticArea>();
                        m_Program2.m_arealist = new System.Collections.Generic.List<Led5kstaticArea>();
                        //还剩X块
                        //牌未加完禁止送电
                        m_Program.m_arealist.Add(AddArea(string.Format(@"\C3还\C3剩\C1{0}\n\C3块\C3牌", BrandCount)));
                        m_Program2.m_arealist.Add(AddArea(@"\C3牌\C3未\C3加\C3完\n\C3禁\C3止\C3送\C3电"));

                        int err = m_Program.SendProgram(m_dwCurHand);
                        if (err != 0)
                        {
                            //失败
                            message = "修改LED灯牌状态失败，失败码:" + err + "。";
                        }
                        err = m_Program2.SendProgram(m_dwCurHand);
                        if (err != 0)
                        {
                            //失败
                            message = "修改LED灯牌状态失败，失败码:" + err + "。";
                        }

                        FileStream fs = new FileStream(@"D:\\LampTest.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs);
                        sw.BaseStream.Seek(0, SeekOrigin.End);
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":还剩" + BrandCount + "块牌，牌未加完禁止送电");
                        sw.Close();
                        fs.Close();
                    }


                    //if (err != 0)
                    //{
                    //    //失败
                    //}
                    //Thread.Sleep(10000);


                    // OFS_DeleteFile(m_dwCurHand, 0, null);



                    Destroy(m_dwCurHand);
                }
            }
            catch (Exception ex)
            {
                message = "LED灯牌改变状态失败。";
                new DAO.LogDAO().AddLog(Enum.LogCode.系统错误, ex.Message, new DBContext.PowerSystemDBContext());
            }

            return message;
        }

        private static int Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ledPort"]);
        private static uint m_dwCurHand = 0;

        private void Connect(string ip)
        {
            byte[] led_ip = System.Text.Encoding.ASCII.GetBytes(ip);

            uint led_port = Convert.ToUInt32(Port);

            uint hand = Led5kSDK.CreateClient(led_ip, led_port, Led5kSDK.bx_5k_card_type.BX_6K1, 1, 0, null);
            m_dwCurHand = hand;
        }


        private Led5kstaticArea AddArea(string message, int y = 0, int x = 0)
        {
            Led5kstaticArea area = new Led5kstaticArea();
            bx_5k_area_header bx_5k_Area_Header = new bx_5k_area_header();


            bx_5k_Area_Header.AreaType = 0x00;

            bx_5k_Area_Header.AreaX = (ushort)(Convert.ToUInt16(x) | 0x8000);
            bx_5k_Area_Header.AreaWidth = (ushort)(Convert.ToUInt16(64) | 0x8000);


            bx_5k_Area_Header.AreaY = Convert.ToUInt16(y);
            bx_5k_Area_Header.AreaHeight = Convert.ToUInt16(32);


            bx_5k_Area_Header.SingleLine = 0x02;

            bx_5k_Area_Header.NewLine = 0x01;

            bx_5k_Area_Header.Lines_sizes = 0;

            bx_5k_Area_Header.Reserved1 = 0;
            bx_5k_Area_Header.Reserved2 = 0;
            bx_5k_Area_Header.Reserved3 = 0;

            bx_5k_Area_Header.DisplayMode = 0x02;

            bx_5k_Area_Header.DynamicAreaLoc = 0xff;
            bx_5k_Area_Header.RunMode = 0;
            bx_5k_Area_Header.Timeout = 0;
            bx_5k_Area_Header.ExitMode = 0x00;

            bx_5k_Area_Header.Speed = 1;

            bx_5k_Area_Header.StayTime = 10;


            bx_5k_Area_Header.DataLen = message.Length;


            area.header = bx_5k_Area_Header;
            area.text = message;

            return area;
        }
    }

}
