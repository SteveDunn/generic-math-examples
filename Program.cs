// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GenericMath;

// List<int> numbers4 = new() { 1, 2 };
// Console.WriteLine(numbers4.Sum());


Blah();
Numeral.Create('0');

List<int> ints = new() { 1, 2 };
Console.WriteLine(Sum(ints));

List<decimal> decimals = new() { 1.1m, 2.2m };
Console.WriteLine(Sum(decimals));

List<Numeral> numerals = new() { Numeral.Parse("V", null), Numeral.Parse("IV", null) };
Console.WriteLine(Sum(numerals));

static T Sum<T>(IEnumerable<T> values)
    where T : INumber<T>
{
    T result = T.Zero;

    foreach (var value in values)
    {
        T number = T.Create(value);
        result += number;
    }

    return result;
}



// static T Sum2<T>(IEnumerable<T> values)
//     where T : IAdditionOperators<T>
// {
//     T result = values.First();
//
//     int n = 0;
//     foreach (T value in values)
//     {
//         if (n++ == 0) continue;
//         //T number = T.Create(value);
//         result += value;
//     }
//
//     return result;
// }

static T InvariantParse<T>(string s)
    where T : IParseable<T>
{
    return T.Parse(s, CultureInfo.InvariantCulture);
}
void Blah()
{
    var name = InvariantParse<Name>("Fred");
    var number = InvariantParse<int>("42");
    Console.WriteLine(name);
    Console.WriteLine(number);
}
