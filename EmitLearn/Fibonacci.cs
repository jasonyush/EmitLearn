using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace EmitLearn
{
    public class Fibonacci
    {

        public static void CalcRun()
        {
            string name = "EmitExamples.DynamicFibonacci";
            //ring asmFileName = name + ".dll";

            #region Step 1 构建程序集

            //创建程序集名
            AssemblyName asmName = new AssemblyName(name);

            //获取程序集所在的应用程序域
            //你也可以选择用AppDomain.CreateDomain方法创建一个新的应用程序域
            //这里选择当前的应用程序域
            //AppDomain domain = AppDomain.CurrentDomain;

            //实例化一个AssemblyBuilder对象来实现动态程序集的构建
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);

            #endregion

            #region Step 2 定义模块

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name);


            #endregion

            #region Step 3 定义类型

            TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);

            #endregion

            #region Step 4 定义方法

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "Calc",
                MethodAttributes.Public,
                typeof(Int32),
                new Type[] { typeof(Int32) });

            #endregion

            #region Step 5 实现方法

            ILGenerator calcIL = methodBuilder.GetILGenerator();

            //定义标签lbReturn1，用来设置返回值为1
            Label lbReturn1 = calcIL.DefineLabel();
            //定义标签lbReturnResutl，用来返回最终结果
            Label lbReturnResutl = calcIL.DefineLabel();

            //加载参数1，和整数1，相比较，如果相等则设置返回值为1
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_1);
            calcIL.Emit(OpCodes.Beq_S, lbReturn1);

            //加载参数1，和整数2，相比较，如果相等则设置返回值为1
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_2);
            calcIL.Emit(OpCodes.Beq_S, lbReturn1);

            //加载参数0和1，将参数1减去1，递归调用自身
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_1);
            calcIL.Emit(OpCodes.Sub);
            calcIL.Emit(OpCodes.Call, methodBuilder);

            //加载参数0和1，将参数1减去2，递归调用自身
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldarg_1);
            calcIL.Emit(OpCodes.Ldc_I4_2);
            calcIL.Emit(OpCodes.Sub);
            calcIL.Emit(OpCodes.Call, methodBuilder);

            //将递归调用的结果相加，并返回
            calcIL.Emit(OpCodes.Add);
            calcIL.Emit(OpCodes.Br, lbReturnResutl);

            //在这里创建标签lbReturn1
            calcIL.MarkLabel(lbReturn1);
            calcIL.Emit(OpCodes.Ldc_I4_1);

            //在这里创建标签lbReturnResutl
            calcIL.MarkLabel(lbReturnResutl);
            calcIL.Emit(OpCodes.Ret);

            #endregion

            #region Step 6 收获

            Type type = typeBuilder.CreateType();

           // assemblyBuilder.Save(asmFileName);

            object ob = Activator.CreateInstance(type);

            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine(type.GetMethod("Calc").Invoke(ob, new object[] { i }));
            }
            #endregion
        }


        public int Calc(int num)
        {
            if (num == 1 || num == 2)
            {
                return 1;
            }
            else
            {
                return Calc(num - 1) + Calc(num - 2);
            }
        }
    }
}
