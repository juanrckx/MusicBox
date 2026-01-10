using System;
using System.Threading;
using System.Collections.Concurrent;
using ManagedBass;

namespace MusicBox.Audio
{
    public class ToneGenerator : IDisposable
    {
        private readonly int _sampleRate = 44100;                               // Muestras por segundo (Calidad CD)
        
        // Tipo de onda: Sine = suave, Square = cuadrada, Sawtooth = diente de sierra
        private readonly OscillatorType _oscillatorType = OscillatorType.Sine;
        
        // ¿Está inicializado el sistema de audio
        private bool _initialized = false;

        // ConcurrentQueue = Cola segura parau sar entre múltiples hilos
        // ToneRequest = Petición de sonido (qué tocar)
        private readonly ConcurrentQueue<ToneRequest> _toneQueue = new();

        // Control de hilo
        private Thread? _playbackThread;        // El hilo que reproduce sonidos
        private bool _isRunning = false;        // ¿Debe seguir recorriendo el hilo?
        private readonly object _lock = new();  // Para sincronización

        // Constructor
        public ToneGenerator()
        {
            if (!Bass.Init())                                                   // Inicializar biblioteca de audio
            {
                throw new Exception("Failed to initialize audio device.");
            }
            _initialized = true;
            
            // Iniciar hilo de reproducción
            StartPlaybackThread();
        }

        // Iniciar hilo de reproducción
        private void StartPlaybackThread()
        {
            _isRunning = true;                              // Activar bandera
            _playbackThread = new Thread(PlaybackWorker)    // Crea un nuevo hilo que ejecutará PlaybackWorker
            {
                Name = "ToneGenerator Playback",            // Nombre para depuración
                Priority = ThreadPriority.Highest           // Prioridad alta para menos lag
            };
            _playbackThread.Start();                        // Iniciar el hilo
        }

        // Encolar una nota
        public void PlayTone(double frequency, double durationSeconds)
        {
            // Si no se está corriendo o inicializando, no hacer nada
            if (!_isRunning || !_initialized) return;

            // Crear una petición de sonido
            var request = new ToneRequest
            {
                Frequency = frequency,              // Frecuencia
                DurationSeconds = durationSeconds,  // Segundos
                CreatedTime = DateTime.Now          // Depuración
            };

            // Poner la petición en colar
            _toneQueue.Enqueue(request);
        }

        // Trabajador de hilo
        private void PlaybackWorker()
        {
            // Corre en un hilo separado
            while (_isRunning)  // Mientras esté corriendo
            {
                // Intentar sacar una petición de la cola
                if (_toneQueue.TryDequeue(out var request))
                {
                    // Si hay una petición, reproducirla
                    PlayToneInternal(request.Frequency, request.DurationSeconds);
                }
                else
                {
                    // Si no hay peticiones
                    Thread.Sleep(1); // Pequeña pausa para no consumir CPU
                }
            }
        }

        // Reproducir una nota
        private void PlayToneInternal(double frequency, double durationSeconds)
        {
            // Solo un hilo puede entrar a la vez
            // Evita que dos notas se mezclen
            lock (_lock)
            {
                try
                {
                    // Crear un procedimiento de stream con posición mantenida
                    double phase = 0.0;                                                 // Preparar la onda
                    double phaseIncrement = 2.0 * Math.PI * frequency / _sampleRate;    // Cuánto avanza la fase por muestra
                    int totalSamples = (int)(durationSeconds * _sampleRate);            // Total de muestras necesarias
                    int samplesPlayed = 0;                                              // Muestras ya generadas

                    // Crear stream con función de callback optimizada
                    var proc = new StreamProcedure((handle, buffer, length, user) =>    // Esta función se llama CUANDO Bass necesita más audio
                    {
                        // ¿Cuántas muestras podemos generar ahora?
                        int sampleCount = length / sizeof(float);   // Cuántas caben en el buffer
                        int samplesToGenerate = Math.Min(sampleCount, totalSamples - samplesPlayed);

                        // Si ya generamos todas, terminar
                        if (samplesToGenerate <= 0)
                            return 0;

                        // Crear un array para las muestras
                        float[] samples = new float[samplesToGenerate];

                        // Generar cada muestra
                        for (int i = 0; i < samplesToGenerate; i++)
                        {
                            // Calcular el valor de la onda en este punto
                            float sample = _oscillatorType switch
                            {
                                // Onda senoidadl: valor entre -1 y 1, suave
                                OscillatorType.Sine => (float)Math.Sin(phase),

                                // Onda cuadrada: -0.5 o 0.5 (cambia bruscamente)
                                OscillatorType.Square => (float)(Math.Sin(phase) >= 0 ? 0.5 : -0.5),

                                // Onda de diente de sierra: sube linealmente y cae bruscamente
                                OscillatorType.Sawtooth => (float)(2.0 * (phase / (2 * Math.PI) - Math.Floor(0.5 + phase / (2 * Math.PI)))),

                                // Por defecto: senoidal
                                _ => (float)Math.Sin(phase)
                            };

                            samples[i] = sample * 0.5f; // 50% volumen
                            phase += phaseIncrement;    // Avanzar fase

                            // Mantener fase entre 0 y 2π
                            if (phase > 2 * Math.PI)
                                phase -= 2 * Math.PI;
                        }

                        // Copiar muestras del C# al buffer de Bass
                        System.Runtime.InteropServices.Marshal.Copy(samples, 0, buffer, samplesToGenerate);
                        samplesPlayed += samplesToGenerate;                                                 // Actualizar contador

                        return samplesToGenerate * sizeof(float);                                           // Bytes generados
                    });

                    // Crear stream con nuestro procedimiento
                    int streamHandle = Bass.CreateStream(_sampleRate, 1, BassFlags.Float, proc);
                    
                    // Verificación si se crea correctamente
                    if (streamHandle == 0)
                    {
                        Console.WriteLine($"Failed to create stream. Error: {Bass.LastError}");
                        return;
                    }

                    // Reproducir y esperar sincrónicamente (pero sin bloquear el hilo principal)
                    Bass.ChannelPlay(streamHandle);
                    
                    // Esperar que termine la nota usando un método más preciso
                    int waitTime = (int)(durationSeconds * 1000);               // Convertir a ms
                    int elapsed = 0;                                            // Tiempo transcurrido

                    // Esperar hasta que termine el tiempo o el stream deje de sonar
                    while (elapsed < waitTime && Bass.ChannelIsActive(streamHandle) != 0)
                    {
                        Thread.Sleep(10);   // Esperar 10ms
                        elapsed += 10;      // Sumar al tiempo transcurrido
                    }

                    // Liberar stream inmediatamente
                    Bass.ChannelStop(streamHandle);
                    Bass.StreamFree(streamHandle);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error playing tone: {ex.Message}");
                }
            }
        }

        // Detener todo
        public void StopAll()
        {
            lock (_lock)
            {
                // Vaciar la cola de peticiones
                while (_toneQueue.TryDequeue(out _)) { }
            }
        }

        // Limpiar recursos
        public void Dispose()
        {
            _isRunning = false;
            StopAll();
            
            if (_playbackThread != null && _playbackThread.IsAlive)
            {
                _playbackThread.Join(500);
            }

            if (_initialized)
            {
                Bass.Free();
                _initialized = false;
            }
        }

        // Estructura para guardar peticiones de sonido
        private struct ToneRequest
        {
            public double Frequency { get; set; }
            public double DurationSeconds { get; set; }
            public DateTime CreatedTime { get; set; }
        }

        // Enum de los tipos de onda
        public enum OscillatorType
        {
            Sine,
            Square,
            Sawtooth
        }
    }
}