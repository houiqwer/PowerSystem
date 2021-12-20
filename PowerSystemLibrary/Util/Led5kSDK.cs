﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class Led5kSDK
    {
        #region
        public class bx5k_err
        {
            public const int ERR_NO = 0; //No Error 
            public const int ERR_OUTOFGROUP = 1; //Command Group Error 
            public const int ERR_NOCMD = 2; //Command Not Found 
            public const int ERR_BUSY = 3; //The Controller is busy now 
            public const int ERR_MEMORYVOLUME = 4; //Out of the Memory Volume 
            public const int ERR_CHECKSUM = 5; //CRC16 Checksum Error 
            public const int ERR_FILENOTEXIST = 6; //File Not Exist 
            public const int ERR_FLASH = 7;//Flash Access Error 
            public const int ERR_FILE_DOWNLOAD = 8; //File Download Error 
            public const int ERR_FILE_NAME = 9; //Filename Error 
            public const int ERR_FILE_TYPE = 10;//File type Error 
            public const int ERR_FILE_CRC16 = 11;//File CRC16 Error 
            public const int ERR_FONT_NOT_EXIST = 12;//Font Library Not Exist 
            public const int ERR_FIRMWARE_TYPE = 13;//Firmware Type Error (Check the controller type) 
            public const int ERR_DATE_TIME_FORMAT = 14;//Date Time format error 
            public const int ERR_FILE_EXIST = 15;//File Exist for File overwrite 
            public const int ERR_FILE_BLOCK_NUM = 16;//File block number error 
            public const int ERR_COMMUNICATE = 100;//通信失败
            public const int ERR_PROTOCOL = 101;//协议数据不正确
            public const int ERR_TIMEOUT = 102;//通信超时
            public const int ERR_NETCLOSE = 103;//通信断开
            public const int ERR_INVALID_HAND = 104;//无效句柄
            public const int ERR_PARAMETER = 105;//参数错误
            public const int ERR_SHOULDREPEAT = 106;//需要重复上次数据包
            public const int ERR_FILE = 107;//无效文件
        }
        #endregion
        //串口停止位
        public enum serial_stopbits : byte
        {
            COM_ONESTOPBIT = 0,
            COM_ONE5STOPBITS = 1,
            COM_TWOSTOPBITS = 2,
        }
        //串口校验模式
        public enum serial_parity : byte
        {
            COM_NOPARITY = 0,
            COM_ODDPARITY = 1,
            COM_EVENPARITY = 2,
            COM_MARKPARITY = 3,
            COM_SPACEPARITY = 4,
        }
        //串口数据位
        public enum serial_databits : byte
        {
            COM_4BITS = 4,
            COM_5BITS = 5,
            COM_6BITS = 6,
            COM_7BITS = 7,
            COM_8BITS = 8,
        }
        //控制器类型
        public enum bx_5k_card_type : byte
        {
            BX_5K1 = 0x51,
            BX_5K2 = 0x58,
            BX_5MK2 = 0x53,
            BX_5MK1 = 0x54,
            BX_5K1Q_YY = 0x5c,
            BX_Any = 0xFE,
            BX_6K1 = 0x61,
            BX_6K2 = 0x62,
            BX_6K3 = 0x63,
            BX_6K1_YY = 0x64,
            BX_6K2_YY = 0x65,
            BX_6K3_YY = 0x66,
            BX_6K1_4G = 0x67,
            BX_6K2_4G = 0x68,
        }
        //-------区域格式------
        // area header | data |
        //---------------------
        //节目内区域定义
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct bx_5k_area_header
        {
            public byte AreaType;
            public ushort AreaX;
            public ushort AreaY;
            public ushort AreaWidth;
            public ushort AreaHeight;
            public byte DynamicAreaLoc;
            public byte Lines_sizes;
            public byte RunMode;
            public short Timeout;
            public byte Reserved1;
            public byte Reserved2;
            public byte Reserved3;
            public byte SingleLine;
            public byte NewLine;
            public byte DisplayMode;
            public byte ExitMode;
            public byte Speed;
            public byte StayTime;
            public int DataLen;
        }
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct bx_5k_sound
        {
            public byte StoreFlag;
            public byte SoundPerson;//一个字节
            public byte SoundVolum;
            public byte SoundSpeed;
            public byte SoundDataMode;
            public int SoundReplayTimes;
            public int SoundReplayDelay;
            public byte SoundReservedParaLen;
            public int SoundDataLen;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.StdCall)]
        public delegate void CallBackClientClose(uint hand, int err);

        //E:\Program Files\WXFile\WeChat Files\wxid_fp3opqqmg8x822\FileStorage\File\2021-11\BX-5K 5MK 6K 6K-YY Font card\DEMO\Led5kSDK\Led5KSDKDemoCSharp\bin\Release
        //初始化动态库
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void InitSdk(byte minorVer, byte majorVer);
        //释放动态库
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void ReleaseSdk();
        //创建广播通讯模式
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateBroadCast(byte[] broad_ip, uint broad_port, bx_5k_card_type card_type, byte[] barcode, byte Option, int mode);
        //创建固定IP通讯模式
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateClient(byte[] led_ip, uint led_port, bx_5k_card_type card_type, int tmout_sec, int mode, CallBackClientClose pCloseFunc);
        //创建TCP Modbus通讯
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateTcpModbus(byte[] led_ip, bx_5k_card_type card_type, CallBackClientClose pCloseFunc);
        //创建串口通讯
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateComClient(byte com, uint baudrate, bx_5k_card_type card_type, int mode, ushort ScreenID);
        //创建串口Modbus通讯
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern uint CreateComModbus(byte com, uint baudrate, serial_parity Parity, serial_databits DataBits,
            serial_stopbits StopBits, bx_5k_card_type card_type, ushort ScreenID);
        //销毁通讯
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void Destroy(uint dwHand);
        //设置通讯超时
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void SetTimeout(uint dwHand, uint nSec);
        //ping
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_PING(uint dwHand);
        //复位
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_Reset(uint dwHand);
        //
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_ControllerStatus(uint dwHand, byte[] pStatus, ref ushort len);
        //查询字库信息
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_CheckCurrentFont(uint dwHand, byte[] fontStatus, ref ushort len);
        //回读客户信息
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_CheckCurrentCustomer(uint dwHand, byte[] CustomerStatus, ref ushort len);
        //参数回读
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_ReadScreen(uint dwHand, byte[] ScreenStatus, ref ushort len);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_SytemClockCorrect(uint dwHand);
        //查询固件状态
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_CheckCurrentFirmware(uint dwHand, byte[] FirmwareName, byte[] FirmwareVersion, byte[] FirmwareDateTime);
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendFirmWareData(uint dwHand, byte overwrite, byte[] pFileName, byte[] FirmWareData, int FirmWareDataLen, CloseFunc pCloseFunc);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_FirmwareActivate(uint dwHand, byte[] FirmwareName);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_SetScreenID(uint dwHand, ushort newScreenID);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_ReadScreenID(uint dwHand, ref ushort pScreenID);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_ForceOnOff(uint dwHand, byte OnOffFlag);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_TimeTurnOnOff(uint dwHand, byte[] pTimer, int nGroup);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_SetBrightness(uint dwHand, byte BrightnessType, byte CurrentBrightness, byte[] BrightnessValue);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_SetWaitTime(uint dwHand, byte WaitTime);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_LockProgram(uint dwHand, byte LockFlag, byte StoreMode, byte[] ProgramFileName);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_DelDynamicArea(uint dwHand, byte DeleteAreaId);

        //动态区
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_SendDynamicArea(uint dwHand, bx_5k_area_header header, ushort TextLen, byte[] AreaText);
        //语音
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_SendSound(uint dwHand, bx_5k_sound sound, int TextLen, byte[] AreaText);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_SendSoundDynamicArea(uint dwHand, bx_5k_area_header header, ushort TextLen, byte[] AreaText, byte SoundMode, byte SoundPerson, byte SoundVolume, byte SoundSpeed, int sound_len, byte[] sounddata);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_Test(uint dwHand, byte TestTime);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SCREEN_CancelTimeOnOff(uint dwHand);

        #region 设置特殊动态区动态
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SetSpecialAppDynamic(uint dwHand, ushort AreaX, ushort AreaY, ushort AreaW, ushort AreaH,
            byte DataType, byte Pagetotal, byte RunState, ushort Timeout, byte SingleLine, byte Lines_sizes, byte NewLine, ushort StayTime);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SendPageData(uint dwHand, byte PageNum, ushort PageDataLen, byte[] PageData);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SendLatticeMessage(uint dwHand, byte BlockFlag, ushort BlockAddr, byte[] BlockData, ushort BlockDataLen);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_DelSpecialAppDynamic(uint dwHand);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SetIPAddress(uint dwHand, byte ConnnectMode, byte[] ip, byte[] SubnetMask, byte[] Gateway, ushort port,
            byte ServerMode, byte[] ServerIPAddress, ushort ServerPort, byte[] ServerAccessPassword, ushort HeartBeatInterval, byte[] NetID);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SetMACAddress(uint dwHand, byte[] MAC);

        //设置特殊动态区动态
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_SetSpecialAppDynamic(int dwHand, ushort AreaX, ushort AreaY, ushort AreaW, ushort AreaH, byte DataType, byte Pagetotal,
            byte RunState, ushort Timeout, byte SingleLine, byte Lines_sizes, byte NewLine, ushort StayTime);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_WebSearch(uint dwHand, ref ushort Status, ref ushort Error, byte[] IP,
            byte[] SubNetMask, byte[] Gate, ref ushort Port, byte[] Mac, byte[] NetID);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_DelPageData(uint dwHand, byte PageLog);
        #endregion

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_Formatting(uint dwHand);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_DeleteFile(uint dwHand, ushort FileNumber, byte[] pFileNameList);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_BeginSendMultiFiles(uint dwHand);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendFile(uint dwHand, byte overwrite, byte[] pFilePath);
        //发送节目
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendFileData(uint dwHand, byte overwrite, byte[] pFileName, ushort DisplayType, byte PlayTimes,
            byte[] ProgramLife, byte ProgramWeek, byte ProgramTime, byte[] Period, byte AreaNum, byte[] AreaDataList, int AreaDataListLen);

        //添加扫描
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendScanData(uint dwHand, byte overwrite, byte[] pFileName, byte[] ScanData, int ScanDataLen);

        //添加字库
        public delegate void CloseFunc(int total, int sendlen);
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendFontData(uint dwHand, byte overwrite, byte[] pFileName, byte FontWidth, byte FontHeight,
            byte[] LibData, int LibData_len, byte FontEncode, CloseFunc pCloseFunc);

        //设置屏参
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SendScreenData(uint dwHand, byte overwrite, byte[] pFileName, ushort Address, byte Baudrate,
            ushort ScreenWith, ushort ScreenHeight, byte Color, byte MirrorMode, byte OE, byte DA, byte RowOrder, byte FreqPar,
            byte OEAngle, byte CommTimeout, byte TipLanguage, byte LatticeMode);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_EndSendMultiFiles(uint dwHand);

        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int CON_SetDispInfo(uint dwHand, byte DispInfo);

        //设置客户信息
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int OFS_SetFontInformation(uint dwHand, byte OverWrite, byte[] ClientMsg);

        public delegate void CallBackCon(uint dwHand, string pid);
        public delegate void CallBackLedClose(uint dwHand, string pid, int err_code);
        //启动gprs服务器
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern bool StartGprsServer(uint port, CallBackCon pCallBackCon, CallBackLedClose pCallBackLedClose);
        //关闭gprs服务器                 
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void CloseGprsServer();
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern void SetGprsAliveTick(uint dwHand, int time_sec);

        //扫描
        //返回值：0：发送数据正确；ERR_TIMEOUT：通讯超时；其它：返回错误代码。
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SendAndRecvBuff(uint dwHand, byte cmd_group, byte cmd, byte[] cmd_data, ushort data_len, byte[] recv_data, ref short p_recv_len);
        //返回值：0：发送数据正确；ERR_TIMEOUT：通讯超时；其它：返回错误代码。
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int SendBuff(uint dwHand, byte cmd_group, byte cmd, byte[] cmd_data, ushort data_len);
        // 网络搜索 
        [DllImport(@"E:\Led5kSDK.dll", CharSet = CharSet.Unicode)]
        public static extern int BX5MK_WebSearch(uint dwHand, byte[] recv_buff, ushort[] recv_len);
    }


    public class ItemObject
    {
        public string Text = "";
        public uint Value = 0;//可以多个

        public ItemObject(string _text, uint _value)
        {
            Text = _text;
            Value = _value;
        }
    }

    public class Led5kstaticArea
    {
        public Led5kSDK.bx_5k_area_header header;
        public string text;
        public byte[] AreaToByteArray()
        {
            //计算header的大小：结构体bx_5k_area_header的大小
            Led5kSDK.bx_5k_area_header tu = new Led5kSDK.bx_5k_area_header();
            int hsz = Marshal.SizeOf(tu);

            //计算len的大小
            text = text.Replace("￦￦F", "\\F");
            List<byte[]> Byte_Area = new List<byte[]>();
            int Byte_t = 0;
            string[] str_Area = text.Split('\\');
            int font = 0;
            for (int n = 0; n < str_Area.Length; n++)
            {
                if (n > 0 && str_Area[n].Length > 1)
                {
                    if (str_Area[n].Substring(0, 2).Equals("FK"))
                    {
                        font = 1;
                        if (str_Area[n].Length > 5)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 5));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 5);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("Unicode").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else if (str_Area[n].Substring(0, 2).Equals("FE") || str_Area[n].Substring(0, 2).Equals("FO") || str_Area[n].Substring(0, 2).Equals("WF") || str_Area[n].Substring(0, 2).Equals("WC"))
                    {
                        font = 0;
                        byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                        byte[] special = new byte[special_1.Length + 1];
                        special[0] = 0x5c;
                        for (int c = 0; c < special_1.Length; c++)
                        {
                            special[c + 1] = special_1[c];
                        }
                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                    else if (str_Area[n].Substring(0, 1).Equals("C") || str_Area[n].Substring(0, 1).Equals("D") || str_Area[n].Substring(0, 1).Equals("B") || str_Area[n].Substring(0, 1).Equals("T"))
                    {
                        if (font == 1)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 2));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 2);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else if (str_Area[n].Substring(0, 1).Equals("n"))
                    {
                        if (font == 1)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 1));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 1);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else
                    {
                        if (font == 1)
                        {
                            byte[] Korean = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[Korean.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < Korean.Length; c++)
                            {
                                special[c + 1] = Korean[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                }
                else
                {
                    if (n > 0)
                    {
                        byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                        byte[] special = new byte[special_1.Length + 1];
                        special[0] = 0x5c;
                        for (int c = 0; c < special_1.Length; c++)
                        {
                            special[c + 1] = special_1[c];
                        }
                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                    else
                    {
                        byte[] special = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);

                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                }
            }
            byte[] tmp = new byte[Byte_t];
            int g = 0;
            for (int n = 0; n < Byte_Area.Count(); n++)
            {
                if (n > 0)
                {
                    for (int j = 0; j < Byte_Area[n].Length; j++)
                    {
                        tmp[g + j] = Byte_Area[n][j];
                    }
                    g += Byte_Area[n].Length;
                }
                else
                {
                    for (int j = 0; j < Byte_Area[n].Length; j++)
                    {
                        tmp[j] = Byte_Area[n][j];
                    }
                    g += Byte_Area[n].Length;
                }
            }
            int len = tmp.Length + hsz + 4;
            header.DataLen = tmp.Length;
            //先copy len
            byte[] bt = new byte[len];
            //byte[] lenToByte = System.BitConverter.GetBytes(len);
            byte[] lenToByte = System.BitConverter.GetBytes(len);
            lenToByte.CopyTo(bt, 0);
            int index = lenToByte.Length;

            //再copy header
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(hsz);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(header, structPtr, false);

            //从内存空间拷到AreaDataList数组
            Marshal.Copy(structPtr, bt, index, hsz);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //copy text
            tmp.CopyTo(bt, index + hsz);
            return bt;
        }
        public int getAreaLen()
        {
            Led5kSDK.bx_5k_area_header tu = new Led5kSDK.bx_5k_area_header();
            int hsz = Marshal.SizeOf(tu);
            //再考header
            text = text.Replace("￦￦F", "\\F");
            List<byte[]> Byte_Area = new List<byte[]>();
            int Byte_t = 0;
            string[] str_Area = text.Split('\\');
            int font = 0;
            for (int n = 0; n < str_Area.Length; n++)
            {
                if (n > 0 && str_Area[n].Length > 1)
                {
                    if (str_Area[n].Substring(0, 2).Equals("FK"))
                    {
                        font = 1;
                        if (str_Area[n].Length > 5)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 5));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 5);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("Unicode").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else if (str_Area[n].Substring(0, 2).Equals("FE") || str_Area[n].Substring(0, 2).Equals("FO") || str_Area[n].Substring(0, 2).Equals("WF") || str_Area[n].Substring(0, 2).Equals("WC"))
                    {
                        font = 0;
                        byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                        byte[] special = new byte[special_1.Length + 1];
                        special[0] = 0x5c;
                        for (int c = 0; c < special_1.Length; c++)
                        {
                            special[c + 1] = special_1[c];
                        }
                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                    else if (str_Area[n].Substring(0, 1).Equals("C") || str_Area[n].Substring(0, 1).Equals("D") || str_Area[n].Substring(0, 1).Equals("B") || str_Area[n].Substring(0, 1).Equals("T"))
                    {
                        if (font == 1)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 2));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 2);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else if (str_Area[n].Substring(0, 1).Equals("n"))
                    {
                        if (font == 1)
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n].Substring(0, 1));
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;

                            string Area_str = str_Area[n].Remove(0, 1);
                            byte[] Korean = System.Text.Encoding.Unicode.GetBytes(Area_str);
                            for (int k = 0; k < Korean.Length / 2; k++)
                            {
                                byte a = Korean[k * 2];
                                Korean[k * 2] = Korean[k * 2 + 1];
                                Korean[k * 2 + 1] = a;
                            }
                            Byte_Area.Add(Korean);
                            Byte_t += Korean.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                    else
                    {
                        if (font == 1)
                        {
                            byte[] Korean = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[Korean.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < Korean.Length; c++)
                            {
                                special[c + 1] = Korean[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                        else
                        {
                            byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                            byte[] special = new byte[special_1.Length + 1];
                            special[0] = 0x5c;
                            for (int c = 0; c < special_1.Length; c++)
                            {
                                special[c + 1] = special_1[c];
                            }
                            Byte_Area.Add(special);
                            Byte_t += special.Length;
                        }
                    }
                }
                else
                {
                    if (n > 0)
                    {
                        byte[] special_1 = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);
                        byte[] special = new byte[special_1.Length + 1];
                        special[0] = 0x5c;
                        for (int c = 0; c < special_1.Length; c++)
                        {
                            special[c + 1] = special_1[c];
                        }
                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                    else
                    {
                        byte[] special = System.Text.Encoding.GetEncoding("GBK").GetBytes(str_Area[n]);

                        Byte_Area.Add(special);
                        Byte_t += special.Length;
                    }
                }
            }
            byte[] tmp = new byte[Byte_t];
            int g = 0;
            for (int n = 0; n < Byte_Area.Count(); n++)
            {
                if (n > 0)
                {
                    for (int j = 0; j < Byte_Area[n].Length; j++)
                    {
                        tmp[g + j] = Byte_Area[n][j];
                    }
                    g += Byte_Area[n].Length;
                }
                else
                {
                    for (int j = 0; j < Byte_Area[n].Length; j++)
                    {
                        tmp[j] = Byte_Area[n][j];
                    }
                    g += Byte_Area[n].Length;
                }
            }
            int len = tmp.Length + hsz + 4;
            return len;
        }
    }
    public class Led5kProgram
    {
        public string name;
        public bool overwrite;

        public ushort DisplayType;
        public byte PlayTimes;

        public bool IsValidAlways;
        public ushort StartYear;
        public byte StartMonth;
        public byte StartDay;
        public ushort EndYear;
        public byte EndMonth;
        public byte EndDay;

        public byte ProgramWeek;

        public bool IsPlayOnTime;
        public byte StartHour;
        public byte StartMinute;
        public byte StartSecond;
        public byte EndHour;
        public byte EndMinute;
        public byte EndSecond;
        public byte AreaNum;
        public List<Led5kstaticArea> m_arealist = new List<Led5kstaticArea>();

        #region//转DCB码
        public static byte byte2bcd(byte num)
        {
            int i = num;
            return (byte)(i / 10 * 16 + i % 10);
        }
        public static byte bcd2byte(byte num)
        {
            int i = num;
            return (byte)(i / 16 * 10 + i % 16);
        }
        public static byte[] short2bcd(ushort num)
        {
            int i = num;
            byte high = (byte)(i / 100);
            byte low = (byte)(i % 100);
            byte[] tmp = new byte[2];
            tmp[0] = byte2bcd(low);
            tmp[1] = byte2bcd(high);
            return tmp;
        }
        #endregion

        public int SendProgram(uint hand)
        {

            byte[] ppFileName;
            byte[] ProgramLife;
            byte PlayPeriodGrpNum;
            byte[] Period;
            byte[] AreaDataList;
            int AreaDataListLen;

            int sum = 0;
            foreach (Led5kstaticArea s in m_arealist)
            {
                sum += s.getAreaLen();
            }
            AreaDataList = new byte[sum];
            int index = 0;
            foreach (Led5kstaticArea s in m_arealist)
            {
                byte[] bt = s.AreaToByteArray();
                bt.CopyTo(AreaDataList, index);
                index += bt.Length;
            }
            AreaDataListLen = sum;
            if (IsValidAlways == true)
            {
                ProgramLife = new byte[8];
                ProgramLife[0] = 0xff;
                ProgramLife[1] = 0xff;
                ProgramLife[2] = 0xff;
                ProgramLife[3] = 0xff;
                ProgramLife[4] = 0xff;
                ProgramLife[5] = 0xff;
                ProgramLife[6] = 0xff;
                ProgramLife[7] = 0xff;
            }
            else
            {

                ProgramLife = new byte[8];
                byte[] tmp = Led5kProgram.short2bcd(StartYear);
                ProgramLife[0] = tmp[0];
                ProgramLife[1] = tmp[1];
                ProgramLife[2] = byte2bcd(StartMonth);
                ProgramLife[3] = byte2bcd(StartDay);

                byte[] tmp1 = Led5kProgram.short2bcd(EndYear);

                ProgramLife[4] = tmp1[0];
                ProgramLife[5] = tmp1[1];
                ProgramLife[6] = byte2bcd(EndMonth);
                ProgramLife[7] = byte2bcd(EndDay);
            }


            ppFileName = System.Text.Encoding.Default.GetBytes(name);

            if (IsPlayOnTime == true)
            {
                Period = new byte[7];
                Period[0] = byte2bcd(StartHour);
                Period[1] = byte2bcd(StartMinute);
                Period[2] = byte2bcd(StartSecond);
                Period[3] = byte2bcd(EndHour);
                Period[4] = byte2bcd(EndMinute);
                Period[5] = byte2bcd(EndSecond);
                Period[6] = 0;

            }
            else
            {
                Period = null;
            }

            PlayPeriodGrpNum = Convert.ToByte(IsPlayOnTime ? 1 : 0);

            return Led5kSDK.OFS_SendFileData(hand, 1, ppFileName, DisplayType, PlayTimes, ProgramLife,
                ProgramWeek, PlayPeriodGrpNum, Period, AreaNum, AreaDataList, AreaDataListLen);
        }
    }
}
