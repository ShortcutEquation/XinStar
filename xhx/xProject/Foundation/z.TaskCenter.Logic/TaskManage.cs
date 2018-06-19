using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using z.Foundation;
using z.Foundation.Data;
using z.Foundation.LogicInvoke;
using z.Logic.Base;
using z.TaskCenter.Entity;

namespace z.TaskCenter.Logic
{
    public class TaskManage : NHLogicBase
    {

        /// <summary>
        /// 服务启动的时候初始化，获取所有待分配的任务
        /// </summary>
        public void InitTaskList()
        {
            //启动配置自动检测
            TaskCore.StartCheckConfig();

            #region 初始化任务
            List<task_info> taskList = Repository.Find<task_info>(a => a.TaskStatus == 0).ToList();
            int iCount = taskList.Count();
            for (int i = 0; i < iCount; i++)
            {
                if (taskList[i].ParentTaskId == 0)
                {
                    TaskCore.Enqueue(taskList[i], null);
                }
                else if (taskList[i].ParentTaskId == -1)
                {
                    List<task_info> childTasks = taskList.Where(a => a.ParentTaskId == taskList[i].Id).ToList();
                    if (childTasks.Count > 0)
                    {
                        TaskCore.Enqueue(taskList[i], childTasks);
                    }
                }
            }
            Console.WriteLine("总共待处理的任务:" + iCount);
            #endregion

        }

        #region 短连接操作

        /// <summary>
        /// 推送任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public BoolResult PushTask(Dictionary<string,object> param)
        {

            BoolResult result = new BoolResult();

            using (ISession session = NHibernateHelper<task_info>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    //前端显示的任务
                    task_main taskMain = new task_main();

                    if (param.ContainsKey("AddUpdate") && param["AddUpdate"].ToString() == "1" && param.ContainsKey("CustomContent"))
                    {

                        #region 删除原来的任务

                        task_main oldTaskMain = Repository.First<task_main>(a => a.TaskType == param["TaskType"].ToString() && a.CustomContent == param["CustomContent"].ToString());
                        if (oldTaskMain != null)
                        {
                            Repository.Delete(oldTaskMain);

                            z.Foundation.Data.MySqlHelper.ExecuteNonQuery("TaskCenterDB", "Delete from task_info where TaskMainId=" + oldTaskMain.Id, System.Data.CommandType.Text);

                            TaskCore.SetTaskDeleted(param["TaskType"].ToString() + "_" + param["CustomContent"].ToString(), oldTaskMain);
                        }

                        #endregion

                        taskMain.CustomContent = param["CustomContent"].ToString();
                    }
                    
                    taskMain.SystemType = Int32.Parse(param["SystemType"].ToString());
                    taskMain.TaskType = param["TaskType"].ToString();
                    taskMain.TaskStatus = 0;
                    taskMain.CreatedBy = param["CreatedBy"].ToString();
                    taskMain.CreatedOn = DateTime.Now;
                    taskMain.TotalCount = 1;
                    if (param.ContainsKey("Content"))
                    {
                        taskMain.Content = param["Content"].ToString();
                    }
                    session.Save(taskMain);

                    //主任务添加
                    task_info taskInfo = ConvertTaskInfo(param);
                    taskInfo.CustomContent = taskMain.CustomContent;
                    taskInfo.TaskMainId = taskMain.Id;
                    session.Save(taskInfo);

                    List<task_info> childTasks = new List<task_info>();
                    if (param.ContainsKey("ChildTasks"))
                    {
                        //子任务添加
                        List<Dictionary<string, object>> childTaskParams = param["ChildTasks"] as List<Dictionary<string, object>>;
                        if (childTaskParams.Count > 0)
                        {
                            foreach (var childParam in childTaskParams)
                            {
                                task_info childTask = ConvertTaskInfo(childParam);
                                childTask.TaskType = taskMain.TaskType;
                                childTask.ParentTaskId = taskInfo.Id;
                                childTask.TaskMainId = taskMain.Id;
                                childTask.CustomContent = taskMain.CustomContent;
                                session.Save(childTask);
                                childTasks.Add(childTask);
                            }

                            taskInfo.ParentTaskId = -1;//标识为主任务
                            session.Update(taskInfo);
                        }
                        taskMain.TotalCount = childTaskParams.Count;
                        session.Update(taskMain);
                    }

                    transaction.Commit();

                    //将任务加入到队列中
                    Console.WriteLine("开始加入队列:" + taskInfo.Id + ",子任务个数：" + childTasks.Count);
                    TaskCore.Enqueue(taskInfo, childTasks);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    transaction.Rollback();
                }
            }
            result.Succeeded = true;
            return result;

        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public BoolResult<Dictionary<string, object>> GetTaskInfo(int taskId)
        {
            BoolResult<Dictionary<string, object>> boolResult = new BoolResult<Dictionary<string, object>>();
            task_info taskInfo = Repository.First<task_info>(a => a.Id == taskId);
            if (taskInfo == null)
            {
                boolResult.Succeeded = false;
                boolResult.Message = "未找到任务数据";
            }
            else
            {
                Dictionary<string, object> taskResult = ConvertFromTaskInfo(taskInfo);
                boolResult.Result = taskResult;
            }
            return boolResult;
        }

        private Dictionary<string,object> ConvertFromTaskInfo(task_info taskInfo)
        {
            Dictionary<string, object> taskResult = new Dictionary<string, object>();
            taskResult.Add("Id", taskInfo.Id);
            taskResult.Add("Content", taskInfo.Content);
            taskResult.Add("TaskType", taskInfo.TaskType);
            taskResult.Add("TaskStatus", taskInfo.TaskStatus);
            taskResult.Add("ParentTaskId", taskInfo.ParentTaskId);
            taskResult.Add("SortNo", taskInfo.SortNo);
            taskResult.Add("CreatedOn", taskInfo.CreatedOn);
            taskResult.Add("FinishedOn", taskInfo.FinishedOn);
            taskResult.Add("ExecuteResult", taskInfo.ExecuteResult);

            return taskResult;
        }

        private task_info ConvertTaskInfo(Dictionary<string, object> param)
        {
            task_info task = new task_info();

            if (param.ContainsKey("Content"))
            {
                task.Content = param["Content"].ToString();
            }else
            {
                task.Content = "";
            }
            
            if (param.ContainsKey("TaskType"))
            {
                task.TaskType = param["TaskType"].ToString();
            }
            task.TaskStatus = 0;
            if (param.ContainsKey("SortNo"))
            {
                task.SortNo = Int32.Parse(param["SortNo"].ToString());
            }
            task.CreatedOn = DateTime.Now;

            return task;
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult UpdateTask(Dictionary<string, object> param)
        {
            BoolResult result = new BoolResult();

            task_info task = Repository.Get<task_info>(Int32.Parse(param["Id"].ToString()));
            if (task != null)
            {
                task.TaskStatus = Int32.Parse(param["TaskStatus"].ToString());
                task.FinishedOn = DateTime.Now;

                if (param.ContainsKey("ExecuteResult"))
                {
                    task.ExecuteResult = param["ExecuteResult"].ToString();
                }

                Repository.Update(task);

                using (ISession session = NHibernateHelper<task_main>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        task_main taskMain = session.Get<task_main>(task.TaskMainId, LockMode.Upgrade);
                        switch (task.TaskStatus)
                        {
                            case 2:
                                taskMain.SuccessCount = taskMain.SuccessCount + 1;
                                break;
                            case -1:
                                taskMain.FailCount = taskMain.FailCount + 1;
                                break;
                        }

                        if (taskMain.TotalCount == taskMain.SuccessCount + taskMain.FailCount)
                        {
                            if (taskMain.SuccessCount == 0)
                            {
                                taskMain.TaskStatus = -1;
                            }
                            else
                            {
                                taskMain.TaskStatus = 2;
                            }
                            taskMain.FinishedOn = DateTime.Now;
                            task_info taskParent = Repository.First<task_info>(a => a.Id == task.ParentTaskId);
                            if (taskParent != null)
                            {
                                taskParent.TaskStatus = 2;
                                taskParent.FinishedOn = DateTime.Now;
                                session.Update(taskParent);
                            }
                            if (taskMain.FailCount > 0)
                            {
                                taskMain.AbnormalStatus = 1;
                            }
                        }
                        else
                        {
                            taskMain.TaskStatus = 1;
                        }

                        session.Update(taskMain);
                        transaction.Commit();

                        if(taskMain.TaskStatus==2 || taskMain.TaskStatus == -1)
                        {
                            TaskCore.EnqueueCompleteTaskMain(taskMain);
                            TaskCore.RemoveTasks(taskMain.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }

                TaskCore.UpdateTask(task);
            }
            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// 更新总任务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult UpdateTaskMain(Dictionary<string, object> param)
        {
            BoolResult boolResult = new BoolResult();
            task_main taskMain = Repository.Get<task_main>(Int32.Parse(param["Id"].ToString()));
            if (taskMain != null)
            {
                if (param.ContainsKey("IsExportCompleted"))
                {
                    taskMain.IsExportCompleted = bool.Parse(param["IsExportCompleted"].ToString());
                }

                Repository.Update(taskMain);
            }

            return boolResult;
        }

        /// <summary>
        /// 获取用户的任务列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<Dictionary<string, object>> GetTaskMainList(IPagedParam<Dictionary<string, object>> param)
        {
            DateTime dtBegin = DateTime.Now;
            IPagedList<Dictionary<string, object>> result = new PagedList<Dictionary<string, object>>();

            IQueryable<task_main> where = Repository.AsQueryable<task_main>();
            if (param.model.ContainsKey("SystemType"))
            {
                where = where.Where(a => a.SystemType == Int32.Parse(param.model["SystemType"].ToString()));
            }

            if (param.model.ContainsKey("CreatedBy"))
            {
                where = where.Where(a => a.CreatedBy == param.model["CreatedBy"].ToString());
            }

            if (param.model.ContainsKey("TaskStatus"))
            {
                where = where.Where(a => a.TaskStatus == Int32.Parse(param.model["TaskStatus"].ToString()));
            }

            where = where.OrderByDescending(a => a.Id);

            IPagedList<task_main> tempPageList = new PagedList<task_main>(where, param.PageIndex, param.PageSize);
            result.TotalCount = tempPageList.TotalCount;
            result.TotalPages = tempPageList.TotalPages;
            result.PageIndex = tempPageList.PageIndex;
            result.PageSize = tempPageList.PageSize;
            result.Rows = new List<Dictionary<string, object>>();

            if (tempPageList.Rows.Count > 0)
            {
                List<task_type> taskTypeList = Repository.AsQueryable<task_type>().ToList();
                foreach (var taskMain in tempPageList.Rows)
                {
                    Dictionary<string, object> dictTaskMain = ConvertFromTaskMainInfo(taskMain);

                    var taskType = taskTypeList.FirstOrDefault(a => a.TaskType == taskMain.TaskType);
                    string taskTypeName = "";
                    int bussinessType = 0;
                    if (taskType != null)
                    {
                        taskTypeName = taskType.Name;
                        bussinessType = taskType.BussinessType;
                    }
                    dictTaskMain.Add("TaskTypeName", taskTypeName);
                    dictTaskMain.Add("BussinessType", bussinessType);

                    result.Rows.Add(dictTaskMain);
                }
            }
            Console.WriteLine("GetTaskMainList:" + DateTime.Now.Subtract(dtBegin).TotalMilliseconds);
            return result;
        }

        /// <summary>
        /// 转换taskmain实体
        /// </summary>
        /// <param name="taskMain"></param>
        /// <returns></returns>
        private Dictionary<string,object> ConvertFromTaskMainInfo(task_main taskMain)
        {
            Dictionary<string, object> dictTaskMain = new Dictionary<string, object>();

            dictTaskMain.Add("Id", taskMain.Id);
            dictTaskMain.Add("TaskType", taskMain.TaskType);
            dictTaskMain.Add("SystemType", taskMain.SystemType);
            dictTaskMain.Add("TaskStatus", taskMain.TaskStatus);
            dictTaskMain.Add("SuccessCount", taskMain.SuccessCount);
            dictTaskMain.Add("TotalCount", taskMain.TotalCount);
            dictTaskMain.Add("FailCount", taskMain.FailCount);
            dictTaskMain.Add("CreatedOn", taskMain.CreatedOn);
            dictTaskMain.Add("CreatedBy", taskMain.CreatedBy);
            dictTaskMain.Add("FinishedOn", taskMain.FinishedOn);
            dictTaskMain.Add("Content", taskMain.Content);
            dictTaskMain.Add("CustomContent", taskMain.CustomContent);
            dictTaskMain.Add("AbnormalStatus", taskMain.AbnormalStatus);
            dictTaskMain.Add("IsExportCompleted", taskMain.IsExportCompleted);

            return dictTaskMain;
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult<Dictionary<string, object>> GetTaskMainInfo(Dictionary<string, object> param)
        {
            int taskMainId = Int32.Parse(param["TaskMainId"].ToString());
            BoolResult<Dictionary<string, object>> result = new BoolResult<Dictionary<string, object>>();

            var taskMain = Repository.First<task_main>(a => a.Id == taskMainId);
            if (taskMain != null)
            {
                result.Result = ConvertFromTaskMainInfo(taskMain);

                IQueryable<task_info> where = Repository.AsQueryable<task_info>().Where(a => a.TaskMainId == taskMainId && a.ParentTaskId != -1);
                if (param.ContainsKey("TaskStatus"))
                {
                    int taskStatus = Int32.Parse(param["TaskStatus"].ToString());
                    where = where.Where(a => a.TaskStatus == taskStatus);
                }

                var taskInfoList = where.ToList();
                List<Dictionary<string, object>> dictTaskList = new List<Dictionary<string, object>>();
                foreach (var task in taskInfoList)
                {
                    dictTaskList.Add(ConvertFromTaskInfo(task));
                }
                result.Result.Add("Tasks", dictTaskList);
                result.Succeeded = true;
            }

            return result;
        }

        /// <summary>
        /// 关闭异常
        /// </summary>
        /// <param name="taskMainId"></param>
        /// <returns></returns>
        public BoolResult CloseTaskMainAbnormal(int taskMainId)
        {
            BoolResult boolResult = new BoolResult();
            task_main taskMain = Repository.First<task_main>(a => a.Id == taskMainId);
            if (taskMain != null)
            {
                taskMain.AbnormalStatus = 2;
                Repository.Update(taskMain);
                boolResult.Succeeded = true;
            }
            return boolResult;
        }

        #endregion

        #region 长连接操作

        /// <summary>
        /// 长连接拉取最新任务
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public void PullTaskInfo(string taskType)
        {
            Console.WriteLine("开始拉取任务");

            LongWCFService.CallTask((param) =>
            {
                task_info task = TaskCore.Dequeue(param.ToString());
                if (task == null) return null;
                return ConvertFromTaskInfo(task);
            },
            (param) =>
            {
                task_main taskMain = TaskCore.DequeueCompleteTaskMain(param.ToString());
                if (taskMain == null) return null;
                return ConvertFromTaskMainInfo(taskMain);
            },
            (task) =>
            {
                return task != null;
            },
            ProcessTaskFail,
            TaskSended,
            taskType);
        }

        /// <summary>
        /// 任务处理失败，加入重试机制
        /// </summary>
        /// <param name="param"></param>
        private void ProcessTaskFail(object param)
        {            
            Dictionary<string, object> dictParam = param as Dictionary<string, object>;
            int taskId = Int32.Parse(dictParam["Id"].ToString());
            task_info task = TaskCore.GetTaskInfo(taskId);
            if (task.TaskStatus != 0)
                return;

            //休眠时间
            int sleepTime = TaskCore.GetTaskRetryTime(taskId);
            if (sleepTime == -1) return;
            Thread.Sleep(sleepTime);

            //重新加入到队列中
            TaskCore.Enqueue(task, null);
         }

        /// <summary>
        /// 标记任务已发送客户端
        /// </summary>
        /// <param name="param"></param>
        private void TaskSended(object param)
        {
            Dictionary<string, object> dictParam = param as Dictionary<string, object>;
            int taskId = Int32.Parse(dictParam["Id"].ToString());

            task_info task = Repository.Get<task_info>(taskId);
            if (task != null)
            {
                task.TaskStatus = 1;
                Repository.Update(task);

                TaskCore.UpdateTask(task);
            }
        }

        #endregion

    }
}
