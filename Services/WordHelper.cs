using ExamSheduleDesign.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;

namespace ExamSheduleDesign.Services
{
    public class WordHelper
    {
        private Word.Application _wordApp;
        private Word.Document _doc;
        private readonly List<Exam> _exams;

        public WordHelper(List<Exam> exams)
        {
            _exams = exams;
        }

        public void CreateAllDocuments(string baseFolderPath, CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report("Начало генерации документов...");

            string dateFolderName = DateTime.Now.ToString("dd.MM.yyyy");
            string targetFolder = Path.Combine(baseFolderPath, $"Расписания {dateFolderName}");
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            progress?.Report("Создание документа по дате...");
            CreateDocumentByDate(Path.Combine(targetFolder, "Расписание промежуточной аттестации (по дате).docx"), cancellationToken, progress);

            progress?.Report("Создание документа по преподавателям...");
            CreateDocumentByTeachers(Path.Combine(targetFolder, "Расписание промежуточной аттестации (по преподавателям).docx"), cancellationToken, progress);

            progress?.Report("Создание документа по дисциплинам...");
            CreateDocumentBySubjects(Path.Combine(targetFolder, "Расписание промежуточной аттестации (по дисциплинам).docx"), cancellationToken, progress);

            progress?.Report("Создание документа по отделениям...");
            CreateDocumentByDepartments(Path.Combine(targetFolder, "Расписание промежуточной аттестации (по отделениям).docx"), cancellationToken, progress);

            progress?.Report("Генерация завершена.");
        }

        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }

        private string GetFullTeacherName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "";
            fullName = NormalizeName(fullName);
            if (fullName.Contains(".")) return fullName;

            string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length >= 2)
            {
                string lastName = nameParts[0];
                string initials = "";
                for (int i = 1; i < nameParts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(nameParts[i]))
                        initials += nameParts[i][0] + ".";
                }
                return $"{lastName} {initials}";
            }
            return fullName;
        }

        private void CreateDocumentByDate(string filePath, CancellationToken cancellationToken, IProgress<string> progress)
        {
            CreateDocumentInternal("Расписание промежуточной аттестации (по дате)",
                CreateTableFromExams, filePath, false, cancellationToken, progress);
        }

        private void CreateDocumentByTeachers(string filePath, CancellationToken cancellationToken, IProgress<string> progress)
        {
            CreateDocumentInternal("Расписание промежуточной аттестации (по преподавателям)",
                CreateTableFromTeachers, filePath, false, cancellationToken, progress);
        }

        private void CreateDocumentBySubjects(string filePath, CancellationToken cancellationToken, IProgress<string> progress)
        {
            CreateDocumentInternal("Расписание промежуточной аттестации (по дисциплинам)",
                CreateTableFromSubjects, filePath, false, cancellationToken, progress);
        }

        private void CreateDocumentByDepartments(string filePath, CancellationToken cancellationToken, IProgress<string> progress)
        {
            CreateDocumentInternal("Расписание промежуточной аттестации (по отделениям)",
                CreateTableFromDepartments, filePath, true, cancellationToken, progress);
        }

        private void CreateDocumentInternal(string title, Action<CancellationToken, IProgress<string>> tableCreationMethod,
                                            string filePath, bool isDepartmentDocument, CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                _wordApp = new Word.Application();
                _wordApp.Visible = false;
                _doc = _wordApp.Documents.Add();

                ConfigurePageSetup(isDepartmentDocument);
                AddPageNumbers();
                CreateHeader(new List<string> { "Утверждаю: ", "директор СПб ГБПОУ \"АТТ\" ", "_________________Корабельников С.К." });
                CreateDocumentTitle(title);

                tableCreationMethod(cancellationToken, progress);
                AddEducationalPartSignature();

                _doc.SaveAs2(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании документа: {ex.Message}", ex);
            }
            finally
            {
                Cleanup();
            }
        }

        private void CreateTableFromExams(CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var examsByDate = _exams
                    .Where(e => e.Date != DateTime.MinValue)
                    .GroupBy(e => e.Date.ToString("dd.MM.yyyy"))
                    .OrderBy(g => DateTime.Parse(g.Key))
                    .ToList();

                int totalRows = 1;
                foreach (var dateGroup in examsByDate)
                {
                    totalRows++;
                    totalRows += dateGroup.Count();
                }

                Word.Table table = _doc.Tables.Add(_doc.Range(_doc.Content.End - 1), totalRows, 6,
                    Word.WdDefaultTableBehavior.wdWord9TableBehavior, Word.WdAutoFitBehavior.wdAutoFitWindow);
                table.Borders.Enable = 0;
                table.Rows[1].HeadingFormat = -1;

                string[] headers = { "Время", "Группа", "Дисциплина", "Преподаватель", "Аудитория", "Тип" };
                for (int i = 0; i < headers.Length; i++)
                {
                    Word.Cell cell = table.Cell(1, i + 1);
                    cell.Range.Text = headers[i];
                    FormatCell(cell, "Times New Roman", 12, true, 12, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                }

                int currentRow = 2;
                foreach (var dateGroup in examsByDate)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string dayOfWeek = CapitalizeFirstLetter(GetDayOfWeekFromDate(dateGroup.Key));
                    string dateDisplay = $"{dateGroup.Key} {dayOfWeek}";
                    Word.Cell dateCell = table.Cell(currentRow, 1);
                    dateCell.Range.Text = dateDisplay;
                    table.Cell(currentRow, 1).Merge(table.Cell(currentRow, 6));
                    FormatCell(dateCell, "Times New Roman", 14, true, 14, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                    currentRow++;

                    var sortedExams = dateGroup.OrderBy(e => ParseTimeForSorting(e.Time)).ToList();
                    foreach (var exam in sortedExams)
                    {
                        AddExamDataToTable(table, currentRow, exam);
                        currentRow++;
                    }
                }
                _doc.Range(_doc.Content.End - 1).InsertParagraphAfter();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании таблицы по датам: {ex.Message}", ex);
            }
        }

        private void AddExamDataToTable(Word.Table table, int rowNumber, Exam exam)
        {
            string teacherDisplay = GetFullTeacherName(exam.Teacher1?.Name);
            if (exam.Teacher2 != null)
                teacherDisplay += $"/{GetFullTeacherName(exam.Teacher2.Name)}";

            string[] data = {
                exam.Time,
                exam.Group?.Name ?? "",
                exam.Discipline?.FullName ?? "",
                teacherDisplay,
                exam.Classroom,
                exam.Type
            };
            for (int i = 0; i < data.Length; i++)
            {
                Word.Cell cell = table.Cell(rowNumber, i + 1);
                cell.Range.Text = data[i];
                FormatCell(cell, "Times New Roman", 11, false, 11, Word.WdParagraphAlignment.wdAlignParagraphLeft);
            }
        }

        private void CreateTableFromTeachers(CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var allTeachers = new Dictionary<string, List<Exam>>();
                foreach (var exam in _exams)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (exam.Teacher1 != null)
                    {
                        string teacherKey = GetFullTeacherName(exam.Teacher1.Name);
                        if (!allTeachers.ContainsKey(teacherKey))
                            allTeachers[teacherKey] = new List<Exam>();
                        allTeachers[teacherKey].Add(exam);
                    }
                    if (exam.Teacher2 != null)
                    {
                        string teacherKey = GetFullTeacherName(exam.Teacher2.Name);
                        if (!allTeachers.ContainsKey(teacherKey))
                            allTeachers[teacherKey] = new List<Exam>();
                        allTeachers[teacherKey].Add(exam);
                    }
                }

                var sortedTeachers = allTeachers.OrderBy(t => t.Key).ToList();
                int totalRows = 1;
                foreach (var teacher in sortedTeachers)
                {
                    totalRows++;
                    totalRows += teacher.Value.Count;
                }

                Word.Table table = _doc.Tables.Add(_doc.Range(_doc.Content.End - 1), totalRows, 6,
                    Word.WdDefaultTableBehavior.wdWord9TableBehavior, Word.WdAutoFitBehavior.wdAutoFitWindow);
                table.Borders.Enable = 0;
                table.Rows[1].HeadingFormat = -1;

                string[] headers = { "Дата", "Время", "Группа", "Дисциплина", "Аудитория", "Тип" };
                for (int i = 0; i < headers.Length; i++)
                {
                    Word.Cell cell = table.Cell(1, i + 1);
                    cell.Range.Text = headers[i];
                    FormatCell(cell, "Times New Roman", 12, true, 12, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                }

                int currentRow = 2;
                foreach (var teacher in sortedTeachers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Word.Cell teacherCell = table.Cell(currentRow, 1);
                    teacherCell.Range.Text = teacher.Key;
                    table.Cell(currentRow, 1).Merge(table.Cell(currentRow, 6));
                    FormatCell(teacherCell, "Times New Roman", 14, true, 14, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                    currentRow++;

                    var sortedExams = teacher.Value
                        .OrderBy(e => e.Date)
                        .ThenBy(e => ParseTimeForSorting(e.Time))
                        .ToList();
                    foreach (var exam in sortedExams)
                    {
                        AddTeacherExamDataToTable(table, currentRow, exam);
                        currentRow++;
                    }
                }
                _doc.Range(_doc.Content.End - 1).InsertParagraphAfter();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании таблицы по преподавателям: {ex.Message}", ex);
            }
        }

        private void AddTeacherExamDataToTable(Word.Table table, int rowNumber, Exam exam)
        {
            string[] data = {
                exam.Date.ToString("dd.MM.yyyy"),
                exam.Time,
                exam.Group?.Name ?? "",
                exam.Discipline?.FullName ?? "",
                exam.Classroom,
                exam.Type
            };
            for (int i = 0; i < data.Length; i++)
            {
                Word.Cell cell = table.Cell(rowNumber, i + 1);
                cell.Range.Text = data[i];
                FormatCell(cell, "Times New Roman", 11, false, 11, Word.WdParagraphAlignment.wdAlignParagraphLeft);
            }
        }

        private void CreateTableFromSubjects(CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var examsBySubject = _exams
                    .Where(e => e.Discipline != null && !string.IsNullOrEmpty(e.Discipline.FullName))
                    .GroupBy(e => e.Discipline.FullName)
                    .OrderBy(g => g.Key)
                    .ToList();

                int totalRows = 1;
                foreach (var subjectGroup in examsBySubject)
                {
                    totalRows++;
                    totalRows += subjectGroup.Count();
                }

                Word.Table table = _doc.Tables.Add(_doc.Range(_doc.Content.End - 1), totalRows, 6,
                    Word.WdDefaultTableBehavior.wdWord9TableBehavior, Word.WdAutoFitBehavior.wdAutoFitWindow);
                table.Borders.Enable = 0;
                table.Rows[1].HeadingFormat = -1;

                string[] headers = { "Дата", "Время", "Группа", "Преподаватель", "Аудитория", "Тип" };
                for (int i = 0; i < headers.Length; i++)
                {
                    Word.Cell cell = table.Cell(1, i + 1);
                    cell.Range.Text = headers[i];
                    FormatCell(cell, "Times New Roman", 12, true, 12, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                }

                int currentRow = 2;
                foreach (var subjectGroup in examsBySubject)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Word.Cell subjectCell = table.Cell(currentRow, 1);
                    subjectCell.Range.Text = subjectGroup.Key;
                    table.Cell(currentRow, 1).Merge(table.Cell(currentRow, 6));
                    FormatCell(subjectCell, "Times New Roman", 14, true, 14, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                    currentRow++;

                    var sortedExams = subjectGroup
                        .OrderBy(e => e.Date)
                        .ThenBy(e => ParseTimeForSorting(e.Time))
                        .ToList();
                    foreach (var exam in sortedExams)
                    {
                        AddSubjectExamDataToTable(table, currentRow, exam);
                        currentRow++;
                    }
                }
                _doc.Range(_doc.Content.End - 1).InsertParagraphAfter();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании таблицы по дисциплинам: {ex.Message}", ex);
            }
        }

        private void AddSubjectExamDataToTable(Word.Table table, int rowNumber, Exam exam)
        {
            string teacherDisplay = GetFullTeacherName(exam.Teacher1?.Name);
            if (exam.Teacher2 != null)
                teacherDisplay += $"/{GetFullTeacherName(exam.Teacher2.Name)}";

            string[] data = {
                exam.Date.ToString("dd.MM.yyyy"),
                exam.Time,
                exam.Group?.Name ?? "",
                teacherDisplay,
                exam.Classroom,
                exam.Type
            };
            for (int i = 0; i < data.Length; i++)
            {
                Word.Cell cell = table.Cell(rowNumber, i + 1);
                cell.Range.Text = data[i];
                FormatCell(cell, "Times New Roman", 11, false, 11, Word.WdParagraphAlignment.wdAlignParagraphLeft);
            }
        }

        private void CreateTableFromDepartments(CancellationToken cancellationToken, IProgress<string> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var examsByDepartment = _exams
                    .GroupBy(e => string.IsNullOrEmpty(e.Group?.Department) ? "Без отделения" : e.Group.Department)
                    .OrderBy(g => g.Key == "Без отделения" ? 1 : 0)
                    .ThenBy(g => g.Key)
                    .ToList();

                int totalRows = 1;
                foreach (var departmentGroup in examsByDepartment)
                {
                    totalRows++;
                    var groupsInDepartment = departmentGroup
                        .GroupBy(e => e.Group?.Name ?? "Без группы")
                        .OrderBy(g => g.Key);
                    foreach (var group in groupsInDepartment)
                    {
                        totalRows++;
                        totalRows += group.Count();
                    }
                }

                Word.Table table = _doc.Tables.Add(_doc.Range(_doc.Content.End - 1), totalRows, 7,
                    Word.WdDefaultTableBehavior.wdWord9TableBehavior, Word.WdAutoFitBehavior.wdAutoFitWindow);
                table.Borders.Enable = 0;
                table.Rows[1].HeadingFormat = -1;

                string[] headers = { "Группа", "Дата", "Время", "Дисциплина", "Преподаватель", "Аудитория", "Тип" };
                for (int i = 0; i < headers.Length; i++)
                {
                    Word.Cell cell = table.Cell(1, i + 1);
                    cell.Range.Text = headers[i];
                    FormatCell(cell, "Times New Roman", 12, true, 12, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                }

                int currentRow = 2;
                foreach (var departmentGroup in examsByDepartment)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Word.Cell deptCell = table.Cell(currentRow, 1);
                    deptCell.Range.Text = departmentGroup.Key;
                    table.Cell(currentRow, 1).Merge(table.Cell(currentRow, 7));
                    FormatCell(deptCell, "Times New Roman", 14, true, 14, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                    currentRow++;

                    var groupsInDepartment = departmentGroup
                        .GroupBy(e => e.Group?.Name ?? "Без группы")
                        .OrderBy(g => g.Key);
                    foreach (var group in groupsInDepartment)
                    {
                        Word.Cell groupCell = table.Cell(currentRow, 1);
                        groupCell.Range.Text = group.Key;
                        FormatCell(groupCell, "Times New Roman", 12, true, 12, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                        for (int i = 2; i <= 7; i++)
                            table.Cell(currentRow, i).Range.Text = "";
                        currentRow++;

                        var sortedExams = group
                            .OrderBy(e => e.Date)
                            .ThenBy(e => ParseTimeForSorting(e.Time))
                            .ToList();
                        foreach (var exam in sortedExams)
                        {
                            AddDepartmentExamDataToTable(table, currentRow, exam);
                            currentRow++;
                        }
                    }
                }
                _doc.Range(_doc.Content.End - 1).InsertParagraphAfter();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании таблицы по отделениям: {ex.Message}", ex);
            }
        }

        private void AddDepartmentExamDataToTable(Word.Table table, int rowNumber, Exam exam)
        {
            string teacherDisplay = GetFullTeacherName(exam.Teacher1?.Name);
            if (exam.Teacher2 != null)
                teacherDisplay += $"/{GetFullTeacherName(exam.Teacher2.Name)}";

            string[] data = {
                exam.Date.ToString("dd.MM.yyyy"),
                exam.Time,
                exam.Discipline?.FullName ?? "",
                teacherDisplay,
                exam.Classroom,
                exam.Type
            };
            for (int i = 0; i < data.Length; i++)
            {
                Word.Cell cell = table.Cell(rowNumber, i + 2);
                cell.Range.Text = data[i];
                FormatCell(cell, "Times New Roman", 11, false, 11, Word.WdParagraphAlignment.wdAlignParagraphLeft);
            }
        }

        private void AddPageNumbers()
        {
            try
            {
                foreach (Word.Section section in _doc.Sections)
                {
                    Word.HeaderFooter footer = section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    footer.Range.Delete();
                    footer.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                    footer.Range.Font.Name = "Times New Roman";
                    footer.Range.Font.Size = 10;
                    footer.Range.Select();
                    Word.Selection selection = _wordApp.Selection;
                    selection.TypeText("Страница ");
                    selection.Fields.Add(selection.Range, Word.WdFieldType.wdFieldPage);
                    selection.TypeText(" из ");
                    selection.Fields.Add(selection.Range, Word.WdFieldType.wdFieldNumPages);
                    selection.Fields.Update();
                }
                _wordApp.Selection.Collapse();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка добавления номеров страниц: {ex.Message}", ex);
            }
        }

        private void CreateHeader(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Word.Paragraph paragraph = _doc.Content.Paragraphs.Add();
                paragraph.Range.Text = "\t" + lines[i];
                SafeSetFont(paragraph.Range.Font, "Times New Roman", 12, 1);
                paragraph.Format.SpaceAfter = 0;
                paragraph.Format.SpaceBefore = (i == 0) ? 12 : 0;
                paragraph.Format.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
                paragraph.Format.TabStops.Add(_wordApp.CentimetersToPoints(9.5f));
                paragraph.Range.InsertParagraphAfter();
            }
        }

        private void CreateDocumentTitle(string titleText)
        {
            Word.Paragraph titleParagraph = _doc.Content.Paragraphs.Add();
            titleParagraph.Range.Text = titleText;
            SafeSetFont(titleParagraph.Range.Font, "Times New Roman", 18, 1);
            titleParagraph.Format.SpaceAfter = 0;
            titleParagraph.Format.SpaceBefore = 18;
            titleParagraph.Format.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
            titleParagraph.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            titleParagraph.Range.InsertParagraphAfter();
        }

        private void AddEducationalPartSignature()
        {
            try
            {
                string todayDate = DateTime.Now.ToString("dd.MM.yyyy");
                List<string> signatureLines = new List<string>
                {
                    "Зав. Учебной частью",
                    "______________________Кожекина И.Ю.",
                    todayDate
                };

                for (int i = 0; i < signatureLines.Count; i++)
                {
                    Word.Paragraph paragraph = _doc.Content.Paragraphs.Add();
                    paragraph.Range.Text = "\t" + signatureLines[i];
                    SafeSetFont(paragraph.Range.Font, "Times New Roman", 12, 0);
                    paragraph.Format.SpaceAfter = 0;
                    paragraph.Format.SpaceBefore = (i == 0) ? 0 : (i == 1) ? 1 : 0;
                    paragraph.Format.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
                    paragraph.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    paragraph.Range.InsertParagraphAfter();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении подписи: {ex.Message}", ex);
            }
        }

        private void ConfigurePageSetup(bool isDepartmentDocument)
        {
            try
            {
                Word.PageSetup pageSetup = _doc.PageSetup;
                if (isDepartmentDocument)
                {
                    pageSetup.TopMargin = _wordApp.CentimetersToPoints(1.0f);
                    pageSetup.BottomMargin = _wordApp.CentimetersToPoints(1.0f);
                    pageSetup.LeftMargin = _wordApp.CentimetersToPoints(1.0f);
                    pageSetup.RightMargin = _wordApp.CentimetersToPoints(1.0f);
                }
                else
                {
                    pageSetup.TopMargin = _wordApp.CentimetersToPoints(2.0f);
                    pageSetup.BottomMargin = _wordApp.CentimetersToPoints(2.0f);
                    pageSetup.LeftMargin = _wordApp.CentimetersToPoints(2.1f);
                    pageSetup.RightMargin = _wordApp.CentimetersToPoints(1.0f);
                }
                pageSetup.Gutter = _wordApp.CentimetersToPoints(0f);
                pageSetup.GutterPos = Word.WdGutterStyle.wdGutterPosLeft;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка настройки страницы: {ex.Message}", ex);
            }
        }

        private string GetDayOfWeekFromDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
                return date.ToString("dddd", new CultureInfo("ru-RU"));
            return "Неизвестный день";
        }

        private string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        private TimeSpan? ParseTimeForSorting(string timeString)
        {
            if (string.IsNullOrEmpty(timeString)) return null;
            if (TimeSpan.TryParse(timeString, out TimeSpan time)) return time;
            string[] parts = timeString.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[0], out int h) && int.TryParse(parts[1], out int m))
                return new TimeSpan(h, m, 0);
            return null;
        }

        private void FormatCell(Word.Cell cell, string fontName, float fontSize, bool isBold, float spaceBefore, Word.WdParagraphAlignment alignment)
        {
            cell.Range.Font.Name = fontName;
            cell.Range.Font.Size = fontSize;
            cell.Range.Font.Bold = isBold ? 1 : 0;
            cell.Range.ParagraphFormat.SpaceAfter = 0;
            cell.Range.ParagraphFormat.SpaceBefore = spaceBefore;
            cell.Range.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
            cell.Range.ParagraphFormat.Alignment = alignment;
            cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
        }

        private void SafeSetFont(Word.Font font, string fontName, float fontSize, int bold)
        {
            try
            {
                font.Name = fontName;
                font.Size = fontSize;
                font.Bold = bold;
            }
            catch (COMException)
            {
                font.Name = "Arial";
                font.Size = fontSize;
                font.Bold = bold;
            }
        }

        private void Cleanup()
        {
            try
            {
                if (_doc != null)
                {
                    _doc.Close();
                    Marshal.ReleaseComObject(_doc);
                    _doc = null;
                }
                if (_wordApp != null)
                {
                    _wordApp.Quit();
                    Marshal.ReleaseComObject(_wordApp);
                    _wordApp = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка очистки Word: {ex.Message}");
            }
        }
    }
}