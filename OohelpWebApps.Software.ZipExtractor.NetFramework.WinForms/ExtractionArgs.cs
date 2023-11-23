using System;
using System.IO;
using System.Text;

namespace OohelpWebApps.Software.ZipExtractor
{
    public sealed class ExtractionArgs
    {
        /// <summary>
        /// Путь к архиву
        /// </summary>
        public string ZipFile { get; }


        /// <summary>
        /// Директория распаковки архива.
        /// </summary>
        public string ExtractionDirectory { get; }


        /// <summary>
        /// Путь к исполняемому файлу приложения
        /// </summary>
        public string ExecutableFile { get; }

        
        /// <summary>
        /// Индикатор, запускаем или нет приложение после установки
        /// </summary>
        public bool StartExecutableAfterExtraction { get; }

        public ExtractionArgs(string zipFile, string extractionDirectory, string executableFile, bool startExecutableAfterExtraction)
        {
            ZipFile = zipFile ?? throw new ArgumentNullException(nameof(zipFile));
            ExtractionDirectory = extractionDirectory ?? throw new ArgumentNullException(nameof(extractionDirectory));
            ExecutableFile = executableFile ?? throw new ArgumentNullException(nameof(executableFile));
            StartExecutableAfterExtraction = startExecutableAfterExtraction;
        }

        public static ExtractionArgs Parse(string[] arguments, StringBuilder _logBuilder)
        {
            if (arguments == null || arguments.Length == 0)
            {
                _logBuilder.AppendLine("ZipExtractor запущен без параметров.");
                return null;
            }

            _logBuilder.AppendLine("Параметры старта ZipExtractor:");
            byte index = 0;
            foreach (var arg in arguments)
            {
                _logBuilder.AppendLine($"[{index++}] {arg}");
            }

            if (arguments.Length < 5)
            {
                _logBuilder.AppendLine("Недостаточное количество параметров (должно быть 5).");
                return null;
            }

            string zipFile = arguments[1];
            if (!File.Exists(zipFile))
            {
                _logBuilder.AppendLine($"Не найден файл архива: {zipFile}");
                return null;
            }

            string extractionDir = arguments[2];
            if (!Directory.Exists(extractionDir))
            {
                _logBuilder.AppendLine($"Не найдена папка для распаковки: {extractionDir}");
                return null;
            }                  

            string executableFile = arguments[3];

            bool runAfter;

            if (arguments[4].Equals("y", StringComparison.OrdinalIgnoreCase))
            { 
                runAfter = true;
            }
            else if(arguments[4].Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                runAfter = false;
            }
            else
            {
                _logBuilder.AppendLine($"Неизвестный индикатор старта: {arguments[4]}. Должен быть y/n");
                return null;
            }            

            return new ExtractionArgs(zipFile, extractionDir, executableFile, runAfter);
        }
    }
}
