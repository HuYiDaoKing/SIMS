using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SIMS.Web.EntityDataModel;

namespace SIMS.Web.Services
{
    public class BaseRepository<T> where T : class
    {
        //实例化EF框架
        SIMSDbContext db = new SIMSDbContext();

        //添加
        public T AddEntities(T entity)
        {
            /*db.Entry<T>(entity).State = EntityState.Added;
            db.SaveChanges();
            return entity;*/
            db.Entry<T>(entity).State = EntityState.Added;
            //db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;
            return entity;
        }

        //修改
        public bool UpdateEntities(T entity)
        {
            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }

        //删除
        public bool DeleteEntities(T entity)
        {
            db.Set<T>().Attach(entity);
            db.Entry<T>(entity).State = EntityState.Deleted;
            return db.SaveChanges() > 0;
        }

        //查询
        public IQueryable<T> LoadEntities(Func<T, bool> wherelambda)
        {
            return db.Set<T>().Where<T>(wherelambda).AsQueryable();
        }

        //分页
        public IQueryable<T> LoadPagerEntities<S>(int pageSize, int pageIndex, out int total,
            Func<T, bool> whereLambda, bool isAsc, Func<T, S> orderByLambda)
        {
            var tempData = db.Set<T>().Where<T>(whereLambda);

            total = tempData.Count();

            //排序获取当前页的数据
            if (isAsc)
            {
                tempData = tempData.OrderBy<T, S>(orderByLambda).
                      Skip<T>(pageSize * (pageIndex - 1)).
                      Take<T>(pageSize).AsQueryable();
            }
            else
            {
                tempData = tempData.OrderByDescending<T, S>(orderByLambda).
                     Skip<T>(pageSize * (pageIndex - 1)).
                     Take<T>(pageSize).AsQueryable();
            }

            return tempData.AsQueryable();
        }
    }
}