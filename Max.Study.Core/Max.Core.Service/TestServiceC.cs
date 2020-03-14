using Max.Core.Interface;
using System;

namespace Max.Core.Service
{
    public class TestServiceC : ITestServiceC
    {
        public TestServiceC(ITestServiceB iTestServiceB)
        {
        }
        public void Show()
        {
            Console.WriteLine("C123456");
        }
    }
}
