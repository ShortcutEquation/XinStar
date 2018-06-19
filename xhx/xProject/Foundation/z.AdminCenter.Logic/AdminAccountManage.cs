using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation;
using z.Foundation.Cache;
using z.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 用户账户管理
    /// </summary>
    public class AdminAccountManage : NHLogicBase
    {
        /// <summary>
        /// 锁定dictUserTicket对象
        /// </summary>
        private static readonly object lockObj_1 = new object();
        private static Dictionary<int, string> dictUserTicket = new Dictionary<int, string>();

        /// <summary>
        /// 锁定bUpdateAllUserSession对象
        /// 修改管理员用户组、权限、系统时需要更新当前在线的用户的所有用户会话数据对象，操作时间可能比较长，故改为延时辅助线程做更新操作
        /// 当对管理员用户组、权限、系统做更新时，只是将标识bUpdateAllUserSession置为True
        /// </summary>
        private static readonly object lockObj_2 = new object();
        private static bool bUpdateAllUserSession = false;

        /// <summary>
        /// 锁定用户会话数据对象
        /// 用户会话数据对象修改场景
        /// 1. 同一用户在不同地方登录
        /// 2. 用户登出
        /// 3. 用户修改个人资料
        /// 4. 超级管理员修改用户资料、管理用户组、权限、系统
        /// 为防止用户会话对象被多用户在某一时刻修改造成数据不一致的情况，故以上三个场景下将锁定用户会话数据对象进行更新
        /// </summary>
        private static readonly object lockObj_3 = new object();

        #region 查询管理员真实姓名

        #endregion

        public AdminUserInfo GetAdminUserInfo(string userName)
        {
            var result = new AdminUserInfo
            {
                status = 1,
                IsExist = false,
                RealName = ""
            };
            if (string.IsNullOrEmpty(userName))
            {
                result.errorCode = "30001";
                result.errorMessage = "userName不存在";
                return result;
            }

            var adminUser = Repository.First<admin_user>(e => e.AdminName == userName && e.Disabled == false && e.Deleted == false);
            if (adminUser != null)
            {
                result.IsExist = true;
                result.RealName = adminUser.RealName;
            }
            
            return result;
        }

        #region 登入、登出

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult<AdminUserExt> Login(AdminUserExt param)
        {
            BoolResult<AdminUserExt> boolResult = new BoolResult<AdminUserExt>();

            admin_user _admin_user = Repository.First<admin_user>(e => e.AdminName == param.AdminName && e.Password == param.Password && e.Disabled == false && e.Deleted == false);
            if (_admin_user != null)
            {
                AdminUserExt adminUserExt = GetMenuAndPermission(_admin_user.AdminUserId);
                _admin_user.CopyTo(adminUserExt);
                //清除会话中的敏感数据
                adminUserExt.Password = "";
                adminUserExt.OldPassword = "";
                //返回唯一会话ID
                adminUserExt.Ticket = Guid.NewGuid().ToString().RegexReplace("-", "");

                #region 写入用户会话

                ICache cache = new HttpCache();

                string sessionId = adminUserExt.Ticket;
                string sessionDataId = "data_" + adminUserExt.Ticket;

                //简单会话
                UserSession session = new UserSession();
                if (param.RememberMe == "on")
                {
                    session.RememberMe = true;
                    session.ExpireTime = DateTime.Now.AddDays(3);

                    cache.SetCache(sessionId, session, TimeSpan.FromDays(3));
                }
                else
                {
                    session.RememberMe = false;
                    session.ExpireTime = DateTime.Now.AddMinutes(60);

                    cache.SetCache(sessionId, session, TimeSpan.FromMinutes(60));
                }

                //标准会话(与记住会话时最长过期时间一致，确保会话不会在最大有效期内过期)
                cache.SetCache(sessionDataId, adminUserExt, TimeSpan.FromDays(3));

                //存储管理员用户ID与用户会话数据的关系
                string oldSessionDataId = "";
                lock (lockObj_1)
                {
                    if (dictUserTicket.ContainsKey(adminUserExt.AdminUserId))
                    {
                        oldSessionDataId = dictUserTicket[adminUserExt.AdminUserId];

                        dictUserTicket[adminUserExt.AdminUserId] = sessionDataId;
                    }
                    else
                    {
                        dictUserTicket.Add(adminUserExt.AdminUserId, sessionDataId);
                    }
                }

                //将在别处登录的同一用户下线
                if (!string.IsNullOrEmpty(oldSessionDataId))
                {
                    lock (lockObj_3)
                    {
                        object obj = cache.GetCache(oldSessionDataId);
                        if (obj != null)
                        {
                            AdminUserExt sessionData = (AdminUserExt)obj;
                            sessionData.IsOffline = true;

                            cache.SetCache(oldSessionDataId, sessionData, TimeSpan.FromDays(3));
                        }
                    }
                }

                #endregion

                boolResult.Succeeded = true;
                adminUserExt.AllAdminSystems = new AdminSystemManage().GetAllAdminSystems();
                boolResult.Result = adminUserExt;
            }
            else
            {
                boolResult.Succeeded = false;
                boolResult.Message = "登录失败";
            }

            return boolResult;
        }

        /// <summary>
        /// 管理员登出
        /// </summary>
        public void Logout(string ticket)
        {
            string sessionId = ticket;
            string sessionDataId = "data_" + ticket;

            ICache cache = new HttpCache();

            object sessionObj = cache.GetCache(sessionId);
            if (sessionObj != null)
            {
                cache.RemoveCache(sessionId);
            }

            AdminUserExt sessoinData = null;
            lock (lockObj_3)
            {
                object sessionDataObj = cache.GetCache(sessionDataId);
                if (sessionDataObj != null)
                {
                    sessoinData = (AdminUserExt)sessionDataObj;
                    cache.RemoveCache(sessionDataId);
                }
            }

            if (sessoinData != null)
            {
                lock (lockObj_1)
                {
                    if (dictUserTicket.ContainsKey(sessoinData.AdminUserId))
                    {
                        dictUserTicket.Remove(sessoinData.AdminUserId);
                    }
                }
            }
        }

        #endregion

        #region 锁定、解锁

        /// <summary>
        /// 管理员锁定
        /// </summary>
        public void Lock(string ticket)
        {
            string sessionId = ticket;
            string sessionDataId = "data_" + ticket;

            ICache cache = new HttpCache();

            object sessionObj = cache.GetCache(sessionId);
            if (sessionObj == null)
            {
                return;
            }

            object sessionDataObj = cache.GetCache(sessionDataId);
            if (sessionDataObj == null)
            {
                return;
            }

            UserSession session = (UserSession)sessionObj;
            if (!session.IsLock)
            {
                session.IsLock = true;
            }

            //更新会话
            if (session.RememberMe)
            {
                cache.SetCache(sessionId, session, session.ExpireTime.Subtract(DateTime.Now));
            }
            else
            {
                session.ExpireTime = DateTime.Now.AddMinutes(60);
                cache.SetCache(sessionId, session, TimeSpan.FromMinutes(60));
            }
        }

        /// <summary>
        /// 管理员解锁 - 页面数据初始化
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public BoolResult<AdminUserExt> UnLockAuthentication(string ticket)
        {
            BoolResult<AdminUserExt> result = new BoolResult<AdminUserExt>();

            //验证会话是否存在
            string sessionId = ticket;
            string sessionDataId = "data_" + ticket;

            ICache cache = new HttpCache();

            object sessionObj = cache.GetCache(sessionId);
            if (sessionObj == null)
            {
                result.Succeeded = false;
                result.Message = "LOGIN_FAIL";

                return result;
            }

            object sessionDataObj = cache.GetCache(sessionDataId);
            if (sessionDataObj == null)
            {
                result.Succeeded = false;
                result.Message = "LOGIN_FAIL";

                return result;
            }

            //获取会话对象
            UserSession sessoin = (UserSession)sessionObj;

            //更新会话
            if (sessoin.RememberMe)
            {
                cache.SetCache(sessionId, sessoin, sessoin.ExpireTime.Subtract(DateTime.Now));
            }
            else
            {
                sessoin.ExpireTime = DateTime.Now.AddMinutes(60);
                cache.SetCache(sessionId, sessoin, TimeSpan.FromMinutes(60));
            }

            //验证会话是否被锁定
            if (!sessoin.IsLock)
            {
                result.Succeeded = false;
                result.Message = "UN_LOCK";

                return result;
            }

            result.Succeeded = true;
            result.Result = (AdminUserExt)sessionDataObj;

            return result;
        }

        /// <summary>
        /// 管理员解锁
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool UnLock(AdminUserExt param)
        {
            admin_user _admin_user = Repository.First<admin_user>(e => e.AdminName == param.AdminName && e.Password == param.Password && e.Disabled == false && e.Deleted == false);
            if (_admin_user != null)
            {
                ICache cache = new HttpCache();

                string sessionId = param.Ticket;

                object sessionObj = cache.GetCache(sessionId);
                if (sessionObj != null)
                {
                    UserSession session = (UserSession)sessionObj;
                    session.IsLock = false;

                    if (session.RememberMe)
                    {
                        cache.SetCache(sessionId, session, session.ExpireTime.Subtract(DateTime.Now));
                    }
                    else
                    {
                        session.ExpireTime = DateTime.Now.AddMinutes(60);
                        cache.SetCache(sessionId, session, TimeSpan.FromMinutes(60));
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 更新个人资料

        /// <summary>
        /// 更新用户资料（密码、用户头像）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool UpdateProfile(AdminUserExt param)
        {
            admin_user _admin_user = Repository.First<admin_user>(e => e.AdminUserId == param.AdminUserId && e.Password == param.OldPassword && e.Disabled == false && e.Deleted == false);
            if (_admin_user != null)
            {
                bool update = false;

                //当更新时输入了密码，则认为需要修改该管理员的登录密码，否则密码维持不变
                if (!string.IsNullOrEmpty(param.Password))
                {
                    _admin_user.Password = param.Password.MD5Encrypt();

                    update = true;
                }
                if (_admin_user.Logo != param.Logo)
                {
                    _admin_user.Logo = param.Logo;

                    update = true;
                }

                if (update)
                {
                    _admin_user.UpdateBy = param.UpdateBy;
                    _admin_user.UpdateOn = param.UpdateOn;
                    Repository.Update<admin_user>(_admin_user);

                    ICache cache = new HttpCache();

                    string sessionDataId = "data_" + param.Ticket;

                    lock (lockObj_3)
                    {
                        object sessionDataObj = cache.GetCache(sessionDataId);
                        if (sessionDataObj != null)
                        {
                            AdminUserExt sessionData = (AdminUserExt)sessionDataObj;
                            if (sessionData.Logo != _admin_user.Logo)
                            {
                                sessionData.Logo = param.Logo;
                                cache.SetCache(sessionDataId, sessionData, TimeSpan.Zero);
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 获取导航菜单及拥有的权限列表

        /// <summary>
        /// 获取用户在各个系统中的导航菜单、拥有的所有权限
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public AdminUserExt GetMenuAndPermission(int adminUserId)
        {
            AdminUserExt result = new AdminUserExt();

            //取出所有管理系统
            IList<admin_system> allAdminSystemList = new AdminSystemManage().GetAllAdminSystems();

            //取出当前用户隶属的管理员组
            //DataTable dtAdminUserGroup = MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("select admin_group.AdminGroupId, admin_group.Type from admin_user_group inner join admin_group on admin_user_group.GroupId = admin_group.AdminGroupId where UserId = {0}", adminUserId), CommandType.Text, null);
            DataTable dtAdminUserGroup = MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format(@"
                SELECT
                admin_group.AdminGroupId,
                admin_group.Type
                FROM
                admin_group
                INNER JOIN admin_group_role ON admin_group_role.GroupId = admin_group.AdminGroupId
                INNER JOIN admin_role_user ON admin_group_role.RoleId = admin_role_user.RoleId
                WHERE
                admin_role_user.UserId = {0}", adminUserId), CommandType.Text, null);

            //并判断是否属于超级管理员组
            bool bSuperUserGroup = dtAdminUserGroup.AsEnumerable().Where(e => e.Field<int>("Type") == 0).Count() > 0 ? true : false;
            if (bSuperUserGroup)
            {
                //属于超级管理员组
                //1. 设置IsSuperUser属性为True
                //2. 设置OwnAdminSystems = 所有管理系统
                //3. 设置OwnPermissionCodes = 所有权限代码
                //4. 设置MenuTrees = 用户在每个系统中对应的导航菜单

                #region 获取数据

                //获取所有有效的权限数据
                DataTable dtAdminPermission = MySqlHelper.ExecuteDataTable("AdminCenterDB", "SELECT * FROM admin_permission WHERE Disabled = 0 AND Deleted = 0 ORDER BY SortNO, AdminPermissionId", CommandType.Text, null);

                //取出所有有效的权限代码
                List<string> permissionCodes = (from dr in dtAdminPermission.AsEnumerable()
                                                select dr.Field<string>("PermissionCode")).ToList();

                //按系统取出当前用户在每个系统的导航菜单
                Dictionary<string, List<AdminPermissionTree>> menuTrees = new Dictionary<string, List<AdminPermissionTree>>();
                foreach (var obj in allAdminSystemList)
                {
                    List<DataRow> dataRows = (from dr in dtAdminPermission.AsEnumerable()
                                              where dr.Field<int>("SystemId") == obj.AdminSystemId && Convert.ToBoolean(dr.Field<UInt64>("IsMenu"))
                                              select dr).ToList();
                    menuTrees.Add(obj.Code, (new AdminPermissionManage()).GetAdminPermissionTree(dataRows, -1));
                }

                #endregion

                result.IsSuperUser = true;
                result.OwnAdminSystems = allAdminSystemList;
                result.OwnPermissionCodes = permissionCodes;
                result.MenuTrees = menuTrees;
            }
            else
            {
                //不属于超级管理员组
                //1. 设置IsSuperUser属性为False
                //2. 设置OwnAdminSystems = 拥有权限的管理系统
                //3. 设置OwnPermissionCodes = 拥有权限的权限代码
                //4. 设置MenuTrees = 用户在拥有权限管理系统中对应的导航菜单

                #region 获取数据

                //获取当前用户隶属的管理员组包含的权限
                List<int> adminGroupIds = dtAdminUserGroup.AsEnumerable().Select(e => e.Field<int>("AdminGroupId")).ToList();

                //获取每个组下面的各个子组集合
                var subGroupList = new List<admin_group>();
                foreach(var gId in adminGroupIds)
                {
                    var groupList = new AdminGroupManageBase().GetAdminGroupList(gId);
                    if (groupList.Count > 0)
                    {
                        subGroupList.AddRange(groupList);
                    }
                }
                if (subGroupList.Count > 0)
                {
                    adminGroupIds.AddRange(subGroupList.Select(e => e.AdminGroupId).Distinct().ToList());
                }

                string strGroupIds = string.Join(",", adminGroupIds.Select(e => e.ToString()).ToArray());
                DataTable dtAdminGroupPermission = string.IsNullOrEmpty(strGroupIds) ? (new DataTable()) : MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("select * from admin_group_permission where GroupId in ({0})", strGroupIds), CommandType.Text, null);

                //获取当前用户隶属的角色组包含的权限
                DataTable dtAdminRolePermission = string.IsNullOrEmpty(strGroupIds) ? (new DataTable()) : MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format(@"
                    SELECT admin_role_permission.RoleId,admin_role_permission.PermissionId
                    FROM admin_role_permission
                    INNER JOIN admin_role_user ON admin_role_user.RoleId = admin_role_permission.RoleId
                    WHERE admin_role_user.UserId = {0};
                ", adminUserId), CommandType.Text, null);

                //获取当前用户隶属的角色组的子组所包含的权限
                var roleIds = dtAdminRolePermission.AsEnumerable().Select(e => e.Field<int>("RoleId")).Distinct().ToList();
                if (roleIds.Count == 0)
                {
                    DataTable dtRole = MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("select * from admin_role_user where UserId = {0}", adminUserId), CommandType.Text, null);
                    roleIds.AddRange(dtRole.AsEnumerable().Select(e => e.Field<int>("RoleId")).Distinct().ToList());
                }

                var subRoleList = new List<admin_role>();
                var subRolePermissions = new List<int>();
                foreach (var rId in roleIds)
                {
                    var roleList = new AdminRoleManageBase().GetAdminRoleList(rId);
                    if (roleList.Count > 0)
                    {
                        subRoleList.AddRange(roleList);
                    }
                }
                if (subRoleList.Count > 0)
                {
                    var subRoleIds = subRoleList.Select(e => e.AdminRoleId).Distinct().ToList();
                    string strRoleIds = string.Join(",", subRoleIds.Select(e => e.ToString()).ToArray());
                    //获取子角色直接包含的权限
                    DataTable dtSubRolePermission = MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("select * from admin_role_permission where RoleId in ({0})", strRoleIds), CommandType.Text, null);
                    subRolePermissions.AddRange(dtSubRolePermission.AsEnumerable().Select(e => e.Field<int>("PermissionId")).ToList());
                }

                //获取当前用户直接包含的权限
                DataTable dtAdminUserPermission = MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("select * from admin_user_permission where UserId = {0}", adminUserId), CommandType.Text, null);

                //合并所有权限ID并去重
                List<int> permissionIds = dtAdminGroupPermission.AsEnumerable().Select(e => e.Field<int>("PermissionId")).ToList();
                permissionIds.AddRange(dtAdminRolePermission.AsEnumerable().Select(e => e.Field<int>("PermissionId")).ToList());
                permissionIds.AddRange(dtAdminUserPermission.AsEnumerable().Select(e => e.Field<int>("PermissionId")).ToList());
                permissionIds.AddRange(subRolePermissions);
                permissionIds = permissionIds.Distinct().ToList();

                //获取当前用户拥有的所有权限
                string strPermissionIds = string.Join(",", permissionIds.Select(e => e.ToString()).ToArray());
                DataTable dtAdminPermission = string.IsNullOrEmpty(strPermissionIds) ? (new DataTable()) : MySqlHelper.ExecuteDataTable("AdminCenterDB", string.Format("SELECT * FROM admin_permission WHERE AdminPermissionId IN ({0}) AND Disabled = 0 AND Deleted = 0 ORDER BY SortNo, AdminPermissionId", strPermissionIds), CommandType.Text, null);

                //获取拥有权限的系统
                List<int> adminSystemIds = dtAdminPermission.AsEnumerable().Select(e => e.Field<int>("SystemId")).Distinct().ToList();
                IList<admin_system> ownAdminSystems = allAdminSystemList.Where(e => adminSystemIds.Contains(e.AdminSystemId)).ToList();

                //取出所有有效的权限代码
                List<string> permissionCodes = (from dr in dtAdminPermission.AsEnumerable()
                                                select dr.Field<string>("PermissionCode")).ToList();

                //按系统取出当前用户在每个系统的导航菜单
                Dictionary<string, List<AdminPermissionTree>> menuTrees = new Dictionary<string, List<AdminPermissionTree>>();
                foreach (var obj in ownAdminSystems)
                {
                    List<DataRow> dataRows = (from dr in dtAdminPermission.AsEnumerable()
                                              where dr.Field<int>("SystemId") == obj.AdminSystemId && Convert.ToBoolean(dr.Field<UInt64>("IsMenu"))
                                              select dr).ToList();
                    menuTrees.Add(obj.Code, (new AdminPermissionManage()).GetAdminPermissionTree(dataRows, -1));
                }

                #endregion

                result.IsSuperUser = false;
                result.OwnAdminSystems = ownAdminSystems;
                result.OwnPermissionCodes = permissionCodes;
                result.MenuTrees = menuTrees;
            }

            return result;
        }

        #endregion

        #region 身份验证

        /// <summary>
        /// 身份统一验证 + 权限验证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Authentication(NameValueCollection param)
        {
            BoolResult result = new BoolResult();

            //判断是否包含必需的参数
            List<string> keys = param.AllKeys.ToList();
            if (keys.IndexOf("sign") < 0 || keys.IndexOf("system_code") < 0 || keys.IndexOf("ticket") < 0)
            {
                result.Succeeded = false;
                result.Message = "ERROR_PARAM";

                return result;
            }

            //对参数进行整理
            string strSign = param["sign"];
            string strSystemCode = param["system_code"];
            string strTicket = param["ticket"];
            string strPostData = string.Format("system_code={0}&ticket={1}", strSystemCode, strTicket);

            //验证SystemCode是否存在
            admin_system _admin_system = new AdminSystemManage().GetAllAdminSystems().FirstOrDefault(e => e.Code == strSystemCode);
            if (_admin_system == null)
            {
                result.Succeeded = false;
                result.Message = "UNAUTH_SYSTEM";

                return result;
            }

            //验证数据签名是否一致
            string strNewSign = (strPostData + _admin_system.SysKey).MD5Encrypt("utf-8");
            if (strNewSign != strSign)
            {
                result.Succeeded = false;
                result.Message = "UNAUTH_KEY";

                return result;
            }

            //验证会话信息
            BoolResult<AdminUserExt> authResult = BaseAuthentication(new string[] { strTicket, strSystemCode });
            if (!authResult.Succeeded)
            {
                result.Message = authResult.Message;
            }
            else
            {
                result.Message = authResult.Result.JsonSerialize();
            }
            result.Succeeded = authResult.Succeeded;

            return result;
        }

        /// <summary>
        /// 基本权限验证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult<AdminUserExt> BaseAuthentication(string[] param)
        {
            BoolResult<AdminUserExt> result = new BoolResult<AdminUserExt>();

            string strTicket = param[0];
            string strSystemCode = param[1];

            //验证会话是否存在
            string sessionId = strTicket;
            string sessionDataId = "data_" + strTicket;

            ICache cache = new HttpCache();

            object sessionObj = cache.GetCache(sessionId);
            if (sessionObj == null)
            {
                result.Succeeded = false;
                result.Message = "LOGIN_FAIL";

                return result;
            }

            object sessionDataObj = cache.GetCache(sessionDataId);
            if (sessionDataObj == null)
            {
                result.Succeeded = false;
                result.Message = "LOGIN_FAIL";

                return result;
            }

            //获取会话对象
            UserSession session = (UserSession)sessionObj;
            AdminUserExt sessionData = (AdminUserExt)sessionDataObj;

            //更新会话
            if (session.RememberMe)
            {
                cache.SetCache(sessionId, session, session.ExpireTime.Subtract(DateTime.Now));
            }
            else
            {
                session.ExpireTime = DateTime.Now.AddMinutes(60);
                cache.SetCache(sessionId, session, TimeSpan.FromMinutes(60));
            }

            //验证是否被踢下线
            if (sessionData.IsOffline)
            {
                result.Succeeded = false;
                result.Message = "OFFLINE";
                return result;
            }

            //验证是否被锁定
            if (session.IsLock)
            {
                result.Succeeded = false;
                result.Message = "LOCKED";
                return result;
            }

            //验证是否有权限（更新用户信息页面无需验证权限）
            if (strSystemCode != "UpdateProfile" && !sessionData.MenuTrees.ContainsKey(strSystemCode))
            {
                result.Succeeded = false;
                result.Message = "UNAUTH_USER";
                return result;
            }

            result.Succeeded = true;
            result.Result = new AdminUserExt();
            sessionData.CopyTo(result.Result);
            if (strSystemCode != "UpdateProfile")
            {
                //精简导航树代码，仅传递当前系统的导航菜单，减少冗余代码传递
                Dictionary<string, List<AdminPermissionTree>> menuTrees = new Dictionary<string, List<AdminPermissionTree>>();
                menuTrees.Add(strSystemCode, sessionData.MenuTrees[strSystemCode]);
                result.Result.MenuTrees = menuTrees;
            }

            return result;
        }

        #endregion

        #region 系统、权限、用户、用户组有变动时主动更新用户会话数据，维护UserTicket对象

        /// <summary>
        /// 更新所有用户的SessionData数据（此处仅修改标识，具体变动由辅助线程来完成）
        /// 使用场景：系统、权限、用户组变动
        /// </summary>
        internal void UpdateUserSession()
        {
            lock (lockObj_2)
            {
                if (!bUpdateAllUserSession)
                {
                    bUpdateAllUserSession = true;
                }
            }
        }

        /// <summary>
        /// 更新指定用户的SessionData数据
        /// 使用场景：修改管理员信息
        /// </summary>
        /// <param name="intAdminUserId"></param>
        internal void UpdateUserSession(int intAdminUserId)
        {
            ICache cache = new HttpCache();

            if (dictUserTicket.ContainsKey(intAdminUserId))
            {
                string sessionDataId = dictUserTicket[intAdminUserId];

                admin_user _admin_user = Repository.First<admin_user>(e => e.AdminUserId == intAdminUserId && e.Disabled == false && e.Deleted == false);
                if (_admin_user == null)
                {
                    lock (lockObj_3)
                    {
                        cache.RemoveCache(sessionDataId);
                    }
                }
                else
                {
                    lock (lockObj_3)
                    {
                        object sessionDataObj = cache.GetCache(sessionDataId);
                        if (sessionDataObj != null)
                        {
                            AdminUserExt sessionData = (AdminUserExt)sessionDataObj;

                            AdminUserExt adminUserExt = GetMenuAndPermission(intAdminUserId);
                            _admin_user.CopyTo(adminUserExt);
                            adminUserExt.IsOffline = sessionData.IsOffline;

                            cache.SetCache(sessionDataId, adminUserExt, TimeSpan.FromDays(3));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新所有用户的SessionData数据
        /// 使用场景：辅助线程判别是否需要更新所有用户SessionData数据
        /// </summary>
        public void UpdateAllUserSession()
        {
            ICache cache = new HttpCache();

            //维护UserTicket
            lock (lockObj_1)
            {
                List<int> keys = dictUserTicket.Keys.ToList();
                foreach (var obj in keys)
                {
                    if (cache.GetCache(dictUserTicket[obj]) == null)
                    {
                        dictUserTicket.Remove(obj);
                    }
                }
            }

            //判断是否需要对SessionData进行维护
            bool bUpdate = false;
            lock (lockObj_2)
            {
                if (bUpdateAllUserSession)
                {
                    bUpdate = true;
                    bUpdateAllUserSession = false;
                }
            }

            //更新所有用户的SessionData
            if (bUpdate)
            {
                foreach (var item in dictUserTicket)
                {
                    string sessionDataId = dictUserTicket[item.Key];

                    lock (lockObj_3)
                    {
                        object sessionDataObj = cache.GetCache(sessionDataId);
                        if (sessionDataObj != null)
                        {
                            AdminUserExt sessionData = (AdminUserExt)sessionDataObj;

                            AdminUserExt adminUserExt = GetMenuAndPermission(item.Key);
                            sessionData.IsSuperUser = adminUserExt.IsSuperUser;
                            sessionData.OwnAdminSystems = adminUserExt.OwnAdminSystems;
                            sessionData.OwnPermissionCodes = adminUserExt.OwnPermissionCodes;
                            sessionData.MenuTrees = adminUserExt.MenuTrees;

                            cache.SetCache(sessionDataId, sessionData, TimeSpan.FromDays(3));
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取在线管理员用户列表
        /// </summary>
        public void GetOnlineAdminUsers()
        {
        }

        /// <summary>
        /// 用户会话对象
        /// </summary>
        class UserSession
        {
            /// <summary>
            /// 会话过期时间，此属性值仅在“记住用户登录状态”时使用
            /// </summary>
            public DateTime ExpireTime
            {
                get;
                set;
            }

            /// <summary>
            /// 是否记住登录状态
            /// </summary>
            public bool RememberMe
            {
                get;
                set;
            }

            /// <summary>
            /// 是否被锁定
            /// </summary>
            public bool IsLock
            {
                get;
                set;
            }
        }
    }
}
