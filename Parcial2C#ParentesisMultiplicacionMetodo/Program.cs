using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        //do{

        Console.WriteLine("Calculadora de Ecuaciones en C# - Para salir escriba salir");
        Console.Write("Ingresa una ecuación (por ejemplo, -9 * (2 * -8) + 2): ");
        string ecuacion = Console.ReadLine();

        try
        {
            if (ValidarParentesis(ecuacion))
            {
                ecuacion = AgregarMultiplicacionImplicita(ecuacion); // Agregar multiplicación implícita
                double resultado = EvaluarEcuacion(ecuacion);
                Console.WriteLine("Resultado: " + resultado);
            }
            else
            {
                Console.WriteLine("Error: Los paréntesis no están balanceados en la ecuación.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al evaluar la ecuación: " + ex.Message);
        }
        //}while(ecuacion.Equals("salir"));
    }

    static bool ValidarParentesis(string ecuacion)
    {
        int contadorParentesis = 0;

        foreach (char caracter in ecuacion)
        {
            if (caracter == '(')
            {
                contadorParentesis++;
            }
            else if (caracter == ')')
            {
                contadorParentesis--;
                if (contadorParentesis < 0)
                {
                    return false; // Paréntesis no balanceados
                }
            }
        }

        return contadorParentesis == 0;
    }

    static double EvaluarEcuacion(string ecuacion)
    {
        // Eliminar espacios en blanco
        ecuacion = ecuacion.Replace(" ", "");

        // Llamar a la función de análisis e evaluación de ecuaciones
        double resultado = EvaluarExpresion(ecuacion);
        return resultado;
    }

    static double EvaluarExpresion(string expresion)
    {
        Stack<double> valores = new Stack<double>();
        Stack<char> operadores = new Stack<char>();
        int i = 0;

        while (i < expresion.Length)
        {
            if (expresion[i] == '(')
            {
                int parCount = 1;
                int j = i + 1;
                while (j < expresion.Length && parCount > 0)
                {
                    if (expresion[j] == '(')
                        parCount++;
                    else if (expresion[j] == ')')
                        parCount--;
                    j++;
                }
                string subExpresion = expresion.Substring(i + 1, j - i - 2); //(2+5)*3
                valores.Push(EvaluarExpresion(subExpresion));
                i = j;
            }
            else if (char.IsDigit(expresion[i]) || (expresion[i] == '-' && (i == 0 || expresion[i - 1] == '(')))
            {
                int j = i + 1;
                while (j < expresion.Length && (char.IsDigit(expresion[j]) || expresion[j] == '.'))
                    j++;
                string numero = expresion.Substring(i, j - i);
                valores.Push(double.Parse(numero));
                i = j;
            }
            else if (esOperador(expresion[i]))
            {
                while (operadores.Count > 0 && prioridadOperador(expresion[i]) <= prioridadOperador(operadores.Peek()))
                {
                    double valor2 = valores.Pop();
                    double valor1 = valores.Pop();
                    char operador = operadores.Pop();
                    valores.Push(AplicarOperador(valor1, valor2, operador));
                }
                operadores.Push(expresion[i]);
                i++;
            }
            else
            {
                throw new Exception("Carácter no válido en la ecuación: " + expresion[i]);
            }
        }

        while (operadores.Count > 0)
        {
            double valor2 = valores.Pop();
            double valor1 = valores.Pop();
            char operador = operadores.Pop();
            valores.Push(AplicarOperador(valor1, valor2, operador));
        }

        return valores.Pop();
    }

    static bool esOperador(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }

    static string AgregarMultiplicacionImplicita(string ecuacion)
    {
        string nuevaEcuacion = "";
        for (int i = 0; i < ecuacion.Length; i++)
        {
            char caracter = ecuacion[i];
            nuevaEcuacion += caracter;

            if (caracter == ')' && i < ecuacion.Length - 1)
            {
                char siguienteCaracter = ecuacion[i + 1];
                if (char.IsDigit(siguienteCaracter) || siguienteCaracter == '(')
                {
                    nuevaEcuacion += '*'; // Agregar multiplicación implícita
                }
            }
            else if (caracter == '(' && i > 0)
            {
                char caracterAnterior = ecuacion[i - 1];
                if (char.IsDigit(caracterAnterior) || caracterAnterior == ')')
                {
                    nuevaEcuacion = nuevaEcuacion.Insert(nuevaEcuacion.Length - 1, "*"); // Agregar multiplicación implícita
                }
            }
        }
        return nuevaEcuacion;
    }

    static int prioridadOperador(char operador)
    {
        switch (operador)
        {
            case '+':
            case '-':
                return 1;
            case '*':
            case '/':
                return 2;
            default:
                return 0;
        }
    }

    static double AplicarOperador(double valor1, double valor2, char operador)
    {
        switch (operador)
        {
            case '+':
                return valor1 + valor2;
            case '-':
                return valor1 - valor2;
            case '*':
                return valor1 * valor2;
            case '/':
                if (valor2 == 0)
                    throw new DivideByZeroException();
                return valor1 / valor2;
            default:
                throw new ArgumentException("Operador no válido: " + operador);
        }
    }
}