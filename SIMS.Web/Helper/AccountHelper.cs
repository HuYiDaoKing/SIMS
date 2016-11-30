using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS.Web.Helper
{
    public static class AccountHelper
    {

    }

    public static class AccountHashKeys
    {
        // Login view model key.
        public static readonly string CaptchaErrorMessage = "CAPTCHA_ERROR";

        public static readonly string EmailOrPasswordErrorMessage = "EMAIL_OR_PASSWORD_ERROR";
        public static readonly string LoginModelStateErrorMessage = "MODEL_STATE_ERROR";

        // Cookie key.
        public static readonly string UserBrowserCookie = "USER_BROWSER_COOKIE";

        public static readonly string AdminUserBrowserCookie = "ADMIN_USER_BROWSER_COOKIE";

        public static readonly string RegisterSuccess = "RRGISTER_SUCCESS";
        public static readonly string RegisterFailure = "REGISTER_FAILURE!";
        public static readonly string PasswordConflict = "PASSWORD_CONFLICT!";
        public static readonly string DuplicateRegister = "DUPLICATE_REGISTER!";

        // Current user key.
        public static readonly string CurrentAccountUser = "CURRENT_ACCOUNT_USER";

        public static readonly string CurrentMerchantUser = "CURRENT_MERCHANT_USER";
        public static readonly string CurrentAdminUser = "CURRENT_ADMIN_USER";
    }
}