using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;

public class ResultsTableGenerator
{
    public void Create(string scenarioName, GameplayRole role, GameplayMode mode, string fullName, string group, DateTime date, TimeSpan duration, List<ResultRecord> results)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using ExcelPackage excelPackage = new();

        double maxPercentPerRecord = 100.0 / results.Count;
        double minPercentPerRecord = 50.0 / results.Count;

        // Создание нового документа Excel
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Результаты экзамена");

        // Форматирование ячеек с информацией о прохождении экзамена
        FormatInformationCells(worksheet, date, fullName, group, duration, role, mode);

        // Заполнение таблицы с результатами экзамена
        FillResultsTable(worksheet, results, duration);

        // Оформление таблицы в виде клеточек
        FormatTableCells(worksheet);

        // Добавление автоматических расчетов
        AddFormula(worksheet, results);

        // Форматирование заголовков для таблицы
        FormatHeaderCells(worksheet);

        // Автонастройка ширины столбцов
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Защита ячеек
        //ProtectCells(worksheet);

        // Сохранение файла с результатами экзамена
        SaveExcelFile(scenarioName, fullName, date, group, excelPackage);
    }

    private static void FormatInformationCells(ExcelWorksheet worksheet, DateTime date, string fullName, string group, TimeSpan duration, GameplayRole role, GameplayMode mode)
    {
        string roleText = role switch
        {
            GameplayRole.TrainDriver => "Машинист поезда",
            GameplayRole.Assistant => "Помощник",
            _ => "Не выбрано"
        };

        string modeText = mode switch
        {
            GameplayMode.Training => "Обучение",
            GameplayMode.Exam => "Экзамен",
            _ => "Не выбрано"
        };

        worksheet.Cells["A1"].Value = "Дата начала:";
        worksheet.Cells["B1"].Value = date.ToString("dd'.'MM'.'yyyy HH:mm:ss");
        worksheet.Cells["A2"].Value = "ФИО:";
        worksheet.Cells["B2"].Value = fullName;
        worksheet.Cells["A3"].Value = "Группа:";
        worksheet.Cells["B3"].Value = group;
        worksheet.Cells["A4"].Value = "Длительность прохождения:";
        worksheet.Cells["B4"].Value = duration.ToString(@"mm\:ss");
        worksheet.Cells["A5"].Value = "Роль:";
        worksheet.Cells["B5"].Value = roleText;
        worksheet.Cells["A6"].Value = "Режим:";
        worksheet.Cells["B6"].Value = modeText;

        using ExcelRange range = worksheet.Cells["A1:A6"];
        range.Style.Font.Bold = true;
    }

    private static void FillResultsTable(ExcelWorksheet worksheet, List<ResultRecord> results, TimeSpan gameplayDuration)
    {
        worksheet.Cells["A8"].Value = "№";
        worksheet.Cells["B8"].Value = "Наименование";
        worksheet.Cells["C8"].Value = "Описание";
        worksheet.Cells["D8"].Value = "Время";
        worksheet.Cells["E8"].Value = "Совершенные нарушения";

        int row = 9;

        for (int i = 0; i < results.Count; i++)
        {
            TimeSpan endTime = i + 1 < results.Count ? results[i + 1].ObservationDiary.StartTime : gameplayDuration;
            TimeSpan duration = endTime - results[i].ObservationDiary.StartTime;

            worksheet.Cells[row, 1].Value = i + 1;
            worksheet.Cells[row, 2].Value = results[i].Name;
            worksheet.Cells[row, 3].Value = results[i].Description;
            worksheet.Cells[row, 4].Value = duration.ToString(@"mm\:ss");
            worksheet.Cells[row, 5].Value = results[i].ObservationDiary.Wrongs[WrongType.Hard].Count;

            row++;
        }
    }

    private static void FormatTableCells(ExcelWorksheet worksheet)
    {
        using ExcelRange range = worksheet.Cells[8, 1, worksheet.Dimension.End.Row, 5];
        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        using ExcelRange range1 = worksheet.Cells[9, 1, worksheet.Dimension.End.Row, 1];
        range1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        using ExcelRange range2 = worksheet.Cells[9, 4, worksheet.Dimension.End.Row, 5];
        range2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    }

    private static void FormatHeaderCells(ExcelWorksheet worksheet)
    {
        using ExcelRange range = worksheet.Cells["A8:E8"];
        range.Style.Font.Bold = true;
        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    }

    private static void AddFormula(ExcelWorksheet worksheet, List<ResultRecord> results)
    {
        int lastRow = results.Count + 8;
        int wrongsCount = CountWrongs(WrongType.Hard, results);
        int percentCorrect = Math.Clamp(100 - ((100 / results.Count) * wrongsCount), 0, 100);

        worksheet.Cells[lastRow + 2, 3].Value = $"Правильность прохождения: {percentCorrect}% ({wrongsCount} нарушений в {results.Count} пунктах)";
        worksheet.Cells[lastRow + 2, 5].Value = $"Итого: {wrongsCount} нарушений";
        worksheet.Cells[lastRow + 2, 5].Style.Font.Bold = true;

        static int CountWrongs(WrongType wrongType, List<ResultRecord> collection)
        {
            int counter = 0;

            foreach (var record in collection)
                counter += record.ObservationDiary.Wrongs[wrongType].Count;

            return counter;
        }
    }

    private static void ProtectCells(ExcelWorksheet worksheet)
    {
        worksheet.Protection.SetPassword("YourPassword");
        worksheet.Protection.AllowAutoFilter = true;
        worksheet.Protection.AllowSort = true;
        worksheet.Protection.AllowInsertColumns = true;
        worksheet.Protection.AllowDeleteColumns = true;
    }

    private static void SaveExcelFile(string scenarioName, string fullName, DateTime date, string group, ExcelPackage excelPackage)
    {
        string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string saveDirectoryPath = Path.Combine(rootDirectory, "Результаты прохождения", group, fullName, CleanFileName(scenarioName));
        Directory.CreateDirectory(saveDirectoryPath);

        string fileName = CleanFileName($@"{date:dd'.'MM'.'yyyy HH:mm}.xlsx");
        string excelFilePath = Path.Combine(saveDirectoryPath, fileName);
        FileInfo fileInfo = new(excelFilePath);
        excelPackage.SaveAs(fileInfo);

        Console.WriteLine($"Файл с результатами сохранен в папке {saveDirectoryPath}");

        static string CleanFileName(string fileName) => string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
    }
}