using System;

namespace Runtime.Scores
{
    /// <summary>
    /// スコアの値を表す構造体.
    /// </summary>
    public readonly record struct Score : IComparable<Score>
    {
        private readonly int _value;
        public static readonly Score Zero = new(0);

        public int Value => _value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        public Score(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "スコアは負の値にできません");
            _value = value;
        }

        public int CompareTo(Score other)
        {
            return _value.CompareTo(other._value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static bool operator <(Score left, Score right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Score left, Score right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Score left, Score right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Score left, Score right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static Score operator +(Score left, Score right)
        {
            return new Score(checked(left._value + right._value));
        }

        public static Score operator -(Score left, Score right)
        {
            return new Score(checked(left._value - right._value));
        }
    }
}
