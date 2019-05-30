using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitLearn
{
    public class EmitException
    {
        private delegate int RunCatchDelegate(string str);
        public static void Run()
        {
            DynamicMethod methodExt = new DynamicMethod("RunCatch", typeof(Int32), new Type[] { typeof(string) });

            ILGenerator methodIL = methodExt.GetILGenerator();
            LocalBuilder num = methodIL.DeclareLocal(typeof(Int32));
            //int num = 0;
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_0);
            //begin try
            Label tryLabel = methodIL.BeginExceptionBlock();
            //num = Convert.ToInt32(str);
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(string) }));
            methodIL.Emit(OpCodes.Stloc_0);
            //end try

            //begin catch 注意，这个时侯堆栈顶为异常信息ex
            methodIL.BeginCatchBlock(typeof(Exception));
            //Console.WriteLine(ex.Message);
            methodIL.Emit(OpCodes.Call, typeof(Exception).GetMethod("get_Message"));
            methodIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            //end catch
            methodIL.EndExceptionBlock();
            //return num;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ret);

            //完成动态方法的创建，并且获取一个可以执行该动态方法的委托
            RunCatchDelegate RunCatch = (RunCatchDelegate)methodExt.CreateDelegate(typeof(RunCatchDelegate));
            int result=RunCatch("1");

            Console.WriteLine(result.ToString());
            RunCatch("s");
        }
    }

    class ExceptionHandler
    {
        public static int ConvertToInt32(string str)
        {
            int num = 0;
            try
            {
                num = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return num;
        }
    }

}
