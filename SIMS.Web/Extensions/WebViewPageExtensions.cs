//using ZebraTicket.Models;
//using ZebraTicket.Models.DO;
//using ZebraTicket.Helpers;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Helper;

namespace System.Web.Mvc
{
    public static class WebViewPageExtensions
    {
        public static AdminUser CurrentAdminUser(this WebViewPage wvp)
        {
            return wvp.Session[AccountHashKeys.CurrentAdminUser] as AdminUser;
        }

        //public static Merchant CurrentMerchant(this WebViewPage wvp)
        //{
        //    return wvp.Session[AccountHashKeys.CurrentMerchantUser] as Merchant;
        //}

        //public static Account CurrentAccountUser(this WebViewPage wvp)
        //{
        //    return wvp.Session[AccountHashKeys.CurrentAccountUser] as Account;
        //}
    }
}