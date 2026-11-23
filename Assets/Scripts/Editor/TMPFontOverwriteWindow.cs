using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// 焼き直したTMPフォントアセットの中身を、既存のフォントアセットへ上書きするツール
    /// フォントアセットとアトラスのGUID/fileIDが変わらないので、
    /// マテリアルプリセット・プレハブ・シーンの参照を貼り替えずにそのまま使える
    ///
    /// 使い方:
    /// 1. MameFontBakerでフォントを焼く(シーンへの自動適用はオフ推奨)
    /// 2. このツールで「上書き先 = 今使っているフォント」「上書き元 = 焼いたフォント」を指定して実行
    /// </summary>
    public class TMPFontOverwriteWindow : EditorWindow
    {
        private static readonly int s_mainTexId = Shader.PropertyToID("_MainTex");
        private static readonly int s_textureWidthId = Shader.PropertyToID("_TextureWidth");
        private static readonly int s_textureHeightId = Shader.PropertyToID("_TextureHeight");
        private static readonly int s_gradientScaleId = Shader.PropertyToID("_GradientScale");

        [SerializeField] private TMP_FontAsset _target;
        [SerializeField] private TMP_FontAsset _source;
        [SerializeField] private bool _deleteSourceAfterOverwrite = true;

        [MenuItem("Tools/SplashMan/TMPフォント上書き")]
        private static void Open() => GetWindow<TMPFontOverwriteWindow>("TMPフォント上書き");

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "焼き直したフォント(上書き元)の文字とアトラスを、今使っているフォント(上書き先)にコピーします。\n" +
                "上書き先のアセット自体は残るので、マテリアルやプレハブの参照はそのまま使えます。\n" +
                "元に戻せないので、実行前にコミットしておくと安心です。",
                MessageType.Info);

            _target = (TMP_FontAsset)EditorGUILayout.ObjectField("上書き先(使用中)", _target, typeof(TMP_FontAsset), false);
            _source = (TMP_FontAsset)EditorGUILayout.ObjectField("上書き元(焼いたもの)", _source, typeof(TMP_FontAsset), false);
            _deleteSourceAfterOverwrite = EditorGUILayout.ToggleLeft("上書き後に上書き元を削除する", _deleteSourceAfterOverwrite);

            var error = Validate(_target, _source);
            if (error != null) EditorGUILayout.HelpBox(error, MessageType.Warning);

            using (new EditorGUI.DisabledScope(error != null))
            {
                if (GUILayout.Button("上書き実行", GUILayout.Height(32)))
                {
                    Execute();
                }
            }
        }

        private static string Validate(TMP_FontAsset target, TMP_FontAsset source)
        {
            if (target == null || source == null) return "上書き先と上書き元を指定してください";
            if (target == source) return "上書き先と上書き元が同じです";
            if (source.atlasTextures == null || source.atlasTextures.Length == 0) return "上書き元にアトラスがありません";

            foreach (var atlas in source.atlasTextures)
            {
                if (atlas != null && !atlas.isReadable) return $"上書き元のアトラス({atlas.name})が読み取り不可です";
            }
            return null;
        }

        private void Execute()
        {
            var sourcePath = AssetDatabase.GetAssetPath(_source);
            var message = $"「{_source.name}」の内容で「{_target.name}」を上書きします。";
            if (_deleteSourceAfterOverwrite) message += $"\n上書き後、{sourcePath} を削除します。";
            if (!EditorUtility.DisplayDialog("TMPフォント上書き", message, "実行", "キャンセル")) return;

            var updatedMaterials = Overwrite(_target, _source);

            if (_deleteSourceAfterOverwrite)
            {
                AssetDatabase.DeleteAsset(sourcePath);
                _source = null;
            }

            Debug.Log($"TMPフォント上書き: {_target.name} を更新しました(マテリアル {updatedMaterials} 個の設定を更新)", _target);
        }

        /// <summary>
        /// sourceの内容でtargetを上書きする
        /// アトラスとマテリアルはtargetのサブアセットをそのまま使い回し、中身だけ差し替える
        /// </summary>
        /// <returns>設定を更新したマテリアルの数</returns>
        private static int Overwrite(TMP_FontAsset target, TMP_FontAsset source)
        {
            // CopySerializedで上書きされる前に、残したいものを退避しておく
            var targetName = target.name;
            var targetMaterial = target.material;
            var targetFallbacks = target.fallbackFontAssetTable;
            var atlases = CopyAtlases(target, source.atlasTextures);

            EditorUtility.CopySerialized(source, target);

            target.name = targetName;
            target.fallbackFontAssetTable = targetFallbacks;

            // 上書き元のサブアセットを指している参照を、上書き先のサブアセットに戻す
            var serializedObject = new SerializedObject(target);
            var atlasProperty = serializedObject.FindProperty("m_AtlasTextures");
            atlasProperty.arraySize = atlases.Length;
            for (var i = 0; i < atlases.Length; i++)
            {
                atlasProperty.GetArrayElementAtIndex(i).objectReferenceValue = atlases[i];
            }
            serializedObject.FindProperty("m_Material").objectReferenceValue = targetMaterial;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (targetMaterial != null) targetMaterial.SetTexture(s_mainTexId, atlases[0]);
            var updatedMaterials = UpdateMaterials(atlases, target.atlasPadding);

            target.ReadFontAssetDefinition();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

            // 開いているシーンのテキストに再描画させる
            TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, target);
            return updatedMaterials;
        }

        /// <summary>
        /// 上書き元のアトラスの画素を、上書き先のアトラスへコピーする
        /// Texture2Dのオブジェクトは使い回すので、マテリアルからの参照(fileID)は変わらない
        /// </summary>
        private static Texture2D[] CopyAtlases(TMP_FontAsset target, Texture2D[] sourceAtlases)
        {
            var targetAtlases = target.atlasTextures ?? new Texture2D[0];
            var result = new Texture2D[sourceAtlases.Length];

            for (var i = 0; i < sourceAtlases.Length; i++)
            {
                var source = sourceAtlases[i];
                var atlas = i < targetAtlases.Length ? targetAtlases[i] : null;
                if (atlas == null)
                {
                    atlas = new Texture2D(source.width, source.height, source.format, false) { name = $"Atlas {i}" };
                    AssetDatabase.AddObjectToAsset(atlas, target);
                }
                else
                {
                    atlas.Reinitialize(source.width, source.height, source.format, false);
                }

                atlas.filterMode = source.filterMode;
                atlas.wrapMode = source.wrapMode;
                atlas.LoadRawTextureData(source.GetRawTextureData<byte>());
                atlas.Apply(false, false);
                EditorUtility.SetDirty(atlas);
                result[i] = atlas;
            }

            // 上書き元の方がアトラスが少ない場合は、余った分を削除する
            for (var i = sourceAtlases.Length; i < targetAtlases.Length; i++)
            {
                if (targetAtlases[i] == null) continue;
                AssetDatabase.RemoveObjectFromAsset(targetAtlases[i]);
                DestroyImmediate(targetAtlases[i], true);
            }

            return result;
        }

        /// <summary>
        /// アトラスを参照しているマテリアル(プリセット含む)の、アトラスサイズとグラデーション幅を合わせる
        /// TMPのSDFマテリアルは、この値がアトラスとずれると文字の太さや縁が崩れる
        /// </summary>
        private static int UpdateMaterials(Texture2D[] atlases, int atlasPadding)
        {
            var atlasSet = new HashSet<Texture>(atlases);
            var count = 0;

            foreach (var guid in AssetDatabase.FindAssets("t:Material"))
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (material == null || !material.HasProperty(s_mainTexId)) continue;

                var mainTex = material.GetTexture(s_mainTexId);
                if (mainTex == null || !atlasSet.Contains(mainTex)) continue;

                if (material.HasProperty(s_textureWidthId)) material.SetFloat(s_textureWidthId, mainTex.width);
                if (material.HasProperty(s_textureHeightId)) material.SetFloat(s_textureHeightId, mainTex.height);
                if (material.HasProperty(s_gradientScaleId)) material.SetFloat(s_gradientScaleId, atlasPadding + 1);
                EditorUtility.SetDirty(material);
                count++;
            }

            return count;
        }
    }
}
