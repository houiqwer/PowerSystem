using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PowerSystemLibrary.Util
{
    public class ParaUtil
    {
        public static readonly string ResourceHtmlpath = "/Resource/";
        public static readonly string TempleteFileHtmlpath = "/TempleteFile/";
        public static readonly string ResourceHtmlImagepath = "/Templete/";
        public static readonly string ResourceImagePath = HttpContext.Current.Server.MapPath("..\\Templete\\");

        public static readonly string ResourcePath = HttpContext.Current.Server.MapPath("~/Resource/");
        public static readonly string HeadImagePath = HttpContext.Current.Server.MapPath("~/Resource/HeadImage/");

        public static readonly string GraduatePhotoPath = HttpContext.Current.Server.MapPath("~/Resource/GraduatePhotoPath/");
        public static readonly string GraduatePhotoHtmlPath = HttpContext.Current.Server.MapPath("../Resource/GraduatePhotoPath/");

        public static readonly string ClassPhotoPath = HttpContext.Current.Server.MapPath("~/Resource/ClassPhotoPath/");
        public static readonly string ClassPhotoHtmlPath = HttpContext.Current.Server.MapPath("../Resource/ClassPhotoPath/");
        public static readonly string HeadImageHtmlPath = "../Templete/";
        public static readonly string ResourceRarPath = HttpContext.Current.Server.MapPath("~/Resource/") + "RarDownLoad/";
        public static readonly string TempleteFile = HttpContext.Current.Server.MapPath("../TempleteFile/");
        public static readonly string TempletePath = HttpContext.Current.Server.MapPath("~/TempleteFile/");

        public static readonly string FolderPath = HttpContext.Current.Server.MapPath("~/Resource/Folder/");

        public static readonly string JsonPath = HttpContext.Current.Server.MapPath("~/JsonCode/PositionRole2.json");

        public static readonly string FolderFormat = ".jpg,.gif,.png,.jpeg,.psd,.bmp,.xls,.xlsx,.doc,.docx,.wps,.ppt,.pps,.ppsx,.txt,.zip,.rar,.pdf,.mp4,.flv,.swf,.pptx,.rtf";
        public static readonly string ImportFormat = ".xls,.xlsx";
        public static readonly string FileFormats = ".jpg,.png,.jpeg,.zip,.rar";

        //设定的最大接收任务人数
        public static readonly int MaxReciveCount = 2;

        //微信
        public static readonly string CorpID = "ww23b2241519609204";
        public static readonly string MessageSecret = "cD9xuGtEevSQF9gLQ4pNqsnp1qJ0T838OAz7V_g1E70";
        public static readonly string MessageAgentid = "1000002";

        public static readonly string PowerSecret = "9nBj2LqkUWVr2dm1sJBXlTOJntlH7Zs4ozXUi4wCDqE";


        public static readonly string UserSecret = "gUewVZ2EC6dKo-6NQV759tMM9rKlT3TILQYYYm5FgV0";

        public static readonly string PrivateKey = "<RSAKeyValue><Modulus>s6vCvkh7j/94qNFyVcZvpMlRyqGLS/eK4+WybtQxmeqv6Trs9xNbtT9dWWhRURqyFWVaRFWoCOqtjqmsNNmBjpMPjwAeJ60MCmAy4iDdrwxbIDq5Z82RtRHtInTEtxYEpmPKYuSKKNhGW4x7dWwMVprO9CQ4Fm7ZSfHEQJ4D79k=</Modulus><Exponent>AQAB</Exponent><P>4IUrJ8WcIgGq+hz+RRV3O2zFIeZK2YnAfn3Z+GhgLwcEH6pOd+4h4xcKVRBC5QCvY5d3uFRw9bAnsI/U4+kODw==</P><Q>zNzK22eZSwxiiFxn81wkhjN1LaeJAKfiBxBBqXQQUG+qOgRvw621hKVb5YHu02OwEfdDM47T6aHaY36gRu4Llw==</Q><DP>DM8sLiG1Da+grJJY42IdIlPLT2rPHGgihlTxyZ1S13wD/TZ/MZJxdn5LBZ1e46fNWdY7a1XN+AZrzUP3dmVGew==</DP><DQ>et62QI/LTkrYUibyJfEO6vqz/jTso9sNYuUqLMMzAbtnorKEgh2OsEcpbebXYiv5L11ZDfRNP2RXArOw17nwcw==</DQ><InverseQ>1beGfu/1qbaSS2lljt9hDJwY4VPzuCJHLHDJCYQIRLhWXB4wguSHJQ9iOLjUTDFY2iaKQGCS2I8qMpreetfRiQ==</InverseQ><D>U/CY6QFACQRXwLcpbX1px1jux3Y1d+ZMkzBK7pBgJAKK8LHA56W5oLHb0Nt1siiArpJNW8OQ36mUMCSA+afsaIEdW25Au0y+QpbsA0GQzGqLasM19scrST8fXjbGocPQy+MdCw6JWK71JRGBo0mZ3VMf8BurNMSTdTXVqxlCB0k=</D></RSAKeyValue>";
        public static readonly string PublicKey = "<RSAKeyValue><Modulus>s6vCvkh7j/94qNFyVcZvpMlRyqGLS/eK4+WybtQxmeqv6Trs9xNbtT9dWWhRURqyFWVaRFWoCOqtjqmsNNmBjpMPjwAeJ60MCmAy4iDdrwxbIDq5Z82RtRHtInTEtxYEpmPKYuSKKNhGW4x7dWwMVprO9CQ4Fm7ZSfHEQJ4D79k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";


    }
}
