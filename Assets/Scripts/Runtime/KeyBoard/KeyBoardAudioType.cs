namespace Runtime.KeyBoard
{
    /// <summary>
    /// キーボードで使うSEの名前定数。
    /// Runtime.Audio.AudioTypeと同じ役割だが、キーボード側で完結させるためここに置いている。
    /// ここに書いた名前と同じ名前で、SeAudioDataBaseにAudioClipを登録しておく必要がある。
    /// </summary>
    public static class KeyBoardAudioType
    {
        /// <summary>
        /// キー・削除・決定ボタンをクリックしたときの音(各ボタンのインスペクタで個別に上書き可能)
        /// </summary>
        public const string Click = "KeyBoardClickSound";
    }
}
