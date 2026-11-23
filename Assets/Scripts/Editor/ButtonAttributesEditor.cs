using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonAttributeEditor : UnityEditor.Editor
    {
        private readonly Dictionary<MethodInfo, object[]> _arguments =
            new Dictionary<MethodInfo, object[]>();

        public override void OnInspectorGUI()
        {
            // 通常のInspectorを表示
            DrawDefaultInspector();

            UnityEditor.EditorGUILayout.Space(10);

            Type targetType = target.GetType();

            MethodInfo[] methods = targetType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly
            );

            foreach (MethodInfo method in methods)
            {
                ButtonAttribute button =
                    (ButtonAttribute)Attribute.GetCustomAttribute(
                        method,
                        typeof(ButtonAttribute)
                    );

                if (button == null)
                {
                    continue;
                }

                DrawButton(method, button);
            }
        }

        private void DrawButton(
            MethodInfo method,
            ButtonAttribute attribute)
        {
            ParameterInfo[] parameters = method.GetParameters();

            var label = attribute.Label;

            if (string.IsNullOrEmpty(label))
            {
                label = UnityEditor.ObjectNames.NicifyVariableName(
                    method.Name
                );
            }

            UnityEditor.EditorGUILayout.BeginVertical("box");

            // 引数がある場合
            if (parameters.Length > 0)
            {
                UnityEditor.EditorGUILayout.LabelField(
                    label,
                    UnityEditor.EditorStyles.boldLabel
                );

                if (!_arguments.TryGetValue(method, out var values) ||
                    values.Length != parameters.Length)
                {
                    values = CreateDefaultArguments(parameters);
                    _arguments[method] = values;
                }

                for (var i = 0; i < parameters.Length; i++)
                {
                    values[i] = DrawParameter(
                        parameters[i],
                        values[i]
                    );
                }
            }

            UnityEditor.EditorGUILayout.Space(3);

            // 実行ボタン
            if (GUILayout.Button(label))
            {
                if (!_arguments.TryGetValue(method, out var values))
                {
                    values = CreateDefaultArguments(parameters);
                }

                ExecuteMethod(method, values);
            }

            UnityEditor.EditorGUILayout.EndVertical();

            UnityEditor.EditorGUILayout.Space(5);
        }

        private object[] CreateDefaultArguments(
            ParameterInfo[] parameters)
        {
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                Type type = parameters[i].ParameterType;

                if (type.IsValueType)
                {
                    values[i] = Activator.CreateInstance(type);
                }
                else
                {
                    values[i] = null;
                }
            }

            return values;
        }

        private object DrawParameter(ParameterInfo parameter, object value) =>
            DrawParameter(
                parameter.ParameterType,
                parameter.Name,
                value
            );

        private object DrawParameter(
            Type type,
            string parameterName,
            object value)
        {
            var displayName =
                UnityEditor.ObjectNames.NicifyVariableName(
                    parameterName
                );

            // =========================================================
            // int
            // =========================================================
            if (type == typeof(int))
            {
                var current = value != null
                    ? (int)value
                    : 0;

                return UnityEditor.EditorGUILayout.IntField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // float
            // =========================================================
            if (type == typeof(float))
            {
                var current = value != null
                    ? (float)value
                    : 0f;

                return UnityEditor.EditorGUILayout.FloatField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // double
            // =========================================================
            if (type == typeof(double))
            {
                var current = value != null
                    ? (double)value
                    : 0d;

                return UnityEditor.EditorGUILayout.DoubleField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // long
            // =========================================================
            if (type == typeof(long))
            {
                var current = value != null
                    ? (long)value
                    : 0L;

                return UnityEditor.EditorGUILayout.LongField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // bool
            // =========================================================
            if (type == typeof(bool))
            {
                var current =
                    value != null && (bool)value;

                return UnityEditor.EditorGUILayout.Toggle(
                    displayName,
                    current
                );
            }

            // =========================================================
            // string
            // =========================================================
            if (type == typeof(string))
            {
                var current = value as string;

                if (current == null)
                {
                    current = "";
                }

                return UnityEditor.EditorGUILayout.TextField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // enum
            // =========================================================
            if (type.IsEnum)
            {
                Enum current;

                if (value != null)
                {
                    current = (Enum)value;
                }
                else
                {
                    Array enumValues = Enum.GetValues(type);

                    current =
                        (Enum)enumValues.GetValue(0);
                }

                return UnityEditor.EditorGUILayout.EnumPopup(
                    displayName,
                    current
                );
            }

            // =========================================================
            // Vector2
            // =========================================================
            if (type == typeof(Vector2))
            {
                Vector2 current = value != null
                    ? (Vector2)value
                    : Vector2.zero;

                return UnityEditor.EditorGUILayout.Vector2Field(
                    displayName,
                    current
                );
            }

            // =========================================================
            // Vector3
            // =========================================================
            if (type == typeof(Vector3))
            {
                Vector3 current = value != null
                    ? (Vector3)value
                    : Vector3.zero;

                return UnityEditor.EditorGUILayout.Vector3Field(
                    displayName,
                    current
                );
            }

            // =========================================================
            // Vector4
            // =========================================================
            if (type == typeof(Vector4))
            {
                Vector4 current = value != null
                    ? (Vector4)value
                    : Vector4.zero;

                return UnityEditor.EditorGUILayout.Vector4Field(
                    displayName,
                    current
                );
            }

            // =========================================================
            // Color
            // =========================================================
            if (type == typeof(Color))
            {
                Color current = value != null
                    ? (Color)value
                    : Color.white;

                return UnityEditor.EditorGUILayout.ColorField(
                    displayName,
                    current
                );
            }

            // =========================================================
            // UnityEngine.Object
            // =========================================================
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                UnityEngine.Object current =
                    value as UnityEngine.Object;

                return UnityEditor.EditorGUILayout.ObjectField(
                    displayName,
                    current,
                    type,
                    true
                );
            }

            // =========================================================
            // ユーザー定義型
            // =========================================================
            return DrawCustomType(
                type,
                displayName,
                value
            );
        }

        private object DrawCustomType(
            Type type,
            string displayName,
            object currentValue)
        {
            // ---------------------------------------------------------
            // 1引数のpublicコンストラクタを探す
            // ---------------------------------------------------------
            ConstructorInfo[] constructors =
                type.GetConstructors(
                    BindingFlags.Public |
                    BindingFlags.Instance
                );

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters =
                    constructor.GetParameters();

                if (parameters.Length != 1)
                {
                    continue;
                }

                Type argumentType =
                    parameters[0].ParameterType;

                // -----------------------------------------------------
                // Carbonation(float) のような型
                // -----------------------------------------------------
                if (argumentType == typeof(float))
                {
                    var current = 0f;

                    // 既存の値からValueを取得できる場合
                    if (currentValue != null)
                    {
                        PropertyInfo valueProperty =
                            type.GetProperty("Value");

                        if (valueProperty != null &&
                            valueProperty.PropertyType == typeof(float))
                        {
                            var propertyValue =
                                valueProperty.GetValue(
                                    currentValue,
                                    null
                                );

                            if (propertyValue != null)
                            {
                                current = (float)propertyValue;
                            }
                        }
                    }

                    var input =
                        UnityEditor.EditorGUILayout.FloatField(
                            displayName,
                            current
                        );

                    // new Carbonation(input)
                    return constructor.Invoke(
                        new object[] { input }
                    );
                }

                // -----------------------------------------------------
                // intを受け取るユーザー定義型
                // -----------------------------------------------------
                if (argumentType == typeof(int))
                {
                    var current = 0;

                    var input =
                        UnityEditor.EditorGUILayout.IntField(
                            displayName,
                            current
                        );

                    return constructor.Invoke(
                        new object[] { input }
                    );
                }

                // -----------------------------------------------------
                // doubleを受け取るユーザー定義型
                // -----------------------------------------------------
                if (argumentType == typeof(double))
                {
                    var current = 0d;

                    var input =
                        UnityEditor.EditorGUILayout.DoubleField(
                            displayName,
                            current
                        );

                    return constructor.Invoke(
                        new object[] { input }
                    );
                }

                // -----------------------------------------------------
                // boolを受け取るユーザー定義型
                // -----------------------------------------------------
                if (argumentType == typeof(bool))
                {
                    var current = false;

                    var input =
                        UnityEditor.EditorGUILayout.Toggle(
                            displayName,
                            current
                        );

                    return constructor.Invoke(
                        new object[] { input }
                    );
                }

                // -----------------------------------------------------
                // stringを受け取るユーザー定義型
                // -----------------------------------------------------
                if (argumentType == typeof(string))
                {
                    var current = "";

                    var input =
                        UnityEditor.EditorGUILayout.TextField(
                            displayName,
                            current
                        );

                    return constructor.Invoke(
                        new object[] { input }
                    );
                }
            }

            // ---------------------------------------------------------
            // 対応できない場合
            // ---------------------------------------------------------
            UnityEditor.EditorGUILayout.HelpBox(
                "未対応の引数型: " + type.FullName,
                UnityEditor.MessageType.Warning
            );

            return currentValue;
        }

        private void ExecuteMethod(
            MethodInfo method,
            object[] arguments)
        {
            try
            {
                method.Invoke(
                    target,
                    arguments
                );

                UnityEditor.EditorUtility.SetDirty(target);
            }
            catch (Exception e)
            {
                Debug.LogException(e, target);
            }
        }
    }
}