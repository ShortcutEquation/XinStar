using z.AdminCenter.Entity;
using z.Foundation.Data;
using z.Logic.Base;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    public class AuthGuestManage : NHLogicBase
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public auth_guest Get(object param)
        {
            return Repository.Get<auth_guest>(param);
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(auth_guest param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<auth_guest>(e => (e.AuthKey == param.AuthKey || e.Code == param.Code || e.Name == param.Name) && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的Key值或Code值或Name值";
                return result;
            }

            object obj = Repository.Save<auth_guest>(param);
            if (obj != null)
            {
                result.Succeeded = true;
            }
            else
            {
                result.Succeeded = false;
                result.Message = "保存对象失败";
            }

            return result;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(auth_guest param)
        {
            BoolResult result = new BoolResult();

            auth_guest _auth_guest = Repository.First<auth_guest>(e => e.AuthGuestId == param.AuthGuestId && e.Deleted == false);
            if (_auth_guest == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            if (_auth_guest.AuthKey != param.AuthKey)
            {
                if (Repository.Exists<auth_guest>(e => e.AuthKey == param.AuthKey && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Key";
                    return result;
                }
            }
            if (_auth_guest.Code != param.Code)
            {
                if (Repository.Exists<auth_guest>(e => e.Code == param.Code && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Code";
                    return result;
                }
            }
            else if (_auth_guest.Name != param.Name)
            {
                if (Repository.Exists<auth_guest>(e => e.Name == param.Name && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name";
                    return result;
                }
            }

            _auth_guest.Code = param.Code;
            _auth_guest.AuthKey = param.AuthKey;
            _auth_guest.Name = param.Name;
            _auth_guest.Description = param.Description;
            _auth_guest.UpdateBy = param.UpdateBy;
            _auth_guest.UpdateOn = param.UpdateOn;
            Repository.Update<auth_guest>(_auth_guest);
            
            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// 单个、批量禁用/启用对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Disabled(AuthGuestExt param)
        {
            BoolResult result = new BoolResult();

            try
            {
                IList<auth_guest> authGuestList = Repository.Find<auth_guest>(e => param.AuthGuestIds.Contains(e.AuthGuestId) && e.Disabled != param.Disabled && e.Deleted == false);
                if (authGuestList.Count == 0)
                {
                    result.Succeeded = false;
                    result.Message = string.Format("{0}的对象不存在", param.Disabled ? "禁用" : "启用");
                    return result;
                }
                else
                {
                    using (ISession session = NHibernateHelper<auth_guest>.OpenSession())
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            int intRecord = 0;
                            foreach (var _auth_guest in authGuestList)
                            {
                                _auth_guest.Disabled = param.Disabled;
                                _auth_guest.UpdateBy = param.UpdateBy;
                                _auth_guest.UpdateOn = param.UpdateOn;
                                session.Update(_auth_guest);

                                intRecord++;
                            }

                            transaction.Commit();
                            result.Succeeded = true;
                            result.Result = intRecord;
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(AuthGuestExt param)
        {
            BoolResult result = new BoolResult();

            IList<auth_guest> authGuestList = Repository.Find<auth_guest>(e => param.AuthGuestIds.Contains(e.AuthGuestId) && e.Deleted == false);
            if (authGuestList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<auth_guest>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _auth_guest in authGuestList)
                        {
                            _auth_guest.Deleted = true;
                            _auth_guest.UpdateBy = param.UpdateBy;
                            _auth_guest.UpdateOn = param.UpdateOn;
                            session.Update(_auth_guest);
                            
                            intRecord++;
                        }

                        transaction.Commit();
                        result.Succeeded = true;
                        result.Result = intRecord;
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 分页获取列表（提供按Code或Name进行模糊搜索，搜索时给Name属性赋值，不提供客户端自定义排序，默认按创建时间倒序排列）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<auth_guest> GetPageList(IPagedParam<auth_guest> param)
        {
            IQueryable<auth_guest> queryable = Repository.AsQueryable<auth_guest>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.Code.Contains(param.model.Name) || e.Name.Contains(param.model.Name));
            }
            queryable = queryable.OrderByDescending(e => e.CreateOn);

            return new PagedList<auth_guest>(queryable, param.PageIndex, param.PageSize);
        }
    }
}
