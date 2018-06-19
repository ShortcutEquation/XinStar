using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using z.Foundation;
using z.Foundation.LogicInvoke;
using z.TaskCenter.Entity;

namespace z.TaskCenter.Logic
{
    /// <summary>
    /// 任务分配中心
    /// </summary>
    public class TaskCore
    {

        //所有的任务
        private static List<task_info> AllTasks = new List<task_info>();

        private static object objAllTasksLock = new object();

        //所有的子任务
        private static ConcurrentDictionary<int, List<task_info>> ChildTasks = new ConcurrentDictionary<int, List<task_info>>();

        //重试时间
        private static int[] TASK_RETRY_TIMES = new int[] { 1000, 2000, 5000, 10000, 20000, 40000, 60000, 120000, 240000, 400000 };

        //正在重试的任务
        private static ConcurrentDictionary<int, int> RetryTasks = new ConcurrentDictionary<int, int>();

        //检测任务重试机制
        private static Thread thTaskCheck = new Thread(new ThreadStart(CheckTaskMethod));

        //等待被删除的任务
        private static Dictionary<string, task_main> WaitDeleted = new Dictionary<string, task_main>();

        //是否停止任务出队列
        private static bool IsStopDequeue = false;

        //检测任务
        private static Thread thCheckConfig = new Thread(new ThreadStart(CheckConfig));

        //已完成的主任务队列
        private static ConcurrentDictionary<string, ConcurrentQueue<task_main>> TaskMainCompleteQueue = new ConcurrentDictionary<string, ConcurrentQueue<task_main>>();

        /// <summary>
        /// 加入任务队列
        /// </summary>
        /// <param name="task"></param>
        /// <param name="childTasks"></param>
        /// <returns></returns>
        public static bool Enqueue(task_info task, List<task_info> childTasks)
        {
            lock (objAllTasksLock)
            {
                //插入任务到全局任务
                if (!AllTasks.Exists(a => a.Id == task.Id))
                {
                    AllTasks.Add(task);
                }
            }

            //子任务
            if (childTasks != null && childTasks.Count > 0)
            {
                if (!ChildTasks.ContainsKey(task.Id))
                {
                    ChildTasks.TryAdd(task.Id, new List<task_info>());
                }
                var mainChildTasks = ChildTasks[task.Id];

                childTasks = childTasks.OrderBy(a => a.SortNo).ToList();
                int iCount = childTasks.Count;

                for (int i = 0; i < iCount; i++)
                {
                    //插入任务到全局子任务，用于后续检测用
                    mainChildTasks.Add(childTasks[i]);
                }
            }

            TaskQueueFactory.EnqueueTask(task, childTasks);

            return true;

        }

        /// <summary>
        /// 从任务队列中获取任务
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public static task_info Dequeue(string taskType)
        {
            if (IsStopDequeue)
            {
                return null;
            }

            task_info task;
            
            while (true)
            {
                task = TaskQueueFactory.DequeueTask(taskType);
                if (task != null)
                {
                    if (task.ParentTaskId == -1)
                    {
                        //主任务目前无需考虑
                        continue;
                    }

                    //检测任务是否被删除（暂时不处理）
                    if (WaitDeleted.ContainsKey(taskType + "_" + task.CustomContent))
                    {
                        var waitDeleteTask = WaitDeleted[taskType + "_" + task.CustomContent];
                        if (waitDeleteTask != null && waitDeleteTask.Id == task.TaskMainId)
                        {
                            Console.WriteLine("跳过任务:" + task.Id);
                            continue;
                        }
                    }
                }
                break;
            }

            if (task != null)
            {
                if (task.ParentTaskId > 0)
                {
                    //检测该任务是否轮到执行
                    if (CheckCanRun(task))
                    {
                        return task;
                    }
                    else
                    {
                        //重新加入队列
                        TaskQueueFactory.EnqueueTask(task, null);
                    }
                }
                else
                {
                    return task;
                }
            }

            return null;
        }

        /// <summary>
        /// 检测任务是否可以运行
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static bool CheckCanRun(task_info task)
        {
            if (ChildTasks.ContainsKey(task.ParentTaskId) && ChildTasks[task.ParentTaskId] != null)
            {
                if (ChildTasks[task.ParentTaskId].Count(a => a.SortNo < task.SortNo && a.TaskStatus < 2) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="task"></param>
        public static void UpdateTask(task_info task)
        {
            task_info taskAll = GetTaskInfo(task.Id);
            if (taskAll != null)
            {
                task.CopyTo(taskAll);
            }

            if (task.ParentTaskId > 0 && ChildTasks.ContainsKey(task.ParentTaskId))
            {
                task_info taskChild = ChildTasks[task.ParentTaskId].FirstOrDefault(a => a.Id == task.Id);
                if (taskChild != null)
                {
                    task.CopyTo(taskChild);
                }
            }

        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static task_info GetTaskInfo(int id)
        {
            lock (objAllTasksLock)
            {
                int taskCount = AllTasks.Count;
                for (int i = 0; i < taskCount; i++)
                {
                    if (AllTasks[i].Id == id)
                    {
                        return AllTasks[i];
                    }
                }
            }
            return null;
        }

        public static void SetTaskDeleted(string key,task_main taskMain)
        {
            WaitDeleted.Remove(key);
            if (!WaitDeleted.ContainsKey(key))
            {
                WaitDeleted.Add(key, taskMain);
            }
        }

        /// <summary>
        /// 获取重试需要休眠的时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetTaskRetryTime(int id)
        {
            if (!RetryTasks.ContainsKey(id))
                RetryTasks.TryAdd(id, 0);

            int index = RetryTasks[id];

            RetryTasks[id] = index + 1;
            if (index >= TASK_RETRY_TIMES.Length)
                return -1;

            return TASK_RETRY_TIMES[index];
        }

        public static void EnqueueCompleteTaskMain(task_main taskMain)
        {
            if (!TaskMainCompleteQueue.ContainsKey(taskMain.TaskType))
            {
                TaskMainCompleteQueue.TryAdd(taskMain.TaskType, new ConcurrentQueue<task_main>());
            }
            ConcurrentQueue<task_main> currentTypeTaskMainQueue = TaskMainCompleteQueue[taskMain.TaskType];

            currentTypeTaskMainQueue.Enqueue(taskMain);
        }

        public static task_main DequeueCompleteTaskMain(string taskType)
        {
            if (IsStopDequeue)
            {
                return null;
            }

            if (TaskMainCompleteQueue.ContainsKey(taskType))
            {
                ConcurrentQueue<task_main> currentTypeTaskMainQueue = TaskMainCompleteQueue[taskType];
                task_main taskMain;
                currentTypeTaskMainQueue.TryDequeue(out taskMain);

                return taskMain;
            }

            return null;
        }

        public static void RemoveTasks(int taskMainId)
        {
            lock (objAllTasksLock)
            {
                for (int i = AllTasks.Count - 1; i >= 0; i--)
                {
                    if (AllTasks[i].TaskMainId == taskMainId)
                    {
                        int parentTaskId = AllTasks[i].Id;
                        Console.WriteLine("移除:" + taskMainId + "," + AllTasks.Count);
                        AllTasks.RemoveAt(i);
                        List<task_info> removedTasks;
                        ChildTasks.TryRemove(parentTaskId, out removedTasks);
                        if (removedTasks != null)
                        {
                            Console.WriteLine("移除:" + parentTaskId + "child," + removedTasks.Count);
                        }
                        removedTasks = null;
                    }
                }
            }
        }

        #region 重试机制  暂时不考虑
        public void CheckTaskThread()
        {
            thTaskCheck.Start();
        }

        public static void CheckTaskMethod()
        {
            while (true)
            {
                List<task_info> taskList = AllTasks.Where(a => a.TaskStatus == 0 && a.CreatedOn.Value.AddMinutes(20) < DateTime.Now).ToList();

            }
        }

        #endregion

        #region 任务中心配置检测

        public static void CheckConfig()
        {
            while (true)
            {
                ConfigurationManager.RefreshSection("appSettings");

                try
                {
                    IsStopDequeue = Utility.GetConfigValue("IsStopDequeue") == "1";
                }
                catch (Exception ex)
                {
                    IsStopDequeue = false;
                }

                Thread.Sleep(5000);
            }
        }

        public static void StartCheckConfig()
        {
            thCheckConfig.Start();
        }

        #endregion
    }
}
