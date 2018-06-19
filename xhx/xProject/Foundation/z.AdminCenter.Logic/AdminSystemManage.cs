using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation;
using z.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 系统管理
    /// </summary>
    public class AdminSystemManage : AdminSystemManageBase
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public admin_system Get(object param)
        {
            return Repository.Get<admin_system>(param);
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(admin_system param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<admin_system>(e => (e.SysKey == param.SysKey || e.Code == param.Code || e.Name == param.Name) && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的Key值或Code值或Name值";
                return result;
            }

            object obj = Repository.Save<admin_system>(param);
            if (obj != null)
            {
                result.Succeeded = true;

                //更新缓存
                SetAllAdminSystems();

                //主动更新用户会话
                new AdminAccountManage().UpdateUserSession();
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
        public BoolResult Update(admin_system param)
        {
            BoolResult result = new BoolResult();

            admin_system _admin_system = Repository.First<admin_system>(e => e.AdminSystemId == param.AdminSystemId && e.Deleted == false);
            if (_admin_system == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            if (_admin_system.SysKey != param.SysKey)
            {
                if (Repository.Exists<admin_system>(e => e.SysKey == param.SysKey && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Key";
                    return result;
                }
            }
            if (_admin_system.Code != param.Code)
            {
                if (Repository.Exists<admin_system>(e => e.Code == param.Code && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Code";
                    return result;
                }
            }
            else if (_admin_system.Name != param.Name)
            {
                if (Repository.Exists<admin_system>(e => e.Name == param.Name && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name";
                    return result;
                }
            }

            _admin_system.Code = param.Code;
            _admin_system.SysKey = param.SysKey;
            _admin_system.Name = param.Name;
            _admin_system.Description = param.Description;
            _admin_system.URL = param.URL;
            _admin_system.CallBackUrl = param.CallBackUrl;
            _admin_system.Logo = param.Logo;
            _admin_system.UpdateBy = param.UpdateBy;
            _admin_system.UpdateOn = param.UpdateOn;
            Repository.Update<admin_system>(_admin_system);

            //更新缓存
            SetAllAdminSystems();

            //主动更新用户会话
            new AdminAccountManage().UpdateUserSession();

            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(AdminSystemExt param)
        {
            BoolResult result = new BoolResult();

            IList<admin_system> adminSystemList = Repository.Find<admin_system>(e => param.AdminSystemIds.Contains(e.AdminSystemId) && e.Deleted == false);
            if (adminSystemList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<admin_system>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_system in adminSystemList)
                        {
                            //删除系统时需要删除系统下包含的所有权限，同时删除权限与用户组、用户的关系

                            _admin_system.Deleted = true;
                            _admin_system.UpdateBy = param.UpdateBy;
                            _admin_system.UpdateOn = param.UpdateOn;
                            session.Update(_admin_system);

                            IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => e.SystemId == _admin_system.AdminSystemId && e.Deleted == false);
                            foreach (var _admin_permission in adminPermissionList)
                            {
                                _admin_permission.Deleted = true;
                                _admin_permission.UpdateBy = param.UpdateBy;
                                _admin_permission.UpdateOn = param.UpdateOn;
                                session.Update(_admin_permission);

                                IList<admin_group_permission> adminGroupPermissionList = Repository.Find<admin_group_permission>(e => e.PermissionId == _admin_permission.AdminPermissionId);
                                foreach (var _admin_group_permission in adminGroupPermissionList)
                                {
                                    session.Delete(_admin_group_permission);
                                }

                                IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(e => e.PermissionId == _admin_permission.AdminPermissionId);
                                foreach (var _admin_user_permission in adminUserPermissionList)
                                {
                                    session.Delete(_admin_user_permission);
                                }
                            }

                            intRecord++;
                        }
                        
                        transaction.Commit();

                        //更新缓存
                        SetAllAdminSystems();
                        //主动更新用户会话
                        new AdminAccountManage().UpdateUserSession();

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
        public IPagedList<admin_system> GetPageList(IPagedParam<admin_system> param)
        {
            IQueryable<admin_system> queryable = Repository.AsQueryable<admin_system>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.Code.Contains(param.model.Name) || e.Name.Contains(param.model.Name));
            }
            queryable = queryable.OrderByDescending(e => e.CreateOn);

            return new PagedList<admin_system>(queryable, param.PageIndex, param.PageSize);
        }

        /// <summary>
        /// 获取全部列表（用于添加/更新权限页面，选择权限所属的系统）
        /// </summary>
        /// <returns></returns>
        public IList<admin_system> GetList()
        {
            IQueryable<admin_system> queryable = Repository.AsQueryable<admin_system>().Where(e => e.Deleted == false);
            queryable = queryable.OrderBy(e => e.CreateOn);
            return queryable.ToList();
        }
    }
}
