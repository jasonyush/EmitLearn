using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace EmitLearn
{
    public class HelloWord
    {
        /// <summary>
        /// 用来调用动态方法的委托
        /// </summary>
        private delegate void HelloDelegate();
        public static void Hello()
        {
            try
            {
                //定义一个名字为HelloWorld的动态方法，没有返回值,没有参数
                DynamicMethod helloMethod = new DynamicMethod("HelloWorld", null, null);
                //创建MSIL生成器，为动态方法生成代码
                ILGenerator iLGenerator = helloMethod.GetILGenerator();
                //将动态方法需要输出的字符串Hello World! 添加到堆栈上
                iLGenerator.Emit(OpCodes.Ldstr, "Hello World@");
                //调用输出方法输出字符串
                iLGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                //方法结束
                iLGenerator.Emit(OpCodes.Ret);
                //完成动态方法的创建，并且获取一个可以执行该动态方法的委托
                HelloDelegate hello = (HelloDelegate)helloMethod.CreateDelegate(typeof(HelloDelegate));

                // 执行动态方法，将在屏幕上打印Hello World!
                 hello();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
