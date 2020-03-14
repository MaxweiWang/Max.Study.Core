using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Study.Core2.Utility
{
    public class CustomAutofacAOP : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"method is {invocation.Method.Name}");
            Console.WriteLine($"Arguments is {string.Join(';', invocation.Arguments)}");

            invocation.Proceed();// 这里表示继续执行，就去执行之前应该执行的动作了

            Console.WriteLine("逐梦，在线久了。。。。。");

        }
    }
      
    public interface IA
    {
        void Show();
    }

    [Intercept(typeof(CustomAutofacAOP))]
    public class A : IA
    {
        public void Show()
        {
            Console.WriteLine("Cm");
        }
    }
}
