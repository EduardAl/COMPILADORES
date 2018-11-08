using System;
using System.Collections.Generic;


namespace COMPILADORES.analizador
{
    class AnalisisSintactico
    {
        List<KeyValuePair<int, string>> tokens;
        //analisar el orden..
        public bool createDB()
        { 
            bool estado = false;
            int cont = 0;
            foreach (KeyValuePair<int, string> token in tokens)
            {
                estado = false;
                cont++;
                //analizar create(1) database(2) Nombre(3), create(1) table(2) nombre(3)
                switch (token.Key)
                { 
                    case 1:
                        if (cont == 1 && (token.Value == "create" || token.Value == "drop")) estado = true; // create
                        else if (cont == 2 && token.Value == "database") estado = true; // database
                        break;
                    case 4:
                        if (cont == 3) estado = true; // nombre de la tabla
                        break;
                    case 6:
                        if (cont == 4) estado = true;
                        break;
                        
                }
                if (cont ==3 )    break;
            }
              return estado;
        }
        public bool createTB(ref string campo)
        {
            bool estado = false;
            int cont = 0;
            for (int i = 0; i< tokens.Count; i++)
            {
                estado = false;
                cont++;
                try
                {
                    switch (tokens[i].Key)
                    {
                        case 1:
                            if (cont == 1 && tokens[i].Value == "create") estado = true; // create
                            else if (cont == 2 && tokens[i].Value == "table") estado = true; // database
                            break;
                        case 4:
                            if (cont == 3) estado = true; // nombre de la tabla
                            else if (tokens[i - 1].Value == "(" || tokens[i - 1].Value == ",")   estado = true; // (campo1 tipo, campo2 tipo)
                            break;
                        case 5:
                            if (cont == 4 || tokens[i].Value == "(") estado = true;
                            else if (tokens[i - 1].Key == 2 || tokens[i].Value == "(") estado = true;
                            else if (tokens[i - 1].Key == 3 || tokens[i].Value == ")") estado = true;
                            else if (cont == tokens.Count || tokens[i].Value == ")") estado = true;
                            else if (cont + 1 == tokens.Count || tokens[i].Value == ")") estado = true;
                            break;
                        case 2:
                            if (tokens[i - 1].Key == 4 )   estado = true; // (campo1 tipo, campo2 tipo)
                            break;
                        case 3:
                            if (tokens[i - 1].Key == 5 && tokens[i+1].Key == 5) estado = true; // (campo1 tipo, campo2 tipo)
                            break;
                        case 8:
                            if (tokens[i + 1].Key == 4 ) estado = true; 
                            break;
                        case 6:
                            if (cont == tokens.Count) estado = true; 
                            break;
                            

                    }
                }
                catch (Exception)
                {
                    return false;
                }
                if (!estado) { campo = tokens[i].Value; break; }
            }
            return estado;
        }
        public bool analisis(string texto, ref string malo)
        {

            AnalisisLexico lex = new AnalisisLexico();
            tokens = lex.getTokens(texto);

           /* if (createDB())
            {
                MessageBox.Show("Creo la tabla");
                return true;
            }*/

            
            if (!createTB(ref malo))
            {
                return false;
            }
            return true;
                
            
        }
    }
}
