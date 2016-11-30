using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Services;
using SIMS.Web.Models;

namespace SIMS.Web.Services
{
    public class MajorService
    {
        public static BaseRepository<Major> oBll = new BaseRepository<Major>();
        private static MajorService _instance = null;
        public static MajorService Instance
        {
            get
            {
                return _instance ?? new MajorService(); ;
            }
        }
        private MajorService()
        {

        }

        #region Public

        #region Query
        public Major GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Major.Find(id);
                return query;
            }
        }

        public List<Major> GetBySomeWhere(string strKeyWord, int iStart, int iLimit, out int iTotal)
        {
            var filtered = GetSearchResult(strKeyWord);
            iTotal = filtered.Count;

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            return query.ToList<Major>();
        }

        #endregion

        #region Create

        public bool Create(Major oItem)
        {
            var oRet = new Major();

            try
            {
                oRet = oBll.AddEntities(oItem);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var bRet = oRet == null ? false : true;
            return bRet;
        }

        #endregion

        #region Update

        public bool Update(Major oIem)
        {
            using (var context = new SIMSDbContext())
            {
                context.Major.Attach(oIem);
                context.Entry<Major>(oIem).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// 注意在调用的时候还要删除对应的物理文件
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool bRet = false;

            try
            {
                using (var db = new SIMSDbContext())
                {
                    Major oItem = GetById(id);
                    db.Major.Attach(oItem);
                    db.Entry<Major>(oItem).State = EntityState.Deleted;
                    bRet = db.SaveChanges() > 0;
                }
            }
            catch
            {
                bRet = false;
            }

            return bRet;
        }

        #endregion

        #endregion

        #region Private

        private List<Major> GetSearchResult(string strKeyWord)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Major.ToList()
                            select c;

                return query
               .Where(m => (string.IsNullOrEmpty(strKeyWord) || m.Name.Contains(strKeyWord)))
               .OrderByDescending(m => m.CreateTime)
                    //.ThenByDescending(m => m.Id)
               .ToList<Major>();
            }
        }

        #endregion
    }
}