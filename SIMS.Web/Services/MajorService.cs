using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Services;
using SIMS.Web.Models;
using SIMS.Web.Areas.Admins.ViewModels;

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

        public bool IsExist(int id, int departmentId, string code)
        {
            bool isExist = false;
            using (var context = new SIMSDbContext())
            {
                if (id == 0)
                {
                    //int count = context.Major.Where(c=>c.DepartmentId ==departmentId && c.Code == code).Count();
                    //if (count > 0)
                    //{
                       
                    //}
                    var query = context.Major.FirstOrDefault(c => c.DepartmentId == departmentId && c.Code == code);
                    //var query = from c in context.Major.Where(c=>c.DepartmentId.Equals(departmentId)) select c as Major;
                    isExist = null==query ? false :true;
                }
                else
                {
                    var query = context.Major.FirstOrDefault(c => c.DepartmentId == departmentId && c.Code == code && c.Id != id);
                    isExist = null == query ? false : true;
                }
                return isExist;
            }
        }

        public Major GetById(int? id)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Major.Find(id);
                return query;
            }
        }

        public Major GetByName(string name)
        {
            using (var context = new SIMSDbContext())
            {
                var query = context.Major.FirstOrDefault(c=>c.Name==name);
                return query;
            }
        }

        public List<MajorViewModel> GetBySomeWhere(int departmentId, int iStart, int iLimit)
        {
            List<MajorViewModel> list = new List<MajorViewModel>();
            var filtered = GetSearchResult(departmentId);

            var query = from c in filtered.Skip(iStart).Take(iLimit) select c;

            List<Major> majors = query.ToList<Major>();
            foreach (Major major in majors)
            {
                string departmentname = string.Empty;
                Department department = DepartmentService.Instance.GetById(major.DepartmentId);
                if (department != null)
                    departmentname = department.Name;

                list.Add(new MajorViewModel
                {
                    Id = major.Id,
                    DepartmentId = major.DepartmentId,
                    Code = major.Code,
                    Name = major.Name,
                    Description = major.Description,
                    DepartmentName = departmentname,
                    CreateTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                });
            }
            return list;
        }
        public List<MajorViewModel> GetMajorsByDepartmentId(int departmentId)
        {
            List<MajorViewModel> list = new List<MajorViewModel>();

            using (var context = new SIMSDbContext())
            {
                List<Major> majors = context.Major.Where(c=>c.DepartmentId==departmentId).ToList();

                foreach(var major in majors)
                {
                    string departmentname = string.Empty;
                    Department department = DepartmentService.Instance.GetById(major.DepartmentId);
                    if (department != null)
                        departmentname = department.Name;

                    if (major != null)
                    {
                        list.Add(new MajorViewModel
                        {
                            Id = major.Id,
                            DepartmentId = departmentId,
                            Code = major.Code,
                            Name = major.Name,
                            Description = major.Description,
                            DepartmentName = departmentname,
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now
                        });
                    }
                }
            }
            return list;
        }
        public int GetCount(int departmentId)
        {
            using (var context = new SIMSDbContext())
            {
                if (departmentId == 0)
                    return context.Major.Count();
                else
                    return context.Major
                        .Where(m => m.DepartmentId == departmentId)
                        .Count();
            }
        }

        public string GetMajorCode(int departmentId)
        {
            //MajorCode=DepartmentCode+MaxId
            using (var context = new SIMSDbContext())
            {
                string code = string.Empty;
                Department department = DepartmentService.Instance.GetById(departmentId);
                string _code = string.Empty;
                int maxMajorId = GetMaxId();
                if (maxMajorId < 10)
                {
                    _code = string.Format("0{0}", maxMajorId+1);
                }
                else
                {
                    _code = maxMajorId.ToString();
                }
                code = String.Format("{0}{1}",department.Code,_code);
                return code;
            }
        }

        /// <summary>
        /// 自增ID
        /// </summary>
        /// <returns></returns>
        public int GetMaxId()
        {
            using (var context = new SIMSDbContext())
            {
                int count = context.Major.Count();
                if (count > 0)
                    return (from c in context.Major select c.Id).Max();
                return 0;
            }
        }

        public List<Major> GetAll()
        {
            using (var context = new SIMSDbContext())
            {
                return context.Major.ToList();
            }
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

        private List<Major> GetSearchResult(int departmentId)
        {
            using (var context = new SIMSDbContext())
            {
                var query = from c in context.Major.ToList()
                            select c;

                if (departmentId != 0)
                {
                    return query
               .Where(m => m.DepartmentId == departmentId)
               .OrderByDescending(m => m.CreateTime)
               .ToList<Major>();
                }

                return query
               .OrderByDescending(m => m.CreateTime)
               .ToList<Major>();
            }
        }



        #endregion
    }
}