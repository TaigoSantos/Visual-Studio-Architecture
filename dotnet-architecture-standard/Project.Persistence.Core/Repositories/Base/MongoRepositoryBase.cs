﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Project.Models.Core.Entities.Base;
using Project.Persistence.Core.Contexts.Base;
using Project.Persistence.Core.Interfaces.Base;

namespace Project.Persistence.Core.Repositories.Base
{
    public abstract class MongoRepositoryBase<TEntity> : RepositoryBase<TEntity>, IRepositoryBase<TEntity>, IRepositoryBaseAsync<TEntity>
        where TEntity : EntityBase
    {
        #region - PROPERTIES -

        public MongoContextBase Context;

        #endregion

        #region - CONSTRUCTORS -

        public MongoRepositoryBase(MongoContextBase context)
        {
            Context = context;
        }

        #endregion

        #region - MAIN METHODS -

        #region - READ METHODS -

        #region - SYNC -

        public virtual IQueryable<TEntity> Query()
        {
            return Context.GetCollection<TEntity>().AsQueryable();
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).AsQueryable();
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.GetCollection<TEntity>().Find(predicate).SingleOrDefault();
        }

        #endregion

        #region - ASYNC -

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.GetCollection<TEntity>().Find(predicate).SingleOrDefaultAsync();
        }

        #endregion

        #endregion

        #region - WRITE METHODS -

        #region - SYNC -

        public new virtual void Create(TEntity obj)
        {
            base.Create(obj);
            Context.GetCollection<TEntity>().InsertOne(obj);
        }

        public virtual void Update(Expression<Func<TEntity, bool>> predicate, TEntity obj)
        {
            base.Update(obj);
            Context.GetCollection<TEntity>().ReplaceOne(predicate, obj);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Context.GetCollection<TEntity>().DeleteOne(predicate);
        }

        #endregion

        #region - ASYNC -

        public virtual async Task CreateAsync(TEntity obj)
        {
            base.Create(obj);
            await Context.GetCollection<TEntity>().InsertOneAsync(obj);
        }

        public virtual async Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity obj)
        {
            base.Update(obj);
            await Context.GetCollection<TEntity>().ReplaceOneAsync(predicate, obj);
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            await Context.GetCollection<TEntity>().DeleteManyAsync(predicate);
        }

        #endregion

        #endregion

        #endregion
    }
}