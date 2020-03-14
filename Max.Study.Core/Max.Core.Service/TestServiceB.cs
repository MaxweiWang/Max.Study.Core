using Max.Core.Interface;
using System;

namespace Max.Core.Service
{
    public class TestServiceB : ITestServiceB
    {
        
        public TestServiceB(ITestServiceA iTestService)
        {
          
        }


        public void Show()
        { 
            Console.WriteLine($"This is TestServiceB B123456");
        }
    }
}
