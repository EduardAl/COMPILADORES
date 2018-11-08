using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace COMPILADORES
{
   class Analisis
    {
        string consultaEjecutar; // Sera la consulta a ejecutar si todo sale bien
        ArrayList definitionTokens, manipulationTokens, objectTokens, dataTypeTokens, helpersDMLTokens, conditionsDMLTokens;
        ArrayList patrones;
        Regex expIdentificador, expEntero, expDecimal, expTexto, expFecha, expOtros; // Expresiones regulares
        int[] tipoToken = new int[1000]; // Arreglo que almacena tipos de tokens
        char[] delimitadoresToken = { ' ', ',', '"', '\t', '\r', '\n' }; // Separadores de tokens
        //char[] delimitadorSentencia = { ';' }; // Separadores de sentencias
        Match exp;

        public Analisis()
        {
            // Inicializacion de variables
            definitionTokens = new ArrayList() { "CREATE", "create", "DROP", "drop" };
            manipulationTokens = new ArrayList() { "INSERT", "insert", "SELECT", "select", "UPDATE", "update", "DELETE", "delete" };
            objectTokens = new ArrayList() { "DATABASE", "database", "TABLE", "table" };
            dataTypeTokens = new ArrayList() { "INTEGER", "integer", "DECIMAL", "decimal", "VARCHAR", "varchar", "DATE", "date","int", "INT" };
            helpersDMLTokens = new ArrayList() { "INTO", "into", "VALUES", "values", "SET", "set", "FROM", "from", "WHERE", "where" };
            conditionsDMLTokens = new ArrayList() { "LIKE", "like", "BETWEEN", "between" };
            patrones = new ArrayList();
            crearPatronesTokens();
            expIdentificador = new Regex("^[a-zA-Z]{1}([a-zA-Z0-9_]+)?$"); // Para validar nombres de bases de datos, tablas y campos
            expEntero = new Regex("^[0-9]+$"); // Validar valores enteros
            expDecimal = new Regex("^[0-9]+(\\.[0-9]+)?$"); // Valida valores decimales
            expTexto = new Regex("^'.*'$"); // Valida texto
            expFecha = new Regex("^'([1|2][0-9]{3})-([0][1-9]|[1][0-2])-([0][1-9]|[12][0-9]|3[01])'$"); // Valida fechas
            expOtros = new Regex("^[=,;()\\*><]$");
        }

        // Esta funcion construira las expresiones regulares para comparar si las palabras ingresadas por el usuario
        // pertenecen a la lista de palabras reservadas aceptadas por el Parser
        private void crearPatronesTokens()
        {
            // construyendo patron para DDL
            construirPatron(ref definitionTokens, ref patrones);
            // construyendo patron para DML
            construirPatron(ref manipulationTokens, ref patrones);
            // construyendo patron para objetos
            construirPatron(ref objectTokens, ref patrones);
            // construyendo patron para los tipos de dato
            construirPatron(ref dataTypeTokens, ref patrones);
            // construyendo patron para las palabras reservada auxiliares de las consultas DML
            construirPatron(ref helpersDMLTokens, ref patrones);
            // construyendo patron para las condiciones despues de la clausula WHERE
            construirPatron(ref conditionsDMLTokens, ref patrones);
        }

        private void construirPatron(ref ArrayList tokens, ref ArrayList patrones)
        {
            // Formato del patron: ^(VALOR|VALOR|VALOR)$
            StringBuilder nuevoPatron = new StringBuilder();
            nuevoPatron.Append("^(");
            for (int i = 0; i < tokens.Count; i++)
            {
                nuevoPatron.Append(tokens[i]);
                if (i != tokens.Count - 1)
                    nuevoPatron.Append("|");
            }
            nuevoPatron.Append(")$");
            patrones.Add(nuevoPatron.ToString());
            nuevoPatron.Clear();
        }

        // Cada palabra, cantidad y simbolo de la sentencia estara separada por un \t
        // Con el fin de hacer mas facil la deteccion de errores y el analisis lexico y sintactico
        private string prepararInstruccion(ref string instruccion)
        {
            StringBuilder instruccionAuxiliar = new StringBuilder(String.Empty);
            bool noActuar = false; // Indica que no se agregara un \t si se encontrase un espacio en blanco adentro de ''

            for (int i = 0; i < instruccion.Length; i++)
            {
                // Caracteres que se pueden encontrar y a los que se les agregara \t antes y despues
                if (instruccion[i] == '(' | instruccion[i] == ')' | instruccion[i] == ',' | instruccion[i] == '=' | instruccion[i] == ';')
                {
                    // Se agregara antes un \t si no existiere uno ya
                    if (instruccionAuxiliar[instruccionAuxiliar.Length - 1] != '\t')
                        instruccionAuxiliar.Append("\t");

                    if (instruccion[i] == ';') // ; Indica el fina de la sentencia o instruccion SQL
                        instruccionAuxiliar.Append(instruccion[i]);
                    else
                        instruccionAuxiliar.Append(instruccion[i] + "\t");

                }
                else if (instruccion[i] == '\'')
                {
                    // En dado caso de encontrar 'Juan Carlos', esto sera visto como un token completo (Incluyendo las comillas simples)
                    instruccionAuxiliar.Append(instruccion[i]);
                    if (noActuar)
                        noActuar = false;
                    else
                        noActuar = true;
                }
                else if (instruccion[i] == ' ')
                {
                    // En el caso de analizar 'Juan Carlos Valencia' o '10-01-1994' solo se agregara el espacio en blanco
                    if (noActuar)
                        instruccionAuxiliar.Append(instruccion[i]);
                    else
                    {
                        // Se agregara antes un \t si no existiere antes uno
                        if (instruccionAuxiliar[instruccionAuxiliar.Length - 1] != '\t')
                            instruccionAuxiliar.Append("\t");
                    }
                }
                else
                {
                    // No se incluiran los siguientes caracateres en la instruccion preparada
                    if (instruccion[i] != '\r' && instruccion[i] != '\t' && instruccion[i] != '\n')
                        instruccionAuxiliar.Append(instruccion[i]);
                }
            }
            return instruccionAuxiliar.ToString();
        }

        private bool verificarSintaxisIdentificador(string palabra, ref Regex rg)
        {
            return rg.IsMatch(palabra);
        }

        // Se comprobara si cada palabra de la sentencia ingresada por el usuario pertenecen
        // al lenguaje SQL del Parser
        public string[] analisisLexico(string instruccion, ref ArrayList palabrasLexicasErroneas)
        {
            if (!instruccion.EndsWith(";")) // Agregandole un ; al final de la sentencia si esta no la tiene
                instruccion += ";";

            // Debido a que el parser solo ejecutara una sentencia SQL al presionar el botón, nos aseguraremos de obtener
            // la sentencia antes del primer ; (En caso de que se haya digitado mas de una sentencia en el TextBox)
            int indPC = instruccion.IndexOf(';', 0, instruccion.Length);
            instruccion = instruccion.Substring(0, indPC + 1);

            string instruccionPreparada = prepararInstruccion(ref instruccion);
            this.consultaEjecutar = instruccionPreparada;
            string[] words = instruccionPreparada.Split('\t'); // Separando cada palabra, cantidad y simbolo en un arreglo

            /*
             * ArrayList patrones
             * Posicion 1 es para DDL
             * Posicion 2 es para DML
             * Posicion 3 es para Objetos
             * Posicion 4 es para Tipos de Datos
            */
            Regex rgx;
            Match match;
            for (int i = 0; i < words.Length; i++) // Recorriendo cada palabra de la sentencia
            {
                int cndc = 0;
                foreach (string patron in patrones) // Comparando cada palabra con cada patron
                {
                    rgx = new Regex(patron);
                    match = rgx.Match(words[i]);

                    if (match.Success)
                        break;
                    else
                        cndc++;
                }

                if (cndc == patrones.Count) // Si no concordo con ninguna palabra reservada del parser
                {
                    // Se comprobara si cumple el formato de un identificador
                    if (verificarSintaxisIdentificador(words[i], ref expIdentificador))
                        goto PalabraCorrecta;
                    if (verificarSintaxisIdentificador(words[i], ref expEntero))
                        goto PalabraCorrecta;
                    if (verificarSintaxisIdentificador(words[i], ref expDecimal))
                        goto PalabraCorrecta;
                    if (verificarSintaxisIdentificador(words[i], ref expTexto))
                        goto PalabraCorrecta;
                    if (verificarSintaxisIdentificador(words[i], ref expFecha))
                        goto PalabraCorrecta;
                    if (verificarSintaxisIdentificador(words[i], ref expOtros))
                        goto PalabraCorrecta;

                    palabrasLexicasErroneas.Add(words[i]); // Se agrega la palabra a la lista de palabras erroneas
                }

            PalabraCorrecta:
                continue;
            }

            if (palabrasLexicasErroneas.Count > 0)
                return new string[0];
            else
                return words;

        }

        // Verificar el orden logico de los tokens (Segun lo que se quiera hacer)
        public string analisiSintatico(ref string[] words, ref ArrayList erroresSintacticos, ref int tipo)
        {
            if (words[0] == "CREATE" || words[0] == "create")
            {   
                if (words[1] == "DATABASE" || words[1] == "database")
                {
                    tipo = 1;
                    if (!verificarSintaxisIdentificador(words[2], ref expIdentificador))
                        erroresSintacticos.Add("Error de sintaxis. Nombre de base de datos incorrecto.");
                }
                else if (words[1] == "TABLE" || words[1] == "table")
                {
                    tipo = 2;
                    esCrearTabla(ref words, 2, ref erroresSintacticos);
                }
                else
                    erroresSintacticos.Add("Error de sintaxis. Después de CREATE se esparaba la palabra DATABASE o TABLE.");
            }
            else if (words[0] == "DROP" || words[0] == "drop")
            {
                if (words[1] == "DATABASE" || words[1] == "database")
                {
                    tipo = 3;
                    if (!verificarSintaxisIdentificador(words[2], ref expIdentificador))
                        erroresSintacticos.Add("Error de sintaxis. Nombre de base de datos incorrecto");
                }
                else if (words[1] == "TABLE" || words[1] == "table")
                {
                    tipo = 4;
                    if (!verificarSintaxisIdentificador(words[2], ref expIdentificador))
                        erroresSintacticos.Add("Error de sintaxis. Nombre de tabla incorrecto");
                }
                else
                    erroresSintacticos.Add("Error de sintaxis. Después de DROP se esparaba la palabra DATABASE o TABLE");
            }
            else if (words[0] == "INSERT" || words[0] == "insert")
            {
                tipo = 5;
                if (words[1] == "INTO" || words[1] == "into")
                {
                    if (!verificarSintaxisIdentificador(words[2], ref expIdentificador))
                        erroresSintacticos.Add("Error de sintaxis. Incorrecto nombre de tabla en la cual se quiere ingresar.");
                    else
                        esInsertarDatos(ref words, 3, ref erroresSintacticos);
                }
                else
                    erroresSintacticos.Add("Error de sintaxis. Se esperaba la palabra INTO");
            }
            else if (words[0] == "UPDATE" || words[0] == "update")
            {
                tipo = 6;
                if (!verificarSintaxisIdentificador(words[1], ref expIdentificador))
                    erroresSintacticos.Add("Error de sintaxis. Nombre de tabla incorrecto");
                else
                    esActualizarDatos(ref words, 2, ref erroresSintacticos);
            }
            else if (words[0] == "DELETE" || words[0] == "delete")
            {
                tipo = 7;
                if (words[1] == "FROM" || words[1] == "from")
                {
                    if (!verificarSintaxisIdentificador(words[2], ref expIdentificador))
                        erroresSintacticos.Add("Error de sintaxis. Nombre de tabla incorrecto");
                    else
                    {
                        if (words[3] == "WHERE" | words[3] == "where")
                        { }
                        else if (words[3] != ";")
                            erroresSintacticos.Add("Error de sintaxis. La instruccion DELETE no termina apropiadamente");
                    }
                }
                else
                    erroresSintacticos.Add("Error de sintaxis. Se esperaba la palabra FROM");

            }
            else if (words[0] == "SELECT" || words[0] == "select")
            {
                tipo = 8;
                //esSelecccionarDatos(ref words, 1, ref erroresSintacticos);
            }
            else
            {
                erroresSintacticos.Add("La consulta no pertenece a ninguna sentencia SQL permitida en el parser");
            }
            
            return (erroresSintacticos.Count > 0) ? null : this.consultaEjecutar;
        }

        // Analisis sintactico si se quiere crear una tabla
        private void esCrearTabla(ref string[] words, int posicion, ref ArrayList erroresSintacticos)
        {

            if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                erroresSintacticos.Add("Error de sintaxis. Nombre de tabla incorrecto.");
            else
            {
                posicion++;
                if (words[posicion] != "(")
                    erroresSintacticos.Add("Error de sintaxis. Se esperaba un ( después del nombre de la tabla.");
                else
                {
                    // Comprobar el formato: nombre_campo tipo de dato,
                    // Si no hay "," quiere decir que se espera el parentesis de cierre de la tabla
                    posicion++;
                    int cont = 1;
                    for (; posicion < words.Length; posicion++)
                    {
                        if (cont == 1) // Se valida el nombre del campo
                        {
                            if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                            {
                                erroresSintacticos.Add("Error de sintaxis. " + words[posicion] + " es un nombre de columna inválido.");
                                break;
                            }
                            else
                                cont++;
                        }
                        else if (cont == 2) // Se valida el tipo de dato
                        {
                            if (dataTypeTokens.Contains(words[posicion]))
                            {
                                if (words[posicion] == "VARCHAR" || words[posicion] == "varchar")
                                {
                                    posicion++;
                                    if (words[posicion] != "(")
                                    {
                                        erroresSintacticos.Add("Error de sintaxis. Se esperaba un ( después de VARCHAR para la columna " + words[posicion - 2] + ".");
                                    }
                                    posicion++;
                                    if (!verificarSintaxisIdentificador(words[posicion], ref expEntero))
                                    {
                                        erroresSintacticos.Add("Error de sintaxis. Especifique una longitud para la columna " + words[posicion - 3] + ".");
                                    }
                                    posicion++;
                                    if (words[posicion] != ")")
                                        erroresSintacticos.Add("Error de sintaxis. Se esperaba un ) después de la longitud de cadena para la columna "
                                            + words[posicion - 4] + ".");
                                }
                            }
                            else
                                erroresSintacticos.Add("Error de sintaxis. " + words[posicion] + " es un nombre de tipo de dato no válido");
                            cont++;
                        }
                        else // Se valida si hay ,
                        {
                            if (words[posicion] == ",") // Hay mas campos en la tabla
                                cont = 1;
                            else
                                break;
                        }
                    }
                    if (erroresSintacticos.Count == 0)
                    {
                        if (words[posicion] != ")")
                            erroresSintacticos.Add("Se esperaba un ) para cerrar la declaración de la tabla. Tambié revise si hay una coma faltante" +
                                " entre la declaración de sus columnas");
                    }
                }
            }
        }

        // Analisis sintactico para insertar datos
        private void esInsertarDatos(ref string[] words, int posicion, ref ArrayList erroresSintacticos)
        {
            if (words[posicion] != "(")
                erroresSintacticos.Add("Error de sintaxis. Se esperaba un ( después del nombre de la tabla en la consulta INSERT");
            else
            {
                posicion++;
                int cont = 1;
                // Formato: INSERT INTO nombre_tabla(columna, columna, columna)
                // El formato del comentario anterior se verificara en el siguiente for
                // a partir del listado del nombre de las columnas
                for(; posicion < words.Length; posicion++)
                {
                    if (cont == 1)
                    {
                        if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                        {
                            erroresSintacticos.Add("Error de sintaxis. La palabra " + words[posicion] + " no es un nombre de columna válido");
                            break;
                        }
                        cont++;
                    }
                    else if (cont == 2)
                    {
                        if (words[posicion] == ",")
                            cont = 1;
                        else
                            break;
                    }
                }

                if (erroresSintacticos.Count == 0)
                {
                    if (words[posicion] != ")")
                        erroresSintacticos.Add("Error de sintaxis. Se esperaba un ) después de la enumeración de las columnas de la tabla");
                    else
                    {
                        posicion++;
                        if (words[posicion] != "VALUES" && words[posicion] != "values")
                            erroresSintacticos.Add("Error de sintaxis. Se esperaba la palabra VALUES");
                        else
                        {
                            posicion++;
                            if (words[posicion] != "(")
                                erroresSintacticos.Add("Error de sintaxis. Se esperaba un ( después del la palabra VALUES");
                            else
                            {
                                posicion++;
                                // Una vez el listado de las columnas en las que se quiere insertar ha sido comprobado
                                // Se verifica lo siguiente VALUES (valor, valor, valor);
                                // El siguiente for es el que se encarga de eso
                                for (; posicion < words.Length; posicion++)
                                {
                                    if (verificarSintaxisIdentificador(words[posicion], ref expEntero) |
                                        verificarSintaxisIdentificador(words[posicion], ref expDecimal) |
                                        verificarSintaxisIdentificador(words[posicion], ref expTexto) |
                                        verificarSintaxisIdentificador(words[posicion], ref expFecha)
                                        )
                                    {
                                        posicion++;
                                        if (words[posicion] == ",")
                                            continue;
                                        else
                                            break;
                                    }
                                    else
                                    {
                                        erroresSintacticos.Add("Error de sintaxis. " + words[posicion] + " no es un valor de asignación válido");
                                        break;
                                    }  
                                }

                                if (erroresSintacticos.Count == 0)
                                {
                                    if (words[posicion] != ")")
                                        erroresSintacticos.Add("Error de sintaxis. Se esperaba un ) después de la declaración de los valores " +
                                            " a asignar");
                                }
                            }
                        }
                    }
                }
            }
        }

        // Analisis sintactico de actualizar datos
        private void esActualizarDatos(ref string[] words, int posicion, ref ArrayList erroresSintacticos)
        {
            if (words[posicion] != "SET" && words[posicion] != "set")
                erroresSintacticos.Add("Error de sintaxis. Se esperaba la palabra SET después del nombre de la tabla");
            else
            {
                posicion++;
                int cont = 1;
                // El for siguiente se encarga de verificar lo siguiente (Despues del SET)
                // nombre_columna = valor, nombre_columna = valor
                for (; posicion < words.Length; posicion++)
                {
                    if (cont == 1)
                    {
                        if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                        {
                            erroresSintacticos.Add("Error de sintaxis. La palabra " + words[posicion] + " no es un nombre de columna válido");
                            break;
                        }
                        cont++;
                    }
                    else if (cont == 2)
                    {
                        if (words[posicion] != "=")
                        {
                            erroresSintacticos.Add("Error de sintaxis. Se esperaba un = después de la columna " + words[posicion - 1]);
                            break;
                        }
                        cont++;
                    }
                    else if (cont == 3)
                    {
                        if (verificarSintaxisIdentificador(words[posicion], ref expEntero) |
                            verificarSintaxisIdentificador(words[posicion], ref expDecimal) |
                            verificarSintaxisIdentificador(words[posicion], ref expTexto) |
                            verificarSintaxisIdentificador(words[posicion], ref expFecha)
                            )
                        {
                            posicion++;
                            if (words[posicion] == ",") // Si se actualizara otro campo, continua el for
                            {
                                cont = 1;
                                continue;
                            }
                            else
                                break;
                        }
                        else
                        {
                            erroresSintacticos.Add("Error de sintaxis. " + words[posicion] + " no es un valor de asignación válido");
                            break;
                        }
                    }
                }

                if (erroresSintacticos.Count == 0)
                {
                    // Si no hay un where se actualizaran todos los registros
                    if (words[posicion] == ";") { }
                    // Si hay un WHERE significa que habra filtros para hacer el update
                    else if (words[posicion] == "WHERE" | words[posicion] == "where")
                    {   
                    }
                    else
                        erroresSintacticos.Add("Error de sintaxis. Se esperaba un ; o la palabra WHERE, en lugar de " + words[posicion]);
                }
            }
        }

        public void esSelecccionarDatos(ref string[] words, int posicion, ref ArrayList erroresSintacticos)
        {
            if (words[posicion] == "*")
                goto deTabla;
            else
            {
                posicion++;
                int cont = 1;
                // Formato: SELECT columna, columna, columna FROM...
                // El formato del comentario anterior se verificara en el siguiente for
                // a partir del listado del nombre de las columnas a obtener
                for (; posicion < words.Length; posicion++)
                {
                    if (cont == 1)
                    {
                        if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                        {
                            erroresSintacticos.Add("Error de sintaxis. La palabra " + words[posicion] + " no es un nombre de columna válido");
                            break;
                        }
                        cont++;
                    }
                    else if (cont == 2)
                    {
                        if (words[posicion] == ",")
                            cont = 1;
                        else
                            break;
                    }
                }
                if (erroresSintacticos.Count == 0)
                {
                    goto deTabla;
                }
            }
        deTabla:
            posicion++;
            if (words[posicion] == "FROM" | words[posicion] == "from")
            {
                posicion++;
                if (!verificarSintaxisIdentificador(words[posicion], ref expIdentificador))
                {
                    erroresSintacticos.Add("Error de sintaxis. La palabra " + words[posicion] + " no es un nombre de tabla válido");
                    goto fin;
                }
                
                posicion++;
                
                if (words[posicion] == ";") { }
                else if (words[posicion] == "WHERE" | words[posicion] == "where") { }
                else
                    erroresSintacticos.Add("Error de sintaxis. Se esperaba un ; o la palabra WHERE, en lugar de " + words[posicion]);
            }
            else
                erroresSintacticos.Add("Error de sintaxis. Se esperaba la palabra FROM después del listado de columnas a obtener");

        fin: { }
            
        }
    }
}

