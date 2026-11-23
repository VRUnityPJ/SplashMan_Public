using System;

namespace Runtime.Carbonation
{
	/// <summary>
	/// 炭酸ゲージの量を表す値オブジェクト
	/// </summary>
	public readonly record struct Carbonation : IComparable<Carbonation>
	{
		private readonly float _value;

		/// <summary>
		/// 炭酸ゲージの現在値
		/// </summary>
		public float Value => _value;

		public Carbonation(float value)
		{
			if (value < 0)
				throw new ArgumentException("炭酸ゲージは0以上である必要があります", nameof(value));
			_value = value;
		}

		//以下インターフェースの実装や演算子オーバーロードの実装

		/// <summary>
		/// 指定された Carbonation オブジェクトと現在のオブジェクトを比較し、順序を決定する。
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(Carbonation other)
		{
			return _value.CompareTo(other._value);
		}

		/// <summary>
		/// 指定された Carbonation オブジェクト(左側)が小さいかどうかを判断する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator <(Carbonation left, Carbonation right)
		{
			return left.CompareTo(right) < 0;
		}
		/// <summary>
		/// 指定された Carbonation オブジェクト(左側)が小さいか、または等しいかどうかを判断する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator <=(Carbonation left, Carbonation right)
		{
			return left.CompareTo(right) <= 0;
		}
		/// <summary>
		/// 指定された Carbonation オブジェクト(左側)が大きいかどうかを判断する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator >(Carbonation left, Carbonation right)
		{
			return left.CompareTo(right) > 0;
		}
		/// <summary>
		/// 指定された Carbonation オブジェクト(左側)が大きいか、または等しいかどうかを判断する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator >=(Carbonation left, Carbonation right)
		{
			return left.CompareTo(right) >= 0;
		}
		/// <summary>
		/// 指定された Carbonation オブジェクトの値を加算する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Carbonation operator +(Carbonation left, Carbonation right)
		{
			return new Carbonation(left._value + right._value);
		}
		/// <summary>
		/// 指定された Carbonation オブジェクトの値を減算する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Carbonation operator -(Carbonation left, Carbonation right)
		{
			return new Carbonation(left._value - right._value);
		}
		/// <summary>
		/// 指定された Carbonation オブジェクトの値を乗算する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Carbonation operator *(Carbonation left, float right)
		{
			return new Carbonation(left._value * right);
		}
		/// <summary>
		/// 指定された Carbonation オブジェクトの値を除算する。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Carbonation operator /(Carbonation left, float right)
		{
			return new Carbonation(left._value / right);
		}
	}
}