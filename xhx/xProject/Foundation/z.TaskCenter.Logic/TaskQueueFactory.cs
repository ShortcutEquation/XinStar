using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.TaskCenter.Entity;

namespace z.TaskCenter.Logic
{
    public class TaskQueueFactory
    {
        private static ConcurrentDictionary<string, List<ConcurrentQueue<task_info>>> TaskQueue = new ConcurrentDictionary<string, List<ConcurrentQueue<task_info>>>();

        /// <summary>
        /// 多通道出队列
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public static task_info DequeueTask(string taskType)
        {
            task_info task = null;
            if (!TaskQueue.ContainsKey(taskType))
            {
                return null;
            }

            List<ConcurrentQueue<task_info>> currentTypeTaskQueue = TaskQueue[taskType].Where(a => a.Count > 0).ToList();
            if (currentTypeTaskQueue.Count == 0)
            {
                return null;
            }

            int iIndex = new Random().Next(0, currentTypeTaskQueue.Count);
            int i = iIndex;
            while (true)
            {
                ConcurrentQueue<task_info> queue = currentTypeTaskQueue[i];
                if (queue.Count == 0)
                {
                    break;
                }

                queue.TryDequeue(out task);
                if (task != null)
                {
                    break;
                }

                i++;
                i = i % currentTypeTaskQueue.Count;
                if (i == iIndex)
                {
                    break;
                }
            }

            return task;
        }

        /// <summary>
        /// 多通道入队列
        /// </summary>
        /// <param name="task"></param>
        /// <param name="childTasks"></param>
        /// <returns></returns>
        public static bool EnqueueTask(task_info task, List<task_info> childTasks)
        {
            if (!TaskQueue.ContainsKey(task.TaskType))
            {
                //默认开启5个通道来处理任务
                TaskQueue.TryAdd(task.TaskType, new List<ConcurrentQueue<task_info>>()
                {
                    new ConcurrentQueue<task_info>(),
                    new ConcurrentQueue<task_info>(),
                    new ConcurrentQueue<task_info>(),
                    new ConcurrentQueue<task_info>(),
                    new ConcurrentQueue<task_info>()
                });
            }

            List<ConcurrentQueue<task_info>> currentTypeTaskQueue = TaskQueue[task.TaskType];
            ConcurrentQueue<task_info> minConcurrentQueue = currentTypeTaskQueue.OrderBy(a => a.Count()).First();

            //插入主任务
            minConcurrentQueue.Enqueue(task);
            //子任务
            if (childTasks != null && childTasks.Count > 0)
            {
                int iCount = childTasks.Count;
                for (int i = 0; i < iCount; i++)
                {
                    minConcurrentQueue.Enqueue(childTasks[i]);
                }
            }

            return true;
        }


    }
}
