using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Lib.Helper;

public static class DummyTextHelper
{
    private static readonly List<string> keywords = ["lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "donec", "mauris", "faucibus", "sed", "ex", "ac", "dictum", "erat", "duis", "vitae", "enim", "in", "hendrerit", "bibendum", "tempor", "lectus", "a", "egestas", "nisl", "tincidunt", "eget", "nulla", "eros", "aliquet", "eu", "tempus", "rutrum", "massa", "nullam", "lacinia", "dui", "placerat", "ornare", "odio", "auctor", "sem", "pretium", "ullamcorper", "metus", "vel", "est", "maecenas", "viverra", "ligula", "ut", "luctus", "augue", "lacus", "pulvinar", "et", "consequat", "arcu", "urna", "vivamus", "feugiat", "ultrices", "risus", "felis", "elementum", "maximus", "convallis", "purus", "suscipit", "nec", "accumsan", "mollis", "velit", "semper", "non", "fringilla", "morbi", "cras", "libero", "euismod", "id", "tellus", "integer", "leo", "porta", "mi", "suspendisse", "sodales", "quisque", "interdum", "posuere", "vestibulum", "ante", "primis", "orci", "cubilia", "curae;", "nunc", "nibh", "diam", "pellentesque", "mattis", "turpis", "at", "condimentum", "iaculis", "sollicitudin", "venenatis", "molestie", "sagittis", "aenean", "quis", "volutpat", "congue", "justo", "proin", "tortor", "magna", "aliquam", "rhoncus", "neque", "nisi", "imperdiet", "praesent", "eleifend", "gravida", "commodo", "sapien", "vehicula", "malesuada", "finibus", "laoreet", "scelerisque", "vulputate", "efficitur", "dignissim", "fermentum", "ultricies", "varius", "porttitor", "pharetra", "curabitur", "blandit", "lobortis", "facilisis", "phasellus", "etiam", "quam", "potenti", "cursus", "dapibus", "fames", "tristique", "fusce", "facilisi", "nam", "habitant", "senectus", "netus", "natoque", "penatibus", "magnis", "dis", "parturient", "montes", "nascetur", "ridiculus", "mus", "hac", "habitasse", "platea", "dictumst", "class", "aptent", "taciti", "sociosqu", "ad", "litora", "torquent", "per", "conubia", "nostra", "inceptos", "himenaeos", "egetdiam", "dolore", "qui", "veritatis", "omnis", "dignissimos", "dolorem", "unde", "impedit", "blanditiis"];

    public static string CleanDummyText(string value, int continueDummyWordCount = 2)
    {
        var textBuilder = new StringBuilder();
        var dummyFragments = new List<string>();
        var originalWordBuilder = new StringBuilder();
        var wordBuilder = new StringBuilder();
        var dummyWordCount = 0;
        for (int i = 0; i < value.Length; i++)
        {
            var item = value[i];
            var isSymbol = IsSymbol(item);
            originalWordBuilder.Append(item);
            if (!isSymbol) wordBuilder.Append(item);

            if (isSymbol || i == value.Length - 1)
            {
                var word = wordBuilder.ToString();
                wordBuilder.Clear();
                var originalWord = originalWordBuilder.ToString();
                originalWordBuilder.Clear();
                var isDummyWord = keywords.Any(a => a.Equals(word, StringComparison.OrdinalIgnoreCase));

                if (!isDummyWord)
                {
                    if (dummyWordCount < continueDummyWordCount)
                    {
                        dummyFragments.ForEach(f => textBuilder.Append(f));
                    }
                    dummyFragments.Clear();
                    dummyWordCount = 0;
                    textBuilder.Append(originalWord);
                }
                else
                {
                    dummyFragments.Add(originalWord);
                    dummyWordCount++;
                }
            }
        }

        if (dummyFragments.Count > 0 && dummyWordCount < continueDummyWordCount)
        {
            dummyFragments.ForEach(f => textBuilder.Append(f));
        }

        return textBuilder.ToString();
    }

    public static bool IsSymbol(char value)
    {
        return char.IsPunctuation(value) || value == ' ' || value == '\n';
    }
}