using System.Collections.Generic;

namespace Main.Data.Formula
{
    internal sealed class IntStr
    {
        private int _int;
        internal int Int => _int;

        private string _str;
        internal string Str => _str;

        internal IntStr(int _int)
        {
            this._int = _int;
        }

        internal IntStr(string _str)
        {
            this._str = _str;
        }

        internal IntStr(int _int, string _str)
        {
            this._int = _int;
            this._str = _str;
        }
    }

    internal sealed class FltStr
    {
        private float _flt;
        internal float Flt => _flt;

        private string _str;
        internal string Str => _str;

        internal FltStr(float _flt)
        {
            this._flt = _flt;
        }

        internal FltStr(string _str)
        {
            this._str = _str;
        }

        internal FltStr(float _flt, string _str)
        {
            this._flt = _flt;
            this._str = _str;
        }
    }

    internal static class Symbol
    {
        internal static readonly IntStr N0 = new(0);
        internal static readonly IntStr N1 = new(1);
        internal static readonly IntStr N2 = new(2);
        internal static readonly IntStr N3 = new(3);
        internal static readonly IntStr N4 = new(4);
        internal static readonly IntStr N5 = new(5);
        internal static readonly IntStr N6 = new(6);
        internal static readonly IntStr N7 = new(7);
        internal static readonly IntStr N8 = new(8);
        internal static readonly IntStr N9 = new(9);
        internal static readonly IntStr OA = new("+");
        internal static readonly IntStr OS = new("-");
        internal static readonly IntStr OM = new("*");
        internal static readonly IntStr OD = new("/");
        internal static readonly IntStr PL = new("(");
        internal static readonly IntStr PR = new(")");

        internal static bool? IsNumber(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return true;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return false;
            }
            else if (symbol == PL || symbol == PR)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        internal static bool? IsOperator(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return false;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return true;
            }
            else if (symbol == PL || symbol == PR)
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        internal static bool? IsParagraph(IntStr symbol)
        {
            if (symbol == N0 || symbol == N1 || symbol == N2 || symbol == N3 || symbol == N4 ||
                symbol == N5 || symbol == N6 || symbol == N7 || symbol == N8 || symbol == N9)
            {
                return false;
            }
            else if (symbol == OA || symbol == OS || symbol == OM || symbol == OD)
            {
                return false;
            }
            else if (symbol == PL || symbol == PR)
            {
                return true;
            }
            else
            {
                return null;
            }
        }
    }

    internal sealed class Formula
    {
        internal List<IntStr> Data { get; set; }

        internal Formula()
        {
            Data = new();
        }

        internal Formula(IEnumerable<IntStr> symbols)
        {
            Data = symbols.ToList();
        }

        internal Formula(params IntStr[] symbols)
        {
            Data = new(symbols);
        }

        internal void Reset()
        {
            Data.Clear();
        }

        internal float? Calcurate()
        {
            try
            {
                if (!IsListOK() || !IsParagraphOK() || !IsOperatorOK()) throw new System.Exception("不正な形式です");

                return
                    UnityEngine.Mathf.Clamp(Data.ConnectNumbers().Convert().Calcurate(), short.MinValue, short.MaxValue);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// リストがnullでないか、リストの要素数が0でないか
        /// </summary>
        private bool IsListOK()
        {
            if (Data == null) return false;
            if (Data.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// 演算子が端になく、(数字の数) - (演算子の数) = 1 であるか
        /// かっこを無視する時、演算子が2個連続していないか
        /// </summary>
        private bool IsOperatorOK()
        {
            if (Symbol.IsOperator(Data[0]) == true) return false;
            if (Symbol.IsOperator(Data[^1]) == true) return false;

            int n = 0;
            foreach (var e in Data)
            {
                if (Symbol.IsNumber(e) == true) n++;
                else if (Symbol.IsOperator(e) == true) n--;
            }
            if (n != 1) return false;

            for (int i = 0; i < Data.Count; i++)
            {
                if (Symbol.IsOperator(Data[i]) == true)
                {
                    for (int j = i + 1; j < Data.Count; j++)
                    {
                        if (Symbol.IsNumber(Data[j]) == true) break;
                        else if (Symbol.IsParagraph(Data[j]) == true) continue;
                        else if (Symbol.IsOperator(Data[j]) == true) return false;
                        else return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// ()が全て対応しているか、またこの順番であるか
        /// ()の中に1つ以上の数字が入っているか
        /// </summary>
        private bool IsParagraphOK()
        {
            int n = 0;
            foreach (var e in Data)
            {
                if (e.Str == Symbol.PL.Str) n++;
                else if (e.Str == Symbol.PR.Str) n--;

                if (n < 0) return false;
            }
            if (n != 0) return false;

            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].Str == Symbol.PL.Str)
                {
                    int j = i + 1;
                    while (j < Data.Count)
                    {
                        if (Data[j].Str == Symbol.PR.Str) break;
                        j++;
                    }

                    if (Data.GetRange(i, j - i + 1).All(e => Symbol.IsNumber(e) != true)) return false;
                }
            }

            return true;
        }
    }

    internal static class Ex
    {
        /// <summary>
        /// 数字を結合して、新しいリストとして返す
        /// </summary>
        internal static List<IntStr> ConnectNumbers(this List<IntStr> list)
        {
            List<IntStr> ret = new();

            for (int i = 0; i < list.Count; i++)
            {
                // 今調べている要素が数字でないなら、結合しない
                IntStr s = list[i];
                if (!s.IsNumber()) { ret.Add(s); continue; }

                // 先の要素を順に見ていき、数字を結合していく
                int n = s.Int;
                for (int j = i + 1; j < list.Count; i++, j++)
                {
                    IntStr _s = list[j];
                    if (!_s.IsNumber()) break;
                    else n = n * 10 + _s.Int;
                }
                ret.Add(new(n));
            }

            return ret;
        }

        /// <summary>
        /// IntStrをFltStrに変換する(リスト)
        /// </summary>
        internal static List<FltStr> Convert(this List<IntStr> intStr)
        {
            return intStr.Map(e => e.Convert()).ToList();
        }

        /// <summary>
        /// 式を計算する
        /// </summary>
        internal static float Calcurate(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // かっこを無くす
            int i = 0, n = 0;
            // 左から見て"("を探す
            while (i < _list.Count)
            {
                if (_list[i].Str != Symbol.PL.Str) { i++; continue; }

                // 右から見て")"を探す
                for (int j = _list.Count - 1; i < j; j--)
                {
                    if (_list[j].Str != Symbol.PR.Str) continue;

                    // "()"の間を再帰的に計算し、_listを更新する
                    float val = _list.GetRange(i + 1, (j - 1) - (i + 1) + 1).Calcurate();
                    _list.RemoveRange(i, j - i + 1);
                    _list.Insert(i, new(val));
                    break;
                }

                if (++n >= byte.MaxValue) throw new System.Exception("無限ループの可能性があります");
            }

            // かっこが無くなった(あるいはそもそも、かっこが無かった)ので、四則演算を行う
            return _list.CalcurateRaw();
        }

        /// <summary>
        /// かっこが無い前提で、式を計算する
        /// </summary>
        private static float CalcurateRaw(this List<FltStr> list)
        {
            List<FltStr> _list = new(list);

            // 乗除
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Str == Symbol.OM.Str)
                {
                    (_list[i - 1].Flt * _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i].Str == Symbol.OD.Str)
                {
                    if (_list[i + 1].Flt == 0) throw new System.Exception("0除算");
                    (_list[i - 1].Flt / _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            // 加減
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Str == Symbol.OA.Str)
                {
                    (_list[i - 1].Flt + _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
                else if (_list[i].Str == Symbol.OS.Str)
                {
                    (_list[i - 1].Flt - _list[i + 1].Flt).ReplaceAroundOperator(ref _list, ref i);
                }
            }

            return _list[0].Flt;
        }

        /// <summary>
        /// リストのインデックス番目の要素について、前後を含めた3つを消し、与えられた値に置き換える。インデックス番号も補正する。
        /// </summary>
        private static void ReplaceAroundOperator(this float val, ref List<FltStr> list, ref int operatorIndex)
        {
            list.RemoveRange(operatorIndex - 1, 3);
            list.Insert(operatorIndex - 1, new(val));
            operatorIndex--;
        }

        /// <summary>
        /// IntStrをFltStrに変換する
        /// </summary>
        private static FltStr Convert(this IntStr intStr)
        {
            return new(intStr.Int, intStr.Str);
        }

        /// <summary>
        /// 数字かどうか判定する
        /// </summary>
        private static bool IsNumber(this IntStr var)
        {
            bool? isNum = Symbol.IsNumber(var);
            return isNum.HasValue && isNum.Value;
        }

        /// <summary>
        /// 範囲内に含まれているか？
        /// </summary>
        internal static bool IsIn(this int val, int min, int max)
        {
            return min <= val && val <= max;
        }
    }

    internal static class Iterator
    {
        internal static IEnumerable<T2> Map<T1, T2>(this IEnumerable<T1> itr, System.Func<T1, T2> f)
        {
            foreach (T1 e in itr)
            {
                yield return f(e);
            }
        }

        internal static List<T> ToList<T>(this IEnumerable<T> itr)
        {
            return new(itr);
        }

        internal static bool All<T>(this IEnumerable<T> itr, System.Func<T, bool> f)
        {
            foreach (T e in itr)
            {
                if (!f(e)) return false;
            }
            return true;
        }

        internal static bool Any<T>(this IEnumerable<T> itr, System.Func<T, bool> f)
        {
            foreach (T e in itr)
            {
                if (f(e)) return true;
            }
            return false;
        }

#if UNITY_EDITOR && false
        internal static T Show<T>(this T val)
        {
            UnityEngine.Debug.Log(val);
            return val;
        }

        internal static void Look(this IEnumerable<object> itr)
        {
            foreach (object e in itr)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        internal static void Look<T1, T2>(this IEnumerable<T1> itr, System.Func<T1, T2> f)
        {
            foreach (T1 e in itr)
            {
                UnityEngine.Debug.Log(f(e));
            }
        }
#endif
    }
}