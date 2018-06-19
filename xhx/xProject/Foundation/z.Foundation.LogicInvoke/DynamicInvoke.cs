using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 动态调用类
    /// </summary>
    internal class DynamicInvoke
    {
        private static readonly object lockObj = new object();
        private static Dictionary<string, DynamicMethodExecutor> dynamicMethodExecutorList = new Dictionary<string, DynamicMethodExecutor>();

        /// <summary>
        /// 普通参数动态调用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static object Call(IRequest request)
        {
            return Invoke(request.Target, request.Class, request.Method, request.Parameter);
        }

        /// <summary>
        /// 泛型参数动态调用
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static object Call<T1>(IRequest<T1> request)
        {
            return Invoke(request.Target, request.Class, request.Method, request.Parameter);
        }

        /// <summary>
        /// 动态调用程序集，并返回object
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Class"></param>
        /// <param name="Method"></param>
        /// <param name="Parameter"></param>
        /// <returns></returns>
        private static object Invoke(string Target, string Class, string Method, object Parameter)
        {
            //string target = string.Format("{0}{1}", Utility.AssemblyPath(), Target);
            //Assembly assembly = Assembly.LoadFile(target);
            Assembly assembly = Assembly.Load(Target.Replace(".dll", ""));
            object instance = assembly.CreateInstance(Class);
            MethodInfo methodInfo = instance.GetType().GetMethod(Method);

            string key = Class + "." + Method;
            if (!dynamicMethodExecutorList.ContainsKey(key))
            {
                lock (lockObj)
                {
                    if (!dynamicMethodExecutorList.ContainsKey(key))
                    {
                        DynamicMethodExecutor executor = new DynamicMethodExecutor(methodInfo);
                        dynamicMethodExecutorList.Add(key, executor);
                    }
                }
            }

            object[] parameters = null;
            if (Parameter != null)
            {
                parameters = new object[] { Parameter };
            }

            DynamicMethodExecutor dynamicMethodExecutor = dynamicMethodExecutorList[key];
            return dynamicMethodExecutor.Execute(instance, parameters);
        }
    }
}
