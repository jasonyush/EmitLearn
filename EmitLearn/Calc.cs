using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace EmitLearn
{
    public class Calc
    {
        public static void Run()
        {
            string name = "EmitExamples.DynamicCalc";
            //创建程序集
            AssemblyName asmName=new AssemblyName(name);
            AssemblyBuilder asmBuilder=AssemblyBuilder.DefineDynamicAssembly(asmName,AssemblyBuilderAccess.RunAndCollect);
            //定义一个模块
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(name);
            //定义类型
            TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);

            //定义私有字段
            FieldBuilder privateABuilder = typeBuilder.DefineField("_a", typeof(Int32), FieldAttributes.Private);
            FieldBuilder privateBBuilder = typeBuilder.DefineField("_b", typeof(Int32), FieldAttributes.Private);
            privateABuilder.SetConstant(0);
            privateBBuilder.SetConstant(0);

            //定义公有属性
            PropertyBuilder pubBuilderA =
                typeBuilder.DefineProperty("A", PropertyAttributes.None, typeof(Int32), null);
            PropertyBuilder pubBuilderB =
                typeBuilder.DefineProperty("B", PropertyAttributes.None, typeof(Int32), null);

            #region 定义属性A的get和set方法
                // 定义属性A的get方法
            MethodBuilder getPropertyABuilder = typeBuilder.DefineMethod("get",
                        MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                           typeof(Int32),Type.EmptyTypes);

            //生成属性A的get方法的IL代码，即返回私有字段_a
            ILGenerator ilGeneratorA = getPropertyABuilder.GetILGenerator();
            ilGeneratorA.Emit(OpCodes.Ldarg_0);
            ilGeneratorA.Emit(OpCodes.Ldfld,privateABuilder);
            ilGeneratorA.Emit(OpCodes.Ret);

            //定义属性A的set方法
            MethodBuilder setPropertyABuilder = typeBuilder.DefineMethod("set",
                     MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                      null, new Type[] { typeof(Int32) });
            //生成属性A的set方法的IL代码，即设置私有字段_a值为传入的参数1的值
            ILGenerator ilGeneratorASet = setPropertyABuilder.GetILGenerator();
            ilGeneratorASet.Emit(OpCodes.Ldarg_0);
            ilGeneratorASet.Emit(OpCodes.Ldarg_1);
            ilGeneratorASet.Emit(OpCodes.Stfld, privateABuilder);
            ilGeneratorASet.Emit(OpCodes.Ret);
            //设置属性A的get和set方法
            pubBuilderA.SetGetMethod(getPropertyABuilder);
            pubBuilderA.SetSetMethod(setPropertyABuilder);
            #endregion
            #region 定义属性B的get和set方法
            // 定义属性A的get方法
            MethodBuilder getPropertyBBuilder = typeBuilder.DefineMethod("get",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(Int32), Type.EmptyTypes);

            //生成属性A的get方法的IL代码，即返回私有字段_b
            ILGenerator ilGeneratorB = getPropertyBBuilder.GetILGenerator();
            ilGeneratorB.Emit(OpCodes.Ldarg_0);
            ilGeneratorB.Emit(OpCodes.Ldfld, privateBBuilder);
            ilGeneratorB.Emit(OpCodes.Ret);

            //定义属性A的set方法
            MethodBuilder setPropertyBBuilder = typeBuilder.DefineMethod("set",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new Type[] { typeof(Int32) });
            //生成属性B的set方法的IL代码，即设置私有字段_b值为传入的参数1的值
            ILGenerator ilGeneratorBSet = setPropertyBBuilder.GetILGenerator();
            ilGeneratorBSet.Emit(OpCodes.Ldarg_0);
            ilGeneratorBSet.Emit(OpCodes.Ldarg_1);
            ilGeneratorBSet.Emit(OpCodes.Stfld, privateBBuilder);
            ilGeneratorBSet.Emit(OpCodes.Ret);
            //设置属性B的get和set方法
            pubBuilderB.SetGetMethod(getPropertyBBuilder);
            pubBuilderB.SetSetMethod(setPropertyBBuilder);
            #endregion

            //定义构造函数
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.HasThis,
                new Type[] {typeof(Int32), typeof(Int32)});
            ILGenerator constGenerator = constructorBuilder.GetILGenerator();
            //加载参数1填充到私有字段_a
            constGenerator.Emit(OpCodes.Ldarg_0);
            constGenerator.Emit(OpCodes.Ldarg_1);
            constGenerator.Emit(OpCodes.Stfld,privateABuilder);
            //加载参数2天填充到私有字段_b;
            constGenerator.Emit(OpCodes.Ldarg_0);
            constGenerator.Emit(OpCodes.Ldarg_2);
            constGenerator.Emit(OpCodes.Stfld, privateBBuilder);
            constGenerator.Emit(OpCodes.Ret);

            //定义方法
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Run", MethodAttributes.Public, typeof(Int32), Type.EmptyTypes);
            ILGenerator calcIL = methodBuilder.GetILGenerator();

            //加载私有字段_a
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, privateABuilder);
            //加载私有字段_b
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, privateBBuilder);

            //相加并返回结果
            calcIL.Emit(OpCodes.Add);
            calcIL.Emit(OpCodes.Ret);

            Type calcType = typeBuilder.CreateType();
            int a = 1;
            int b = 24;
            object calcObj = Activator.CreateInstance(calcType,new object[]{a,b});
            object result = calcType.GetMethod("Run").Invoke(calcObj, null);
            Console.WriteLine(result.ToString());
        }
    }

    public class Add

    {

        private int _a = 0;

        public int A

        {

            get { return _a; }

            set { _a = value; }

        }



        private int _b = 0;

        public int B

        {

            get { return _b; }

            set { _b = value; }

        }



        public Add(int a, int b)

        {

            _a = a;

            _b = b;

        }



        public int Calc()

        {

            return _a + _b;

        }
    }
}
