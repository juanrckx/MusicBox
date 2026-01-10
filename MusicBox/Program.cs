using MusicBox.DataStructures;
using MusicBox.Audio;
using System;

namespace MusicBox
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Music Box Player ===");
            Console.WriteLine("========================\n");
            
            try
            {
                // 1. Configurar duración base
                double baseDuration = GetBaseDuration();
                
                // 2. Crear parser
                var parser = new ScoreParser(baseDuration);
                
                // 3. Obtener partitura
                string scoreInput = GetScoreInput();
                
                // 4. Parsear partitura
                var score = parser.Parse(scoreInput);
                Console.WriteLine($"✓ {score.Count} han sido almacenadas en la lista doblemente enlazada.");
                
                // 5. Crear motor de audio
                using (var audioEngine = new AudioEngine())
                {
                    audioEngine.SetBaseDuration(baseDuration);
                    
                    // 6. Reproducir hacia adelante
                    Console.WriteLine("\n--- Reproduciendo hacia adelante ---");
                    audioEngine.PlayScore(score, forward: true);
                    
                    // 7. Reproducir hacia atrás
                    Console.WriteLine("\n--- Reproduciendo hacia atrás ---");
                    audioEngine.PlayScore(score, forward: false);
                    
                    // 8. Cambiar duración base
                    if (AskYesNo("¿Cambiar la duración base?"))
                    {
                        baseDuration = GetBaseDuration();
                        audioEngine.SetBaseDuration(baseDuration);
                        
                        Console.WriteLine("\n--- Reproduciendo con la nueva duración base ---");
                        audioEngine.PlayScore(score, forward: true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        static double GetBaseDuration()
        {
            while (true)
            {
                try
                {
                    Console.Write("Introduzca la duración base (Entre 0.1 y 5.0 segundos , default 1.0): ");
                    string input = Console.ReadLine();
                    
                    if (string.IsNullOrEmpty(input))
                        return 1.0;
                    
                    double duration = double.Parse(input);
                    
                    if (duration < 0.1 || duration > 5.0)
                        throw new ArgumentException("La duración debe estar entre 0.1 y 5.0 segundos");
                    
                    return duration;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Input inválido: {ex.Message}. Intene nuevamente.");
                }
            }
        }

        static string GetScoreInput()
        {
            Console.WriteLine("\nFormato de entrada: (Nota, Figura), (Nota, Figura), ...");
            Console.WriteLine("Notas: Do, Re, Mi, Fa, Sol, La, Si");
            Console.WriteLine("Figuras: redonda, blanca, negra, corchea, semicorchea");
            Console.Write("\nIntroduzca su partitura (Presione Enter para ejemplo): ");
            
            string input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                input = "(Do, negra), (Re, blanca), (Mi, corchea), (Fa, semicorchea), (Sol, redonda)";
                Console.WriteLine($"Usando ejemplo: {input}");
            }
            
            return input;
        }

        static bool AskYesNo(string question)
        {
            Console.Write($"\n{question} (y/n): ");
            string response = Console.ReadLine().ToLower();
            return response == "y" || response == "yes";
        }
    }
}