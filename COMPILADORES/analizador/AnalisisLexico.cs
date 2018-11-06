using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace COMPILADORES.analizador
{
    class AnalisisLexico
    {
        public List<KeyValuePair<int, string>> getTokens(string texto)
        {
            //Console.WriteLine("asdf entro");
            string pattern = (@"\b(create|drop|database|table|primary|key|foreign|null|default|not|add|column|insert|into|update|delete|where|order|by|from|select|as|desc|asc|values|set)\b|\b(int|char|varchar|decimal|float|date|datetime|text)\b|([0-9]+)|\b([a-zA-Z0-9-_]+)\b|([(|)])|(;)|(\*)|(,)|(')|([>|<|==|>=|<=|!=|=]+)|(.)");
            //MatchCollection matches = Regex.Matches(texto, pattern, options);
            // Instantiate the regular expression object.
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            List<KeyValuePair<int, string>> tokens = new List<KeyValuePair<int,string>>();
            // Match the regular expression pattern against a text string.
            Match m = r.Match(texto);
            int matchCount = 0;
            while (m.Success)
            {
                Console.WriteLine("Match" + (++matchCount));
                for (int i = 1; i <= 9; i++)
                {
                    Group g = m.Groups[i];
                    if (g.ToString() != "")
                    {
                        tokens.Add(new KeyValuePair<int, string>(i, g.ToString()));
                        Console.WriteLine("Group" + i + "='" + g + "'");
                    }
                }
                m = m.NextMatch();
            }
            Console.WriteLine();
            //foreach (KeyValuePair<int, string> token in tokens)
            //    Console.WriteLine("token: " + token.Value + " - value: "+ token.Key);
            return tokens;
            /*
             * Groups
             * 
             * 1.  Palabra reservada
             * 2. Tipo de dato
             * 3.  Numero (tamaño etc)
             * 4.  Campoo o nombre de tabla
             * 5. Parentesis
             * 6.  Punto y coma
             * 7.  Asterisco
             * 8.  Coma
             * 9. Comilla simple
             * 10.  Operador
             * 11. Todo lo demas esta mal
             * 
            }*/
        }
    }
}
