using SIMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS.Web.Services
{
    public class ProfessionManager
    {
        private static Dictionary<int, string> _dicProfession;

        public static Dictionary<int, string> DicProfession
        {
            get
            {
                Init();
                return _dicProfession;
            }
            set { _dicProfession = value; }
        }

        public static void Init()
        {
            try
            {
                _dicProfession = new Dictionary<int, string>();

                _dicProfession.Add(0, "管理员");
                _dicProfession.Add(1, "学生");
                _dicProfession.Add(2, "教师");
            }
            catch (Exception ex)
            {
                string message = string.Format("初始化职业状态失败:{0}", ex.Message);
                LogHelper.Log(LogType.Error, message);
            }
        }
    }
}