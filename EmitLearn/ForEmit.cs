using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace EmitLearn
{
    public class ForEmit
    {        /// <summary>
             /// 用来调用动态方法的委托
             /// </summary>
        private delegate Int32 ForDelegate(int[] ints);
        private delegate Int32 ForEachDelegate(int[] ints);
        public static void ForMethod()
        {
           //一， 其中,取数组里的元素使用如下的指令流程:
           // l 首先, 加载数组指针到堆栈： methodIL.Emit(OpCodes.Ldarg_0);
           // 2 然后, 加载现在要加载的元素在数组中的索引：methodIL.Emit(OpCodes.Ldloc_1);
           // 3 最后，获得数组中的元素：methodIL.Emit(OpCodes.Ldelem_I4); 这里的Ldelem_I4表示加载的数组元素为Int32类型。

           //  二， 取数组的长度使用下面这样的指令流程：
           // l 首先, 加载数组指针到堆栈： methodIL.Emit(OpCodes.Ldarg_0);
           // 2 然后, 加载数组的长度：methodIL.Emit(OpCodes.Ldlen);
           // 3 最后，由于加载的数组长度为无符号整数，所以还需要转换成Int32类型来使用：methodIL.Emit(OpCodes.Conv_I4);

            //定义一个传入参数为Int32[],返回值为INT32的方法
            DynamicMethod forMethod = new DynamicMethod("ForRun", typeof(Int32), new Type[] { typeof(Int32[]) });
            ILGenerator forGenerator = forMethod.GetILGenerator();
            //用来保存求和结果的局部变量
            LocalBuilder sum = forGenerator.DeclareLocal(typeof(Int32)); 
            //循环中使用的局部变量
            LocalBuilder i = forGenerator.DeclareLocal(typeof(Int32));

            Label compareLabel = forGenerator.DefineLabel();
            Label enterLoopLabel = forGenerator.DefineLabel();
            //int sum = 0;
            forGenerator.Emit(OpCodes.Ldc_I4_0);
            forGenerator.Emit(OpCodes.Stloc_0);
            //int i = 0
            forGenerator.Emit(OpCodes.Ldc_I4_0);
            forGenerator.Emit(OpCodes.Stloc_1);
            forGenerator.Emit(OpCodes.Br, compareLabel);

            //定义一个标签，表示从下面开始进入循环体
            forGenerator.MarkLabel(enterLoopLabel);
            //sum += ints[i];
            //其中Ldelem_I4用来加载一个数组中的Int32类型的元素
            forGenerator.Emit(OpCodes.Ldloc_0);
            forGenerator.Emit(OpCodes.Ldarg_0);
            forGenerator.Emit(OpCodes.Ldloc_1);
            forGenerator.Emit(OpCodes.Ldelem_I4);
            forGenerator.Emit(OpCodes.Add);
            forGenerator.Emit(OpCodes.Stloc_0);
            //i++
            forGenerator.Emit(OpCodes.Ldloc_1);
            forGenerator.Emit(OpCodes.Ldc_I4_1);
            forGenerator.Emit(OpCodes.Add);
            forGenerator.Emit(OpCodes.Stloc_1);

            //定义一个标签，表示从下面开始进入循环的比较
            forGenerator.MarkLabel(compareLabel);
            //i < ints.Length
            forGenerator.Emit(OpCodes.Ldloc_1);
            forGenerator.Emit(OpCodes.Ldarg_0);
            forGenerator.Emit(OpCodes.Ldlen);
            forGenerator.Emit(OpCodes.Conv_I4);
            forGenerator.Emit(OpCodes.Clt);
            forGenerator.Emit(OpCodes.Brtrue_S, enterLoopLabel);

            //return sum;
            forGenerator.Emit(OpCodes.Ldloc_0);
            forGenerator.Emit(OpCodes.Ret);

            //完成动态方法的创建，并且获取一个可以执行该动态方法的委托
            ForDelegate forRun = (ForDelegate)forMethod.CreateDelegate(typeof(ForDelegate));

            // 执行动态方法，将在屏幕上打印Hello World!
           int result= forRun(new int[] { 1,2,3,4,5});
            Console.WriteLine(result.ToString());
        }

        public static void ForEachMethod()
        {
            DynamicMethod foreachMethod = new DynamicMethod("ForEachRun", typeof(Int32), new Type[] { typeof(Int32[]) });
            ILGenerator methodIL = foreachMethod.GetILGenerator();
           
            //用来保存求和结果的局部变量
            LocalBuilder sum = methodIL.DeclareLocal(typeof(Int32));
            //foreach 中的 int i 
            LocalBuilder i = methodIL.DeclareLocal(typeof(Int32));
            //用来保存传入的数组
            LocalBuilder ints = methodIL.DeclareLocal(typeof(Int32[]));
            //数组循环用临时变量
            LocalBuilder index = methodIL.DeclareLocal(typeof(Int32));

            Label compareLabel = methodIL.DefineLabel();
            Label enterLoopLabel = methodIL.DefineLabel();

            //首先，它用一个局部变量保存了整个数组，并用它替换了所有原先直接使用数组的地方；
            //最后，它把sum += ints[i];的操作分解成为i = ints[index]和sum += i两个步骤

            //int sum = 0;
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_0);
            //ints = ints
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Stloc_2);
            //int index = 0
            methodIL.Emit(OpCodes.Ldc_I4_0);
            methodIL.Emit(OpCodes.Stloc_3);
            methodIL.Emit(OpCodes.Br, compareLabel);

            //定义一个标签，表示从下面开始进入循环体
            methodIL.MarkLabel(enterLoopLabel);
            //其中Ldelem_I4用来加载一个数组中的Int32类型的元素
            //加载 i = ints[index]
            methodIL.Emit(OpCodes.Ldloc_2);
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldelem_I4);
            methodIL.Emit(OpCodes.Stloc_1);
            //sum += i;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ldloc_1);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_0);

            //index++
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldc_I4_1);
            methodIL.Emit(OpCodes.Add);
            methodIL.Emit(OpCodes.Stloc_3);

            //定义一个标签，表示从下面开始进入循环的比较
            methodIL.MarkLabel(compareLabel);
            //index < ints.Length
            methodIL.Emit(OpCodes.Ldloc_3);
            methodIL.Emit(OpCodes.Ldloc_2);
            methodIL.Emit(OpCodes.Ldlen);
            methodIL.Emit(OpCodes.Conv_I4);
            methodIL.Emit(OpCodes.Clt);
            methodIL.Emit(OpCodes.Brtrue_S, enterLoopLabel);

            //return sum;
            methodIL.Emit(OpCodes.Ldloc_0);
            methodIL.Emit(OpCodes.Ret);

            //完成动态方法的创建，并且获取一个可以执行该动态方法的委托
            ForEachDelegate forRun = (ForEachDelegate)foreachMethod.CreateDelegate(typeof(ForEachDelegate));

            // 执行动态方法，将在屏幕上打印Hello World!
            int result = forRun(new int[] { 1, 2, 3, 4, 5,5 });
            Console.WriteLine(result.ToString());
        }
    }

    class Iterator
    {
        public int ForMethod(int[] ints)
        {
            int sum = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                sum += ints[i];
            }
            return sum;
        }

        public int ForeachMethod(int[] ints)
        {
            int sum = 0;
            foreach (int i in ints)
            {
                sum += i;
            }
            return sum;
        }
    }

}
