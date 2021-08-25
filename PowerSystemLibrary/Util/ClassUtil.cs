using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class ClassUtil
    {
        public static bool Validate<T>(T t, ref string message)
        {
            Type type = t.GetType();
            message = string.Empty;

            foreach (PropertyInfo prop in type.GetProperties())//pr
            {
                if (prop.IsDefined(typeof(ValidationAttribute), true))
                {
                    object oValue = prop.GetValue(t, null);

                    foreach (ValidationAttribute iTemp in prop.GetCustomAttributes(typeof(ValidationAttribute), true))//获取字段所有的特性
                    {
                        if (!iTemp.IsValid(oValue))
                        {
                            message = iTemp.ErrorMessage;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetEntityName<T>(T t)
        {
            Type type = t.GetType();
            string result = string.Empty;
            Attribute attribute = type.GetCustomAttribute(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attribute != null)
            {
                result = ((System.ComponentModel.DescriptionAttribute)attribute).Description;
            }
            return result;
        }

        internal static string GetEntityName(object data)
        {
            throw new NotImplementedException();
        }

        public void EditEntity(object oldObject, object newObject)
        {
            Type T = oldObject.GetType();
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                if (P.GetCustomAttribute(typeof(ExchangeType), false) != null)
                {
                    P.SetValue(oldObject, P.GetValue(newObject) ?? P.GetValue(oldObject));
                }
            }
        }

    }

    public class ExchangeType : Attribute
    {

    }
}
