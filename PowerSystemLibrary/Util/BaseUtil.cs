using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using PowerSystemLibrary.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace PowerSystemLibrary.Util
{
    public class BaseUtil
    {
        public string BuildPassword(string username, string password)
        {
            return GetMD5Hash(GetMD5Hash(username + password) + "gkdz3821930");
        }

        public string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        public string GetMD5Hash(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return "";
            }
            else
            {
                string output = string.Empty;
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
                StringBuilder result = new StringBuilder();

                foreach (byte @byte in data)
                {
                    result.Append(@byte.ToString("x2"));
                }
                return result.ToString();
            }
        }


        public void Base64ToImage(string base64image, string path)
        {
            if (!Directory.Exists(ParaUtil.ResourceImagePath))
            {
                Directory.CreateDirectory(ParaUtil.ResourceImagePath);
            }
            byte[] bytes = Convert.FromBase64String(base64image);
            MemoryStream memoryStream = new MemoryStream(bytes);
            Bitmap bitmap = new Bitmap(memoryStream);

            bitmap.Save(path, ImageFormat.Jpeg);
        }
        public void Base64ToGraduatePhoto(string base64image, string path)
        {
            if (!Directory.Exists(ParaUtil.GraduatePhotoPath))
            {
                Directory.CreateDirectory(ParaUtil.GraduatePhotoPath);
            }
            byte[] bytes = Convert.FromBase64String(base64image);
            MemoryStream memoryStream = new MemoryStream(bytes);
            Bitmap bitmap = new Bitmap(memoryStream);

            bitmap.Save(path, ImageFormat.Jpeg);
        }
        public string UpLoad(HttpPostedFile file, string filename = "", bool type = true)
        {
            string result = "";
            if (string.IsNullOrEmpty(filename))
            {
                filename = file.FileName;
            }
            string uploadPath = ParaUtil.ResourcePath;
            if (!type)
            {
                uploadPath = uploadPath + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower()) + "\\";
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string exten = "";
            if (type)
            {
                exten = System.IO.Path.GetExtension(filename).ToLower();
            }
            else
            {
                exten = System.IO.Path.GetExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            }

            if (type)
            {
                result = string.Format("Resource_{0}{1}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff"), exten);
            }
            else
            {
                result = filename;
            }
            file.SaveAs(uploadPath + result);

            return result;
        }

        public string UpLoad(HttpPostedFile file, string validatedexten, string filename = "", bool type = true)
        {
            string result = "";
            if (string.IsNullOrEmpty(filename))
            {
                filename = file.FileName;
            }
            string uploadPath = ParaUtil.ResourcePath;
            if (!type)
            {
                uploadPath = uploadPath + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower()) + "\\";
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }


            bool isexten = false;
            string exten = "";
            if (type)
            {
                exten = System.IO.Path.GetExtension(filename).ToLower();
            }
            else
            {
                exten = System.IO.Path.GetExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            }
            //验证后缀

            string[] formatdata = validatedexten.Replace("*", "").Split(',');
            if (formatdata.Length < 0)
            {
                isexten = true;
            }
            else
            {
                for (int i = 0; i < formatdata.Length; i++)
                {
                    if (formatdata[i] == exten)
                    {
                        isexten = true;
                    }
                }
            }

            if (isexten)
            {
                if (type)
                {
                    result = string.Format("Resource_{0}{1}", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff"), exten);
                }
                else
                {
                    result = filename;
                }
                file.SaveAs(uploadPath + result);
            }
            return result;
        }

        public string PartToFile(string filename, out int total)
        {
            string uploadPath = ParaUtil.ResourcePath;
            Random rand = new Random(100);
            string ranPart = rand.Next(100, 1000).ToString();
            string originalFileName = filename.Substring(0, filename.LastIndexOf("."));
            string fullfilename = originalFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ranPart + System.IO.Path.GetExtension(filename);
            FileStream FileOut = new FileStream(ParaUtil.ResourcePath + fullfilename, FileMode.CreateNew, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(FileOut);
            //获取临时存储目录下的所有切割文件
            string[] allFile = Directory.GetFiles(ParaUtil.ResourcePath + System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            //将文件进行排序拼接
            allFile = allFile.OrderBy(s => int.Parse(Regex.Match(s, @"\d+$").Value)).ToArray();
            total = 0;
            for (int i = 0; i < allFile.Length; i++)
            {
                FileStream FileIn = new FileStream(allFile[i], FileMode.Open);
                BinaryReader br = new BinaryReader(FileIn);
                byte[] data = new byte[1048576];   //流读取,缓存空间
                int readLen = 0;                //每次实际读取的字节大小
                readLen = br.Read(data, 0, data.Length);
                bw.Write(data, 0, readLen);
                //关闭输入流
                FileIn.Close();
                total = total + readLen;
            };
            //关闭二进制写入
            bw.Close();
            FileOut.Close();

            Directory.Delete(ParaUtil.ResourcePath + System.IO.Path.GetFileNameWithoutExtension(filename).ToLower(), true);

            return fullfilename;
        }

        /// <summary>
        ///  枚举转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<EnumEntity> EnumToList<T>()
        {
            List<EnumEntity> enumEntityList = new List<EnumEntity>();
            foreach (var e in System.Enum.GetValues(typeof(T)))
            {               
                EnumEntity enumEntity = new EnumEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    enumEntity.Description = da.Description;
                }
                enumEntity.EnumValue = Convert.ToInt32(e);
                enumEntity.EnumName = e.ToString();
                enumEntityList.Add(enumEntity);
            }
            return enumEntityList;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static bool UpLoadFunciotn(string path, string filename)
        {
            try
            {
                string filekind = Path.GetExtension(path);
                string name = filename + filekind;
                //提供下载的文件并且编码
                string fileName = HttpContext.Current.Server.UrlEncode(name);
                fileName = fileName.Replace("+", "%20");
                string filePath = HttpContext.Current.Server.MapPath(path);
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[(int)fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    //string myStr = System.Text.Encoding.UTF8.GetString(bytes);
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    //HttpContext.Current.Response.ContentType = "text/plain";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    //HttpContext.Current.Response.AddHeader("Content-Length", fs.Length.ToString()); 
                    HttpContext.Current.Response.BinaryWrite(bytes);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 获取枚举的名字和描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDescription<T>()
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            FieldInfo[] fields = typeof(T).GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    string description = attr.Length == 0 ? field.Name : ((DescriptionAttribute)attr[0]).Description;

                    dic.Add(field.Name, description);
                }
            }

            return dic;

        }



        #region RSA加密
        public string RSAEncrypt(string content)
        {
            string publickey = ParaUtil.PublicKey;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            rsa.FromXmlString(publickey);
            PlainTextBArray = (new UnicodeEncoding()).GetBytes(content);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            string result = Convert.ToBase64String(CypherTextBArray);
            return result;
        }

        //解密
        public string RSADecrypt(string content)
        {
            string privatekey = ParaUtil.PrivateKey;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
            string result = Encoding.UTF8.GetString(cipherbytes);
            return result;
        }
        #endregion

        public string UpLoad(string uploadPath, string parentUrl, HttpPostedFile file, string validatedexten, string filename = "", bool type = true)
        {
            string result = "";
            if (string.IsNullOrEmpty(filename))
            {
                filename = file.FileName;
            }
            //string uploadPath = ParaUtils.CompanyFolderPath;

            if (string.IsNullOrEmpty(parentUrl))
            {
                uploadPath = uploadPath + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower()) + "\\";
            }
            else
            {
                uploadPath = uploadPath + parentUrl + "\\" + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower()) + "\\";
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }


            bool isexten = false;
            string exten = "";
            if (type)
            {
                exten = System.IO.Path.GetExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            }
            else
            {
                exten = System.IO.Path.GetExtension(filename).ToLower();
            }
            //验证后缀

            string[] formatdata = validatedexten.Replace("*", "").Split(',');
            if (formatdata.Length < 0)
            {
                isexten = true;
            }
            else
            {
                for (int i = 0; i < formatdata.Length; i++)
                {
                    if (formatdata[i] == exten)
                    {
                        isexten = true;
                        break;
                    }
                }
            }

            if (isexten)
            {


                result = filename;

                file.SaveAs(uploadPath + result);
            }
            return result;
        }

        public string PartToFile(string uploadPath, string parentUrl, string filename, out int total)
        {
            //string uploadPath = ParaUtils.CompanyFolderPath;
            Random rand = new Random(100);
            string ranPart = rand.Next(100, 1000).ToString();
            string originalFileName = filename.Substring(0, filename.LastIndexOf("."));
            string fullfilename = originalFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ranPart + System.IO.Path.GetExtension(filename);
            FileStream FileOut = new FileStream(uploadPath + parentUrl + "\\" + fullfilename, FileMode.CreateNew, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(FileOut);
            //获取临时存储目录下的所有切割文件
            string[] allFile = Directory.GetFiles(uploadPath + parentUrl + "\\" + System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            allFile = allFile.OrderBy(s => int.Parse(Regex.Match(s, @"\d+$").Value)).ToArray();
            total = 0;
            for (int i = 0; i < allFile.Length; i++)
            {
                FileStream FileIn = new FileStream(allFile[i], FileMode.Open);
                BinaryReader br = new BinaryReader(FileIn);
                byte[] data = new byte[1048576];   //流读取,缓存空间
                int readLen = 0;                //每次实际读取的字节大小
                readLen = br.Read(data, 0, data.Length);
                bw.Write(data, 0, readLen);
                //关闭输入流
                FileIn.Close();
                total = total + readLen;
            };
            //关闭二进制写入
            bw.Close();
            FileOut.Close();

            Directory.Delete(uploadPath + parentUrl + "\\" + System.IO.Path.GetFileNameWithoutExtension(filename).ToLower(), true);
            return fullfilename;
        }

        public JObject PostData(string url, string postData)
        {
            byte[] data = Encoding.UTF8.GetBytes(postData);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.Method = "POST";
            //myRequest.ContentType = "text/json";
            myRequest.ContentType = "text/json;charset=utf-8";
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            return (JObject)JsonConvert.DeserializeObject(content);
        }

        public JObject PostData(string url, JObject data)
        {
            try
            {
                string returnData = data.ToString().Trim();
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.ContentType = "application/json; charset=utf-8";

                Stream reqstream = request.GetRequestStream();
                reqstream.Write(bytes, 0, bytes.Length);
                //声明一个HttpWebRequest请求  
                request.Timeout = 90000;
                //设置连接超时时间
                request.Headers.Set("Pragma", "no-cache");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.UTF8;


                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                string strResult = streamReader.ReadToEnd();
                streamReceive.Dispose();
                streamReader.Dispose();

                JObject ReturnObject = (JObject)JsonConvert.DeserializeObject(strResult);

                return ReturnObject;
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public string newUpLoad(HttpPostedFile file, string filename = "", bool type = true)
        {
            string result = "";
            if (string.IsNullOrEmpty(filename))
            {
                filename = file.FileName;
            }
            string uploadPath = ParaUtil.ResourcePath;
            if (!type)
            {
                uploadPath = uploadPath + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower()) + "\\";
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string exten = "";
            if (type)
            {
                exten = System.IO.Path.GetExtension(filename).ToLower();
            }
            else
            {
                exten = System.IO.Path.GetExtension(System.IO.Path.GetFileNameWithoutExtension(filename).ToLower());
            }

            if (type)
            {
                Random rand = new Random(100);
                string ranPart = rand.Next(100, 1000).ToString();
                string originalFileName = filename.Substring(0, filename.LastIndexOf("."));
                result = originalFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ranPart + System.IO.Path.GetExtension(filename);
            }
            else
            {
                result = filename;
            }
            file.SaveAs(uploadPath + result);

            return result;
        }

        public JObject GetData(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            Stream streamReceive = myResponse.GetResponseStream();
            Encoding encoding = Encoding.UTF8;
            //Stream stm = new System.IO.Compression.GZipStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
            StreamReader reader = new StreamReader(streamReceive, encoding);
            string content = reader.ReadToEnd();
            reader.Close();
            return (JObject)JsonConvert.DeserializeObject(content);
        }


        public static string ToDescription(object enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] memInfo = type.GetMember(enumeration.ToString());
            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (null != attrs && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return enumeration.ToString();
        }

        public static void ExportByWeb(string strFileName, HSSFWorkbook workbook)
        {
            try
            {
                HttpContext curContext = HttpContext.Current;
                // 设置编码和附件格式
                curContext.Response.ContentType = "application/vnd.ms-excel";
                curContext.Response.ContentEncoding = Encoding.UTF8;
                curContext.Response.Charset = "";
                curContext.Response.AppendHeader("Content-Disposition",
                    "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));
                //调用导出具体方法Export()
                curContext.Response.BinaryWrite(Export(workbook).GetBuffer());
                curContext.Response.End();
            }
            catch (Exception ex)
            {
                
            }
        }

        public static MemoryStream Export(HSSFWorkbook workbook)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                workbook.Write(ms);                
                ms.Flush();                
                ms.Dispose();
                return ms;
            }
        }

    }
}
