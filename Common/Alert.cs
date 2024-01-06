using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Alert
    {
        public enum Entity
        {
            [Description("کاربر")]
            User,
        }
        public static string GetAddAlert(Entity entity)
        {
            return $"{entity.GetDescription()} با موفقیت ثبت گردید";
        }
        public static string GetRemoveAlert(Entity entity)
        {
            return $"{entity.GetDescription()} با موفقیت حذف گردید";
        }
        public static string GetEditAlert(Entity entity)
        {
            return $"{entity.GetDescription()} با موفقیت بروزرسانی گردید";
        }
        public enum Public
        {
            [Description("خطایی سمت سرور رخ داده است")]
            ServerException,
            [Description("موفق")]
            Success,
            [Description("ناموفق")]
            UnSuccess,
            [Description("موردی یافت نشد")]
            NotFound,
            [Description("خطای احراز هویت")]
            AuthError,
            [Description("بازیابی انجام شد")]
            Recovery,
        }
        public enum Field
        {
            [Description("نام")]
            Name,
            [Description("نام خانوادگی")]
            LastName,
            [Description("ایمیل")]
            Email,
            [Description("گذرواژه")]
            Password,
            [Description("موبایل")]
            Mobile,
            [Description("کدملی")]
            NationalCode,
            [Description("کدپستی")]
            PostalCode,
        }

        public static string GetValidateAlert(Field field)
        {
            return $"{field.GetDescription()} را بدرستی وارد کنید";

        }
    }
}
