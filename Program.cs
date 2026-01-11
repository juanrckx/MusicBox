using MusicBox.DataStructures;
using MusicBox.Audio;
using System;
using System.Threading;
using MusicBox.Models;
using System.Runtime.CompilerServices;

namespace MusicBox
{
    class Program
    {
        private static double _baseDuration = 1.0;
        private static string _currentScore = "";
        private static DoublyLinkedList<Note> _scoreNotes;
        private static ScoreParser _parser;

        static void Main(string[] args)
        {
            Console.Title = "Music Box Player - Reproductor de Partituras";

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                DisplayHeader();
                DisplayCurrentSettings();
                DisplayMenu();

                var option = GetMenuOption();

                switch (option)
                {
                    case 1:
                        EnterNewScore();
                        break;

                    case 2:
                        ChangeBaseDuration();
                        break;

                    case 3:
                        PlayScoreForward();
                        break;

                    case 4:
                        PlayScoreBackward();
                        break;

                    case 5:
                        PlayBothDirections();
                        break;

                    case 6:
                        DisplayScoreInfo();
                        break;

                    case 7:
                        exit = true;
                        Console.WriteLine("\nSaliendo del programa...");
                        break;
                    
                    default:
                        Console.WriteLine("\nOpción no válida. Intente nuevamente.");
                        WaitForUser();
                        break;
                }
            }

            Console.WriteLine("\nPresione cualquiertecla para salir...");
            Console.ReadKey();
        }

        static void DisplayHeader()
        {
            Console.WriteLine("====================================================");
            Console.WriteLine("MUSIC BOX PLAYER | PROYECTO DE ALG. Y ESTR. DE DATOS");
            Console.WriteLine("====================================================\n");
        }

        static void DisplayCurrentSettings()
        {
            Console.WriteLine("CONFIGURACIÓN ACTUAL:");
            Console.WriteLine($"- Duración base (negra): {_baseDuration:F2} segundos");
            Console.WriteLine($"- Notas en partitura: {(_scoreNotes?.Count ??0)}");
            
            if (!string.IsNullOrEmpty(_currentScore))
            {
                Console.WriteLine($"- Partitura actual: {_currentScore}");
            }
            Console.WriteLine();
        }

        static void DisplayMenu()
        {
            Console.WriteLine("MENÚ PRINCIPAL:");
            Console.WriteLine("1. Ingresar nueva partitura");
            Console.WriteLine("2. Cambiar duración base");
            Console.WriteLine("3. Reproducir hacia ADELANTE");
            Console.WriteLine("4. Reproducir hacia ATRÁS");
            Console.WriteLine("5. Reproducir en AMBAS direcciones");
            Console.WriteLine("6. Ver información de la partitura");
            Console.WriteLine("7. Salir");
            Console.Write("\nSeleccione una opción (1-7): ");
        }

        static int GetMenuOption()
        {
            if (int.TryParse(Console.ReadLine(), out int option))
            {
                return option;
            }
            return 0;
        }

        static void EnterNewScore()
        {
            Console.Clear();
            Console.WriteLine("=== INGRESAR NUEVA PARTITURA ===\n");
            Console.WriteLine("Formato requerido: (Nota, Figura), (Nota, Figura), ...");
            Console.WriteLine("Notas disponibles: Do, Re, Mi, Fa, Sol, La, Si");
            Console.WriteLine("Figuras disponibles: Redonda, Blanca, Negra, Corchea, Semicorchea");
            Console.WriteLine("\nEjemplo: (Do, negra), (Re, blanca), (Mi, corchea)");

            Console.Write("\nIngrese su partitura:  ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("\nNo se ingresó ninguna partitura, cancelando...");
                WaitForUser();
                return;
            }

            try
            {
                _parser = new ScoreParser(_baseDuration);
                _scoreNotes = _parser.Parse(input);
                _currentScore = input;

                Console.WriteLine($"\n ¡Partitura procesada correctamente!");
                Console.WriteLine($" Se han almacenado {_scoreNotes.Count}");
                Console.WriteLine($"Duración base configurada : {_baseDuration:F2}s");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"\nError al procesar la partitura: {ex.Message}");
                Console.WriteLine("Revise el formato e intente neuvamente.");
            }
            WaitForUser();
        }

        static void ChangeBaseDuration()
        {
            Console.Clear();
            Console.WriteLine("=== CAMBIAR DURACIÓN BASE === \n");
            Console.WriteLine("La duración base define cuánto dura una NEGRA.");
            Console.WriteLine("El resto de figuras se ajustan proporcionalmente.");
            Console.WriteLine("Rango permitido: 0.1 a 5.0 segundos\n");

            while (true)
            {
                Console.Write($"Duración actual: {_baseDuration:F2}s | Nueva duración:  ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Operación cancelada.");
                    break;
                }

                if (double.TryParse(input, out double newDuration))
                {
                    if (newDuration >= 0.1 && newDuration <= 5.0)
                    {
                        _baseDuration = newDuration;

                        // Re-parsea la partitura actual con la nueva duración
                        if (!string.IsNullOrEmpty(_currentScore))
                        {
                            try
                            {
                                _parser = new ScoreParser(_baseDuration);
                                _scoreNotes = _parser.Parse(_currentScore);
                            }
                            catch
                            {
                                // Si hay error, mantenemos las notas anteriores
                            }
                        }

                        Console.WriteLine($"\n-Duración base actualizada a {_baseDuration:F2} segundos.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: La duración debe estar entre 0.1 y 5.0 segundos.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Ingrese un número válido.");
                }
            }

            WaitForUser();
        }

        static void PlayScoreForward()
        {
            PlayScore(forward: true, showDirection: true);
        }

        static void PlayScoreBackward()
        {
            PlayScore(forward: false, showDirection: true);
        }

        static void PlayBothDirections()
        {
            Console.Clear();
            Console.WriteLine("=== REPRODUCCIÓN EN AMBAS DIRECCIONES ===\n");

            if (!ValidateScoreLoaded()) return;

            try
            {
                using (var audioEngine = new AudioEngine())
                {
                    audioEngine.SetBaseDuration(_baseDuration);

                    Console.WriteLine("Reproduciendo hacia ADELANTE...");
                    audioEngine.PlayScore(_scoreNotes, forward: true);

                    Console.WriteLine("\n. Pausa entre reproducciones...");
                    Thread.Sleep(1500);

                    Console.WriteLine("\n Reproduciendo hacia ATRÁS...");
                    audioEngine.PlayScore(_scoreNotes, forward: false);

                    Console.WriteLine("\n Reproducción completa en ambas direcciones.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error durante la reproducción: {ex.Message}");
            }

            WaitForUser();
        }

        static void PlayScore(bool forward, bool showDirection = false)
        {
            Console.Clear();

            if (showDirection)
            {
                string direction = forward ? "ADELANTE" : "ATRÁS";
                Console.WriteLine($"=== REPRODUCCIÓN HACIA {direction} ===\n");
            }

            if (!ValidateScoreLoaded()) return;
            try
            {
                using (var audioEngine = new AudioEngine())
                {
                    audioEngine.SetBaseDuration(_baseDuration);
                    audioEngine.PlayScore(_scoreNotes, forward);
                    Console.WriteLine($"\n  Reproducción {(forward ? "hacia adelante" : "hacia atrás")} completada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error durante la reproducción: {ex.Message}");
            }

            WaitForUser();
        }

        static void DisplayScoreInfo()
        {
            Console.Clear();
            Console.WriteLine("=== INFORMACIÓN DE LA PARTITURA ===\n");

            if (!ValidateScoreLoaded()) return;

            Console.WriteLine($"Notas totales: {_scoreNotes.Count}");
            Console.WriteLine($"Duración base: {_baseDuration:F2} segundos (negra)\n");

            Console.WriteLine("LISTA DE NOTAS:");
            Console.WriteLine("---------------");

            int counter = 1;
            foreach (var note in _scoreNotes.TraverseForward())
            {
                double duration = note.Duration?.GetSeconds() ?? 0;
                Console.WriteLine($"{counter}. {note.Name} - {note.Frequency:F2} Hz - {duration:F2}s");
                
                counter++;
            }

            WaitForUser();
        }

        static bool ValidateScoreLoaded()
        {
            if (_scoreNotes == null || _scoreNotes.Count == 0)
            {
                Console.WriteLine(" No hay ninguna partitura cargada.");
                Console.WriteLine("Por favor, ingrese una partitura primero (Opción 1).");
                WaitForUser();
                return false;
            }
            return true;
        }

        static void WaitForUser()
        {
            Console.Write("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}