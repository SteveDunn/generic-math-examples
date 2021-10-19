// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace GenericMath
{
    public static class Extensions
    {
        public static T SumNoNo<T>(this IEnumerable<T> source) {
            T sum = default;
                foreach (T v in source) {
                    sum += v;
                }
            return sum;
        }
    }

    // public interface IMishMash<TSelf>
    //     where TSelf : IParseable<TSelf>
    // {
    //     static abstract TSelf Parse(string s);
    //     static abstract TSelf Add(string s);
    // }

    public class Name : IParseable<Name>
    {
        private readonly string _value;

        private Name(string s)
        {
            _value = s;
        }

        public static Name Parse(string s, IFormatProvider? provider) => new Name(s);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Name result)
        {
            result = new Name(s!);
            return true;
        }

        public override string ToString() => _value;
    }

    public readonly struct Numeral : INumber<Numeral>
    {
        private static Dictionary<char, int> RomanMap = new Dictionary<char, int>()
        {
            {'I', 1},
            {'V', 5},
            {'X', 10},
            {'L', 50},
            {'C', 100},
            {'D', 500},
            {'M', 1000}
        };

        private readonly int _value;

        public Numeral(int value) => _value = value;

        private Numeral(Numeral value) : this(value._value)
        {}

        public override int GetHashCode()
        {
            return _value;
        }

        public static Numeral One => new(1);

        public static Numeral Zero => new(0);

        public static Numeral AdditiveIdentity => throw new NotImplementedException();

        public static Numeral MultiplicativeIdentity => throw new NotImplementedException();

        public int CompareTo(object? obj)
        {
            return _value.CompareTo(obj);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return IntToRoman(_value);
        }

        private static int RomanToInt(string roman)
        {
            int number = 0;
            for (int i = 0; i < roman.Length; i++)
            {
                if (i + 1 < roman.Length && RomanMap[roman[i]] < RomanMap[roman[i + 1]])
                {
                    number -= RomanMap[roman[i]];
                }
                else
                {
                    number += RomanMap[roman[i]];
                }
            }
            return number;
        }

        private static string IntToRoman(int number)
        {
            if (number is < 0 or > 3999)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "insert value between 1 and 3999");
            }
            if (number < 1) return string.Empty;            
            if (number >= 1000) return "M" + IntToRoman(number - 1000);
            if (number >= 900) return "CM" + IntToRoman(number - 900); 
            if (number >= 500) return "D" + IntToRoman(number - 500);
            if (number >= 400) return "CD" + IntToRoman(number - 400);
            if (number >= 100) return "C" + IntToRoman(number - 100);            
            if (number >= 90) return "XC" + IntToRoman(number - 90);
            if (number >= 50) return "L" + IntToRoman(number - 50);
            if (number >= 40) return "XL" + IntToRoman(number - 40);
            if (number >= 10) return "X" + IntToRoman(number - 10);
            if (number >= 9) return "IX" + IntToRoman(number - 9);
            if (number >= 5) return "V" + IntToRoman(number - 5);
            if (number >= 4) return "IV" + IntToRoman(number - 4);
            return "I" + IntToRoman(number - 1);
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            return _value.TryFormat(destination, out charsWritten, format, provider);
        }

        public int CompareTo(Numeral other) => _value.CompareTo(other._value);

        public bool Equals(Numeral other) => _value.Equals(other._value);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is Numeral))
            {
                return false;
            }
            return _value == ((Numeral)obj)._value;
        }


        public static Numeral Abs(Numeral value) => Create(Math.Abs(value._value));

        public static Numeral Clamp(Numeral value, Numeral min, Numeral max)
            => Create(Math.Clamp(value._value, min._value, max._value));

        public static TSelf Create<TSelf, TOther>(TOther input)
            where TSelf : INumber<TSelf>
            where TOther : INumber<TOther>
        {
            return TSelf.Create<TOther>(input);
        }

        public static Numeral Create<TOther>(TOther value) where TOther : INumber<TOther>
        {
            if (typeof(TOther) == typeof(Numeral))
            {
                return new(((Numeral)(object)value)._value);
            }
            
            return new(Create<int, TOther>(value));
        }

        public static Numeral CreateSaturating<TOther>(TOther value) where TOther : INumber<TOther> => Create(value);

        public static Numeral CreateTruncating<TOther>(TOther value) where TOther : INumber<TOther> => Create(value);

        public static (Numeral Quotient, Numeral Remainder) DivRem(Numeral left, Numeral right)
        {
            var r = Math.DivRem(left._value, right._value);
            return (new(r.Quotient), new(r.Remainder));
        }

        public static Numeral Max(Numeral x, Numeral y) => new(Math.Max(x._value, y._value));

        public static Numeral Min(Numeral x, Numeral y) => new(Math.Min(x._value, y._value));

        public static Numeral Parse(string s, NumberStyles style, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public static Numeral Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public static Numeral Sign(Numeral value) => Create(Math.Sign(value._value));

        public static bool TryCreate<TOther>(TOther value, out Numeral result) where TOther : INumber<TOther>
        {
            if (typeof(TOther) == typeof(byte))
            {
                result = new((byte)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(char))
            {
                result = new((char)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(decimal))
            {
                var actualValue = (decimal)(object)value;

                if ((actualValue < MinValue) || (actualValue > MaxValue))
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(double))
            {
                var actualValue = (double)(object)value;

                if ((actualValue < MinValue) || (actualValue > MaxValue))
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(short))
            {
                result = new((short)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(int))
            {
                result = new((int)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(long))
            {
                var actualValue = (long)(object)value;

                if ((actualValue < MinValue) || (actualValue > MaxValue))
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(nint))
            {
                var actualValue = (nint)(object)value;

                if ((actualValue < MinValue) || (actualValue > MaxValue))
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(sbyte))
            {
                result = new((sbyte)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(float))
            {
                var actualValue = (float)(object)value;

                if ((actualValue < MinValue) || (actualValue > MaxValue))
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(ushort))
            {
                result = new((ushort)(object)value);
                return true;
            }
            else if (typeof(TOther) == typeof(uint))
            {
                var actualValue = (uint)(object)value;

                if (actualValue > MaxValue)
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(ulong))
            {
                var actualValue = (ulong)(object)value;

                if (actualValue > MaxValue)
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else if (typeof(TOther) == typeof(nuint))
            {
                var actualValue = (nuint)(object)value;

                if (actualValue > MaxValue)
                {
                    result = default;
                    return false;
                }

                result = new((int)actualValue);
                return true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public const int MaxValue = 3_999;
        public const int MinValue = 1;

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Numeral result)
        {
            result = new(0);
            
            if (s is null)
            {
                return false;
            }

            var v = Parse(s, style, provider);
            result = v;
            return true;
        }

        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Numeral result)
        {
            var v = Parse(s, style, provider);
            result = v;
            return true;
        }

        public static Numeral Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            return Parse(s, provider);
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Numeral result)
        {
            var v = Parse(s, provider);
            result = v;
            return true;
        }

        public static Numeral Parse(string s, IFormatProvider? provider)
        {
            return new(RomanToInt(s));
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Numeral result)
        {
            if (s == null)
            {
                result = new(0);
                return false;
            }

            result = new(RomanToInt(s));
            
            return true;
        }

        public static Numeral operator +(Numeral left, Numeral right) => Create(left._value + right._value);

        public static bool operator <(Numeral left, Numeral right) => left._value < right._value;

        public static bool operator <=(Numeral left, Numeral right) => left._value <= right._value;

        public static bool operator >(Numeral left, Numeral right) => left._value > right._value;

        public static bool operator >=(Numeral left, Numeral right) => left._value >= right._value;

        public static bool operator ==(Numeral left, Numeral right) => left._value == right._value;

        public static bool operator !=(Numeral left, Numeral right)  => left._value != right._value;

        public static Numeral operator --(Numeral value) => new(value._value-1);

        public static Numeral operator /(Numeral left, Numeral right)  => new(left._value / right._value);

        public static Numeral operator ++(Numeral value) => new(value._value+1);

        public static Numeral operator %(Numeral left, Numeral right) => new(left._value % right._value);

        public static Numeral operator *(Numeral left, Numeral right) => new(left._value * right._value);

        public static Numeral operator -(Numeral left, Numeral right) => new(left._value - right._value);

        public static Numeral operator -(Numeral value) => new(-(value._value));

        public static Numeral operator +(Numeral value) => new(+(value._value));
    }
}
