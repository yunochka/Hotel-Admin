using HotelAdm2App;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;

namespace HotelAdm2App
{
    class HotelReportGenerator
    {
        public static void GenerateHotelReport(string filePath)
        {
            try
            {
                using (var context = new HotelsqlEntities())
                {
                    var data = new
                    {
                        Bookings = context.Booking.Include("Hotel_Room").Include("Guest").ToList(),
                        Guests = context.Guest.Include("Staff").ToList(),
                        Staffs = context.Staff.Include("Role").ToList(),
                        Hotel_Rooms = context.Hotel_Room.Include("Staff").ToList(),
                        Roles = context.Role.ToList()
                    };

                    if (File.Exists(filePath)) File.Delete(filePath);

                    Word.Application wordApp = new Word.Application();
                    wordApp.Visible = false;
                    Word.Document doc = wordApp.Documents.Add();

                    SetDocumentStyles(doc);
                    AddTitle(doc, "Отчет по управлению отелем");

                    AddBookingsSection(doc, data.Bookings);
                    AddPageBreak(doc);

                    AddGuestsSection(doc, data.Guests);
                    AddPageBreak(doc);

                    AddHotelRoomsSection(doc, data.Hotel_Rooms);
                    AddPageBreak(doc);

                    AddStaffSection(doc, data.Staffs);
                    AddPageBreak(doc);

                    AddRolesSection(doc, data.Roles);

                    doc.SaveAs2(filePath);
                    doc.Close();
                    wordApp.Quit();

                    ReleaseWordObjects(doc, wordApp);
                    OpenGeneratedReport(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void OpenGeneratedReport(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть отчет: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void SetDocumentStyles(Word.Document doc)
        {
            doc.Content.Font.Name = "Times New Roman";
            doc.Content.Font.Size = 14;
            doc.Content.ParagraphFormat.LineSpacing = 18f;
            doc.Content.ParagraphFormat.SpaceBefore = 0;
            doc.Content.ParagraphFormat.SpaceAfter = 0;
            doc.Content.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
        }

        private static void AddTitle(Word.Document doc, string text)
        {
            Word.Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = text;
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;
            title.Format.SpaceBefore = 0;
            title.Format.SpaceAfter = 0;
            title.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();
        }

        private static void AddPageBreak(Word.Document doc)
        {
            Word.Paragraph lastParagraph = doc.Paragraphs.Add();
            lastParagraph.Range.InsertBreak(Word.WdBreakType.wdPageBreak);
        }

        private static void AddBookingsSection(Word.Document doc, List<Booking> bookings)
        {
            AddSectionTitle(doc, "Бронирования");
            Word.Table table = CreateTable(doc, new string[] { "ID", "Гость", "Номер", "Дата заезда", "Дата выезда" });

            foreach (var item in bookings)
            {
                AddRowToTable(table, new string[] {
                    item.Booking_ID.ToString(),
                    item.Guest?.Full_Name ?? "Неизвестно",
                    item.Hotel_Room?.Room_Number ?? "Неизвестно",
                    item.Check_In_Date.ToShortDateString(),
                    item.Check_Out_Date.ToShortDateString()
                });
            }
            FormatTable(table);
        }

        private static void AddGuestsSection(Word.Document doc, List<Guest> guests)
        {
            AddSectionTitle(doc, "Гости");
            Word.Table table = CreateTable(doc, new string[] { "ID", "ФИО", "Паспорт", "Телефон", "Email", "Заезд", "Выезд", "Сотрудник" });

            foreach (var item in guests)
            {
                AddRowToTable(table, new string[] {
                    item.Guest_ID.ToString(),
                    item.Full_Name,
                    item.Passport_Number,
                    item.Phone,
                    item.Email,
                    item.Check_In_Date.ToShortDateString(),
                    item.Check_Out_Date.ToShortDateString(),
                    item.Staff?.Full_Name ?? "Неизвестно"
                });
            }
            FormatTable(table);
        }

        private static void AddHotelRoomsSection(Word.Document doc, List<Hotel_Room> rooms)
        {
            AddSectionTitle(doc, "Номера отеля");
            Word.Table table = CreateTable(doc, new string[] { "ID", "Номер", "Цена", "Статус", "Описание", "Сотрудник" });

            foreach (var item in rooms)
            {
                AddRowToTable(table, new string[] {
                    item.Hotel_Room_ID.ToString(),
                    item.Room_Number,
                    item.Room_Price.ToString("C"),
                    item.Room_Status,
                    item.Room_Description,
                    item.Staff?.Full_Name ?? "Неизвестно"
                });
            }
            FormatTable(table);
        }

        private static void AddStaffSection(Word.Document doc, List<Staff> staff)
        {
            AddSectionTitle(doc, "Сотрудники");
            Word.Table table = CreateTable(doc, new string[] { "ID", "ФИО", "Логин", "Телефон", "Смена", "Роль" });

            foreach (var item in staff)
            {
                AddRowToTable(table, new string[] {
                    item.Staff_ID.ToString(),
                    item.Full_Name,
                    item.Login,
                    item.Phone,
                    item.Shift,
                    item.Role?.Role_Name ?? "Неизвестно"
                });
            }
            FormatTable(table);
        }

        private static void AddRolesSection(Word.Document doc, List<Role> roles)
        {
            AddSectionTitle(doc, "Роли");
            Word.Table table = CreateTable(doc, new string[] { "ID", "Название роли" });

            foreach (var item in roles)
            {
                AddRowToTable(table, new string[] {
                    item.Role_ID.ToString(),
                    item.Role_Name
                });
            }
            FormatTable(table);
        }

        private static void AddSectionTitle(Word.Document doc, string title)
        {
            Word.Paragraph sectionTitle = doc.Paragraphs.Add();
            sectionTitle.Range.Text = title;
            sectionTitle.Range.Font.Bold = 1;
            sectionTitle.Range.Font.Size = 14;
            sectionTitle.Format.SpaceBefore = 0;
            sectionTitle.Format.SpaceAfter = 0;
            sectionTitle.Format.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            sectionTitle.Range.InsertParagraphAfter();
        }

        private static Word.Table CreateTable(Word.Document doc, string[] headers)
        {
            Word.Table table = doc.Tables.Add(doc.Range(doc.Content.End - 1), 1, headers.Length);
            for (int i = 0; i < headers.Length; i++)
            {
                table.Cell(1, i + 1).Range.Text = headers[i];
                table.Cell(1, i + 1).Range.Font.Bold = 1;
                table.Cell(1, i + 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            }
            return table;
        }

        private static void AddRowToTable(Word.Table table, string[] values)
        {
            table.Rows.Add();
            int rowIndex = table.Rows.Count;
            for (int i = 0; i < values.Length; i++)
            {
                table.Cell(rowIndex, i + 1).Range.Text = values[i] ?? "";
                table.Cell(rowIndex, i + 1).Range.Font.Bold = 0;
                table.Cell(rowIndex, i + 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            }
        }

        private static void FormatTable(Word.Table table)
        {
            table.Columns.AutoFit();
            table.Borders.Enable = 1;
            foreach (Word.Row row in table.Rows)
            {
                foreach (Word.Cell cell in row.Cells)
                {
                    cell.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                }
            }
        }

        private static void ReleaseWordObjects(params object[] objects)
        {
            foreach (var obj in objects)
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                }
                catch { }
                finally
                {
                    GC.Collect();
                }
            }
        }
    }
}